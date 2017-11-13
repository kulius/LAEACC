<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="PAY_ACF010.aspx.vb" Inherits="LAEACC.PAY_ACF010" %>
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
            <table id="table-serch" rules="all">                                            
                <tr>
                    <td style="text-align:center;">
                        年度：<asp:TextBox ID="nudYear"  Width="80" TextMode="Number" runat="server" min="0"  step="1" />
                    </td>                                                                                                
                    <td style="text-align:center;">
                        <asp:RadioButton id="rdbFile1" Text="未處理" GroupName="rdbFile" runat="server"/><asp:RadioButton id="rdbFile2" Text="己處理" Checked="true" GroupName="rdbFile" runat="server"/><asp:RadioButton id="rdbFile3" Text="全部" GroupName="rdbFile" runat="server"/>
                    </td>
                    <td style="text-align:center;">
                        <asp:RadioButton id="rdbKind1" Text="收入" GroupName="rdbKind" runat="server"/><asp:RadioButton id="rdbKind2" Text="支出" Checked="true" GroupName="rdbKind" runat="server"/><asp:RadioButton id="rdbKind3" Text="轉帳" GroupName="rdbKind" runat="server"/>
                    </td>
                    <td style="text-align:center;">
                        <span>起號：</span>
                        <asp:TextBox ID="TxtStartNo"  Width="80" TextMode="Number" runat="server" min="0"  step="1" />
                        <span>迄號：</span>
                        <asp:TextBox ID="TxtEndNo"  Width="80" TextMode="Number" runat="server" min="0"  step="1" />
                    </td>                                              
                    <td style="width:120px; text-align:center;">
                        <asp:Button ID="btnSearch" Text="蒐尋資料" CssClass="btn btn-primary" runat="server" />                                                   
                    </td>

                </tr>
                <tr>
                    <td style="text-align:center;">
                        科目代號：<asp:TextBox ID="txtQaccno"   runat="server"  />
                    </td>                                                                                                
                    <td style="text-align:center;">
                        摘要：<asp:TextBox ID="txtQremark"   runat="server"  />
                    </td>
                    <td style="text-align:center;">
                       金額：<asp:TextBox ID="txtQamt"   runat="server"  /> 
                    </td>
                    <td style="text-align:center;">
                       管理處：<asp:TextBox ID="txtQarea"   runat="server"  /> 
                    </td>                                              
                    <td style="width:120px; text-align:center;">
                       匯款通知單序號：<asp:TextBox ID="txtQpayseq"   runat="server"  /> 
                    </td>

                </tr>
            </table>
   
            <!--主項目區-->
            <div style="margin: 10px 0px 0px 10px;">
                <section id="widget-grid" style="width:98%;">
                    <div class="row">
                        <article class="col-sm-12 col-md-12 col-lg-12">
                            <!--控制項-->
                            <div style="margin:5px 0px 5px 0px;">
                                <uc1:UCBase ID="UCBase1" runat="server" />
                            </div>
                            <div style="clear:both; height:5px;"></div> 

                            <!--詳細內容顯示區-->                           
                            <AjaxToolkit:TabContainer ID="TabContainer1" Width="100%" CssClass="Tab" runat="server" ActiveTabIndex="1">
                                <AjaxToolkit:TabPanel ID="TabPanel1" runat="server">
                                    <HeaderTemplate>多筆瀏灠</HeaderTemplate>
                                    <ContentTemplate>
                                        <div class="table-responsive">
                                            &nbsp;&nbsp;
                                            <asp:Label ID="Label1" ForeColor="Red" Font-Size="14" Font-Bold="true" Text="0" runat="server" />
                                            <div style="float:right; padding-right:10px;">
                                                共<asp:Label ID="lbl_GrdCount" ForeColor="Red" Font-Size="14" Font-Bold="true" Text="0" runat="server" />筆符合&nbsp;                                                
                                                <asp:Label ID="lbl_sort" runat="server" />
                                            </div>
                                            <asp:DataGrid ID="DataGridView" Width="2000" AllowSorting="false" AllowPaging="true" style="font-size:14px;" CssClass="table table-bordered table-condensed smart-form" runat="server" >
                                                <columns>
                                                    <asp:TemplateColumn HeaderText="管理" HeaderStyle-Width="40" ItemStyle-HorizontalAlign="Center">
                                                        <itemtemplate>                                                                                                            
                                                            <asp:ImageButton ID="Show" AlternateText="查閱" ImageUrl="~/active/images/icon/items/zoom.png" CommandName="btnShow" runat="server" />
                                                            <asp:Label ID="id" Text='<%# Container.DataItem("autono").ToString%>' Visible="false" runat="server" />
                                                            <asp:UpdatePanel ID="DataGridView_UpdatePanel" runat="server">                                                                
                                                                <Triggers><asp:AsyncPostBackTrigger ControlID="Show" EventName="Click" /></Triggers>
                                                            </asp:UpdatePanel>
                                                        </itemtemplate>                                                         
                                                    </asp:TemplateColumn>
                                                                            
                                                    <asp:TemplateColumn HeaderText="年度" HeaderStyle-Width="32" ItemStyle-HorizontalAlign="Center">
                                                        <itemtemplate><asp:Label ID="年度" Text='<%# Container.DataItem("accyear").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>                                                   
                                                    <asp:TemplateColumn HeaderText="類別" HeaderStyle-Width="32" >
                                                        <itemtemplate><asp:Label ID="類別" Text='<%# Container.DataItem("kind").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>                                                    
                                                    <asp:TemplateColumn HeaderText="製票號" HeaderStyle-Width="50" ItemStyle-HorizontalAlign="right">
                                                        <itemtemplate><asp:Label ID="製票號" Text='<%# Container.DataItem("no_1_no").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="傳票號" HeaderStyle-Width="50" ItemStyle-HorizontalAlign="right">
                                                        <itemtemplate><asp:Label ID="傳票號" Text='<%# Container.DataItem("no_2_no").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="頁次" HeaderStyle-Width="20" ItemStyle-HorizontalAlign="right">
                                                        <itemtemplate><asp:Label ID="頁次" Text='<%# Container.DataItem("seq").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="項次" HeaderStyle-Width="20" ItemStyle-HorizontalAlign="right">
                                                        <itemtemplate><asp:Label ID="項次" Text='<%# Container.DataItem("item").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="製票日期" SortExpression="DATE_1" HeaderStyle-Width="70" ItemStyle-HorizontalAlign="Center">
                                                        <itemtemplate><asp:Label ID="製票日期" Text='<%# Master.Models.strDateADToChiness(Container.DataItem("DATE_1").ToString)%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="收付日期" SortExpression="DATE_2" HeaderStyle-Width="70" ItemStyle-HorizontalAlign="Center">
                                                        <itemtemplate><asp:Label ID="收付日期" Text='<%# Master.Models.strDateADToChiness(Container.DataItem("DATE_2").ToString)%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="借貸方" HeaderStyle-Width="32" ItemStyle-HorizontalAlign="Center">
                                                        <itemtemplate><asp:Label ID="借貸方" Text='<%# Container.DataItem("dc").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="會計科目" HeaderStyle-Width="60" ItemStyle-HorizontalAlign="Center">
                                                        <itemtemplate><asp:Label ID="會計科目" Text='<%# Container.DataItem("accno").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="摘要" HeaderStyle-Width="200" ItemStyle-HorizontalAlign="Center">
                                                        <itemtemplate><asp:Label ID="摘要" Text='<%# Container.DataItem("remark").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>   
                                                    <asp:TemplateColumn HeaderText="金額" HeaderStyle-Width="110" ItemStyle-HorizontalAlign="right">
                                                        <itemtemplate><asp:Label ID="金額" Text='<%# FormatNumber(Container.DataItem("amt").ToString, 0)%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>   
                                                    <asp:TemplateColumn HeaderText="實收付數" HeaderStyle-Width="110" ItemStyle-HorizontalAlign="right">
                                                        <itemtemplate><asp:Label ID="實收付數" Text='<%# FormatNumber(Container.DataItem("act_amt").ToString, 0)%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>   
                                                    <asp:TemplateColumn HeaderText="銀行" HeaderStyle-Width="32" ItemStyle-HorizontalAlign="Center">
                                                        <itemtemplate><asp:Label ID="銀行" Text='<%# Container.DataItem("bank").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>   
                                                    <asp:TemplateColumn HeaderText="支票號碼" HeaderStyle-Width="80" ItemStyle-HorizontalAlign="Center">
                                                        <itemtemplate><asp:Label ID="支票號碼" Text='<%# Container.DataItem("chkno").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>   
<%--                                                    <asp:TemplateColumn HeaderText="管理處" HeaderStyle-Width="75" ItemStyle-HorizontalAlign="Center">
                                                        <itemtemplate><asp:Label ID="管理處" Text='<%# Container.DataItem("area").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>  --%> 
                                                    <asp:TemplateColumn HeaderText="過帳碼" HeaderStyle-Width="32" ItemStyle-HorizontalAlign="Center">
                                                        <itemtemplate><asp:Label ID="過帳碼" Text='<%# Container.DataItem("books").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>   
<%--                                                    <asp:TemplateColumn HeaderText="匯款" HeaderStyle-Width="75" ItemStyle-HorizontalAlign="Center">
                                                        <itemtemplate><asp:Label ID="匯款" Text='<%# Container.DataItem("payseq").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>     --%>                                                                                                                                                            
                                                </columns>
                                            </asp:DataGrid>
                                        </div>
                                    </ContentTemplate>
                                </AjaxToolkit:TabPanel>

                                <AjaxToolkit:TabPanel ID="TabPanel2" runat="server">
                                    <HeaderTemplate>單筆明細</HeaderTemplate>
                                    <ContentTemplate>
                                        <table id="table-data" rules="all">
                                            <tr>
                                                <th>年度：</th>
                                                <td>
                                                    <asp:TextBox ID="txtYear" CssClass="form-control" Width="100px" runat="server" />
                                                    <asp:Label ID="lblkey" ForeColor="Blue" Font-Size="12pt" Font-Bold="True" Visible="False" runat="server" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <th>傳票類別：</th>
                                                <td><asp:TextBox ID="txtKind" CssClass="form-control" Width="80px" runat="server" /></td>
                                            </tr>
                                            <tr>
                                                <th>製票編號：</th>
                                                <td><asp:TextBox ID="txtNo1"  Width="80px" runat="server" />1:收 2:支 3:轉</td>
                                            </tr>
                                            <tr>
                                                <th>傳票編號：</th>
                                                <td><asp:TextBox ID="txtNo2" CssClass="form-control" Width="100px" runat="server" /></td>
                                            </tr>
                                            <tr>
                                                <th>頁次：</th>
                                                <td><asp:TextBox ID="txtSeq" CssClass="form-control" Width="100px"  runat="server" /></td>
                                            </tr>
                                            <tr>
                                                <th>項次：</th>
                                                <td><asp:TextBox ID="txtItem" CssClass="form-control" Width="100px" runat="server" /></td>
                                            </tr>
                                            <tr>
                                                <th>製票日期：</th>
                                                <td><asp:TextBox ID="txtDate1" Width="100px" CssClass="form-control " onClick="WdatePicker({dateFmt:'yyy-MM-dd',skin:'whyGreen'})" runat="server" /></td>
                                            </tr>
                                            <tr>
                                                <th>收付日期：</th>
                                                <td><asp:TextBox ID="txtDate2" Width="100px" CssClass="form-control " onClick="WdatePicker({dateFmt:'yyy-MM-dd',skin:'whyGreen'})" runat="server" /></td>
                                            </tr>
                                            <tr>
                                                <th>借貸方：</th>
                                                <td><asp:TextBox ID="txtDC"  Width="100px" runat="server" />1:借方　 2:貸方</td>
                                            </tr>
                                            <tr>
                                                <th>會計科目：</th>
                                                <td><asp:TextBox ID="vxtAccno"  Width="200px" runat="server" /><asp:Button ID="btnAccname" Text="名稱" runat="server" /><asp:Label ID="lblAccname"  ForeColor="Blue" Font-Size="12pt" Font-Bold="True"  runat="server" /></td>
                                                <AjaxToolkit:MaskedEditExtender ID="vxtAccno1_Mask" runat="server"
                                                            TargetControlID="vxtAccno"
                                                            Mask="N-NNNN" AutoCompleteValue=" " ClearTextOnInvalid="True"
                                                            Filtered=" " BehaviorID="_content_vxtAccno1_Mask" Century="2000" CultureAMPMPlaceholder="" CultureCurrencySymbolPlaceholder="" CultureDateFormat="" CultureDatePlaceholder="" CultureDecimalPlaceholder="" CultureThousandsPlaceholder="" CultureTimePlaceholder="" />
                                            </tr>
                                            <tr>
                                                <th>摘要：</th>
                                                <td><asp:TextBox ID="txtRemark" CssClass="form-control" runat="server" /></td>
                                            </tr>
                                            <tr>
                                                <th>金額：</th>
                                                <td><asp:TextBox ID="txtamt" CssClass="form-control" runat="server" /></td>
                                                <AjaxToolkit:MaskedEditExtender    ID="Amt1_Mask" runat="server"
                                                        TargetControlID="txtamt"   Mask="9,999,999,999"    MaskType="Number" 
                                                        InputDirection="RightToLeft" BehaviorID="_content_Amt1_Mask" Century="2000" CultureAMPMPlaceholder="" CultureCurrencySymbolPlaceholder="" CultureDateFormat="" CultureDatePlaceholder="" CultureDecimalPlaceholder="" CultureThousandsPlaceholder="" CultureTimePlaceholder="" />
                                            </tr>
                                            <tr>
                                                <th>實收付金額：</th>
                                                <td><asp:TextBox ID="txtActamt" CssClass="form-control" runat="server" /></td>
                                                <AjaxToolkit:MaskedEditExtender    ID="MaskedEditExtender1" runat="server"
                                                        TargetControlID="txtActamt"   Mask="9,999,999,999"    MaskType="Number" 
                                                        InputDirection="RightToLeft" BehaviorID="_content_MaskedEditExtender1" Century="2000" CultureAMPMPlaceholder="" CultureCurrencySymbolPlaceholder="" CultureDateFormat="" CultureDatePlaceholder="" CultureDecimalPlaceholder="" CultureThousandsPlaceholder="" CultureTimePlaceholder="" />
                                            </tr>
                                            <tr>
                                                <th>銀行：</th>
                                                <td><asp:TextBox ID="txtBank" CssClass="form-control" runat="server" /></td>
                                            </tr>
                                            <tr>
                                                <th>支票號碼：</th>
                                                <td><asp:TextBox ID="txtchkno" CssClass="form-control" runat="server" /></td>
                                            </tr>
                                            <tr>
                                                <th>管理處：</th>
                                                <td><asp:TextBox ID="txtArea" CssClass="form-control" runat="server" /></td>
                                            </tr>
                                            <tr>
                                                <th>匯款通知單序號：</th>
                                                <td><asp:TextBox ID="txtPayseq" CssClass="form-control" runat="server" /></td>
                                            </tr>
                                            <tr>
                                                <th>過帳碼：</th>
                                                <td><asp:TextBox ID="txtBooks"  runat="server" />Y:表示已過帳</td>
                                            </tr>
                                        </table>                                        
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
