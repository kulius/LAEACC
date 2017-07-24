<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="PAY021.aspx.vb" Inherits="LAEACC.PAY021" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="AjaxToolkit" %>

<%@ MasterType VirtualPath="~/Site.master" %>
<%@ Register SRC="~/LAE10406/UserControl/UCBase.ascx" TagName="UCBase" TagPrefix="uc1" %>
<%@ Register SRC="~/LAE10406/UserControl/AccText.ascx" TagName="AccText" TagPrefix="Acc1" %>

<asp:Content ID="Head" ContentPlaceHolderID="MainHead" runat="server">

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
                                                  
                                       
                            <!--詳細內容顯示區-->                           
                            <AjaxToolkit:TabContainer ID="TabContainer1" Width="100%" CssClass="Tab" runat="server" ActiveTabIndex="1">
                                <AjaxToolkit:TabPanel ID="TabPanel1" runat="server">
                                    <HeaderTemplate>來源傳票</HeaderTemplate>
                                    <ContentTemplate>
                                        <table id="table-data" rules="all">
                                            <tr>
                                                <th style="color:#00f;">請輸入製票編號：</th>
                                                <td colspan="5">
                                                    <asp:TextBox ID="txtNo1" runat="server" />
                                                    <asp:Button ID="btnSureNo" Text="確定" CssClass="btn btn-primary" runat="server" />
                                                    <asp:Button ID="btnSure" Text="開立支票" CssClass="btn btn-primary" runat="server" />
                                                    <asp:Button ID="btnSureTR" Visible="false" Text="開立電子支票" CssClass="btn btn-primary" runat="server" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <th>傳票：</th>
                                                <td><asp:Label ID="lblNo1" ForeColor="Blue" Font-Size="12pt" Font-Bold="True"  runat="server" /></td>
                                                <th>金額：</th>
                                                <td><asp:Label ID="lblAct_Amt" ForeColor="Blue" Font-Size="12pt" Font-Bold="True"  runat="server" /></td>
                                                <th>銀行：</th>
                                                <td>
                                                    <asp:Label ID="lblBank" ForeColor="Blue" Font-Size="12pt" Font-Bold="True"  runat="server" />
                                                    <asp:Label ID="lblMsg" ForeColor="Blue" Font-Size="12pt" Font-Bold="True"  runat="server" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <th>摘要：</th>
                                                <td colspan="5"><asp:Label ID="lblRemark" ForeColor="Blue" Font-Size="12pt" Font-Bold="True"  runat="server" /></td>
                                            </tr>
                                        </table>
                                        <div style="line-height:10px;"></div>

                                        <asp:DataGrid ID="DataGridView" Width="100%" runat="server">
                                            <Columns>
                                                <asp:TemplateColumn HeaderText="管理" HeaderStyle-Width="40px" ItemStyle-HorizontalAlign="Center">
                                                    <ItemTemplate>
                                                        <asp:ImageButton ID="Delete" AlternateText="刪除" ImageUrl="~/active/images/icon/items/delete.png" CommandName="btnDelete" runat="server" />
                                                        <asp:Label ID="id" Text='<%# Container.DataItem("no_1_no").ToString%>' Visible="false" runat="server" />
                                                        <asp:UpdatePanel ID="DataGridView_UpdatePanel" runat="server">
                                                            <Triggers>
                                                                <asp:AsyncPostBackTrigger ControlID="Delete" EventName="Click" />
                                                            </Triggers>
                                                        </asp:UpdatePanel>
                                                    </ItemTemplate>

                                                </asp:TemplateColumn>
                                                <asp:TemplateColumn HeaderText="傳票號">
                                                    <ItemTemplate>
                                                        <asp:Label ID="傳票號" Text='<%# Container.DataItem("no_1_no").ToString%>' runat="server" /></ItemTemplate>
                                                    <HeaderStyle Width="60px" />
                                                </asp:TemplateColumn>
                                                <asp:TemplateColumn HeaderText="摘要">
                                                    <ItemTemplate>
                                                        <asp:Label ID="摘要" Text='<%# Container.DataItem("REMARK").ToString%>' runat="server" /></ItemTemplate>
                                                    <HeaderStyle Width="120px" />
                                                </asp:TemplateColumn>
                                                <asp:TemplateColumn HeaderText="實付數">
                                                    <ItemTemplate>
                                                        <asp:Label ID="實付數" Text='<%# FormatNumber(Container.DataItem("act_amt").ToString, 0)%>' runat="server" /></ItemTemplate>
                                                    <HeaderStyle Width="120px" />
                                                    <ItemStyle HorizontalAlign="Right" />
                                                </asp:TemplateColumn>

                                                <asp:TemplateColumn HeaderText="銀行">
                                                    <ItemTemplate>
                                                        <asp:Label ID="銀行" Text='<%# Container.DataItem("bank").ToString%>' runat="server" /></ItemTemplate>
                                                    <HeaderStyle Width="50px" />
                                                </asp:TemplateColumn>
                                            </Columns>
                                        </asp:DataGrid>
                                        <div style="line-height:10px;"></div>

                                        <table id="table-data" rules="all">
                                            <tr>
                                                <th>帳戶餘額：</th>
                                                <td><asp:Label ID="lblBalance" ForeColor="Blue" Font-Size="12pt" Font-Bold="True"  runat="server" /></td>
                                                <th>支票總額：</th>
                                                <td><asp:Label ID="lblTotAmt" ForeColor="Blue" Font-Size="12pt" Font-Bold="True"  runat="server" /></td>
                                                <th>傳票張數：</th>
                                                <td><asp:Label ID="lblTotNo" ForeColor="Blue" Font-Size="12pt" Font-Bold="True"  runat="server" /></td>
                                            </tr>
                                        </table>
                                    </ContentTemplate>
                                </AjaxToolkit:TabPanel>                               
                                <AjaxToolkit:TabPanel ID="TabPanel2" runat="server">
                                    <HeaderTemplate>開立支票</HeaderTemplate>
                                    <ContentTemplate>
                                        <table id="table-data" rules="all">
                                            <tr>
                                                <th>銀行：</th>
                                                <td><asp:Label ID="lblBankName" ForeColor="Blue" Font-Size="12pt" Font-Bold="True"  runat="server" /></td>
                                            </tr>
                                            <tr>
                                                <th>支票金額：</th>
                                                <td><asp:Label ID="lblTotamt2" ForeColor="Blue" Font-Size="12pt" Font-Bold="True"  runat="server" /></td>
                                            </tr>
                                            <tr>
                                                <th>支票號碼：</th>
                                                <td>
                                                    <asp:TextBox ID="txtChkNo" runat="server" />
                                                    <asp:Label ID="lblChkTR" ForeColor="Blue" Font-Size="12pt" Font-Bold="True"  runat="server" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <th>受款人：：</th>
                                                <td>
                                                    <asp:TextBox ID="txtChkname" Width="520px" runat="server" />
                                                    <AjaxToolkit:AutoCompleteExtender 
                                                            ID="AutoCompleteExtender1"                             
                                                            runat="server"                
                                                            TargetControlID="txtChkname"
                                                            ServicePath="~/active/WebService.asmx"
                                                            ServiceMethod="GetPayRemark"
                                                            MinimumPrefixLength="0" 
                                                            CompletionInterval="100"
                                                            CompletionSetCount="12" DelimiterCharacters="" BehaviorID="_content_AutoCompleteExtender1" />
                                                    <asp:Button ID="btnAddPsname" Text="將受款人增入片語" CssClass="btn btn-primary" runat="server" />
                                                    <!--#伺服器驗證-->
                                                    <AjaxToolkit:ConfirmButtonExtender ID="ConfirmButtonExtender2" runat="server"
                                                        TargetControlID="btnAddPsname" ConfirmText="您是否將受款人新增至片語檔??" BehaviorID="_content_ConfirmButtonExtender2">
                                                    </AjaxToolkit:ConfirmButtonExtender>
                                                </td>
                                            </tr>
                                            <tr>
                                                <th>摘要：</th>
                                                <td><asp:TextBox ID="txtRemark" Width="520px" runat="server" /></td>
                                            </tr>
                                            <tr>
                                                <td style="text-align:center;" colspan="2">
                                                    <asp:CheckBox runat="server" ID="ckprint1" Text="劃線"/>
                                                    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                                    <asp:CheckBox runat="server" ID="ckprint2" Text="禁止背書轉讓"/>
                                                    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                                    
                                                
                                                <asp:CheckBox runat="server" ID="ckprint3" Text="支票日期"/><asp:TextBox ID="txtDate" Width="100px"  onClick="WdatePicker({dateFmt:'yyy-MM-dd',skin:'whyGreen'})" runat="server" /></td>
                                            </tr>
                                            <tr>
                                                <td colspan="2" style="text-align:center;">
                                                    <asp:Button ID="btnFinish" Text="確定" CssClass="btn btn-primary" runat="server" />
                                                    <asp:Button ID="btnBack" Text="放棄" CssClass="btn btn-primary" runat="server" />
                                                    <asp:Button ID="btnPrintSet" Text="印表設定" CssClass="btn btn-primary" runat="server" />
                                                </td>
                                                <asp:UpdatePanel ID="UpdatePanel3" runat="server">
                                                                <ContentTemplate></ContentTemplate>
                                                                <Triggers>
                                                                    <asp:PostBackTrigger ControlID="btnFinish" />
                                                                </Triggers>
                                                            </asp:UpdatePanel> 
                                            </tr>
                                        </table>
                                    </ContentTemplate>
                                </AjaxToolkit:TabPanel>
                                <AjaxToolkit:TabPanel ID="TabPanel3" runat="server">
                                    <HeaderTemplate>印表設定</HeaderTemplate>
                                    <ContentTemplate>
                                        <table id="table-data" rules="all">                                
                                            <tr>   
                                                <th>所屬銀行</th>                                             
                                                <td colspan="3"><asp:DropDownList ID="cbobank" AutoPostBack="True" runat="server" /></td>
                                                <td><asp:Button ID="btnPrintSerach" Text="查詢" CssClass="btn btn-primary" runat="server" /></td>
                                                <th>中文字型名稱</th>                                             
                                                <td colspan="3"><asp:TextBox ID="中文字型名稱" runat="server"  /></td>
                                                <th>公司全銜</th>                                             
                                                <td colspan="3"><asp:TextBox ID="公司全銜" runat="server" /></td>
                                            </tr>
                                            <tr> 
                                                <th>左側日期</th>    
                                                <th>下</th>                                             
                                                <td><asp:TextBox ID="左側日期X" runat="server" Width="40px" /></td>
                                                <th>右</th>                                             
                                                <td><asp:TextBox ID="左側日期Y" runat="server" Width="40px" /></td>
                                                <th>寬</th>                                             
                                                <td><asp:TextBox ID="左側日期W" runat="server" Width="40px" /></td>
                                                <th>高</th>                                             
                                                <td><asp:TextBox ID="左側日期H" runat="server" Width="40px" /></td>
                                                <th>字體大小</th>                                             
                                                <td><asp:TextBox ID="左側日期字體大小" runat="server" Width="40px" /></td>
                                                <th>是否列印</th>                                             
                                                <td><asp:TextBox ID="左側日期要列印" runat="server" Width="40px" /></td>
                                            </tr>
                                            <tr> 
                                                <th>左側受款人</th>    
                                                <th>下</th>                                             
                                                <td><asp:TextBox ID="左側受款人X" runat="server" Width="40px" /></td>
                                                <th>右</th>                                             
                                                <td><asp:TextBox ID="左側受款人Y" runat="server" Width="40px" /></td>
                                                <th>寬</th>                                             
                                                <td><asp:TextBox ID="左側受款人W" runat="server" Width="40px" /></td>
                                                <th>高</th>                                             
                                                <td><asp:TextBox ID="左側受款人H" runat="server" Width="40px" /></td>
                                                <th>字體大小</th>                                             
                                                <td><asp:TextBox ID="左側受款人字體大小" runat="server" Width="40px" /></td>
                                                <th>是否列印</th>                                             
                                                <td><asp:TextBox ID="左側受款人要列印" runat="server" Width="40px" /></td>
                                            </tr>
                                            <tr> 
                                                <th>左側金額</th>    
                                                <th>下</th>                                             
                                                <td><asp:TextBox ID="左側金額X" runat="server" Width="40px" /></td>
                                                <th>右</th>                                             
                                                <td><asp:TextBox ID="左側金額Y" runat="server" Width="40px" /></td>
                                                <th>寬</th>                                             
                                                <td><asp:TextBox ID="左側金額W" runat="server" Width="40px" /></td>
                                                <th>高</th>                                             
                                                <td><asp:TextBox ID="左側金額H" runat="server" Width="40px" /></td>
                                                <th>字體大小</th>                                             
                                                <td><asp:TextBox ID="左側金額字體大小" runat="server" Width="40px" /></td>
                                                <th>是否列印</th>                                             
                                                <td><asp:TextBox ID="左側金額要列印" runat="server" Width="40px" /></td>
                                            </tr>

                                            <tr> 
                                                <th>左側用途</th>    
                                                <th>下</th>                                             
                                                <td><asp:TextBox ID="左側用途X" runat="server" Width="40px" /></td>
                                                <th>右</th>                                             
                                                <td><asp:TextBox ID="左側用途Y" runat="server" Width="40px" /></td>
                                                <th>寬</th>                                             
                                                <td><asp:TextBox ID="左側用途W" runat="server" Width="40px" /></td>
                                                <th>高</th>                                             
                                                <td><asp:TextBox ID="左側用途H" runat="server" Width="40px" /></td>
                                                <th>字體大小</th>                                             
                                                <td><asp:TextBox ID="左側用途字體大小" runat="server" Width="40px" /></td>
                                                <th>是否列印</th>                                             
                                                <td><asp:TextBox ID="左側用途要列印" runat="server" Width="40px" /></td>
                                            </tr>

                                            <tr> 
                                                <th>左側餘額</th>    
                                                <th>下</th>                                             
                                                <td><asp:TextBox ID="左側餘額X" runat="server" Width="40px" /></td>
                                                <th>右</th>                                             
                                                <td><asp:TextBox ID="左側餘額Y" runat="server" Width="40px" /></td>
                                                <th>寬</th>                                             
                                                <td><asp:TextBox ID="左側餘額W" runat="server" Width="40px" /></td>
                                                <th>高</th>                                             
                                                <td><asp:TextBox ID="左側餘額H" runat="server" Width="40px" /></td>
                                                <th>字體大小</th>                                             
                                                <td><asp:TextBox ID="左側餘額字體大小" runat="server" Width="40px" /></td>
                                                <th>是否列印</th>                                             
                                                <td><asp:TextBox ID="左側餘額要列印" runat="server" Width="40px" /></td>
                                            </tr>
                                            <tr>
                                                <td colspan="13"></td>
                                            </tr>
                                            <tr> 
                                                <th>右側受款人</th>    
                                                <th>下</th>                                             
                                                <td><asp:TextBox ID="右側受款人X" runat="server" Width="40px" /></td>
                                                <th>右</th>                                             
                                                <td><asp:TextBox ID="右側受款人Y" runat="server" Width="40px" /></td>
                                                <th>寬</th>                                             
                                                <td><asp:TextBox ID="右側受款人W" runat="server" Width="40px" /></td>
                                                <th>高</th>                                             
                                                <td><asp:TextBox ID="右側受款人H" runat="server" Width="40px" /></td>
                                                <th>字體大小</th>                                             
                                                <td><asp:TextBox ID="右側受款人字體大小" runat="server" Width="40px" /></td>
                                                <th>是否列印</th>                                             
                                                <td><asp:TextBox ID="右側受款人要列印" runat="server" Width="40px" /></td>
                                            </tr>
                                            <tr> 
                                                <th>右側受款人</th>    
                                                <th>超過幾個字縮小</th>                                             
                                                <td><asp:TextBox ID="右側受款人字數臨界點" runat="server" Width="40px" /></td>
                                                <th>縮小字體大小</th>                                             
                                                <td><asp:TextBox ID="右側受款人字體大小臨界點" runat="server" Width="40px" /></td>
                                                <td colspan="8"></td>                                             
                                            </tr>
                                            
                                            <tr> 
                                                <th>右側金額</th>    
                                                <th>下</th>                                             
                                                <td><asp:TextBox ID="右側金額X" runat="server" Width="40px" /></td>
                                                <th>右</th>                                             
                                                <td><asp:TextBox ID="右側金額Y" runat="server" Width="40px" /></td>
                                                <th>寬</th>                                             
                                                <td><asp:TextBox ID="右側金額W" runat="server" Width="40px" /></td>
                                                <th>高</th>                                             
                                                <td><asp:TextBox ID="右側金額H" runat="server" Width="40px" /></td>
                                                <th>字體大小</th>                                             
                                                <td><asp:TextBox ID="右側金額字體大小" runat="server" Width="40px" /></td>
                                                <th>是否列印</th>                                             
                                                <td><asp:TextBox ID="右側金額要列印" runat="server" Width="40px" /></td>
                                            </tr>
                                            <tr> 
                                                <th>右側金額</th>    
                                                <th>超過幾個字縮小</th>                                             
                                                <td><asp:TextBox ID="右側金額字數臨界點" runat="server" Width="40px" /></td>
                                                <th>縮小字體大小</th>                                             
                                                <td><asp:TextBox ID="右側金額字體大小臨界點" runat="server" Width="40px" /></td>
                                                <th>小數點幾位</th>                                             
                                                <td><asp:TextBox ID="右側金額格子精確度" runat="server" Width="40px" /></td>
                                                <td colspan="6"></td>                                             
                                            </tr>
                                            <tr>
                                                <th>右側中文金額</th>    
                                                <th>下</th>                                             
                                                <td><asp:TextBox ID="右側中文金額X" runat="server" Width="40px" /></td>
                                                <th>右</th>                                             
                                                <td><asp:TextBox ID="右側中文金額Y" runat="server" Width="40px" /></td>
                                                <th>寬</th>                                             
                                                <td><asp:TextBox ID="右側中文金額W" runat="server" Width="40px" /></td>
                                                <th>高</th>                                             
                                                <td><asp:TextBox ID="右側中文金額H" runat="server" Width="40px" /></td>
                                                <th>字體大小</th>                                             
                                                <td><asp:TextBox ID="右側中文金額字體大小" runat="server" Width="40px" /></td>
                                                <th>是否列印</th>                                             
                                                <td><asp:TextBox ID="右側中文金額要列印" runat="server" Width="40px" /></td>
                                            </tr>
                                            <tr> 
                                                <th>右側中文金額</th>    
                                                <th>超過幾個字縮小</th>                                             
                                                <td><asp:TextBox ID="右側中文金額字數臨界點" runat="server" Width="40px" /></td>
                                                <th>縮小字體大小</th>                                             
                                                <td><asp:TextBox ID="右側中文金額字體大小臨界點" runat="server" Width="40px" /></td>
                                                <td colspan="8"></td>                                             
                                            </tr>
                                           <tr> 
                                                <th>右側日期</th>    
                                                <th>下</th>                                             
                                                <td><asp:TextBox ID="右側日期X" runat="server" Width="40px" /></td>
                                                <th>右</th>                                             
                                                <td><asp:TextBox ID="右側日期Y" runat="server" Width="40px" /></td>
                                                <th>寬</th>                                             
                                                <td><asp:TextBox ID="右側日期W" runat="server" Width="40px" /></td>
                                                <th>高</th>                                             
                                                <td><asp:TextBox ID="右側日期H" runat="server" Width="40px" /></td>
                                                <th>字體大小</th>                                             
                                                <td><asp:TextBox ID="右側日期字體大小" runat="server" Width="40px" /></td>
                                                <th>是否列印</th>                                             
                                                <td><asp:TextBox ID="右側日期要列印" runat="server" Width="40px" /></td>
                                            </tr> 
                                            <tr> 
                                                <th>禁止背書轉讓</th>    
                                                <th>下</th>                                             
                                                <td><asp:TextBox ID="禁止背書轉讓X" runat="server" Width="40px" /></td>
                                                <th>右</th>                                             
                                                <td><asp:TextBox ID="禁止背書轉讓Y" runat="server" Width="40px" /></td>
                                                <th>寬</th>                                             
                                                <td><asp:TextBox ID="禁止背書轉讓W" runat="server" Width="40px" /></td>
                                                <th>高</th>                                             
                                                <td><asp:TextBox ID="禁止背書轉讓H" runat="server" Width="40px" /></td>
                                                <th>字體大小</th>                                             
                                                <td><asp:TextBox ID="禁止背書轉讓字體大小" runat="server" Width="40px" /></td>
                                                <th>是否列印</th>                                             
                                                <td><asp:TextBox ID="禁止背書轉讓要列印" runat="server" Width="40px" /></td>
                                            </tr>
                                            <tr> 
                                                <th>禁止背書轉讓</th>    
                                                <th>樣式</th>                                             
                                                <td colspan="11"><asp:TextBox ID="禁止背書轉讓樣式" runat="server" Width="40px" /></td>
                                                
                                            </tr>
                                            <tr> 
                                                <th>雙斜線</th>    
                                                <th>下</th>                                             
                                                <td><asp:TextBox ID="雙斜線X" runat="server" Width="40px" /></td>
                                                <th>右</th>                                             
                                                <td><asp:TextBox ID="雙斜線Y" runat="server" Width="40px" /></td>
                                                <th>寬</th>                                             
                                                <td><asp:TextBox ID="雙斜線W" runat="server" Width="40px" /></td>
                                                <th>高</th>                                             
                                                <td><asp:TextBox ID="雙斜線H" runat="server" Width="40px" /></td>
                                                <th>字體大小</th>                                             
                                                <td><asp:TextBox ID="雙斜線字體大小" runat="server" Width="40px" /></td>
                                                <th>是否列印</th>                                             
                                                <td><asp:TextBox ID="雙斜線要列印" runat="server" Width="40px" /></td>
                                            </tr>
                                            <tr> 
                                                <th>公司全銜</th>    
                                                <th>下</th>                                             
                                                <td><asp:TextBox ID="公司全銜X" runat="server" Width="40px" /></td>
                                                <th>右</th>                                             
                                                <td><asp:TextBox ID="公司全銜Y" runat="server" Width="40px" /></td>
                                                <th>寬</th>                                             
                                                <td><asp:TextBox ID="公司全銜W" runat="server" Width="40px" /></td>
                                                <th>高</th>                                             
                                                <td><asp:TextBox ID="公司全銜H" runat="server" Width="40px" /></td>
                                                <th>字體大小</th>                                             
                                                <td><asp:TextBox ID="公司全銜字體大小" runat="server" Width="40px" /></td>
                                                <th>是否列印</th>                                             
                                                <td><asp:TextBox ID="公司全銜要列印" runat="server" Width="40px" /></td>
                                            </tr>

                                            <tr>   
                                                <th>測試抬頭</th>                                             
                                                <td colspan="3"><asp:TextBox ID="testChkname" runat="server"  /></td>
                                                <th>測試金額</th>                                             
                                                <td colspan="3"><asp:TextBox ID="testAmt" runat="server"  /></td>
                                                <th >測試摘要</th>                                             
                                                <td colspan="4"><asp:TextBox ID="testRemark" runat="server"  /></td>
                                                
                                            </tr>
                                            <tr>
                                                <td colspan="6"><asp:Button ID="btnPrintSave" Text="儲存" CssClass="btn btn-primary" runat="server" /></td>
                                                <td colspan="7"><asp:Button ID="btnPrintTes" Text="測試列印" CssClass="btn btn-primary" runat="server" /></td>
                                            </tr>
                                        </table>   
                                        <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                                                                <ContentTemplate></ContentTemplate>
                                                                <Triggers>
                                                                    <asp:PostBackTrigger ControlID="btnPrintTes" />
                                                                </Triggers>
                                                            </asp:UpdatePanel> 
                                    </ContentTemplate>
                                </AjaxToolkit:TabPanel>                               
                            </AjaxToolkit:TabContainer>
                            <div style="padding-bottom:1px;">&nbsp;</div>                            
                        </article>
                    </div>
                </section>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
