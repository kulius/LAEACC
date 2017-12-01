<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="CHF040.aspx.vb" Inherits="LAEACC.CHF040" %>
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
                        起日<asp:TextBox ID="dtpDateS" Width="80px"  onClick="WdatePicker({dateFmt:'yyy-MM-dd',skin:'whyGreen'})" runat="server" />
                    </td>
                    <td style="width:120px; text-align:center;">
                        <asp:Button ID="btnSearch" Text="查詢" CssClass="btn btn-primary" runat="server" />                                                   
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
                                                                            
                                                                                                      
                             
                                                    <asp:TemplateColumn HeaderText="日期" HeaderStyle-Width="100" ItemStyle-HorizontalAlign="Center">
                                                        <itemtemplate><asp:Label ID="日期" Text='<%# Master.Models.strDateADToChiness(Container.DataItem("rdate").ToShortDateString.ToString)%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="繳款人" HeaderStyle-Width="170" >
                                                        <itemtemplate><asp:Label ID="繳款人" Text='<%# Container.DataItem("name").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn> 
                                                    <asp:TemplateColumn HeaderText="事由" HeaderStyle-Width="220" >
                                                        <itemtemplate><asp:Label ID="事由" Text='<%# Container.DataItem("because").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn> 
                                                    <asp:TemplateColumn HeaderText="金額" HeaderStyle-Width="90" ItemStyle-HorizontalAlign="right">
                                                        <itemtemplate><asp:Label ID="金額" Text='<%# FormatNumber(Container.DataItem("amt").ToString, 0)%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>   
                                                    <asp:TemplateColumn HeaderText="備註" HeaderStyle-Width="250" >
                                                        <itemtemplate><asp:Label ID="備註" Text='<%# Container.DataItem("REMARK").ToString%>' runat="server" /></itemtemplate>                                                       
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
                                            <caption>
                                                <tr>
                                                    <th>日期：</th>
                                                    <td>
                                                        <asp:TextBox ID="dtpRDate" runat="server" CssClass="form-control " onClick="WdatePicker({dateFmt:'yyy-MM-dd',skin:'whyGreen'})" Width="100px"></asp:TextBox>
                                                    </td>
                                                    <asp:Label ID="lblkey" runat="server" Font-Bold="True" Font-Size="12pt" ForeColor="Blue" Visible="False"></asp:Label>
                                                </tr>
                                                <tr>
                                                    <th>繳款人：</th>
                                                    <td>
                                                        <asp:TextBox ID="txtName" Width="200px" runat="server"></asp:TextBox>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <th>事由：</th>
                                                    <td>
                                                        <asp:TextBox ID="txtBecause" TextMode="MultiLine" Width="100%" Rows="5" runat="server"></asp:TextBox>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <th>備註欄：</th>
                                                    <td>
                                                        <asp:TextBox ID="txtRemark" TextMode="MultiLine" Width="100%" Rows="5" runat="server"></asp:TextBox>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <th>金額：</th>
                                                    <td>
                                                        <asp:TextBox ID="txtAmt" Width="120px" runat="server"></asp:TextBox>元
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td colspan="2">
                                                        <asp:Button ID="btnPrint" runat="server" CssClass="btn btn-info" Text="重印" />
                                                    </td>
                                                    <asp:UpdatePanel ID="UpdatePanel4" runat="server">
                                                        <ContentTemplate>
                                                        </ContentTemplate>
                                                        <Triggers>
                                                            <asp:PostBackTrigger ControlID="btnPrint" />
                                                        </Triggers>
                                                    </asp:UpdatePanel>
                                                </tr>
                                            </caption>
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
