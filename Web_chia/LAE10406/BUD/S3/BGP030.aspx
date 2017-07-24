<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="BGP030.aspx.vb" Inherits="LAEACC.BGP030" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="AjaxToolkit" %>

<%@ MasterType VirtualPath="~/Site.master" %>

<asp:Content ID="Head" ContentPlaceHolderID="MainHead" runat="server">
</asp:content>


<asp:Content ID="Content" ContentPlaceHolderID="MainContent" runat="server">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <Triggers>
            <asp:PostBackTrigger ControlID="BtnPrint" />
            <asp:PostBackTrigger ControlID="btnExport" />
       </Triggers>
        <ContentTemplate>        
            <!--關鍵值隱藏區-->
            <asp:Label ID="txtKey1" Text="" Visible="false" runat="server" />
   
            <!--主項目區-->
            <div style="margin: 10px 0px 0px 10px;">
                <section id="widget-grid" style="width:98%;">
                    <div class="row">
                        <article class="col-sm-12 col-md-12 col-lg-12">
                            <table id="table-data" rules="all">
                                <tr><td colspan="2" style="color:#f84444;font-size:16px;font-weight:bold;text-align:center;">預算執行統計表列印BGP030</td></tr>                                 
                                <tr>   
                                    <th>請輸入列印範圍：</th>                                             
                                    <td><asp:DropDownList ID="cboUser" AutoPostBack="True" runat="server" /></td>
                                </tr>
                                <tr>   
                                    <th>年度：</th>                                             
                                    <td>
                                        <asp:TextBox ID="nudYear" Width="80" TextMode="Number" runat="server" min="0" max="999" step="1"/>
                                        <asp:Label ID="labInfo1" Text="列印已往年度請以12/31日進入系統" runat="server" />
                                    </td>
                                </tr>
                                <tr>   
                                    <th>請輸入科目：</th>                                             
                                    <td>
                                        <asp:TextBox ID="vxtStartNo" MaxLength="17" runat="server" />                                        
                                        <AjaxToolkit:MaskedEditExtender ID="MaskedEditExtender2" runat="server"
                                                TargetControlID="vxtStartNo"
                                                MaskType="None" Mask="?-????-??-??-???????-?"
                                                InputDirection="LeftToRight" />
                                        ～
                                        <asp:TextBox ID="vxtEndNo" MaxLength="17" runat="server" />
                                        <AjaxToolkit:MaskedEditExtender ID="MaskedEditExtender4" runat="server"
                                                TargetControlID="vxtEndNo"
                                                MaskType="None" Mask="?-????-??-??-???????-?"
                                                InputDirection="LeftToRight" />
                                    </td>
                                </tr>                               
                                <tr>   
                                    <th>是否要逐級統計：</th>                                             
                                    <td>
                                        <asp:RadioButton id="rdoSumYes" Text="要逐級往上統計" GroupName="rdbKind" runat="server"/>
                                        <asp:RadioButton id="rdoSumNo" Text="印明細即可" Checked="true" GroupName="rdbKind" runat="server"/>
                                    </td>
                                </tr>                                            
                                <tr>                                                
                                    <td colspan="2" style="text-align:center;">
                                        <asp:Button ID="btnPrint" Text="列印" CssClass="btn btn-primary" runat="server" />
                                        <asp:Button ID="btnExport" Text="匯出" CssClass="btn btn-primary" runat="server" />
                                    </td>                                   
                                </tr>                                
                            </table>
                            <div style="padding-bottom:1px;">&nbsp;</div>                            
                        </article>
                    </div>
                </section>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>

</asp:Content>
