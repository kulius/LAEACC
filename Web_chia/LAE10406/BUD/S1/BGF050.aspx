<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="BGF050.aspx.vb" Inherits="LAEACC.BGF050" %>
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
                            <!-- 查詢項目值 -->                             
                            <div class="easyui-panel" style="width:100%;" title="查詢條件值" data-options="iconCls:'icon-search'">
                                <table id="table-serch" rules="all">
                                    <tr>
                                        <th>年度：</th>
                                        <td><asp:TextBox ID="nudYear" TextMode="Number" runat="server" min="0" max="999" step="1" /></td>
                                    </tr>
                                    <tr>
                                        <td colspan="2" style="text-align:center;">
                                            <asp:Button ID="BtnSearch" Text="查詢" CssClass="btn btn-primary" runat="server" />
                                            <asp:Button ID="btnClear" Text="清除條件" CssClass="btn btn-primary" runat="server" />
                                        </td>
                                    </tr>
                                </table>
                            </div> 

                            <!--控制項-->
                            <div style="margin:5px 0px 5px 0px;">
                                <uc1:UCBase ID="UCBase1" runat="server" />
                            </div>
                            <div style="clear:both; height:5px;"></div>

                            <!--詳細內容顯示區-->                           
                            <AjaxToolkit:TabContainer ID="TabContainer1" Width="100%" CssClass="Tab" runat="server" ActiveTabIndex="0">
                                <AjaxToolkit:TabPanel ID="TabPanel1" runat="server">
                                    <HeaderTemplate>多筆瀏灠</HeaderTemplate>
                                    <ContentTemplate>
                                        <div class="table-responsive">
                                            <div style="float:right; padding-right:10px;">
                                                共<asp:Label ID="lbl_GrdCount" ForeColor="Red" Font-Size="14" Font-Bold="true" Text="0" runat="server" />筆符合&nbsp;                                                
                                                <asp:Label ID="lbl_sort" runat="server" />
                                            </div>
                                            <asp:DataGrid ID="DataGridView" Width = "100%" AllowSorting="true" AllowPaging="false" style="font-size:14px;" CssClass="table table-bordered table-condensed smart-form" runat="server" >
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
                                                    
                                                    <asp:TemplateColumn HeaderText="收入日期" HeaderStyle-Width="80" ItemStyle-HorizontalAlign="Center" SortExpression="DATE2">
                                                        <itemtemplate><asp:Label ID="收入日期" Text='<%# Master.Models.strDateADToChiness(Container.DataItem("DATE2").ToShortDateString.ToString)%>' runat="server" /></itemtemplate>
                                                    </asp:TemplateColumn>                                                                                
                                                    <asp:TemplateColumn HeaderText="會計科目" HeaderStyle-Width="100" SortExpression="accno">
                                                        <itemtemplate><asp:Label ID="會計科目" Text='<%# Container.DataItem("accno").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>                                                   
                                                    <asp:TemplateColumn HeaderText="科目名稱" HeaderStyle-Width="160" SortExpression="accname">
                                                        <itemtemplate><asp:Label ID="科目名稱" Text='<%# Container.DataItem("accname").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="金額" HeaderStyle-Width="100" ItemStyle-HorizontalAlign="right" SortExpression="AMT">
                                                        <itemtemplate><asp:Label ID="金額" Text='<%# FormatNumber(Container.DataItem("AMT").ToString, 2)%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="摘要" HeaderStyle-Width="220" SortExpression="REMARK">
                                                        <itemtemplate><asp:Label ID="摘要" Text='<%# Container.DataItem("REMARK").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn> 
                                                    <asp:TemplateColumn HeaderText="傳票編號" HeaderStyle-Width="80" ItemStyle-HorizontalAlign="Center" SortExpression="NO_1_NO">
                                                        <itemtemplate><asp:Label ID="傳票編號" Text='<%# Container.DataItem("NO_1_NO").ToString%>' runat="server" /></itemtemplate>                                                       
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
                                                <th>收入日期：</th>
                                                <td>
                                                    <asp:TextBox ID="dtpDate2" Width="80" onClick="WdatePicker({dateFmt:'yyy-MM-dd',skin:'whyGreen'})" runat="server" />
                                                    <asp:Label ID="lblkey" ForeColor="Blue" Font-Size="12pt" Font-Bold="True" Visible="False" runat="server" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <th>科目編號：</th>
                                                <td>
                                                    <asp:DropDownList ID="cboAccno" CssClass="form-control" AutoPostBack="True" runat="server" />
                                                    <asp:Label ID="lblAccno" ForeColor="Blue" Font-Size="12pt" Font-Bold="True"  runat="server" />

                                                    <asp:Label ID="lblAccname" ForeColor="Blue" Font-Size="12pt" Font-Bold="True"  runat="server" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <th>摘要：</th>
                                                <td>
                                                    <asp:TextBox ID="txtRemark" CssClass="form-control" runat="server" />
                                                    <asp:Label ID="lblUseAmt" ForeColor="Blue" Font-Size="12pt" Font-Bold="True" runat="server" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <th>金額：</th>
                                                <td>
                                                    <asp:TextBox ID="txtAmt" CssClass="form-control" Width="25%" runat="server" />
                                                    <asp:Label ID="Label2" ForeColor="Blue" Font-Size="12pt" Font-Bold="True" runat="server" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <th>傳票編號：</th>
                                                <td>
                                                    <asp:TextBox ID="txtNo1" CssClass="form-control" Width="40%" runat="server" />
                                                    <asp:Label ID="Label3" ForeColor="Blue" Font-Size="12pt" Font-Bold="True" runat="server" />
                                                </td>
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
