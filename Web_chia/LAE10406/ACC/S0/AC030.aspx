<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="AC030.aspx.vb" Inherits="LAEACC.AC030" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="AjaxToolkit" %>

<%@ MasterType VirtualPath="~/Site.master" %>
<%@ Register SRC="~/LAE10406/UserControl/UCBase.ascx" TagName="UCBase" TagPrefix="uc1" %>
<%@ Register SRC="~/LAE10406/UserControl/AccText.ascx" TagName="AccText" TagPrefix="Acc1" %>

<asp:Content ID="Head" ContentPlaceHolderID="MainHead" runat="server">
        <script type="text/javascript">
        function openNewWin(url) {

            var x = window.open(url, 'mynewwin', 'width=600,height=600,toolbar=1');

            x.focus();

        }
        function stringBytes(c) {
            var n = c.length, s;
            var len = 0;
            for (var i = 0; i < n; i++) {
                s = c.charCodeAt(i);
                while (s > 0) {
                    len++;
                    s = s >> 8;
                }
            }
            return len;
        }
        function substr(str, len) {
            if (!str || !len) { return ''; }
            var a = 0;
            var i = 0;
            var temp = '';

            for (i = 0; i < str.length; i++) {
                if (str.charCodeAt(i) > 255) {
                    a += 2;
                }
                else {
                    a++;
                }
                if (a > len) { return temp; }
                temp += str.charAt(i);
            }
            return str;
        }

        function ismaxlength(obj) {
            var mlength = obj.getAttribute ? parseInt(obj.getAttribute("maxlength")) : ""
            if (obj.getAttribute && stringBytes(obj.value) > mlength)
                obj.value = substr(obj.value, mlength)
        }

        function sumamt() {
            var amt1 = document.getElementById("<%=txtAmt1.ClientID%>")
            var amt2 = document.getElementById("<%=txtAmt2.ClientID%>")
            var amt3 = document.getElementById("<%=txtAmt3.ClientID%>")
            var amt4 = document.getElementById("<%=txtAmt4.ClientID%>")
            var amt5 = document.getElementById("<%=txtAmt5.ClientID%>")
            var amt6 = document.getElementById("<%=txtAmt6.ClientID%>")
            var a2 = ((isNaN(amt2.value.replace(/,/g, ""))) ? 0 : amt2.value.replace(/,/g, ""));
            var a3 = ((isNaN(amt3.value.replace(/,/g, ""))) ? 0 : amt3.value.replace(/,/g, ""));
            var a4 = ((isNaN(amt4.value.replace(/,/g, ""))) ? 0 : amt4.value.replace(/,/g, ""));
            var a5 = ((isNaN(amt5.value.replace(/,/g, ""))) ? 0 : amt5.value.replace(/,/g, ""));
            var a6 = ((isNaN(amt6.value.replace(/,/g, ""))) ? 0 : amt6.value.replace(/,/g, ""));


            amt1.value = Number(a2) + Number(a3) + Number(a4) + Number(a5) + Number(a6);
        }



    </script>
</asp:content>


<asp:Content ID="Content" ContentPlaceHolderID="MainContent" runat="server">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>        
            <!--關鍵值隱藏區-->
            <asp:Label ID="txtKey1" Text="" Visible="false" runat="server" />
   
            <!--主項目區-->
            <div style="margin: 10px 0px 0px 10px;">
                <section id="widget-grid" style="width:98%;">
                    <div class="row">
                        <article class="col-sm-12 col-md-12 col-lg-12">
                            <!-- 查詢項目值 -->                             
                            <div class="widget-body form-horizontal" style="font-size:16px; margin:10px 0px 10px 0px;" >
                                  <fieldset>
                                    
                                        <table id="table-serch" rules="all">
                                            
                                            <tr>
                                                <asp:Panel ID="gbxCreate" runat="server">
                                                <td style="text-align:center;">
                                                    <asp:RadioButton id="rdbFile1" Text="預算轉帳" Checked="true" GroupName="GroupBox2" runat="server"/><asp:RadioButton id="rdbFile2" Text="保證品" GroupName="GroupBox2" runat="server"/><asp:RadioButton id="rdbFile3" Text="空白" GroupName="GroupBox2" runat="server"/>
                                                </td>                                                                                                
                                                <td style="width:120px; text-align:center;">
                                                    <asp:Button ID="btnSearch" Text="搜尋" CssClass="btn btn-primary" runat="server" />
                                                </td>
                                                </asp:Panel>
                                                <asp:Panel ID="gbxModify" runat="server">
                                                    <tr>
                                                <td>
                                                    轉帳傳票號:<asp:TextBox ID="txtOldNo" Width="80px" runat="server" />
                                                </td>                                                                                                
                                                <td>
                                                    <asp:Button ID="btnOldNo" Text="確認" CssClass="btn btn-primary" runat="server" />
                                                </td>
                                                </tr>
                                                </asp:Panel>
                                                <td style="width:160px;">
                                                    <asp:Label ID="lblNOkind" Text=" 上張編號=" Font-Size="12pt" Font-Bold="True"  runat="server" /><asp:Label ID="lblUseNO" ForeColor="blue" Font-Size="12pt" Font-Bold="True"  runat="server" />
                                                </td>                                                
                                            </tr>
                                        </table>

								    
							    </fieldset> 
                            </div>                          
                                       
                            <!--詳細內容顯示區-->                           
                            <AjaxToolkit:TabContainer ID="TabContainer1" Width="100%" CssClass="Tab" runat="server" ActiveTabIndex="1">
                                <AjaxToolkit:TabPanel ID="TabPanel1" runat="server">
                                    <HeaderTemplate>資料來源</HeaderTemplate>
                                    <ContentTemplate>
                                        <div style="font-size:14px;">
                                            <asp:Label ID="Label1" Text="傳票內容選入區" ForeColor="Blue" Font-Size="12pt" Font-Bold="True"  runat="server" />
                                            <asp:Label ID="lbl_dtgTargetGrdCount" Visible="False" Text="0" runat="server" />                                            
                                            <asp:DataGrid ID="dtgTarget" Width = "100%" runat="server" >
                                                <columns>
                                                    <asp:TemplateColumn HeaderText="管理">
                                                        <itemtemplate>        
                                                                                                                                                              
                                                            <asp:ImageButton ID="Delete" AlternateText="刪除" ImageUrl="~/active/images/icon/items/delete.png" CommandName="btnDelete" runat="server" />
                                                            <asp:Label ID="id" Text='<%# Container.DataItem("BGNO").ToString%>' Visible="false" runat="server" />
                                                            <asp:UpdatePanel ID="DataGridView_UpdatePanel" runat="server">                                                                
                                                                <Triggers><asp:AsyncPostBackTrigger ControlID="Delete" EventName="Click" /></Triggers>
                                                            </asp:UpdatePanel>
                                                        </itemtemplate>                                                         
                                                        <HeaderStyle Width="40px" />
                                                        <ItemStyle HorizontalAlign="Center" />
                                                    </asp:TemplateColumn>                                                                                
                                                                                                                                   
                                                    <asp:TemplateColumn HeaderText="請購編號">
                                                        <itemtemplate><asp:Label ID="請購編號" Text='<%# Container.DataItem("bgno").ToString%>' runat="server" /></itemtemplate>                                                       
                                                        <HeaderStyle Width="40px" />
                                                    </asp:TemplateColumn>
                                                   
                                                    <asp:TemplateColumn HeaderText="會計科目">
                                                        <itemtemplate><asp:Label ID="會計科目" Text='<%# Container.DataItem("accno").ToString%>' runat="server" /></itemtemplate>                                                       
                                                        <HeaderStyle Width="80px" />
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="收支">
                                                        <itemtemplate><asp:Label ID="kind" Text='<%# Container.DataItem("kind").ToString%>' runat="server" /></itemtemplate>                                                       
                                                        <HeaderStyle Width="40px" />
                                                        <ItemStyle HorizontalAlign="Center" />
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="摘要">
                                                        <itemtemplate><asp:Label ID="摘要" Text='<%# Container.DataItem("remark").ToString%>' runat="server" /></itemtemplate>                                                       
                                                        <HeaderStyle Width="100px" />
                                                    </asp:TemplateColumn>
                                                    
                                                    <asp:TemplateColumn HeaderText="金額">
                                                        <itemtemplate><asp:Label ID="金額" Text='<%# FormatNumber(Container.DataItem("useamt").ToString,0)%>' runat="server" /></itemtemplate>                                                       
                                                        <HeaderStyle Width="80px" />
                                                        <ItemStyle HorizontalAlign="Right" />
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="受款人" >
                                                        <itemtemplate><asp:Label ID="受款人" Text='<%# Container.DataItem("subject").ToString%>' runat="server" /></itemtemplate>                                                       
                                                        <HeaderStyle Width="100px" />
                                                    </asp:TemplateColumn>
                                                    
                                                    <asp:TemplateColumn >
                                                        <itemtemplate><asp:Label ID="autono" Text='<%# Container.DataItem("autono").ToString%>' runat="server" /></itemtemplate>                                                       
                                                        <HeaderStyle Width="100px" />
                                                    </asp:TemplateColumn> 
                                                    <asp:TemplateColumn >
                                                        <itemtemplate><asp:Label ID="dc" Text='<%# Container.DataItem("dc").ToString%>' runat="server" /></itemtemplate>                                                       
                                                        <HeaderStyle Width="20px" />
                                                    </asp:TemplateColumn>                               
                                                </columns>
                                            </asp:DataGrid>

                                            <table>
                                                <tr>
                                                    <td><asp:Label ID="lblFile" Text="請輸入請購編號:" ForeColor="Blue" Font-Size="12pt" Font-Bold="True"  runat="server" /></td>
                                                    <td>
                                                        <asp:Label ID="lblYear" ForeColor="Blue" Font-Size="12pt" Font-Bold="True"  runat="server" /><asp:TextBox ID="txtNo" Width="80px" runat="server" />
                                                        <asp:Button ID="btnNo" Text="輸入" CssClass="btn btn-primary" runat="server" />
                                                    </td>
                                                    <td>
                                                        <asp:Button ID="btnSure" Text="傳票來源確定" CssClass="btn btn-primary" runat="server" />
                                                        <asp:Button ID="btnFirstScreen" Text="放棄(回上頁)" CssClass="btn btn-primary" runat="server" />
                                                    </td>
                                                </tr>
                                            </table>
                                                                                                                                  
                                           <asp:Label ID="lbl_dtgSourceGrdCount" Visible="false" ForeColor="Red" Font-Size="14pt" Font-Bold="True" Text="0" runat="server" />
                                            
                                            <asp:DataGrid ID="dtgSource" Width = "100%" AllowSorting="True" runat="server" >
                                                <columns>
                                                    <asp:TemplateColumn HeaderText="管理">
                                                        <itemtemplate>                                                                                                            
                                                            <asp:ImageButton ID="Show" AlternateText="點選" ImageUrl="~/active/images/icon/items/zoom.png" CommandName="btnShow" runat="server" />
                                                            <asp:Label ID="id" Text='<%# Container.DataItem("BGNO").ToString%>' Visible="false" runat="server" />
                                                            <asp:UpdatePanel ID="DataGridView_UpdatePanel" runat="server">                                                                
                                                                <Triggers><asp:AsyncPostBackTrigger ControlID="Show" EventName="Click" /></Triggers>
                                                            </asp:UpdatePanel>
                                                        </itemtemplate>                                                         
                                                        <HeaderStyle Width="40px" />
                                                        <ItemStyle HorizontalAlign="Center" />
                                                    </asp:TemplateColumn>                                                                                
                                                    <asp:TemplateColumn HeaderText="請購編號" SortExpression="BGNO">
                                                        <itemtemplate><asp:Label ID="請購編號" Text='<%# Container.DataItem("BGNO").ToString%>' runat="server" /></itemtemplate>                                                       
                                                        <HeaderStyle Width="80px" />
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="科目" SortExpression="ACCNO">
                                                        <itemtemplate><asp:Label ID="科目" Text='<%# Container.DataItem("ACCNO").ToString%>' runat="server" /></itemtemplate>                                                       
                                                        <HeaderStyle Width="100px" />
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="收支" >
                                                        <itemtemplate><asp:Label ID="kind" Text='<%# Container.DataItem("kind").ToString%>' runat="server" /></itemtemplate>                                                       
                                                        <HeaderStyle Width="40px" />
                                                    </asp:TemplateColumn>

                                                    <asp:TemplateColumn HeaderText="摘要">
                                                        <itemtemplate><asp:Label ID="摘要" Text='<%# Container.DataItem("REMARK").ToString%>' runat="server" /></itemtemplate>                                                       
                                                        <HeaderStyle Width="220px" />
                                                    </asp:TemplateColumn>   
                                                    <asp:TemplateColumn HeaderText="金額" SortExpression="useamt" >
                                                        <itemtemplate><asp:Label ID="金額" Text='<%# Container.DataItem("useamt").ToString%>' runat="server" /></itemtemplate>                                                       
                                                        <HeaderStyle Width="80px" />
                                                    </asp:TemplateColumn>     
                                                    <asp:TemplateColumn HeaderText="受款人" SortExpression="subject" >
                                                        <itemtemplate><asp:Label ID="受款人" Text='<%# Container.DataItem("subject").ToString%>' runat="server" /></itemtemplate>                                                       
                                                        <HeaderStyle Width="100px" />
                                                    </asp:TemplateColumn>
                                                    
                                                    <asp:TemplateColumn >
                                                        <itemtemplate><asp:Label ID="autono" Text='<%# Container.DataItem("autono").ToString%>' runat="server" /></itemtemplate>                                                       
                                                        <HeaderStyle Width="100px" />
                                                    </asp:TemplateColumn>  
                                                    <asp:TemplateColumn >
                                                        <itemtemplate><asp:Label ID="dc" Text='<%# Container.DataItem("dc").ToString%>' runat="server" /></itemtemplate>                                                       
                                                        <HeaderStyle Width="20px" />
                                                    </asp:TemplateColumn>  
                                                                                                        
                                                </columns>
                                            </asp:DataGrid>

                                        </div>
                                    </ContentTemplate>
                                </AjaxToolkit:TabPanel>

                                <AjaxToolkit:TabPanel ID="TabPanel2" runat="server">
                                    <HeaderTemplate>開立傳票</HeaderTemplate>
                                    <ContentTemplate>
                                        <div class="widget-body form-horizontal">
                                            <div style="background-color:<%=strBackColor%>; font-size:14px; padding-top:20px;">
                                                <fieldset>
                                                  <table>
                                                    <tr>
                                                        <td>製票日期：<asp:Label ID="lblDate1" ForeColor="Blue" Font-Size="12pt" Font-Bold="True"  runat="server" /></td>
                                                        <td style="text-align:center;"><asp:Label ID="lblDC" Text="轉帳借方" ForeColor="Blue" Font-Size="12pt" Font-Bold="True"  runat="server" /></td>
                                                        <td><asp:Button ID="btnPageUp" Text="上頁" CssClass="btn btn-primary" runat="server" TabIndex="103" /><asp:Button ID="btnPageDown" Text="下頁" CssClass="btn btn-primary" runat="server" TabIndex="104" /></td>
                                                        <td>
                                                            <asp:Button ID="btnDebit" Text="借方" CssClass="btn btn-primary" runat="server" TabIndex="105" />
                                                            <asp:Button ID="btnCredit" Text="貸方" CssClass="btn btn-primary" runat="server" TabIndex="106" />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>製票編號：<asp:Label ID="lblNowNO" ForeColor="Blue" Font-Size="12pt" Font-Bold="True"  runat="server" /></td>
                                                        <td style="text-align:center;" >第<asp:Label ID="lblPage" ForeColor="Blue" Font-Size="12pt" Font-Bold="True"  runat="server" />頁</td>
                                                        <td></td>
                                                        <td><asp:Label ID="lblNo_1_no" ForeColor="Blue" Font-Size="12pt" Font-Bold="True"  runat="server" /><asp:Label ID="lblkey" Visible="False"  runat="server" /></td>                                            
                                                    </tr>

                                                    <tr>
                                                           
                                                        <td><asp:Label ID="Label2" Text="會計科目" Width="210px" style="TEXT-ALIGN:center;"  Font-Size="12pt" Font-Bold="True"  runat="server" /></td>
                                                        <td><asp:Label ID="Label3" Text="摘要" Width="540px" style="TEXT-ALIGN:center;"  Font-Size="12pt" Font-Bold="True"  runat="server" /></td>
                                                        <td><asp:Label ID="Label5" Text="金額" Width="120px" style="TEXT-ALIGN:center;" Font-Size="12pt" Font-Bold="True"  runat="server" /></td>
                                                        <td><asp:Label ID="Label6" Text="內容別" Width="60px" style="TEXT-ALIGN:center;"  Font-Size="12pt" Font-Bold="True"  runat="server" /></td>                                       
                                                    </tr>
                                                     <tr>
                                                        <td><asp:TextBox ID="vxtAccno1" Width="100px" runat="server" /></td>
                                                        <td><asp:TextBox ID="txtRemark1" Width="520px"   runat="server" /></td>
                                                        <td><asp:TextBox ID="txtAmt1"  Width="100px" style="text-align:right" runat="server" /></td>
                                                        <td></td>
                                                    </tr>
                                                    <tr>
                                                        <td><asp:Label ID="lblAccName1" ForeColor="Blue" Font-Size="8pt" Font-Bold="True"  runat="server" /></td>
                                                        <td>
                                                            <asp:Button ID="btnOther" Text="相關科目" runat="server" TabIndex="100" />
                                                            <asp:Button ID="btnCopy1" Text="Copy1" runat="server" TabIndex="101" />
                                                            <asp:Button ID="btnCopy5" Text="Copy5" runat="server" TabIndex="102" />
                                                        </td>
                                                        <td></td>
                                                        <td></td>
                                                    </tr>
                                                    <tr>
                                                        <td><asp:TextBox ID="vxtAccno2" Width="190px"  AutoPostBack="True" runat="server" /></td>
                                                        <td><asp:TextBox ID="txtRemark2" Width="520px"  runat="server" /></td>
                                                        <td><asp:TextBox ID="txtAmt2" Width="100px" style="text-align:right" runat="server" /></td>
                                                        <td><asp:TextBox ID="txtCode2" Width="40px" runat="server" /></td>
                                                    </tr>
                                                    <tr>
                                                        <td>&nbsp;<asp:Label ID="lblAccName2" ForeColor="Blue" Font-Size="8pt" Font-Bold="True"  runat="server" /></td>
                                                        <td><asp:TextBox ID="vxtOther2" AutoPostBack="True" runat="server" /><asp:Button ID="btnF42" Text="F4" Width="40px" CssClass="btn btn-primary" runat="server" /><asp:Label ID="lblOtherName2" ForeColor="Blue" Font-Size="8pt" Font-Bold="True"  runat="server" /></td>
                                                        <td></td>
                                                        <td></td>
                                                    </tr>
                                                    <tr>
                                                        <td><asp:TextBox ID="vxtAccno3" Width="190px"  AutoPostBack="True" runat="server" /></td>
                                                        <td><asp:TextBox ID="txtRemark3" Width="520px"  runat="server" /></td>
                                                        <td><asp:TextBox ID="txtAmt3" Width="100px" style="text-align:right" runat="server" /></td>
                                                        <td><asp:TextBox ID="txtCode3" Width="40px" runat="server" /></td>
                                                    </tr>
                                                    <tr>
                                                        <td>&nbsp;<asp:Label ID="lblAccName3" ForeColor="Blue" Font-Size="8pt" Font-Bold="True"  runat="server" /></td>
                                                        <td><asp:TextBox ID="vxtOther3" AutoPostBack="True" runat="server" /><asp:Button ID="btnF43" Text="F4" Width="40px" CssClass="btn btn-primary" runat="server" /><asp:Label ID="lblOtherName3" ForeColor="Blue" Font-Size="8pt" Font-Bold="True"  runat="server" /></td>
                                                        <td></td>
                                                        <td></td>
                                                    </tr>
                                                      <tr>
                                                        <td><asp:TextBox ID="vxtAccno4" Width="190px"  AutoPostBack="True" runat="server" /></td>
                                                        <td><asp:TextBox ID="txtRemark4" Width="520px"  runat="server" /></td>
                                                        <td><asp:TextBox ID="txtAmt4" Width="100px" style="text-align:right" runat="server" /></td>
                                                        <td><asp:TextBox ID="txtCode4" Width="40px" runat="server" /></td>
                                                    </tr>
                                                    <tr>
                                                        <td>&nbsp;<asp:Label ID="lblAccName4" ForeColor="Blue" Font-Size="8pt" Font-Bold="True"  runat="server" /></td>
                                                        <td><asp:TextBox ID="vxtOther4" AutoPostBack="True" runat="server" /><asp:Button ID="btnF44" Text="F4" Width="40px" CssClass="btn btn-primary" runat="server" /><asp:Label ID="lblOtherName4" ForeColor="Blue" Font-Size="8pt" Font-Bold="True"  runat="server" /></td>
                                                        <td></td>
                                                        <td></td>
                                                    </tr>
                                                      <tr>
                                                        <td><asp:TextBox ID="vxtAccno5" Width="190px"  AutoPostBack="True" runat="server" /></td>
                                                        <td><asp:TextBox ID="txtRemark5" Width="520px"  runat="server" /></td>
                                                        <td><asp:TextBox ID="txtAmt5" Width="100px" style="text-align:right" runat="server" /></td>
                                                        <td><asp:TextBox ID="txtCode5" Width="40px" runat="server" /></td>
                                                    </tr>
                                                    <tr>
                                                        <td>&nbsp;<asp:Label ID="lblAccName5" ForeColor="Blue" Font-Size="8pt" Font-Bold="True"  runat="server" /></td>
                                                        <td><asp:TextBox ID="vxtOther5" AutoPostBack="True" runat="server" /><asp:Button ID="btnF45" Text="F4" Width="40px" CssClass="btn btn-primary" runat="server" /><asp:Label ID="lblOtherName5" ForeColor="Blue" Font-Size="8pt" Font-Bold="True"  runat="server" /></td>
                                                        <td></td>
                                                        <td></td>
                                                    </tr>
                                                      <tr>
                                                        <td><asp:TextBox ID="vxtAccno6" Width="190px"  AutoPostBack="True" runat="server" /></td>
                                                        <td><asp:TextBox ID="txtRemark6" Width="520px"  runat="server" /></td>
                                                        <td><asp:TextBox ID="txtAmt6" Width="100px" style="text-align:right" runat="server" /></td>
                                                        <td><asp:TextBox ID="txtCode6" Width="40px" runat="server" /></td>
                                                    </tr>
                                                    <tr>
                                                        <td>&nbsp;<asp:Label ID="lblAccName6" ForeColor="Blue" Font-Size="8pt" Font-Bold="True"  runat="server" /></td>
                                                        <td><asp:TextBox ID="vxtOther6" AutoPostBack="True" runat="server" /><asp:Button ID="btnF46" Text="F4" Width="40px" CssClass="btn btn-primary" runat="server" /><asp:Label ID="lblOtherName6" ForeColor="Blue" Font-Size="8pt" Font-Bold="True"  runat="server" /></td>
                                                        <td></td>
                                                        <td></td>
                                                    </tr>
                                                  </table>    
                                                     
                                                    <asp:Panel ID="gbxQty" GroupingText="材料數量23456項" runat="server">
                                                        <table>
                                                            <tr>
                                                                <td><asp:TextBox ID="txtQty2" Width="80px" runat="server" /></td>
                                                                <td><asp:TextBox ID="txtQty3" Width="80px" runat="server" /></td>
                                                                <td><asp:TextBox ID="txtQty4" Width="80px" runat="server" /></td>
                                                                <td><asp:TextBox ID="txtQty5" Width="80px" runat="server" /></td>
                                                                <td><asp:TextBox ID="txtQty6" Width="80px" runat="server" /></td>                                                
                                                            </tr>
                                                        </table>
                                                    </asp:Panel>
                                                    <table>
                                                        <tr>
                                                            <td><asp:Button ID="btnIntCopy" Text="傳票印一份" CssClass="btn btn-primary" runat="server" /></td>
                                                            <td><asp:Button ID="btnFinish" Text="確定" CssClass="btn btn-primary" runat="server" />
                                                                        <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                                                                <ContentTemplate></ContentTemplate>
                                                                <Triggers>
                                                                    <asp:PostBackTrigger ControlID="btnFinish" />
                                                                </Triggers>
                                                            </asp:UpdatePanel></td>
                                                            <td><asp:Button ID="btnReturn" Text="回上頁" CssClass="btn btn-primary" runat="server" /></td>
                                                        </tr>
                                                    </table>
                                                    
                                                   
                                                    <AjaxToolkit:MaskedEditExtender ID="MaskedEditExtender11" runat="server"
                                                        TargetControlID="vxtAccno1" Mask="N-NNNN" BehaviorID="_content_MaskedEditExtender11" Century="2000" CultureAMPMPlaceholder="" CultureCurrencySymbolPlaceholder="" CultureDateFormat="" CultureDatePlaceholder="" CultureDecimalPlaceholder="" CultureThousandsPlaceholder="" CultureTimePlaceholder="" />
                                                    
                                                    <AjaxToolkit:AutoCompleteExtender 
                                                            ID="AutoCompleteExtender2"                             
                                                            runat="server"                
                                                            TargetControlID="vxtAccno2"
                                                            ServicePath="~/active/WebService.asmx"
                                                            ServiceMethod="GetAC010Accno"
                                                            MinimumPrefixLength="0" 
                                                            CompletionInterval="100"
                                                            CompletionSetCount="12" DelimiterCharacters="" BehaviorID="_content_AutoCompleteExtender2" />
                                                    <AjaxToolkit:AutoCompleteExtender 
                                                            ID="AutoCompleteExtender3"                             
                                                            runat="server"                
                                                            TargetControlID="vxtAccno3"
                                                            ServicePath="~/active/WebService.asmx"
                                                            ServiceMethod="GetAC010Accno"
                                                            MinimumPrefixLength="0" 
                                                            CompletionInterval="100"
                                                            CompletionSetCount="12" DelimiterCharacters="" BehaviorID="_content_AutoCompleteExtender3" />
                                                    <AjaxToolkit:AutoCompleteExtender 
                                                            ID="AutoCompleteExtender4"                             
                                                            runat="server"                
                                                            TargetControlID="vxtAccno4"
                                                            ServicePath="~/active/WebService.asmx"
                                                            ServiceMethod="GetAC010Accno"
                                                            MinimumPrefixLength="0" 
                                                            CompletionInterval="100"
                                                            CompletionSetCount="12" DelimiterCharacters="" BehaviorID="_content_AutoCompleteExtender4" />
                                                    <AjaxToolkit:AutoCompleteExtender 
                                                            ID="AutoCompleteExtender5"                             
                                                            runat="server"                
                                                            TargetControlID="vxtAccno5"
                                                            ServicePath="~/active/WebService.asmx"
                                                            ServiceMethod="GetAC010Accno"
                                                            MinimumPrefixLength="0" 
                                                            CompletionInterval="100"
                                                            CompletionSetCount="12" DelimiterCharacters="" BehaviorID="_content_AutoCompleteExtender5" />
                                                    <AjaxToolkit:AutoCompleteExtender 
                                                            ID="AutoCompleteExtender6"                             
                                                            runat="server"                
                                                            TargetControlID="vxtAccno6"
                                                            ServicePath="~/active/WebService.asmx"
                                                            ServiceMethod="GetAC010Accno"
                                                            MinimumPrefixLength="0" 
                                                            CompletionInterval="100"
                                                            CompletionSetCount="12" DelimiterCharacters="" BehaviorID="_content_AutoCompleteExtender6" />

                                                    
                                                                                             
                                                </fieldset>
                                            </div>
                                        </div>
                                    </ContentTemplate>
                                </AjaxToolkit:TabPanel>
                               
                            </AjaxToolkit:TabContainer>
                            <div style="padding-bottom:1px;">&nbsp;</div>                            
                        </article>
                    </div>
                </section>
            </div>
            <!--控制項-->
                            <div style="margin:10px 0px 10px 0px;">
                                <uc1:UCBase ID="UCBase1" runat="server" />
                            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
