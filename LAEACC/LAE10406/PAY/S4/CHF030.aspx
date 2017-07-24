<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="CHF030.aspx.vb" Inherits="LAEACC.CHF030" %>
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
                        銀行起訖：<asp:TextBox ID="txtStartBank"  Width="80" runat="server"  />-<asp:TextBox ID="txtEndBank"  Width="80" runat="server"  />
                    </td>                                                                                                
                    <td style="text-align:center;">
                        起日<asp:TextBox ID="dtpStartDate" Width="80px"  onClick="WdatePicker({dateFmt:'yyy-MM-dd',skin:'whyGreen'})" runat="server" />
                    </td>
                    <td style="text-align:center;">
                        訖日<asp:TextBox ID="dtpEndDate" Width="80px"  onClick="WdatePicker({dateFmt:'yyy-MM-dd',skin:'whyGreen'})" runat="server" />
                    </td>
                    <td style="text-align:center;">
                        排序<asp:RadioButton id="rdbSortBank" Text="銀行" GroupName="rdbsort"  Checked="true" runat="server"/><asp:RadioButton id="rdbSortDate" Text="日期" GroupName="rdbsort" runat="server"/>
                    </td>
                    <td style="width:120px; text-align:center;">
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
                                                            <asp:Label ID="id" Text='<%# Container.DataItem("autono").ToString%>' Visible="false" runat="server" />
                                                            <asp:UpdatePanel ID="DataGridView_UpdatePanel" runat="server">                                                                
                                                                <Triggers><asp:AsyncPostBackTrigger ControlID="Show" EventName="Click" /></Triggers>
                                                            </asp:UpdatePanel>
                                                        </itemtemplate>                                                         
                                                    </asp:TemplateColumn>
                                                                            
                                                                                                      
                                                    <asp:TemplateColumn HeaderText="銀行" HeaderStyle-Width="30" >
                                                        <itemtemplate><asp:Label ID="銀行" Text='<%# Container.DataItem("bank").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>
                                                    
                                                    <asp:TemplateColumn HeaderText="收付日" HeaderStyle-Width="100" ItemStyle-HorizontalAlign="Center">
                                                        <itemtemplate><asp:Label ID="收付日" Text='<%# Master.Models.strDateADToChiness(Container.DataItem("date_2").ToShortDateString.ToString)%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="本日共收" HeaderStyle-Width="90" ItemStyle-HorizontalAlign="Center">
                                                        <itemtemplate><asp:Label ID="本日共收" Text='<%# FormatNumber(Container.DataItem("day_income").ToString, 0)%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>   
                                                    <asp:TemplateColumn HeaderText="本日共支" HeaderStyle-Width="90" ItemStyle-HorizontalAlign="Center">
                                                        <itemtemplate><asp:Label ID="本日共支" Text='<%# FormatNumber(Container.DataItem("day_pay").ToString, 0)%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="本日結存" HeaderStyle-Width="90" ItemStyle-HorizontalAlign="Center">
                                                        <itemtemplate><asp:Label ID="本日結存" Text='<%# FormatNumber(Container.DataItem("balance").ToString, 0)%>' runat="server" /></itemtemplate>                                                       
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
                                                <th>收付日期：</th>
                                                <td><asp:TextBox ID="dtpDate_2" Width="100px" CssClass="form-control " onClick="WdatePicker({dateFmt:'yyy-MM-dd',skin:'whyGreen'})" runat="server" /></td>
                                            </tr>
                                            
                                            <tr>
                                                <th>本日共收：</th>
                                                <td><asp:TextBox ID="txtDay_income" CssClass="form-control" runat="server" /></td>
                                                <AjaxToolkit:MaskedEditExtender    ID="Amt1_Mask" runat="server"
                                                        TargetControlID="txtDay_income"   Mask="9,999,999,999"    MaskType="Number" 
                                                        InputDirection="RightToLeft" BehaviorID="_content_Amt1_Mask" Century="2000" CultureAMPMPlaceholder="" CultureCurrencySymbolPlaceholder="" CultureDateFormat="" CultureDatePlaceholder="" CultureDecimalPlaceholder="" CultureThousandsPlaceholder="" CultureTimePlaceholder="" />
                                            </tr>
                                            <tr>
                                                <th>本日共支：</th>
                                                <td><asp:TextBox ID="txtDay_pay" CssClass="form-control" runat="server" /></td>
                                                <AjaxToolkit:MaskedEditExtender    ID="MaskedEditExtender1" runat="server"
                                                        TargetControlID="txtDay_pay"   Mask="9,999,999,999"    MaskType="Number" 
                                                        InputDirection="RightToLeft" BehaviorID="_content_Amt1_Mask" Century="2000" CultureAMPMPlaceholder="" CultureCurrencySymbolPlaceholder="" CultureDateFormat="" CultureDatePlaceholder="" CultureDecimalPlaceholder="" CultureThousandsPlaceholder="" CultureTimePlaceholder="" />
                                            </tr>
                                            <tr>
                                                <th>本日結存：</th>
                                                <td><asp:TextBox ID="txtBalance" CssClass="form-control" runat="server" /></td>
                                                <AjaxToolkit:MaskedEditExtender    ID="MaskedEditExtender2" runat="server"
                                                        TargetControlID="txtBalance"   Mask="9,999,999,999"    MaskType="Number" 
                                                        InputDirection="RightToLeft" BehaviorID="_content_Amt1_Mask" Century="2000" CultureAMPMPlaceholder="" CultureCurrencySymbolPlaceholder="" CultureDateFormat="" CultureDatePlaceholder="" CultureDecimalPlaceholder="" CultureThousandsPlaceholder="" CultureTimePlaceholder="" />
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
