using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace FastReport.Export.TTF
{

    internal enum FontType
    {
        TrueTypeFont = 0x00000000,
        TrueTypeCollection = 0x66637474
    }

    #region Helpers
    internal abstract class TTF_Helpers
    {
        public ushort SwapUInt16(ushort v)
        {
            return (ushort)(((v & 0xff) << 8) | ((v >> 8) & 0xff));
        }

        public short SwapInt16(short v)
        {
            return (short)(((v & 0xff) << 8) | ((v >> 8) & 0xff));
        }

        public uint SwapUInt32(uint v)
        {
            return (uint)(((SwapUInt16((ushort)v) & 0xffff) << 0x10) | (SwapUInt16((ushort)(v >> 0x10)) & 0xffff));
        }

        public ulong SwapUInt64(ulong v)
        {
            return (ulong)(((SwapUInt32((uint)v) & 0xffffffffL) << 0x20) | (SwapUInt32((uint)(v >> 0x20)) & 0xffffffffL));
        }

        public IntPtr Increment(IntPtr ptr, int cbSize)
        {
            return new IntPtr(ptr.ToInt64() + cbSize);
        }
    }
    #endregion
    
    /////////////////////////////////////////////////////////////////////////////////////////////////
    // Describes any table in TrueType font or collection
    /////////////////////////////////////////////////////////////////////////////////////////////////
    internal class TrueTypeTable : TTF_Helpers
    {
        #region "Type definitions"
        [StructLayout(LayoutKind.Explicit, Pack = 1)]
        public struct TableEntry
        {
            [FieldOffset(0)]
            public uint tag;
            [FieldOffset(4)]
            public uint checkSum;
            [FieldOffset(8)]
            public uint offset;
            [FieldOffset(12)]
            public uint length;
        }
        #endregion

        protected TableEntry entry;

        #region "Public properties"
        public string TAG 
        { 
            get 
            {
                return "" + 
                    (char)(0xff & entry.tag) + 
                    (char)(0xff & (entry.tag >> 8)) + 
                    (char)(0xff & (entry.tag >> 16)) + 
                    (char)(0xff & (entry.tag >> 24)); 
            } 
        }
        public uint tag { get { return entry.tag; } }
        public uint length { get { return entry.length; } set { entry.length = value; } }
        public uint offset { get { return entry.offset; } set { entry.offset = value; } }
        public uint checkSum { get { return entry.checkSum; } set { entry.checkSum = value; } }
        public int descriptor_size { get { return Marshal.SizeOf(entry); } }
        #endregion

        private void ChangeEndian()
        {
            entry.checkSum = SwapUInt32(entry.checkSum);
            entry.offset = SwapUInt32(entry.offset);
            entry.length = SwapUInt32(entry.length);
        }

        private uint StoreTable(IntPtr source_ptr, IntPtr destination_ptr, uint output_offset)
        {
            int length = (int)((entry.length + 3) / 4);

            IntPtr src = Increment(source_ptr, (int)entry.offset);
            IntPtr dst = Increment(destination_ptr, (int)output_offset);

            if (src != dst)
            {
                int[] buffer = new int[length];
                Marshal.Copy(src, buffer, 0, length);
                Marshal.Copy(buffer, 0, dst, length);
                buffer = null;

                entry.offset = output_offset;
            }
            else
            {
                ;
            }

            output_offset += (uint)(length * 4);

            return output_offset;
        }

        internal virtual void Load(IntPtr font)
        {
            // Как бы это ни было странно, но пока ничего здесь
        }

        internal virtual uint Save(IntPtr font, uint offset)
        {
            uint table_size = StoreTable(font, font, offset);
            return table_size;
        }

        internal IntPtr StoreDescriptor(IntPtr descriptor_ptr)
        {
            ChangeEndian();
            Marshal.StructureToPtr(entry, descriptor_ptr, false);
            ChangeEndian();
            return Increment(descriptor_ptr, Marshal.SizeOf(entry));
        }

        // Конструктор копирования
        public TrueTypeTable(TrueTypeTable parent)
        {
            entry = parent.entry;
        }

        // Обычный конструтор
        public TrueTypeTable(IntPtr entry_ptr)
        {
            entry = (TableEntry)Marshal.PtrToStructure(entry_ptr, typeof(TableEntry));
            ChangeEndian();
        }

    }

    /////////////////////////////////////////////////////////////////////////////////////////////////
    // True Type Font Collectiom
    /////////////////////////////////////////////////////////////////////////////////////////////////
    class TrueTypeCollection : TTF_Helpers
    {
        private ArrayList fonts_collection;

        #region DLL import
        [DllImport("Gdi32.dll")]
        private static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);
        [DllImport("Gdi32.dll")]
        private static extern IntPtr DeleteObject(IntPtr hgdiobj);
        [DllImport("Gdi32.dll", CharSet = CharSet.Auto)]
        private static extern uint GetFontData(IntPtr hdc, uint dwTable, uint dwOffset, [In, Out] byte[] lpvBuffer, uint cbData);
        [DllImport("Gdi32.dll", CharSet = CharSet.Auto)]
        private static extern uint GetFontData(IntPtr hdc, uint dwTable, uint dwOffset, [In, Out] IntPtr lpvBuffer, uint cbData);
        #endregion

        #region "Public properties"
        public IList FontCollection { get { return fonts_collection; } }
        #endregion

        #region "Collection structures"
        [StructLayout(LayoutKind.Explicit, Pack = 1)]
        public struct TTCollectionHeader
        {
            [FieldOffset(0)]
            public uint TTCTag;     //  	TrueType Collection ID string: 'ttcf'
            [FieldOffset(4)]
            public uint Version;    // 	Version of the TTC Header (1.0), 0x00010000
            [FieldOffset(8)]
            public uint numFonts;   // 	Number of fonts in TTC
        }

        #endregion

        public TrueTypeFont this[Font index]
        {
            get
            {
                foreach (TrueTypeFont font in fonts_collection)
                {
                    NameTableClass names = font.Names;
                    if (names[NameTableClass.NameID.FamilyName] == index.Name) return font;
                }
                return (TrueTypeFont)fonts_collection[0];
            }
        }

        public TrueTypeCollection(Font CollectionFont)
        {
            FontType CollectionMode;
            IntPtr font_data = IntPtr.Zero;
            Bitmap tempBitmap = new Bitmap(1, 1);

            fonts_collection = new ArrayList();

            using (Graphics g = Graphics.FromImage(tempBitmap))
            {
                IntPtr hdc = g.GetHdc();
                IntPtr f = CollectionFont.ToHfont();
                SelectObject(hdc, f);

                try
                {
                    uint fontDataSize;
                    // Try to read TrueTypeCollection
                    CollectionMode = FontType.TrueTypeCollection;
                    fontDataSize = GetFontData(hdc, (uint) CollectionMode, 0, IntPtr.Zero, 0);
                    if (fontDataSize == uint.MaxValue)
                    {
                        CollectionMode = FontType.TrueTypeFont;
                        fontDataSize = GetFontData(hdc, (uint)CollectionMode, 0, IntPtr.Zero, 0);
                    }
                    font_data = Marshal.AllocHGlobal((int)fontDataSize);
                    GetFontData(hdc, (uint) CollectionMode, 0, font_data, fontDataSize);

                    if (CollectionMode == FontType.TrueTypeFont)
                    {
                        fonts_collection.Add(new TrueTypeFont(font_data, font_data, TrueTypeFont.ChecksumFaultAction.Warn));
                    }
                    else 
                    {
                        TTCollectionHeader ch = (TTCollectionHeader)Marshal.PtrToStructure(font_data, typeof(TTCollectionHeader));
                        ch.Version = SwapUInt32(ch.Version);
                        ch.numFonts = SwapUInt32(ch.numFonts);

                        IntPtr subfont_table = Increment(font_data, Marshal.SizeOf(ch));
                        UInt32[] indexes = new UInt32[ch.numFonts];
                        for (int i = 0; i < ch.numFonts; i++)
                        {
                            UInt32 subfont_idx = SwapUInt32((UInt32)Marshal.ReadInt32(subfont_table, i * sizeof(UInt32)));
                            IntPtr subfont_ptr = Increment(font_data, (int)subfont_idx);
                            fonts_collection.Add(new TrueTypeFont(font_data, subfont_ptr, TrueTypeFont.ChecksumFaultAction.Warn));
                        }
                    }

                }
                finally
                {
                    DeleteObject(f);
                    g.ReleaseHdc(hdc);
                    tempBitmap.Dispose();
                }
            }
        }
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////
    // FontHeader table
    /////////////////////////////////////////////////////////////////////////////////////////////////
    class FontHeaderClass : TrueTypeTable
    {
        #region "Type definitions"
        [StructLayout(LayoutKind.Explicit, Pack = 1)]
        public struct FontHeader
        {
            [FieldOffset(0)]
            public uint version;     // FIXED	Table version number	0x00010000 for version 1.0.
            [FieldOffset(4)]
            public uint revision; // FIXED	fontRevision	Set by font manufacturer.
            [FieldOffset(8)]
            public uint checkSumAdjustment; // ULONG	checkSumAdjustment	To compute:  set it to 0, sum the entire font as ULONG, then store 0xB1B0AFBA - sum.
            [FieldOffset(12)]
            public uint magicNumber; // ULONG	magicNumber	Set to 0x5F0F3CF5.
            [FieldOffset(16)]
            public ushort flags; // USHORT	flags	Bit 0 - baseline for font at y=0;
            // Bit 1 - left sidebearing at x=0;
            // Bit 2 - instructions may depend on point size;
            // Bit 3 - force ppem to integer values for all internal scaler math; may use fractional ppem sizes if this bit is clear;
            // Bit 4 - instructions may alter advance width (the advance widths might not scale linearly);
            // Note: All other bits must be zero.
            [FieldOffset(18)]
            public ushort unitsPerEm; // USHORT	unitsPerEm	Valid range is from 16 to 16384
            [FieldOffset(20)]
            public ulong CreatedDateTime; // created	International date (8-byte field).
            [FieldOffset(28)]
            public ulong ModifiedDateTime; // modified	International date (8-byte field).
            [FieldOffset(36)]
            public short xMin; //	For all glyph bounding boxes.
            [FieldOffset(38)]
            public short yMin; // For all glyph bounding boxes.
            [FieldOffset(40)]
            public short xMax; // For all glyph bounding boxes.
            [FieldOffset(42)]
            public short yMax; // For all glyph bounding boxes.
            [FieldOffset(44)]
            public ushort macStyle; // Bit 0 bold (if set to 1); Bit 1 italic (if set to 1) Bits 2-15 reserved (set to 0).
            [FieldOffset(46)]
            public ushort lowestRecPPEM; // Smallest readable size in pixels.
            [FieldOffset(48)]
            public short fontDirectionHint;
            // 0   Fully mixed directional glyphs;
            // 1   Only strongly left to right;
            // 2   Like 1 but also contains neutrals ;
            //-1   Only strongly right to left;
            //-2   Like -1 but also contains neutrals.
            [FieldOffset(50)]
            public short indexToLocFormat; //	0 for short offsets, 1 for long.
            [FieldOffset(52)]
            public short glyphDataFormat; //	0 for current format.
        }

        public enum IndexToLoc
        {
            ShortType = 0,
            LongType = 1
        }
        #endregion

        private FontHeader  font_header;

        internal IndexToLoc indexToLocFormat { get { return (IndexToLoc)font_header.indexToLocFormat; } }
        internal uint checkSumAdjustment { set { font_header.checkSumAdjustment = value;  } }
        internal ushort unitsPerEm { get { return font_header.unitsPerEm; } }

        private void ChangeEndian()
        {
            font_header.indexToLocFormat = SwapInt16(font_header.indexToLocFormat);
            font_header.magicNumber = SwapUInt32(font_header.magicNumber);
            font_header.unitsPerEm = SwapUInt16(font_header.unitsPerEm);
        }


        internal override void Load(IntPtr font)
        {
            IntPtr header_ptr = Increment(font, (int)entry.offset);
            font_header = (FontHeader)Marshal.PtrToStructure(header_ptr, typeof(FontHeader));
            font_header.checkSumAdjustment = 0;
            Marshal.StructureToPtr(font_header, header_ptr, false);

            ChangeEndian();
        }

        internal void SaveFontHeader(IntPtr header_ptr, uint CheckSum)
        {
            ChangeEndian();

            header_ptr = Increment(header_ptr, (int)entry.offset);
            font_header.checkSumAdjustment = SwapUInt32(CheckSum);
            Marshal.StructureToPtr(font_header, header_ptr, true);
        }

        public FontHeaderClass(TrueTypeTable src) : base(src) {}
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////
    // PreProgramm table
    /////////////////////////////////////////////////////////////////////////////////////////////////
    class PreProgramClass : TrueTypeTable
    {
        private byte[] program;

        internal override void Load(IntPtr font)
        {
            uint length = ((entry.length + 3) / 4) * 4;
            program = new byte[length];
            IntPtr program_ptr = Increment(font, (int)entry.offset);
            Marshal.Copy(program_ptr, program, 0, program.Length);
        }

        internal override uint Save(IntPtr font, uint offset)
        {
            entry.offset = offset;
            IntPtr program_ptr = Increment(font, (int)entry.offset);
            Marshal.Copy(program, 0, program_ptr, program.Length);
            return offset + (uint)program.Length;
        }

        public PreProgramClass(TrueTypeTable src) : base(src) { }
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////
    // Name table
    /////////////////////////////////////////////////////////////////////////////////////////////////
    class NameTableClass : TrueTypeTable
    {
        #region "Structure dfinition"
        public enum NameID
        {
            CopyrightNotice = 0,
            FamilyName = 1,
            SubFamilyName = 2,
            UniqueID = 3,
            FullName = 4,
            Version = 5,
            PostscriptName = 6,
            Trademark = 7,
            Manufacturer = 8,
            Designer = 9,
            Description = 10,
            URL_Vendor = 11,
            URL_Designer = 12,
            LicenseDescription = 13,
            LicenseInfoURL = 14,
            PreferredFamily = 16,
            PreferredSubFamily = 17,
            CompatibleFull = 18,
            SampleText = 19,
            PostscriptCID = 20,
            WWS_Family_Name = 21,
            WWS_SubFamily_Name = 22
        }


        [StructLayout(LayoutKind.Explicit, Pack = 1)]
        public struct NamingTableHeader
        {
            [FieldOffset(0)]
            public ushort TableVersion;
            [FieldOffset(2)]
            public ushort Count;
            [FieldOffset(4)]
            public ushort stringOffset;
        }

        [StructLayout(LayoutKind.Explicit, Pack = 1)]
        public struct NamingRecord
        {
            [FieldOffset(0)]
            public ushort PlatformID;
            [FieldOffset(2)]
            public ushort EncodingID;
            [FieldOffset(4)]
            public ushort LanguageID;
            [FieldOffset(6)]
            public ushort NameID;
            [FieldOffset(8)]
            public ushort Length;
            [FieldOffset(10)]
            public ushort Offset;
        }
        #endregion

        NamingTableHeader   name_header;
        IntPtr              namerecord_ptr;
        IntPtr              string_storage_ptr;

        private void ChangeEndian()
        {
            name_header.TableVersion = SwapUInt16(name_header.TableVersion);
            name_header.Count = SwapUInt16(name_header.Count);
            name_header.stringOffset = SwapUInt16(name_header.stringOffset);
        }

        internal override void Load(IntPtr font)
        {
            IntPtr nameheader_ptr = Increment(font, (int)entry.offset);
            name_header = (NamingTableHeader)Marshal.PtrToStructure(nameheader_ptr, typeof(NamingTableHeader));

            ChangeEndian();

            namerecord_ptr = Increment(nameheader_ptr, Marshal.SizeOf(name_header));
            string_storage_ptr = Increment(nameheader_ptr, (int)name_header.stringOffset);
        }

//        public string Name(NameID Index) 
        public string this[NameID Index]
        {
            get
            {
                IntPtr record_ptr = namerecord_ptr;

                for (int i = 0; i < name_header.Count; i++)
                {
                    NamingRecord name_rec = (NamingRecord)Marshal.PtrToStructure(record_ptr, typeof(NamingRecord));
                    record_ptr = Increment(record_ptr, Marshal.SizeOf(name_rec));

                    name_rec.PlatformID = SwapUInt16(name_rec.PlatformID);
                    name_rec.EncodingID = SwapUInt16(name_rec.EncodingID);
                    name_rec.LanguageID = SwapUInt16(name_rec.LanguageID);
                    name_rec.NameID = SwapUInt16(name_rec.NameID);
                    name_rec.Length = SwapUInt16(name_rec.Length);
                    name_rec.Offset = SwapUInt16(name_rec.Offset);

                    if (((name_rec.PlatformID == 3 && name_rec.EncodingID == 1) || name_rec.PlatformID == 0) &&
                        (NameID)name_rec.NameID == Index)
                    {
                        byte[] Temp = new byte[name_rec.Length];
                        IntPtr string_ptr = Increment(string_storage_ptr, name_rec.Offset);
                        Marshal.Copy(string_ptr, Temp, 0, (int)Temp.Length);

                        return Encoding.GetEncoding(1201).GetString(Temp);
                    }
                }

                return null;
            }
        }

        public NameTableClass(TrueTypeTable src) : base(src) { }
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////
    // Cmap table
    /////////////////////////////////////////////////////////////////////////////////////////////////
    class CmapTableClass : TrueTypeTable
    {
        #region "Type definition"
        [StructLayout(LayoutKind.Explicit, Pack = 1)]
        public struct Table_CMAP
        {
            [FieldOffset(0)]
            public ushort TableVersion;
            [FieldOffset(2)]
            public ushort NumSubTables;
        }

        [StructLayout(LayoutKind.Explicit, Pack = 1)]
        public struct Table_SUBMAP
        {
            [FieldOffset(0)]
            public ushort Platform;
            [FieldOffset(2)]
            public ushort EncodingID;
            [FieldOffset(4)]
            public uint TableOffset;
        }

        [StructLayout(LayoutKind.Explicit, Pack = 1)]
        public struct Table_Encode
        {
            [FieldOffset(0)]
            public ushort Format;
            [FieldOffset(2)]
            public ushort Length;
            [FieldOffset(4)]
            public ushort Version;
        }

        [StructLayout(LayoutKind.Explicit, Pack = 1)]
        public struct SegmentMapping
        {
            [FieldOffset(0)]
            public ushort segCountX2;       // 2 x segCount.
            [FieldOffset(2)]
            public ushort searchRange;      // 2 x (2**floor(log2(segCount)))
            [FieldOffset(4)]
            public ushort entrySelector;    // log2(searchRange/2)
            [FieldOffset(6)]
            public ushort rangeShift;       // 2 x segCount - searchRange
        }

        public enum EncodingFormats
        {
            ByteEncoding = 0,
            HighByteMapping = 2,
            SegmentMapping = 4,
            TrimmedTable = 6,
        }

        #endregion

        // Platform ID 3 Encoding 1
        int segment_count;
        ushort[] endCount;
        ushort[] startCount;
        short[] idDelta;
        ushort[] idRangeOffset;
        ushort[] GlyphIndexArray;

        private ushort[] LoadCmapSegment(IntPtr segment_ptr, int segment_count, short[] temp_mem)
        {
            ushort[] result = new ushort[segment_count];

            Marshal.Copy(segment_ptr, temp_mem, 0, segment_count);
            for (int i = 0; i < segment_count; i++)
            {
                result[i] = SwapUInt16((ushort)temp_mem[i]);
            }

            return result;
        }

        private short[] LoadSignedCmapSegment(IntPtr segment_ptr, int segment_count, short[] temp_mem)
        {
            short[] result = new short[segment_count];

            Marshal.Copy(segment_ptr, temp_mem, 0, segment_count);
            for (int i = 0; i < segment_count; i++)
            {
                result[i] = SwapInt16(temp_mem[i]);
            }

            return result;
        }

        internal void LoadCmapTable(IntPtr font)
        {
            IntPtr cmap_ptr = Increment(font, (int)entry.offset);
            Table_CMAP cmap = (Table_CMAP)Marshal.PtrToStructure(cmap_ptr, typeof(Table_CMAP));
            int subtables_count = SwapUInt16(cmap.NumSubTables);

            IntPtr submap_ptr = Increment(cmap_ptr, Marshal.SizeOf(cmap));

            for (int j = 0; j < subtables_count; j++)
            {
                Table_SUBMAP submap = (Table_SUBMAP)Marshal.PtrToStructure(submap_ptr, typeof(Table_SUBMAP));
                submap_ptr = Increment(submap_ptr, Marshal.SizeOf(submap));

                submap.Platform = SwapUInt16(submap.Platform);
                submap.EncodingID = SwapUInt16(submap.EncodingID);
                submap.TableOffset = SwapUInt32(submap.TableOffset);

                // Skip non microsft unicode charmaps
                if (submap.Platform != 3 || submap.EncodingID != 1) continue;

                IntPtr encode_ptr = Increment(cmap_ptr, (int)submap.TableOffset);
                Table_Encode encode = (Table_Encode)Marshal.PtrToStructure(encode_ptr, typeof(Table_Encode));

                encode.Format = SwapUInt16(encode.Format);
                encode.Length = SwapUInt16(encode.Length);
                encode.Version = SwapUInt16(encode.Version);

                switch ((EncodingFormats)encode.Format)
                {
                    case EncodingFormats.ByteEncoding:
                        throw new Exception("TO DO: ByteEncoding");
                        //break;

                    case EncodingFormats.HighByteMapping:
                        throw new Exception("TO DO: HighByteMapping");
                        //break;

                    case EncodingFormats.SegmentMapping:
                        encode_ptr = Increment(encode_ptr, Marshal.SizeOf(encode));
                        SegmentMapping segment = (SegmentMapping)Marshal.PtrToStructure(encode_ptr, typeof(SegmentMapping));
                        segment.segCountX2 = SwapUInt16(segment.segCountX2);        // 2 x segCount.
                        segment.searchRange = SwapUInt16(segment.searchRange);      // 2 x (2**floor(log2(segCount)))
                        segment.entrySelector = SwapUInt16(segment.entrySelector);  // log2(searchRange/2)
                        segment.rangeShift = SwapUInt16(segment.rangeShift);        // 2 x segCount - searchRange

                        segment_count = segment.segCountX2 / 2;

                        short[] toFix = new short[segment_count];
                        encode_ptr = Increment(encode_ptr, Marshal.SizeOf(segment));
                        endCount = LoadCmapSegment(encode_ptr, segment_count, toFix);
                        encode_ptr = Increment(encode_ptr, segment.segCountX2 + sizeof(ushort));
                        startCount = LoadCmapSegment(encode_ptr, segment_count, toFix);
                        encode_ptr = Increment(encode_ptr, segment.segCountX2);
                        idDelta = LoadSignedCmapSegment(encode_ptr, segment_count, toFix);
                        encode_ptr = Increment(encode_ptr, segment.segCountX2);
                        idRangeOffset = LoadCmapSegment(encode_ptr, segment_count, toFix);
                        toFix = null;

                        // Just added
                        uint index_array_size = (8 + 4 * (uint) segment_count) * 2;
                        index_array_size = (this.length - index_array_size) / 2;
                        encode_ptr = Increment(encode_ptr, segment.segCountX2);
                        toFix = new short[index_array_size];
                        GlyphIndexArray = LoadCmapSegment(encode_ptr, (int)index_array_size, toFix);
                        toFix = null;

#if false
    string[] debug = new string[segment_count];
    for (int z = 0; z < segment_count; z++)
    { 
        debug[z] = ""+(char)startCount[z]+" - "+(char)endCount[z] +" = " + idDelta[z].ToString() + " & " + idRangeOffset[z].ToString();
    }
#endif

        break;

                    case EncodingFormats.TrimmedTable:
                        throw new Exception("TO DO: TrimmedTable");
                        //break;
                }
            }
        }

        internal ushort GetGlyphIndex(ushort ch)
        {
            ushort GlyphIDX = 0;
            for (int i = 0; i < segment_count; i++)
            {
                if (endCount[i] >= ch)
                {
                    if (startCount[i] <= ch)
                    {
                        if (idRangeOffset[i] == 0)
                        {
                            GlyphIDX = (ushort) ((ch + idDelta[i]) % 65536);
                        }
                        else
                        {
                            int j = (ushort)(idRangeOffset[i] / 2 + (ch - startCount[i]) - (segment_count - i));
                            GlyphIDX = this.GlyphIndexArray[j];
                        }
                    }
                    break;
                }
            }
            return GlyphIDX;
        }

        public CmapTableClass(TrueTypeTable src) : base(src) { }
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////
    // IndexToLocation table
    /////////////////////////////////////////////////////////////////////////////////////////////////
    class IndexToLocationClass : TrueTypeTable
    {
        // Используем только одну таблицу в зависимости от типа таблицы
        private ushort[] ShortIndexToLocation = null;
        private uint[] LongIndexToLocation = null;

        internal ushort[] Short { get { return ShortIndexToLocation; } }
        internal uint[] Long { get { return LongIndexToLocation; } }

        internal void LoadIndexToLocation(IntPtr font, FontHeaderClass font_header)
        {
            int count;
            IntPtr i2l_ptr = Increment(font, (int)(entry.offset));
            switch (font_header.indexToLocFormat)
            {
                case FontHeaderClass.IndexToLoc.ShortType:
                    count = (int)entry.length / 2;
                    short[] ShortTemp = new short[count];
                    Marshal.Copy(i2l_ptr, ShortTemp, 0, count);
                    ShortIndexToLocation = new ushort[count];
                    for (int i = 0; i < count; i++)
                    {
                        ShortIndexToLocation[i] = SwapUInt16((ushort)ShortTemp[i]);
                    }
                    ShortTemp = null;
                    break;

                case FontHeaderClass.IndexToLoc.LongType:
                    count = (int)entry.length / 4;
                    int[] LongTemp = new int[count];
                    Marshal.Copy(i2l_ptr, LongTemp, 0, count);
                    LongIndexToLocation = new uint[count];
                    for (int i = 0; i < count; i++)
                    {
                        LongIndexToLocation[i] = SwapUInt32((uint)LongTemp[i]);
                    }
                    LongTemp = null;
                    break;

                default:
                    throw new Exception("Unsupported Index to Location format");
            }
        }

        internal ushort GetGlyph(ushort i2l_idx, FontHeaderClass font_header, out uint location)
        {
            location = 0;
            ushort length = 0;

            switch (font_header.indexToLocFormat)
            {
                case FontHeaderClass.IndexToLoc.ShortType:
                    location = (uint)(2 * ShortIndexToLocation[i2l_idx]);
                    length = (ushort)(2 * (ShortIndexToLocation[i2l_idx + 1] - ShortIndexToLocation[i2l_idx]));
                    break;

                case FontHeaderClass.IndexToLoc.LongType:
                    location = LongIndexToLocation[i2l_idx];
                    length = (ushort)(LongIndexToLocation[i2l_idx + 1] - LongIndexToLocation[i2l_idx]);
                    break;

            }
            return length;
        }

        public IndexToLocationClass(TrueTypeTable src) : base(src) { }
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////
    // MaximumProfile table
    /////////////////////////////////////////////////////////////////////////////////////////////////
    class MaximumProfileClass : TrueTypeTable
    {
        #region "Structure definition"
        [StructLayout(LayoutKind.Explicit, Pack = 1)]
        public struct MaximumProfile
        {
            [FieldOffset(0)]
            public uint Version; // version number	0x00010000 for version 1.0.
            [FieldOffset(4)]
            public ushort numGlyphs; //	The number of glyphs in the font.
            [FieldOffset(6)]
            public ushort maxPoints; //	Maximum points in a non-composite glyph.
            [FieldOffset(8)]
            public ushort maxContours; //	Maximum contours in a non-composite glyph.
            [FieldOffset(10)]
            public ushort maxCompositePoints; //	Maximum points in a composite glyph.
            [FieldOffset(12)]
            public ushort maxCompositeContours; //	Maximum contours in a composite glyph.
            [FieldOffset(14)]
            public ushort maxZones; //	1 if instructions do not use the twilight zone (Z0), or 2 if instructions do use Z0; should be set to 2 in most cases.
            [FieldOffset(16)]
            public ushort maxTwilightPoints; //	Maximum points used in Z0.
            [FieldOffset(18)]
            public ushort maxStorage; //	Number of Storage Area locations. 
            [FieldOffset(20)]
            public ushort maxFunctionDefs; //	Number of FDEFs.
            [FieldOffset(22)]
            public ushort maxInstructionDefs; //	Number of IDEFs.
            [FieldOffset(24)]
            public ushort maxStackElements; //	Maximum stack depth .
            [FieldOffset(26)]
            public ushort maxSizeOfInstructions; //	Maximum byte count for glyph instructions.
            [FieldOffset(28)]
            public ushort maxComponentElements; //	Maximum number of components referenced at “top level” for any composite glyph.
            [FieldOffset(30)]
            public ushort maxComponentDepth; //	Maximum levels of recursion; 1 for simple components.
        }
        #endregion

        private MaximumProfile profile;

        private void ChangeEndian()
        {
            profile.Version = SwapUInt32(profile.Version);
            profile.numGlyphs = SwapUInt16(profile.numGlyphs);
            profile.maxPoints = SwapUInt16(profile.maxPoints);
            profile.maxContours = SwapUInt16(profile.maxContours);
            profile.maxCompositePoints = SwapUInt16(profile.maxCompositePoints);
            profile.maxCompositeContours = SwapUInt16(profile.maxCompositeContours);
            profile.maxZones = SwapUInt16(profile.maxZones);
            profile.maxTwilightPoints = SwapUInt16(profile.maxTwilightPoints);
            profile.maxStorage = SwapUInt16(profile.maxStorage);
            profile.maxFunctionDefs = SwapUInt16(profile.maxFunctionDefs);
            profile.maxInstructionDefs = SwapUInt16(profile.maxInstructionDefs);
            profile.maxStackElements = SwapUInt16(profile.maxStackElements);
            profile.maxSizeOfInstructions = SwapUInt16(profile.maxSizeOfInstructions);
            profile.maxComponentElements = SwapUInt16(profile.maxComponentElements);
            profile.maxComponentDepth = SwapUInt16(profile.maxComponentDepth);
        }

        internal override void Load(IntPtr font)
        {
            IntPtr mprofile_ptr = Increment(font, (int)entry.offset);
            profile = (MaximumProfile)Marshal.PtrToStructure(mprofile_ptr, typeof(MaximumProfile));
            ChangeEndian();
        }

        internal override uint Save(IntPtr font, uint offset)
        {
            entry.offset = offset;

            ChangeEndian();
            IntPtr profile_ptr = Increment(font, (int)entry.offset);
            Marshal.StructureToPtr(profile, profile_ptr, false);
            ChangeEndian();

            return offset + (uint)entry.length;
        }

        public MaximumProfileClass(TrueTypeTable src) : base(src) { }
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////
    // GlyphSubstitution table
    /////////////////////////////////////////////////////////////////////////////////////////////////
    class GlyphSubstitutionClass : TrueTypeTable
    {
        #region "Structure definition"
        [StructLayout(LayoutKind.Explicit, Pack = 1)]
        public struct GSUB_Header
        {
            [FieldOffset(0)]
            public uint  	Version; //  	Version of the GSUB table-initially set to 0x00010000
            [FieldOffset(4)]
            public ushort ScriptList; // 	Offset to ScriptList table-from beginning of GSUB table
            [FieldOffset(6)]
            public ushort FeatureList; // 	Offset to FeatureList table-from beginning of GSUB table
            [FieldOffset(8)]
            public ushort LookupList; // 	Offset to LookupList table-from beginning of GSUB table
        }

        [StructLayout(LayoutKind.Explicit, Pack = 1)]
        public struct ScriptListTable
        {
            [FieldOffset(0)]
            public ushort CountScripts; //  	Count of ScriptListRecord
        }

        [StructLayout(LayoutKind.Explicit, Pack = 1)]
        public struct ScriptListRecord
        {
            [FieldOffset(0)]
            public uint  	ScriptTag; //  	4-byte ScriptTag identifier
            [FieldOffset(4)]
            public ushort ScriptOffset; // 	Offset to Script table-from beginning of ScriptList        
        }

        [StructLayout(LayoutKind.Explicit, Pack = 1)]
        public struct ScriptTable
        {
            [FieldOffset(0)]
            public ushort DefaultLangSys; //  	Offset to DefaultLangSys table-from beginning of Script table-may be NULL
            [FieldOffset(2)]
            public ushort LangSysCount;   // 	Number of LangSysRecords for this script-excluding the DefaultLangSys
        }

        [StructLayout(LayoutKind.Explicit, Pack = 1)]
        public struct LangSysRecord
        {
            [FieldOffset(0)]
            public uint LangSysTag; //  	4-byte LangSysTag identifier
            [FieldOffset(4)]
            public ushort LangSys;    // 	Offset to LangSys table-from beginning of Script table        
        }

        [StructLayout(LayoutKind.Explicit, Pack = 1)]
        public struct LangSysTable
        {
            [FieldOffset(0)]
            public ushort LookupOrder; //  	= NULL (reserved for an offset to a reordering table)
            [FieldOffset(2)]
            public ushort ReqFeatureIndex; // 	Index of a feature required for this language system- if no required features = 0xFFFF
            [FieldOffset(4)]
            public ushort FeatureCount; // 	Number of FeatureIndex values for this language system-excludes the required feature    
        }

        // Related to feature table

        [StructLayout(LayoutKind.Explicit, Pack = 1)]
        public struct FeatureRecord
        {
            [FieldOffset(0)]
            public uint FeatureTag; //  	4-byte feature identification tag
            [FieldOffset(4)]
            public ushort Feature; // 	Offset to Feature table-from beginning of FeatureList        
        }

        #endregion

        private GSUB_Header header;
        private IntPtr gsub_ptr;

        private void LoadFeatureList(uint feature_idx)
        {
            IntPtr feature_list_table_ptr = Increment(gsub_ptr, (int)header.FeatureList);
            ushort feature_count = SwapUInt16((ushort)Marshal.PtrToStructure(feature_list_table_ptr, typeof(ushort)));
            if (feature_idx >= feature_count) throw new Exception("Feature index out of bound");

            IntPtr feature_record_ptr = Increment(feature_list_table_ptr, (int)(sizeof(ushort) + feature_idx * 6));
            FeatureRecord feature_record = (FeatureRecord)Marshal.PtrToStructure(feature_record_ptr, typeof(FeatureRecord));

            string FeatureTag = "" +
                    (char)(0xff & feature_record.FeatureTag) +
                    (char)(0xff & (feature_record.FeatureTag >> 8)) +
                    (char)(0xff & (feature_record.FeatureTag >> 16)) +
                    (char)(0xff & (feature_record.FeatureTag >> 24));
        }

        private void LoadScriptList()
        {
            IntPtr script_list_table_ptr = Increment(gsub_ptr, (int)header.ScriptList);
            ScriptListTable script_list_table = (ScriptListTable)Marshal.PtrToStructure(script_list_table_ptr, typeof(ScriptListTable));

            script_list_table.CountScripts = SwapUInt16(script_list_table.CountScripts);

            IntPtr script_record_ptr = Increment(script_list_table_ptr, Marshal.SizeOf(script_list_table));

            for (int i = 0; i < script_list_table.CountScripts; i++)
            {
                ScriptListRecord script_record = (ScriptListRecord)Marshal.PtrToStructure(script_record_ptr, typeof(ScriptListRecord));
                script_record.ScriptOffset = SwapUInt16(script_record.ScriptOffset);

                string ScriptTag = "" +
                        (char)(0xff & script_record.ScriptTag) +
                        (char)(0xff & (script_record.ScriptTag >> 8)) +
                        (char)(0xff & (script_record.ScriptTag >> 16)) +
                        (char)(0xff & (script_record.ScriptTag >> 24));

                IntPtr script_table_ptr = Increment(script_list_table_ptr, script_record.ScriptOffset);
                ScriptTable script_table = (ScriptTable)Marshal.PtrToStructure(script_table_ptr, typeof(ScriptTable));

                script_table.DefaultLangSys = SwapUInt16(script_table.DefaultLangSys);
                script_table.LangSysCount = SwapUInt16(script_table.LangSysCount);

                IntPtr lang_sys_rec_ptr;

                if (script_table.DefaultLangSys != 0)
                {
                    lang_sys_rec_ptr = Increment(script_table_ptr, script_table.DefaultLangSys);

                    LangSysTable lang_sys_table = (LangSysTable)Marshal.PtrToStructure(lang_sys_rec_ptr, typeof(LangSysTable));
                    lang_sys_table.LookupOrder = SwapUInt16(lang_sys_table.LookupOrder);
                    lang_sys_table.ReqFeatureIndex = SwapUInt16(lang_sys_table.ReqFeatureIndex);
                    lang_sys_table.FeatureCount = SwapUInt16(lang_sys_table.FeatureCount);

                    IntPtr feature_index_ptr = Increment(lang_sys_rec_ptr, Marshal.SizeOf(lang_sys_table));
                    for (int k = 0; k < lang_sys_table.FeatureCount; k++)
                    {
                        ushort feature_idx = SwapUInt16((ushort)Marshal.PtrToStructure(feature_index_ptr, typeof(ushort)));

                        LoadFeatureList(feature_idx);

                        feature_index_ptr = Increment(feature_index_ptr, sizeof(ushort));
                    }
                }

                lang_sys_rec_ptr = Increment(script_table_ptr, Marshal.SizeOf(script_table));
                for (int j = 0; j < script_table.LangSysCount; j++)
                {
                    LangSysRecord lang_sys_rec = (LangSysRecord)Marshal.PtrToStructure(lang_sys_rec_ptr, typeof(LangSysRecord));
                    lang_sys_rec.LangSys = SwapUInt16(lang_sys_rec.LangSys);
                    //lang_sys_rec.LangSysTag = SwapUInt32(lang_sys_rec.LangSysTag);

                    string LangSysTag = "" +
                        (char)(0xff & lang_sys_rec.LangSysTag) +
                        (char)(0xff & (lang_sys_rec.LangSysTag >> 8)) +
                        (char)(0xff & (lang_sys_rec.LangSysTag >> 16)) +
                        (char)(0xff & (lang_sys_rec.LangSysTag >> 24));


                    lang_sys_rec_ptr = Increment(lang_sys_rec_ptr, Marshal.SizeOf(lang_sys_rec));
                }

                script_record_ptr = Increment(script_record_ptr, Marshal.SizeOf(script_record));
            }
        }
        
        internal override void Load(IntPtr font)
        {
            gsub_ptr = Increment(font, (int)entry.offset);
            header = (GSUB_Header)Marshal.PtrToStructure(gsub_ptr, typeof(GSUB_Header));
            ChangeEndian();

//            LoadScriptList();
        }

        internal override uint Save(IntPtr font, uint offset)
        {
            return base.Save(font, offset);
        }

        private void ChangeEndian()
        {
            header.Version = SwapUInt32(header.Version);
            header.ScriptList = SwapUInt16(header.ScriptList);
            header.LookupList = SwapUInt16(header.LookupList);
            header.FeatureList = SwapUInt16(header.FeatureList);
        }

        public GlyphSubstitutionClass(TrueTypeTable src) : base(src) { }
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////
    // Glyph table
    /////////////////////////////////////////////////////////////////////////////////////////////////
    class GlyphTableClass : TrueTypeTable
    {
        #region "Type definitions"
        [StructLayout(LayoutKind.Explicit, Pack = 1)]
        public struct GlyphHeader
        {
            [FieldOffset(0)]
            public short numberOfContours;
            [FieldOffset(2)]
            public short xMin;
            [FieldOffset(4)]
            public short yMin;
            [FieldOffset(6)]
            public short xMax;
            [FieldOffset(8)]
            public short yMax;
        }

        [StructLayout(LayoutKind.Explicit, Pack = 1)]
        public struct CompositeGlyphHeader
        {
            [FieldOffset(0)]
            public ushort flags;        //  	component flag
            [FieldOffset(2)]
            public ushort glyphIndex;   // 	glyph index of component        
        }

        public enum CompositeFlags
        {
            ARG_1_AND_2_ARE_WORDS = 0x0001, 	// If this is set, the arguments are words; otherwise, they are bytes.
            ARGS_ARE_XY_VALUES = 0x0002, 	    // If this is set, the arguments are xy values; otherwise, they are points.
            ROUND_XY_TO_GRID = 0x0004, 	        // For the xy values if the preceding is true.
            WE_HAVE_A_SCALE = 0x0008, 	        // This indicates that there is a simple scale for the component. Otherwise, scale = 1.0.
            RESERVED = 0x0010, 	                // This bit is reserved. Set it to 0.
            MORE_COMPONENTS = 0x0020, 	        // Indicates at least one more glyph after this one.
            WE_HAVE_AN_X_AND_Y_SCALE = 0x0040, 	// The x direction will use a different scale from the y direction.
            WE_HAVE_A_TWO_BY_TWO = 0x0080, 	    // There is a 2 by 2 transformation that will be used to scale the component.
            WE_HAVE_INSTRUCTIONS = 0x0100, 	    // Following the last component are instructions for the composite character.
            USE_MY_METRICS = 0x0200, 	        // If set, this forces the aw and lsb (and rsb) for the composite to be equal to those from this original glyph. This works for hinted and unhinted characters.
            OVERLAP_COMPOUND = 0x0400, 	        // Used by Apple in GX fonts.
            SCALED_COMPONENT_OFFSET = 0x0800,   // Composite designed to have the component offset scaled (designed for Apple rasterizer).
            UNSCALED_COMPONENT_OFFSET = 0x10000 // Composite designed not to have the component offset scaled (designed for the Microsoft TrueType rasterizer).        
        }

        #endregion

        IntPtr glyph_table_ptr;

        internal override void Load(IntPtr font)
        {
            glyph_table_ptr = Increment(font, (int)(entry.offset));
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////
        // Вернуть идексы для композитного глифа или пустую таблицу если глиф не композитный
        /////////////////////////////////////////////////////////////////////////////////////////////////
        internal ArrayList CheckGlyph(int glyph_offset, int glyph_size)
        {
            ArrayList CompositeIndexes = new ArrayList();

            IntPtr glyph_ptr = Increment(glyph_table_ptr, glyph_offset );
            GlyphHeader gheader = (GlyphHeader)Marshal.PtrToStructure(glyph_ptr, typeof(GlyphHeader));

            gheader.numberOfContours = SwapInt16(gheader.numberOfContours);
            gheader.xMax = SwapInt16(gheader.xMax);
            gheader.yMax = SwapInt16(gheader.yMax);
            gheader.xMin = SwapInt16(gheader.xMin);
            gheader.yMin = SwapInt16(gheader.yMin);

            if (gheader.numberOfContours < 0)
            {
                CompositeGlyphHeader cgh;
                IntPtr composite_header_ptr = Increment(glyph_ptr, Marshal.SizeOf(gheader));
                do 
                {
                    cgh = (CompositeGlyphHeader)Marshal.PtrToStructure(composite_header_ptr, typeof(CompositeGlyphHeader));
                    cgh.flags = SwapUInt16(cgh.flags);
                    cgh.glyphIndex = SwapUInt16(cgh.glyphIndex);

                    // Add glyph inex to list of composite indexes
                    CompositeIndexes.Add(cgh.glyphIndex);

                    composite_header_ptr = Increment(composite_header_ptr, Marshal.SizeOf(cgh) );

                    if( (cgh.flags & (ushort) CompositeFlags.ARG_1_AND_2_ARE_WORDS) != 0 )
                    {
                        composite_header_ptr = Increment(composite_header_ptr, 4);
                    }
                    else
                    {
                        composite_header_ptr = Increment(composite_header_ptr, 2);
                    }

                    if ((cgh.flags & (ushort) CompositeFlags.WE_HAVE_A_SCALE) != 0)
                    {
                        composite_header_ptr = Increment(composite_header_ptr, 2);
                        //F2Dot14 scale;    /* Format 2.14 */
                    }
                    else if ((cgh.flags & (ushort) CompositeFlags.WE_HAVE_AN_X_AND_Y_SCALE) != 0)
                    {
                        composite_header_ptr = Increment(composite_header_ptr, 4);
                        //F2Dot14 xscale;    /* Format 2.14 */
                        //F2Dot14 yscale;    /* Format 2.14 */
                    }
                    else if ((cgh.flags & (ushort)CompositeFlags.WE_HAVE_A_TWO_BY_TWO) != 0)
                    {
                        composite_header_ptr = Increment(composite_header_ptr, 8);
                        //F2Dot14 xscale;    /* Format 2.14 */
                        //F2Dot14 scale01;   /* Format 2.14 */
                        //F2Dot14 scale10;   /* Format 2.14 */
                        //F2Dot14 yscale;    /* Format 2.14 */
                    }

                }
                while( (cgh.flags & (ushort) CompositeFlags.MORE_COMPONENTS) != 0 );

                if ((cgh.flags & (ushort)CompositeFlags.WE_HAVE_INSTRUCTIONS) != 0)
                {
                    ushort num_instr = SwapUInt16((ushort)Marshal.PtrToStructure(composite_header_ptr, typeof(ushort)));
                    composite_header_ptr = Increment(composite_header_ptr, 2 + num_instr);
                    //USHORT numInstr
                    //BYTE instr[numInstr]
                }
            }
            else
            {
                ; // Simple glyph
            }
            return CompositeIndexes;
        }

        private IntPtr ReadRawByte(IntPtr ptr, out byte val)
        {
            val = (byte)Marshal.ReadByte(ptr);
            return Increment(ptr, 1);

        }

        public enum GlyphFlags
        {
            ON_CURVE = 0x01, //  	If set, the point is on the curve; otherwise, it is off the curve.
            X_SHORT = 0x02, // 	If set, the corresponding x-coordinate is 1 byte long. If not set, 2 bytes.
            Y_SHORT = 0x04, // 	If set, the corresponding y-coordinate is 1 byte long. If not set, 2 bytes.
            REPEAT = 0x08, //    If set, the next byte specifies the number of additional times this set of flags is to be repeated. In this way, the number of flags listed can be smaller than the number of points in a character.
            X_SAME = 0x10, //    This flag has two meanings, depending on how the x-Short Vector flag is set. If x-Short Vector is set, this bit describes the sign of the value, with 1 equalling positive and 0 negative. If the x-Short Vector bit is not set and this bit is set, then the current x-coordinate is the same as the previous x-coordinate. If the x-Short Vector bit is not set and this bit is also not set, the current x-coordinate is a signed 16-bit delta vector.
            Y_SAME = 0x20,  //    This flag has two meanings, depending on how the y-Short Vector flag is set. If y-Short Vector is set, this bit describes the sign of the value, with 1 equalling positive and 0 negative. If the y-Short Vector bit is not set and this bit is set, then the current y-coordinate is the same as the previous y-coordinate. If the y-Short Vector bit is not set and this bit is also not set, the current y-coordinate is a signed 16-bit delta vector.        }
            X_POSITIVE = 0x10,
	        Y_POSITIVE = 0x20
        }

        internal class GlyphPoint
        {
            public float    x;
            public float    y;
            public bool     on_curve;
            public bool     end_of_contour;

            public PointF Point { get { return new PointF(x,y); } }
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////
        // Произвести декодирование глифа
        /////////////////////////////////////////////////////////////////////////////////////////////////
        internal GraphicsPath GetGlyph(
            int glyph_offset, 
            int glyph_data_size, 
            float font_rsize, 
            Point position, 
            out GlyphHeader gheader)
        {

            IntPtr glyph_ptr = Increment(glyph_table_ptr, glyph_offset);
            gheader = (GlyphHeader)Marshal.PtrToStructure(glyph_ptr, typeof(GlyphHeader));

            gheader.numberOfContours = SwapInt16(gheader.numberOfContours);
            gheader.xMax = SwapInt16(gheader.xMax);
            gheader.yMax = SwapInt16(gheader.yMax);
            gheader.xMin = SwapInt16(gheader.xMin);
            gheader.yMin = SwapInt16(gheader.yMin);

            ushort instructions_count;
            ushort[] endPtsOfContours = new ushort[gheader.numberOfContours];
            IntPtr ptr = Increment(glyph_ptr, Marshal.SizeOf(gheader));

            for (int i = 0; i < endPtsOfContours.Length; i++)
            {
                endPtsOfContours[i] = SwapUInt16((ushort)Marshal.PtrToStructure(ptr, typeof(ushort)));
                ptr = Increment(ptr, sizeof(ushort));
            }
            instructions_count = SwapUInt16((ushort)Marshal.PtrToStructure(ptr, typeof(ushort)));
            ptr = Increment(ptr, sizeof(ushort));

            byte[] instructions = new byte[instructions_count];
            Marshal.Copy(ptr, instructions, 0, (int)instructions.Length);
            ptr = Increment(ptr, instructions.Length);

            int number_of_points = endPtsOfContours[endPtsOfContours.Length - 1] + 1;

            byte[] flags = new byte[number_of_points];
            GlyphPoint[] points = new GlyphPoint[number_of_points];

            byte repeatCount = 0;
            byte repeatFlag = 0;

            for (int i = 0; i < number_of_points; i++) 
            {
                if (repeatCount > 0) 
                {
                     flags[i] = repeatFlag;
                     repeatCount--;
                } 
                else 
                {
                    ptr = ReadRawByte(ptr, out flags[i]);
                    if ((flags[i] & (byte) GlyphFlags.REPEAT) != 0) 
                    {
                        ptr = ReadRawByte(ptr, out repeatCount);
                        repeatFlag = flags[i];
                    }
                }

                points[i] = new GlyphPoint();
                points[i].on_curve  = (flags[i] & (byte)GlyphFlags.ON_CURVE) != 0;
            }

            for (int i = 0; i < endPtsOfContours.Length; i++)
            {
                points[endPtsOfContours[i]].end_of_contour = true;
            }

            short last = 0;
            for (int i = 0; i < number_of_points; i++)
            {
                byte val;
                bool sign = (flags[i] & (byte)GlyphFlags.X_POSITIVE) != 0;

                if ((flags[i] & (byte) GlyphFlags.X_SHORT) != 0)
                {
                    val = Marshal.ReadByte(ptr);
                    ptr = Increment(ptr, 1);
                    last += (short)(sign ? val : -val);
                }
                else
                {
                    if (!sign)
                    {
                        last += SwapInt16( Marshal.ReadInt16(ptr) );
                        ptr = Increment(ptr, 2);
                    }
                }
                points[i].x = last / font_rsize;
            }

            last = 0;
            for (int i = 0; i < number_of_points; i++)
            {
                byte val;
                bool sign = (flags[i] & (byte)GlyphFlags.Y_POSITIVE) != 0;

                if ((flags[i] & (byte)GlyphFlags.Y_SHORT) != 0)
                {
                    val = Marshal.ReadByte(ptr);
                    ptr = Increment(ptr, 1);
                    last += (short)(sign ? val : -val);
                }
                else
                {
                    if (!sign)
                    {
                        last += SwapInt16(Marshal.ReadInt16(ptr));
                        ptr = Increment(ptr, 2);
                    }
                }
                points[i].y = last / font_rsize;
            }

            // Приготовить GraphicsPath для выбранного символа
            bool start_new_contour = true;
            int idx = 0;
            GraphicsPath path = new GraphicsPath(FillMode.Winding);
            GlyphPoint first_point;

            first_point = points[idx];

#if true
            start_new_contour = true;
            PointF beg, first, end, next, implied;
            bool curent_on_curve, next_on_curve;

            first = beg = new PointF(points[0].Point.X + position.X, position.Y - points[0].Point.Y);

            for (idx = 0; idx < points.Length; idx++)
            {
                curent_on_curve = points[idx].on_curve;
                if (idx + 1 < points.Length)
                {
                    next = new PointF(points[idx + 1].Point.X + position.X, position.Y - points[idx + 1].Point.Y);
                    next_on_curve = points[idx + 1].on_curve;
                }
                else
                {
                    next = new PointF(points[0].Point.X + position.X, position.Y - points[0].Point.Y);
                    next_on_curve = points[0].on_curve;
                }

                if (start_new_contour == true)
                {
                    path.StartFigure();
                    first = beg;
                    start_new_contour = false;
                }

                if (points[idx].end_of_contour)
                {
                    start_new_contour = true;
                    //path.CloseFigure();

                    implied = new PointF(points[idx].Point.X + position.X, position.Y - points[idx].Point.Y);
                    end = first;
                    if (curent_on_curve)
                    {
                        //end = next;
                        path.AddLine(beg, end);
                    }
                    else
                    {
                        AddSpline(path, beg, implied, end, position);
                    }
                    beg = next;
                    continue;
                }

                ////////////////////////////////////////////////////////////////////
                if (curent_on_curve) // текущая точка на линии
                {
                    if (next_on_curve) // следующая точка на линии = прямая
                    {
                        end = next;
                        path.AddLine(beg, end);
                        beg = end;
                    }
                    else
                    {
                        // ничего не делаем, всё произойдёт дальше. Хотя...
                    }
                }
                else // Точка искривления
                {
                    implied = new PointF(points[idx].Point.X + position.X, position.Y - points[idx].Point.Y);
                    if (next_on_curve) // следующая точка на линии = рисовать кривую
                    {
                        end = next;
                        AddSpline(path, beg, implied, end, position);
                        beg = end;
                    }
                    else
                    {
                        float X = position.X + ((points[idx + 1].x - points[idx].x) / 2) + points[idx].x;
                        float Y = position.Y - (((points[idx + 1].y - points[idx].y) / 2) + points[idx].y);
                        end = new PointF(X, Y);
                        AddSpline(path, beg, implied, end, position);
                        beg = end;
                    }
                }
            }
            
            
                ////////////////////////////////////////////////////////////////////
#if false
            Font f = new Font("Arial", 8, FontStyle.Regular);
            int t = 1;

            for (idx = 0; idx < points.Length; idx++)
            {
                RectangleF r = new RectangleF(points[idx].x + 6, position.Y-6 - points[idx].y, 5, 5);

                if (points[idx].on_curve)
                {
                    path.AddRectangle(r);
                }
                else
                {
                    path.AddArc(r, 0, 180);
                }

                path.AddString(t.ToString(), f.FontFamily, (int)FontStyle.Regular, 12, new PointF(r.X, r.Y + 10), StringFormat.GenericDefault);
                t++;

                if (idx + 1 < points.Length && !points[idx].on_curve && !points[idx + 1].on_curve)
                {
                    path.StartFigure();
                    float X = ((points[idx + 1].x - points[idx].x) / 2) + points[idx].x - 6;
                    float Y = position.Y - (((points[idx + 1].y - points[idx].y) / 2) + points[idx].y) - 6;
                    path.AddArc(X, Y, 8, 7, 0, 360);
                    path.CloseFigure();

                    path.StartFigure();
                    path.AddString(t.ToString(), f.FontFamily, (int)FontStyle.Regular, 12, new PointF(X, Y + 10), StringFormat.GenericDefault);
                    path.CloseFigure();
                    t++;
                }
            }
#endif

#endif
            return path;
        }

        private void AddSpline(GraphicsPath path, PointF pntStart, PointF pntB, PointF pntEnd, Point position)
        { 
            // Start and end points are unmodified.
            PointF pnt1 = pntStart;        // Первая контрольная точка Безье
            pnt1.X += (2.0f / 3.0f) * (pntB.X - pntStart.X);
            pnt1.Y += (2.0f / 3.0f) * (pntB.Y - pntStart.Y);

            PointF pnt2 = pntB;            // Вторая контрольная точка Безье
            pnt2.X += (pntEnd.X - pntB.X) / 3.0f;
            pnt2.Y += (pntEnd.Y - pntB.Y) / 3.0f;

            path.AddBezier(pntStart, pnt1, pnt2, pntEnd);
        }

        public GlyphTableClass(TrueTypeTable src) : base(src) { }
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////
    // True Type Font
    /////////////////////////////////////////////////////////////////////////////////////////////////
    class TrueTypeFont : TTF_Helpers
    {
        #region Structures

        [StructLayout(LayoutKind.Explicit, Pack = 1)]
        public struct TableDirectory
        {
            [FieldOffset(0)]
            public uint sfntversion;     // 4 bytes
            [FieldOffset(4)]
            public ushort numTables;       // 2 bytes
            [FieldOffset(6)]
            public ushort searchRange;    // 2 bytes
            [FieldOffset(8)]
            public ushort entrySelector;  // 1 byte
            [FieldOffset(10)]
            public ushort rangeShift;
        }

        [StructLayout(LayoutKind.Explicit, Pack = 1)]
        public struct HorizontalHeader
        {
            [FieldOffset(0)]
            public uint Version; // version number	0x00010000 for version 1.0.
            [FieldOffset(4)]
            public short Ascender; // Typographic ascent.
            [FieldOffset(6)]
            public short Descender; // Typographic descent.
            [FieldOffset(8)]
            public short LineGap; //	Typographic line gap. Negative LineGap values are treated as zero in Windows 3.1, System 6, and System 7.
            [FieldOffset(10)]
            public ushort advanceWidthMax; //	Maximum advance width value in ‘hmtx’ table.
            [FieldOffset(12)]
            public short minLeftSideBearing; // Minimum left sidebearing value in ‘hmtx’ table.
            [FieldOffset(14)]
            public short minRightSideBearing; // Minimum right sidebearing value; calculated as Min(aw - lsb - (xMax - xMin)).
            [FieldOffset(16)]
            public short xMaxExtent;     //  Max(lsb + (xMax - xMin)).
            [FieldOffset(18)]
            public short caretSlopeRise; // Used to calculate the slope of the cursor (rise/run); 1 for vertical.
            [FieldOffset(20)]
            public short caretSlopeRun;  // 0 for vertical.
            [FieldOffset(22)]
            public short reserved1;  // set to 0
            [FieldOffset(24)]
            public short reserved2;  // set to 0
            [FieldOffset(26)]
            public short reserved3;  // set to 0
            [FieldOffset(28)]
            public short reserved4;  // set to 0
            [FieldOffset(30)]
            public short reserved5;  // set to 0
            [FieldOffset(32)]
            public short metricDataFormat; //	0 for current format.
            [FieldOffset(34)]
            public ushort numberOfHMetrics; //	Number of hMetric entries in  ‘hmtx’ table; may be smaller than the total number of glyphs in the font.
        }

        public enum TablesID
        {
            FontHeader = 0x64616568,
            CMAP = 0x70616D63,
            Glyph = 0x66796c67,
            IndexToLocation = 0x61636f6c,
            MaximumProfile = 0x7078616d,
            HorizontalHeader = 0x61656868,
            HorizontalMetrix = 0x78746d68,
            OS2Table = 0x322f534f,
            Name = 0x656d616e,
            Postscript = 0x74736f70,

            GlyphSubstitution = 0x42555347,
            PreProgram = 0x70657270,
            HorizontakDeviceMetrix = 0x786d6468,
            ControlValueTable = 0x20747663,
            DigitalSignature = 0x47495344,
            GridFittingAndScanConversion = 0x70736167,
            GlyphDefinition = 0x46454447,
            FontProgram = 0x6d677066,
            GlyphPosition = 0x534f5047,
            LinearThreshold = 0x4853544c,
            /* Found in aial */
            Justification = 0x4654534a,
            VerticalDeviceMetrix = 0x584d4456,
            PCL5Table = 0x544c4350,
            KerningTable = 0x6e72656b,
            /* Found in Gulim */
            VertivalMetrixHeader = 0x61656876,
            VerticalMetrix = 0x78746d76,
            EmbedBitmapLocation = 0x434c4245,
            EmbededBitmapData = 0x54444245
        }

        public enum ChecksumFaultAction
        {
            IgnoreChecksum,
            ThrowException,
            Warn
        }

        #endregion

        #region "Data"

        IntPtr                          selector_ptr;   // Pointer to subfont
        IntPtr                          beginfile_ptr;
        private Hashtable               ListOfUsedGlyphs;
        private Hashtable               ListOfTables;
        TableDirectory                  dir;                // Заголовок фонта
        // Классы обвязки основных таблиц фонта
        private FontHeaderClass         font_header;
        private MaximumProfileClass     profile;
        private NameTableClass          name_table;
        private IndexToLocationClass    index_to_location;
        private CmapTableClass          cmap_table;
        private GlyphSubstitutionClass  gsub_table;
        private GlyphTableClass         glyph_table;
        #endregion

        #region "Public propertirs"

        public ICollection TablesList { get { return ListOfTables.Values; } }
        public NameTableClass Names { get { return name_table; } }
        public ChecksumFaultAction checksum_action;
//        public FontType output_format;

        #endregion

        ///////////////////////////////////////////////////////////////////////////////////////
        // Изменить byte-order
        ///////////////////////////////////////////////////////////////////////////////////////
        private void ChangeEndian()
        {
            dir.sfntversion = SwapUInt32(dir.sfntversion);
            dir.numTables = SwapUInt16(dir.numTables);
            dir.searchRange = SwapUInt16(dir.searchRange);
            dir.entrySelector = SwapUInt16(dir.entrySelector);
            dir.rangeShift = SwapUInt16(dir.rangeShift);
        }

        ///////////////////////////////////////////////////////////////////////////////////////
        // Посчитать контрольную сумму таблицы фонта
        ///////////////////////////////////////////////////////////////////////////////////////
        private uint CalcTableChecksum(IntPtr font, TrueTypeTable entry)
        {
            uint Sum = 0;
            uint Length = (entry.length + 3) / 4;
            IntPtr TablePtr = Increment(font, (int)entry.offset);

            int[] Temp = new int[Length];
            Marshal.Copy(TablePtr, Temp, 0, (int)Length);

            for (uint i = 0; i < Length; i++)
            {
                Sum += SwapUInt32((uint)Temp[i]);
            }

            Temp = null;

            return Sum;
        }

        ///////////////////////////////////////////////////////////////////////////////////////
        // Проверить контрольные суммы всех таблиц фонта
        ///////////////////////////////////////////////////////////////////////////////////////
        private void CheckTablesChecksum()
        {
            if (this.checksum_action == ChecksumFaultAction.IgnoreChecksum) return;
            foreach (TrueTypeTable entry in ListOfTables.Values)
            {
                try
                {
                    uint cs = CalcTableChecksum(this.beginfile_ptr, entry);
                    if (cs != entry.checkSum) throw new Exception("Table ID \"" + entry.TAG + "\" checksum error.\r\nContinue?");
                }
                catch (Exception ex)
                {
                    if (this.checksum_action == ChecksumFaultAction.ThrowException) throw ex;
                    DialogResult dr = MessageBox.Show(ex.Message, "Font table checksum error", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk);
                    if (dr == DialogResult.No) throw ex;
                }
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////
        // Посчитать общую котнтрольную сумму всех таблиц фонта и обновить заголовок фонта
        ///////////////////////////////////////////////////////////////////////////////////////
        private void CalculateFontChecksum(IntPtr start_offset, uint font_length)
        {
            uint Sum = 0;
            int length = (int)font_length / 4;
            int[] Temp = new int[length];
            Marshal.Copy(start_offset, Temp, 0, length);
            for (uint i = 0; i < length; i++)
            {
                Sum += SwapUInt32((uint)Temp[i]);
            }
            Temp = null;
            Sum = 0xb1b0afba - Sum;

            font_header.SaveFontHeader(this.beginfile_ptr, Sum);
        }

        ///////////////////////////////////////////////////////////////////////////////////////
        // Отсортировать таблицы в порядке их расположения в файле
        ///////////////////////////////////////////////////////////////////////////////////////
        private ArrayList GetTablesOrder()
        {
            Hashtable TablesOrder = new Hashtable();
            foreach (TrueTypeTable entry in ListOfTables.Values)
            {
                TablesOrder.Add(entry.offset, entry.tag);
            }
            ArrayList tables_positions = new ArrayList(TablesOrder.Keys);
            tables_positions.Sort();
            ArrayList indexed_tags = new ArrayList();
            foreach (uint offset in tables_positions)
            {
                uint tag = (uint)TablesOrder[offset];
                indexed_tags.Add((TablesID)tag);
            }
            tables_positions = null;
            TablesOrder = null;

            return indexed_tags;
        }

        ///////////////////////////////////////////////////////////////////////////////////////
        // Загрузить и преобразовать основные таблицы для будущего использования
        ///////////////////////////////////////////////////////////////////////////////////////
        private void LoadCoreTables()
        {
            if (!ListOfTables.ContainsKey(TablesID.FontHeader)) throw new Exception("FontHeader not found."); ;
            font_header = (FontHeaderClass)ListOfTables[TablesID.FontHeader];
            font_header.Load(this.beginfile_ptr);

            if (!ListOfTables.ContainsKey(TablesID.MaximumProfile)) throw new Exception("MaximuProfile not found."); ;
            profile = (MaximumProfileClass)ListOfTables[TablesID.MaximumProfile];
            profile.Load(this.beginfile_ptr);

            if (!ListOfTables.ContainsKey(TablesID.IndexToLocation)) throw new Exception("IndexToLocation not found."); ;
            index_to_location = (IndexToLocationClass) ListOfTables[TablesID.IndexToLocation];
            index_to_location.LoadIndexToLocation(this.beginfile_ptr, font_header);

            if (!ListOfTables.ContainsKey(TablesID.CMAP)) throw new Exception("CMAP not found."); ;
            cmap_table = (CmapTableClass)ListOfTables[TablesID.CMAP];
            cmap_table.LoadCmapTable(this.beginfile_ptr);

            if (!ListOfTables.ContainsKey(TablesID.Name)) throw new Exception("Name not found."); ;
            name_table = (NameTableClass)ListOfTables[TablesID.Name];
            name_table.Load(this.beginfile_ptr);

            if (!ListOfTables.ContainsKey(TablesID.Glyph)) throw new Exception("Glyphs not found."); ;
            glyph_table = (GlyphTableClass)ListOfTables[TablesID.Glyph];
            glyph_table.Load(this.beginfile_ptr);

            if (ListOfTables.ContainsKey(TablesID.GlyphSubstitution))
            {
                gsub_table = (GlyphSubstitutionClass)ListOfTables[TablesID.GlyphSubstitution];
                gsub_table.Load(this.beginfile_ptr);
            }

            if (ListOfTables.ContainsKey(TablesID.PreProgram))
            {
                PreProgramClass preprogram = (PreProgramClass)ListOfTables[TablesID.PreProgram];
                preprogram.Load(this.beginfile_ptr);
            }

        }

        ///////////////////////////////////////////////////////////////////////////////////////
        // Загрузить дескрипторы всех таблиц фонта
        ///////////////////////////////////////////////////////////////////////////////////////
        private void LoadDescriptors(ArrayList skip_array)
        {
            dir = (TableDirectory)Marshal.PtrToStructure(this.selector_ptr, typeof(TableDirectory));
            ChangeEndian();

            IntPtr tbls = Increment(selector_ptr, Marshal.SizeOf(dir));

            for (int i = 0; i < dir.numTables; i++)
            {
                TrueTypeTable table = new TrueTypeTable(tbls);

                if (!skip_array.Contains( (TablesID) table.tag))
                {
                    TrueTypeTable parsed_table;
                    switch ((TablesID)table.tag)
                    {
                        case TablesID.FontHeader: parsed_table = new FontHeaderClass(table); break;
                        case TablesID.MaximumProfile: parsed_table = new MaximumProfileClass(table); break;
                        case TablesID.Name: parsed_table = new NameTableClass(table); break;
                        case TablesID.IndexToLocation: parsed_table = new IndexToLocationClass(table); break;
                        case TablesID.CMAP: parsed_table = new CmapTableClass(table); break;
                        case TablesID.Glyph: parsed_table = new GlyphTableClass(table); break;
                        case TablesID.GlyphSubstitution: parsed_table = new GlyphSubstitutionClass(table); break;
                        case TablesID.PreProgram: parsed_table = new PreProgramClass(table); break;

                        default:    parsed_table = table;   break;
                    }
                    try
                    {
                        ListOfTables.Add((TablesID)table.tag, parsed_table);
                    }
                    catch (Exception ex)
                    {
                        DialogResult dr = MessageBox.Show(ex.Message, "Font format error", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk);
                    }
                }
                tbls = Increment(tbls, table.descriptor_size );
            }
            dir.numTables = (ushort) ListOfTables.Count;
        }

        ///////////////////////////////////////////////////////////////////////////////////////
        // Отсортировать тэги таблиц фонта и сохранить дескрипторы всех таблиц фонта
        ///////////////////////////////////////////////////////////////////////////////////////
        private void SaveDescriptors(IntPtr position)
        {
            ChangeEndian();
            Marshal.StructureToPtr(dir, position, false);
            ChangeEndian();

            IntPtr tbls = Increment(position, Marshal.SizeOf(dir));

            ArrayList descriptor_list = new ArrayList(ListOfTables.Keys);
            for (int i = 0; i < descriptor_list.Count; i++)
            {
                descriptor_list[i] = SwapUInt32((uint)(int)descriptor_list[i]);
            }
            descriptor_list.Sort();
            for (int i = 0; i < descriptor_list.Count; i++)
            {
                descriptor_list[i] = (TablesID)SwapUInt32((uint)descriptor_list[i]);
            }

            foreach (TablesID tag in descriptor_list)
            {
                TrueTypeTable entry = (TrueTypeTable)ListOfTables[tag];
                tbls = entry.StoreDescriptor(tbls);
            }
            descriptor_list = null;
        }

        ///////////////////////////////////////////////////////////////////////////////////////
        // Подготовить индексы глифов и индексы композитных глифов
        ///////////////////////////////////////////////////////////////////////////////////////
        private ArrayList BuildGlyphIndexList(
            ArrayList       used_glyphs,    // Список используемых глифов
            bool            uniscribe)      // Если false, список глифов - коды символов, если true - коды uniscribe 
        {
            ushort length;
            uint location;
            ArrayList composite_indexes = new ArrayList();
            
            foreach (ushort key in used_glyphs)
            {
                ushort idx = uniscribe ? key : this.cmap_table.GetGlyphIndex(key);

                ListOfUsedGlyphs[key] = idx;
                length = this.index_to_location.GetGlyph(idx, font_header, out location);
                if (length != 0)
                {
                    foreach (ushort composed_idx in this.glyph_table.CheckGlyph((int)location, (int)length))
                    {
                        if (!composite_indexes.Contains(composed_idx)) composite_indexes.Add(composed_idx);
                    }
                    if (!composite_indexes.Contains(idx)) composite_indexes.Add(idx);
                }
            }
            used_glyphs = null;
            
            composite_indexes.Sort();

            return composite_indexes;
        }

        ///////////////////////////////////////////////////////////////////////////////////////
        // Упаковка таблицы глифов и соответственное обновление таблицы IndexToLocation
        ///////////////////////////////////////////////////////////////////////////////////////
        private void ReorderGlyphTable(IntPtr position, bool uniscribe)
        {
            ushort[] ShortIndexToLocation = this.index_to_location.Short;
            uint[] LongIndexToLocation = this.index_to_location.Long;

            ArrayList used_glyphs = new ArrayList(ListOfUsedGlyphs.Keys);
            ArrayList composite_indexes = BuildGlyphIndexList(used_glyphs, uniscribe);

            uint glyph_table_size = 0;
            ushort length = 0;
            uint location = 0;

            ///////////////////////////////////////////////////////////////////////////////////////
            // Рассчитать размер выходной таблицы глифов
            foreach (ushort idx in composite_indexes)
            {
                glyph_table_size += this.index_to_location.GetGlyph(idx, font_header, out location);
            }

            TrueTypeTable table_entry = (TrueTypeTable)ListOfTables[TablesID.Glyph];
            IntPtr glyph_table_ptr = Increment(this.beginfile_ptr, (int)(table_entry.offset));
            byte[] SelectedGlyphs = new byte[glyph_table_size];

            uint out_index = 0;
            int sqz_index = 0;

            foreach (ushort i2l_idx in composite_indexes)
            {
                ///////////////////////////////////////////////////////////////////////////////////////
                // Получить размер Глифа и его позицию
                length = this.index_to_location.GetGlyph(i2l_idx, font_header, out location);

                ///////////////////////////////////////////////////////////////////////////////////////
                // Cкорректировать таблицу Index_To_Location
                switch (font_header.indexToLocFormat)
                {
                    case FontHeaderClass.IndexToLoc.ShortType:
                        for (; sqz_index <= i2l_idx; sqz_index++) ShortIndexToLocation[sqz_index] = (ushort)(out_index / 2);
                        break;

                    case FontHeaderClass.IndexToLoc.LongType:
                        for (; sqz_index <= i2l_idx; sqz_index++) LongIndexToLocation[sqz_index] = out_index;
                        break;
                }

                ///////////////////////////////////////////////////////////////////////////////////////
                // Копировать данные Глифов
                if (length != 0)
                {
                    IntPtr glyph_ptr = Increment(glyph_table_ptr, (int)location);
                    Marshal.Copy(glyph_ptr, SelectedGlyphs, (int)(out_index), length);
                    out_index += length;
                }
            }
            Marshal.Copy(SelectedGlyphs, 0, glyph_table_ptr, SelectedGlyphs.Length);
            table_entry.length = out_index;

            SelectedGlyphs = null;

            ///////////////////////////////////////////////////////////////////////////////////////
            // Заполнить до конца таблицу Index_To_Location
            // Переставить байты в таблицы Index_To_Location
            // Скопировать в исходную позицию и посчитать контрольную сумму
            table_entry = (TrueTypeTable)ListOfTables[TablesID.IndexToLocation];
            IntPtr i2l_ptr = Increment(this.beginfile_ptr, (int)(table_entry.offset));
            switch (font_header.indexToLocFormat)
            {
                case FontHeaderClass.IndexToLoc.ShortType:
                    for (; sqz_index < ShortIndexToLocation.Length; sqz_index++)
                    {
                        ShortIndexToLocation[sqz_index] = (ushort)(out_index / 2);
                    }
                    short[] ShortTemp = new short[ShortIndexToLocation.Length];
                    for (int i = 0; i < ShortIndexToLocation.Length; i++)
                    {
                        ShortTemp[i] = (short)SwapUInt16((ushort)ShortIndexToLocation[i]);
                    }
                    Marshal.Copy(ShortTemp, 0, i2l_ptr, ShortIndexToLocation.Length);
                    //ShortTemp = null;
                    break;

                case FontHeaderClass.IndexToLoc.LongType:
                    for (; sqz_index < LongIndexToLocation.Length; sqz_index++)
                    {
                        LongIndexToLocation[sqz_index] = out_index;
                    }
                    int[] LongTemp = new int[LongIndexToLocation.Length];
                    for (int i = 0; i < LongIndexToLocation.Length; i++)
                    {
                        LongTemp[i] = (int)SwapUInt32((uint)LongIndexToLocation[i]);
                    }
                    Marshal.Copy(LongTemp, 0, i2l_ptr, LongIndexToLocation.Length);
                    break;
            }
            table_entry.checkSum = CalcTableChecksum(this.beginfile_ptr, table_entry);

            ///////////////////////////////////////////////////////////////////////////////////////
            // Посчитать контрольную сумму таблицы Глифов и обновить дескритор таблицы
            table_entry = (TrueTypeTable)ListOfTables[TablesID.Glyph];
            table_entry.checkSum = CalcTableChecksum(this.beginfile_ptr, table_entry);
        }

        #region "Public methods"

        ///////////////////////////////////////////////////////////////////////////////////////
        // Упаковать фонт
        ///////////////////////////////////////////////////////////////////////////////////////
        public byte[] PackFont(FontType translate_to, bool uniscribe)
        {
            uint current_offset;

            ArrayList indexed_tags = GetTablesOrder();

            ReorderGlyphTable(this.beginfile_ptr, uniscribe);

            ///////////////////////////////////////////////////////////////////////////////////////
            // sizeof(TrueTypeFont.TableDirectory) + this.dir.numTables * sizeof(TrueTypeTable.TableEntry));
            ///////////////////////////////////////////////////////////////////////////////////////
            current_offset = (uint)(Marshal.SizeOf(dir) + this.dir.numTables * 16);

            ///////////////////////////////////////////////////////////////////////////////////////
            // Упаковать таблицы в исходных позициях и обновить соответствующее поле в дескрипторе
            ///////////////////////////////////////////////////////////////////////////////////////
            foreach (TablesID tag in indexed_tags)
            {
                TrueTypeTable entry = (TrueTypeTable)ListOfTables[tag];
                current_offset = entry.Save( this.beginfile_ptr, current_offset);

                if ((current_offset % 4) != 0) throw new Exception("Align error");
            }

            SaveDescriptors(this.beginfile_ptr);

            CalculateFontChecksum(this.beginfile_ptr, current_offset);

            ///////////////////////////////////////////////////////////////////////////////////////
            // Освободить память
            ///////////////////////////////////////////////////////////////////////////////////////
            indexed_tags = null;

            byte[] buff = new byte[current_offset];
            Marshal.Copy(this.beginfile_ptr, buff, 0, (int)current_offset);

            return buff;
        }

        ///////////////////////////////////////////////////////////////////////////////////////
        // Добавить символ в фонт
        ///////////////////////////////////////////////////////////////////////////////////////
        public void AddCharacterToKeepList(char ch)
        {
            ushort key = (ushort) ch;

            if (!ListOfUsedGlyphs.ContainsKey(key)) 
            {
                ListOfUsedGlyphs.Add(key, null);
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////
        // Подготовить фонт к дальнейшей обработке
        ///////////////////////////////////////////////////////////////////////////////////////
        public void PrepareFont(TablesID[] skip_list)
        {
            ArrayList skip_array = new ArrayList(skip_list);
            LoadDescriptors(skip_array);
            LoadCoreTables();
            CheckTablesChecksum();
            skip_array = null;
        }

        ///////////////////////////////////////////////////////////////////////////////////////
        // Вернуть Graphics path для заданного глифа
        ///////////////////////////////////////////////////////////////////////////////////////
        public GraphicsPath GetGlyph(char ch, int size, Point position)
        {
            GlyphTableClass.GlyphHeader gheader;
            uint location;
            ushort i2l_idx = this.cmap_table.GetGlyphIndex((ushort)ch);
            ushort length  = this.index_to_location.GetGlyph(i2l_idx, this.font_header, out location);

            float rsize = (float)this.font_header.unitsPerEm / size;

            return this.glyph_table.GetGlyph((int)location, length, rsize, position, out gheader);
        }

        public GraphicsPath DrawString(string text, Point position, int size)
        {
            GraphicsPath path = new GraphicsPath(FillMode.Winding);
            float rsize = (float)this.font_header.unitsPerEm / size;
            bool uniscribe = false;
            uint location;
            ushort glyph_size;
            GlyphTableClass.GlyphHeader     gheader;

            foreach (char ch in text)
            {
                int glyph_width = 0;
                gheader.xMin = 0;
                gheader.xMax = 10;

                ushort idx = (uniscribe ? ch : this.cmap_table.GetGlyphIndex(ch));
                glyph_size = this.index_to_location.GetGlyph(idx, font_header, out location);
                if (glyph_size != 0)
                {
                    ArrayList composed_indexes = this.glyph_table.CheckGlyph((int)location, (int)glyph_size);
                    if (composed_indexes.Count != 0)
                    {
                        foreach (ushort composed_idx in composed_indexes)
                        {
                            glyph_size = this.index_to_location.GetGlyph(composed_idx, font_header, out location);
                            GraphicsPath glyph_path = this.glyph_table.GetGlyph((int)location, glyph_size, rsize, position, out gheader);
                            path.AddPath(glyph_path, false);
                        }
                    }
                    else
                    {
                        GraphicsPath glyph_path = this.glyph_table.GetGlyph((int)location, glyph_size, rsize, position, out gheader);
                        path.AddPath(glyph_path, false);
                    }
                }

                glyph_width = (int) ((gheader.xMax/* - gheader.xMin*/) / rsize);
                glyph_width += 4; // Четыре точки межсимвольный интервал

                position.X += glyph_width;
            }

            return path;
        }

        #endregion

        ///////////////////////////////////////////////////////////////////////////////////////
        // Конструктор фонта
        ///////////////////////////////////////////////////////////////////////////////////////
        public TrueTypeFont(IntPtr begin, IntPtr font, ChecksumFaultAction action)
        {
            beginfile_ptr = begin;
            selector_ptr = font;
            checksum_action = action;
            ListOfTables = new Hashtable();
            ListOfUsedGlyphs = new Hashtable();
            ushort key = 0;
            ListOfUsedGlyphs.Add(key, null);
        }
    }
}
