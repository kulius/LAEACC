<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="CHF010.aspx.vb" Inherits="LAEACC.CHF010" %>
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
                        銀行：<asp:TextBox ID="txtStartBank"  Width="80" runat="server"  />
                    </td>                                                                                                
                    <td style="text-align:center;">
                        <asp:RadioButton id="rdbKind1" Text="收入" GroupName="rdbKind" runat="server"/><asp:RadioButton id="rdbKind2" Text="支出" Checked="true" GroupName="rdbKind" runat="server"/>
                    </td>
                    <td style="text-align:center;">
                        <asp:RadioButton id="rdbCheck1" Text="己領" GroupName="rdbCheck" Checked="true"  runat="server"/><asp:RadioButton id="rdbCheck2" Text="未領" GroupName="rdbCheck" runat="server"/>
                    </td>
                    <td style="text-align:center;">
                        支票起日<asp:TextBox ID="dtpStartDate" Width="80px"  onClick="WdatePicker({dateFmt:'yyy-MM-dd',skin:'whyGreen'})" runat="server" />
                    </td>
                    <td style="text-align:center;">
                        支票訖日<asp:TextBox ID="dtpEndDate" Width="80px"  onClick="WdatePicker({dateFmt:'yyy-MM-dd',skin:'whyGreen'})" runat="server" />
                    </td>
                    <td style="text-align:center;">
                        排序<asp:RadioButton id="rdbSortBank" Text="銀行" GroupName="rdbsort"  Checked="true" runat="server"/><asp:RadioButton id="rdbSortDate" Text="日期" GroupName="rdbsort" runat="server"/>
                    </td>

                </tr>
                <tr>
                    <td style="text-align:center;">
                        受款人：<asp:TextBox ID="txtQchkname"   runat="server"  />
                    </td>                                                                                                
                    <td style="text-align:center;">
                        摘要：<asp:TextBox ID="txtQremark"   runat="server"  />
                    </td>
                    <td style="text-align:center;">
                       金額：<asp:TextBox ID="txtQamt"   runat="server"  /> 
                    </td>
                    
                    <td style="text-align:center;">
                        <span>起號：</span>
                        <asp:TextBox ID="TxtStartNo"  Width="80"  runat="server"  />
                    </td>      
                    <td style="text-align:center;">
                        <span>迄號：</span>
                        <asp:TextBox ID="TxtEndNo"  Width="80"  runat="server"/>
                    </td>                                               
                    <td style="width:300px; text-align:center;">
                        <asp:Button ID="btnSearch" Text="蒐尋" runat="server" /> &nbsp; 
                        <asp:Button ID="btnExport" Text="匯出" runat="server" />                                                 
                    </td>

                    <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                        <ContentTemplate></ContentTemplate>
                        <Triggers>
                            <asp:PostBackTrigger ControlID="btnExport" />
                        </Triggers>
                    </asp:UpdatePanel> 


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
                                                                            
                                                    <asp:TemplateColumn HeaderText="年" HeaderStyle-Width="30" ItemStyle-HorizontalAlign="Center">
                                                        <itemtemplate><asp:Label ID="年" Text='<%# Container.DataItem("accyear").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>                                                   
                                                    <asp:TemplateColumn HeaderText="收支" HeaderStyle-Width="20" >
                                                        <itemtemplate><asp:Label ID="收支" Text='<%# Container.DataItem("kind").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>                                                    
                                                    <asp:TemplateColumn HeaderText="銀行" HeaderStyle-Width="30" >
                                                        <itemtemplate><asp:Label ID="銀行" Text='<%# Container.DataItem("bank").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="支票號" HeaderStyle-Width="80" ItemStyle-HorizontalAlign="Center">
                                                        <itemtemplate><asp:Label ID="支票號" Text='<%# Container.DataItem("chkno").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="開票日" HeaderStyle-Width="100" ItemStyle-HorizontalAlign="Center">
                                                        <itemtemplate><asp:Label ID="開票日" Text='<%# Master.Models.strDateADToChiness(Container.DataItem("date_1").ToShortDateString.ToString)%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="收付日" HeaderStyle-Width="100" ItemStyle-HorizontalAlign="Center">
                                                        <itemtemplate><asp:Label ID="收付日" Text='<%# Master.Models.strDateADToChiness(Container.DataItem("date_2").ToShortDateString.ToString)%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="受款人" HeaderStyle-Width="120" ItemStyle-HorizontalAlign="left">
                                                        <itemtemplate><asp:Label ID="受款人" Text='<%# Container.DataItem("chkname").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="金額" HeaderStyle-Width="90" ItemStyle-HorizontalAlign="Center">
                                                        <itemtemplate><asp:Label ID="金額" Text='<%# FormatNumber(Container.DataItem("amt").ToString, 0)%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>   
                                                    <asp:TemplateColumn HeaderText="摘要" HeaderStyle-Width="200" ItemStyle-HorizontalAlign="left">
                                                        <itemtemplate><asp:Label ID="摘要" Text='<%# Container.DataItem("remark").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>   
                                                    <asp:TemplateColumn HeaderText="傳票起號" HeaderStyle-Width="75" ItemStyle-HorizontalAlign="right">
                                                        <itemtemplate><asp:Label ID="傳票起號" Text='<%# Container.DataItem("start_no").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="傳票訖號" HeaderStyle-Width="75" ItemStyle-HorizontalAlign="right">
                                                        <itemtemplate><asp:Label ID="傳票訖號" Text='<%# Container.DataItem("end_no").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>     
                                                    <asp:TemplateColumn HeaderText="製票號" HeaderStyle-Width="75" ItemStyle-HorizontalAlign="right">
                                                        <itemtemplate><asp:Label ID="製票號" Text='<%# Container.DataItem("no_1_no").ToString%>' runat="server" /></itemtemplate>                                                       
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
                                                <th>年度：</th>
                                                <td>
                                                    <asp:TextBox ID="txtYear" CssClass="form-control" Width="100px" runat="server" />
                                                    <asp:Label ID="lblkey" ForeColor="Blue" Font-Size="12pt" Font-Bold="True" Visible="False" runat="server" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <th>支票類別：</th>
                                                <td><asp:TextBox ID="txtKind" CssClass="form-control" Width="80px" runat="server" />1:收 2:支</td>
                                            </tr>
                                            <tr>
                                                <th>銀行：</th>
                                                <td><asp:TextBox ID="txtBank"  Width="80px" runat="server" /></td>
                                            </tr>
                                            <tr>
                                                <th>支票號：</th>
                                                <td><asp:TextBox ID="txtChkno" CssClass="form-control" Width="100px" runat="server" /></td>
                                            </tr>

                                            <tr>
                                                <th>開票日：</th>
                                                <td><asp:TextBox ID="txtDate1" Width="100px" CssClass="form-control " onClick="WdatePicker({dateFmt:'yyy-MM-dd',skin:'whyGreen'})" runat="server" /></td>
                                            </tr>
                                            <tr>
                                                <th>收付日：</th>
                                                <td><asp:TextBox ID="txtDate2" Width="100px" CssClass="form-control " onClick="WdatePicker({dateFmt:'yyy-MM-dd',skin:'whyGreen'})" runat="server" /></td>
                                            </tr>
                                            <tr>
                                                <th>金額：</th>
                                                <td><asp:TextBox ID="txtamt" CssClass="form-control" runat="server" /></td>
                                                <AjaxToolkit:MaskedEditExtender    ID="Amt1_Mask" runat="server"
                                                        TargetControlID="txtamt"   Mask="9,999,999,999"    MaskType="Number" 
                                                        InputDirection="RightToLeft" BehaviorID="_content_Amt1_Mask" Century="2000" CultureAMPMPlaceholder="" CultureCurrencySymbolPlaceholder="" CultureDateFormat="" CultureDatePlaceholder="" CultureDecimalPlaceholder="" CultureThousandsPlaceholder="" CultureTimePlaceholder="" />
                                            </tr>

                                            <tr>
                                                <th>受款人：</th>
                                                <td><asp:TextBox ID="txtChkname" CssClass="form-control" Width="100px"  runat="server" /></td>
                                            </tr>
                                            <tr>
                                                <th>摘要：</th>
                                                <td><asp:TextBox ID="txtRemark" CssClass="form-control" Width="100px" runat="server" /></td>
                                            </tr>
                                            
                                            <tr>
                                                <th>傳票起號：</th>
                                                <td><asp:TextBox ID="txtStart_no" CssClass="form-control" runat="server" /></td>
                                            </tr>
                                            <tr>
                                                <th>傳票訖號：</th>
                                                <td><asp:TextBox ID="txtEnd_no" CssClass="form-control" runat="server" /></td>
                                            </tr>
                                            <tr>
                                                <th>製票編號：</th>
                                                <td><asp:TextBox ID="txtNO1" CssClass="form-control" runat="server" /></td>
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
