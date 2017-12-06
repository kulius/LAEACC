<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="CHF020.aspx.vb" Inherits="LAEACC.CHF020" %>
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
                    <td style="width:120px; text-align:left;">
                        <asp:Button ID="btnSearch" Text="蒐尋資料" CssClass="btn btn-primary" runat="server" />                                                   
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
                                            <asp:DataGrid ID="DataGridView" Width="100%" AllowSorting="false" AllowPaging="true" style="font-size:14px;" CssClass="table table-bordered table-condensed smart-form" runat="server" >
                                                <columns>
                                                    <asp:TemplateColumn HeaderText="管理" HeaderStyle-Width="40" ItemStyle-HorizontalAlign="Center">
                                                        <itemtemplate>                                                                                                            
                                                            <asp:ImageButton ID="Show" AlternateText="查閱" ImageUrl="~/active/images/icon/items/zoom.png" CommandName="btnShow" runat="server" />
                                                            <asp:Label ID="id" Text='<%# Container.DataItem("bank").ToString%>' Visible="false" runat="server" />
                                                            <asp:UpdatePanel ID="DataGridView_UpdatePanel" runat="server">                                                                
                                                                <Triggers><asp:AsyncPostBackTrigger ControlID="Show" EventName="Click" /></Triggers>
                                                            </asp:UpdatePanel>
                                                        </itemtemplate>                                                         
                                                    </asp:TemplateColumn>
                                                                            
                                                
                                                    <asp:TemplateColumn HeaderText="銀行" HeaderStyle-Width="40" >
                                                        <itemtemplate><asp:Label ID="銀行" Text='<%# Container.DataItem("bank").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="帳號" HeaderStyle-Width="60" ItemStyle-HorizontalAlign="Center">
                                                        <itemtemplate><asp:Label ID="帳號" Text='<%# Container.DataItem("ACCOUNT").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="帳戶名稱" HeaderStyle-Width="150" ItemStyle-HorizontalAlign="Center">
                                                        <itemtemplate><asp:Label ID="帳戶名稱" Text='<%# Container.DataItem("BANKNAME").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="昨日結存" HeaderStyle-Width="90" ItemStyle-HorizontalAlign="right">
                                                        <itemtemplate><asp:Label ID="昨日結存" Text='<%# FormatNumber(Container.DataItem("BALANCE").ToString, 2)%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="本日共收" HeaderStyle-Width="75" ItemStyle-HorizontalAlign="right">
                                                        <itemtemplate><asp:Label ID="本日共收" Text='<%# FormatNumber(Container.DataItem("DAY_INCOME").ToString, 2)%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="本日共支" HeaderStyle-Width="75" ItemStyle-HorizontalAlign="right">
                                                        <itemtemplate><asp:Label ID="本日共支" Text='<%# FormatNumber(Container.DataItem("DAY_PAY").ToString, 2)%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>
                                                    
                                                    <asp:TemplateColumn HeaderText="收付日" HeaderStyle-Width="75" ItemStyle-HorizontalAlign="right">
                                                        <itemtemplate><asp:Label ID="收付日" Text='<%# Container.DataItem("DATE_2").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="列印否" HeaderStyle-Width="35" ItemStyle-HorizontalAlign="left">
                                                        <itemtemplate><asp:Label ID="列印否" Text='<%# Container.DataItem("PRT_CODE").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="已開未領支票" HeaderStyle-Width="75" ItemStyle-HorizontalAlign="right">
                                                        <itemtemplate><asp:Label ID="已開未領支票" Text='<%# FormatNumber(Container.DataItem("UNPAY").ToString, 2)%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>   
                                                    <asp:TemplateColumn HeaderText="目前支票號" HeaderStyle-Width="75" ItemStyle-HorizontalAlign="left">
                                                        <itemtemplate><asp:Label ID="目前支票號" Text='<%# Container.DataItem("CHKNO").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>   
                                                    <asp:TemplateColumn HeaderText="信貸額度" HeaderStyle-Width="75" ItemStyle-HorizontalAlign="right">
                                                        <itemtemplate><asp:Label ID="信貸額度" Text='<%# FormatNumber(Container.DataItem("CREDIT").ToString, 2)%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="記帳科目" HeaderStyle-Width="65" ItemStyle-HorizontalAlign="left">
                                                        <itemtemplate><asp:Label ID="記帳科目" Text='<%# Container.DataItem("ACCNO").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>     
                                                    <asp:TemplateColumn HeaderText="帳戶備注" HeaderStyle-Width="65" ItemStyle-HorizontalAlign="left">
                                                        <itemtemplate><asp:Label ID="帳戶備注" Text='<%# Container.DataItem("REMARK").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>     
                                                    <asp:TemplateColumn HeaderText="列印格式" HeaderStyle-Width="30" ItemStyle-HorizontalAlign="left">
                                                        <itemtemplate><asp:Label ID="列印格式" Text='<%# Container.DataItem("chkform").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>     
                                                     
                                                                                                                                                   
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
                                                <th>銀行：</th>
                                                <td>
                                                    <asp:TextBox ID="txtBank" CssClass="form-control" Width="100px" runat="server" />
                                                    <asp:Label ID="lblkey" ForeColor="Blue" Font-Size="12pt" Font-Bold="True" Visible="False" runat="server" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <th>記帳科目：</th>
                                                <td><asp:TextBox ID="vxtAccno"  Width="200px" runat="server" /><asp:Label ID="lblAccname"  ForeColor="Blue" Font-Size="12pt" Font-Bold="True"  runat="server" /></td>
                                                <AjaxToolkit:MaskedEditExtender ID="vxtAccno1_Mask" runat="server"
                                                            TargetControlID="vxtAccno"
                                                            Mask="N-NNNN" AutoCompleteValue=" " ClearTextOnInvalid="True"
                                                            Filtered=" " BehaviorID="_content_vxtAccno1_Mask" Century="2000" CultureAMPMPlaceholder="" CultureCurrencySymbolPlaceholder="" CultureDateFormat="" CultureDatePlaceholder="" CultureDecimalPlaceholder="" CultureThousandsPlaceholder="" CultureTimePlaceholder="" />

                                            </tr>
                                            <tr>
                                                <th>銀行帳號：</th>
                                                <td><asp:TextBox ID="txtAccount"  Width="80px" runat="server" /></td>
                                            </tr>
                                            <tr>
                                                <th>帳戶名稱：</th>
                                                <td><asp:TextBox ID="txtBankname" CssClass="form-control" Width="100px" runat="server" /></td>
                                            </tr>

                                            <tr>
                                                <th>收付日期：</th>
                                                <td><asp:TextBox ID="txtDate_2" Width="100px" CssClass="form-control " onClick="WdatePicker({dateFmt:'yyy-MM-dd',skin:'whyGreen'})" runat="server" /></td>
                                            </tr>
                                            <tr>
                                                <th>昨日結存：</th>
                                                <td><asp:TextBox ID="txtBalance" CssClass="form-control" runat="server" /></td>
                                            </tr>
                                            <tr>
                                                <th>本日共收：</th>
                                                <td><asp:TextBox ID="txtDay_income" CssClass="form-control" runat="server" /></td>
                                            </tr>
                                            <tr>
                                                <th>本日共支：</th>
                                                <td><asp:TextBox ID="txtDay_pay" CssClass="form-control" runat="server" /></td>
                                            </tr>
                                            <tr>
                                                <th>結存數：</th>
                                                <td><asp:Label ID="lblBalance"  ForeColor="Blue" Font-Size="12pt" Font-Bold="True"  runat="server" /></td>
                                            </tr>
                                            <tr>
                                                <th>已開未領支票額：</th>
                                                <td><asp:TextBox ID="txtUnpay" CssClass="form-control" runat="server" /></td>
                                            </tr>

                                            <tr>
                                                <th>已開支票號碼：</th>
                                                <td><asp:TextBox ID="txtChkno" CssClass="form-control" Width="100px"  runat="server" /></td>
                                            </tr>
                                            <tr>
                                                <th>信用貸款額度：</th>
                                                <td><asp:TextBox ID="txtCredit" CssClass="form-control" Width="100px" runat="server" /></td>
                                            </tr>
                                            
                                            <tr>
                                                <th>日計表列印碼：</th>
                                                <td><asp:TextBox ID="txtPrt_code" Width="80px" CssClass="form-control" runat="server" /></td>
                                            </tr>
                                            <tr>
                                                <th>備註欄：</th>
                                                <td><asp:TextBox ID="txtRemark" CssClass="form-control" runat="server" /></td>
                                            </tr>
                                            <tr>
                                                <th>支票列印格式代碼：</th>
                                                <td><asp:TextBox ID="txtChkForm" Width="80px" CssClass="form-control" runat="server" /></td>
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
