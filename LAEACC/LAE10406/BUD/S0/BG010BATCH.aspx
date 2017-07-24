<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="BG010BATCH.aspx.vb" Inherits="LAEACC.BG010BATCH" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="AjaxToolkit" %>

<%@ MasterType VirtualPath="~/Site.master" %>
<%@ Register SRC="~/LAE10406/UserControl/UCBase.ascx" TagName="UCBase" TagPrefix="uc1" %>

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
                            <!--控制項-->
                            <div style="margin:5px 0px 5px 0px; display:none;">
                                <uc1:UCBase ID="UCBase1" runat="server" />
                            </div>

                            <!--詳細內容顯示區-->                           
                            <AjaxToolkit:TabContainer ID="TabContainer1" Width="100%" CssClass="Tab" runat="server" ActiveTabIndex="1">
                                <AjaxToolkit:TabPanel ID="TabPanel2" runat="server">
                                    <HeaderTemplate>整批請購</HeaderTemplate>
                                    <ContentTemplate>
                                        <table id="table-data" rules="all">
                                            <tr>
                                                <th>請購科目：</th>
                                                <td><asp:DropDownList ID="cboAccno" CssClass="form-control" AutoPostBack="True" runat="server" /></td>                                                
                                            </tr>
                                            <tr>
                                                <th>請購日期：</th>
                                                <td><asp:TextBox ID="dtpDate1a" Width="80px" onClick="WdatePicker({dateFmt:'yyy-MM-dd',skin:'whyGreen'})" runat="server" /></td>                                          
                                            </tr>
                                            <tr>
                                                <th>單位起：</th>
                                                <td>
                                                    <asp:TextBox ID="txtUnitS" CssClass="form-control td-left" Width="60px" runat="server" />
                                                    <asp:DropDownList ID="cboUnitS" CssClass="form-control td-right" Width="80%" AutoPostBack="True" runat="server" />
                                                    <asp:Label ID="lblshortNameS" runat="server" />
                                                    <asp:Label ID="lblcashierS" runat="server" />
                                                </td>                                                
                                            </tr>
                                            <tr>
                                                <th>單位迄：</th>
                                                <td>
                                                    <asp:TextBox ID="txtUnitE"  CssClass="form-control td-left" Width="60px" runat="server" />
                                                    <asp:DropDownList ID="cboUnitE" CssClass="form-control td-right" Width="80%" AutoPostBack="True" runat="server" />
                                                    <asp:Label ID="lblshortNameE" runat="server" />
                                                    <asp:Label ID="lblcashierE" runat="server" />
                                                </td>                                                
                                            </tr>
                                            <tr>
                                                <th>請購事由：</th>
                                                <td><asp:TextBox ID="txtRemarka" CssClass="form-control" placeholder="請購事由" runat="server" /></td>                                                
                                            </tr>                                            
                                            <tr>
                                                <td colspan="2" align="center">
                                                    <asp:Button ID="btnAdd" Text="整批確定" CssClass="btn btn-primary" runat="server" />
                                                </td>                                                
                                            </tr>
                                        </table>
                                        <div style="display:none;">
                                            <asp:TextBox ID="txtAreaA" CssClass="form-control" placeholder="匯管理處" runat="server" />
                                            <asp:DropDownList ID="cboAreaA" CssClass="form-control" AutoPostBack="True" runat="server" />
                                        </div>
                                    </ContentTemplate>
                                </AjaxToolkit:TabPanel>

                                <AjaxToolkit:TabPanel ID="TabPanel1" runat="server">
                                    <HeaderTemplate>整批請購清單</HeaderTemplate>
                                    <ContentTemplate>                                        
                                        <table id="table-data" rules="all">
                                            <tr>
                                                <td>
                                                    <asp:Button ID="btnAddbatch" Text="整批請購確定" CssClass="btn btn-primary" runat="server" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <asp:DataGrid ID="DataGridView" Width = "100%" AllowSorting="False" AllowPaging="False" style="font-size:14px;" CssClass="table table-bordered table-condensed smart-form" runat="server" >
                                                        <columns>
                                                            <asp:TemplateColumn HeaderText="管理"  HeaderStyle-Width="40" ItemStyle-HorizontalAlign="Center">
                                                                <itemtemplate>        
                                                                    <asp:LinkButton ID="Delete" Text='刪除' CommandName="btnDelete" runat="server" />                                                                                                    
                                                                    <asp:ImageButton ID="Show" AlternateText="查閱" ImageUrl="~/active/images/icon/items/zoom.png" CommandName="btnShow" runat="server" />
                                                                    <asp:Label ID="id" Text='<%# Container.DataItem("BGNO").ToString%>' Visible="false" runat="server" />
                                                                    <asp:UpdatePanel ID="DataGridView_UpdatePanel" runat="server">                                                                
                                                                        <Triggers><asp:AsyncPostBackTrigger ControlID="Delete" EventName="Click" /></Triggers>
                                                                    </asp:UpdatePanel>
                                                                </itemtemplate>                                                                                                                         
                                                            </asp:TemplateColumn>                                                                                
                                                            <asp:TemplateColumn HeaderText="請購編號" HeaderStyle-Width="80" ItemStyle-HorizontalAlign="Center" SortExpression="a.BGNO">
                                                                <itemtemplate><asp:Label ID="請購編號" Text='<%# Container.DataItem("BGNO").ToString%>' runat="server" /></itemtemplate>                                                                
                                                            </asp:TemplateColumn>
                                                            <asp:TemplateColumn HeaderText="請購日期" HeaderStyle-Width="80" ItemStyle-HorizontalAlign="Center" SortExpression="a.DATE1">
                                                                <itemtemplate><asp:Label ID="請購日期" Text='<%# Container.DataItem("DATE1").ToString%>' runat="server" /></itemtemplate>
                                                            </asp:TemplateColumn>                                                            
                                                            <asp:TemplateColumn HeaderText="年度" HeaderStyle-Width="40" ItemStyle-HorizontalAlign="Center" SortExpression="a.ACCYEAR">
                                                                <itemtemplate><asp:Label ID="年度" Text='<%# Container.DataItem("ACCYEAR").ToString%>' runat="server" /></itemtemplate>                                                       
                                                            </asp:TemplateColumn>
                                                            <asp:TemplateColumn HeaderText="科目" HeaderStyle-Width="100" SortExpression="a.ACCNO">
                                                                <itemtemplate><asp:Label ID="科目" Text='<%# Container.DataItem("ACCNO").ToString%>' runat="server" /></itemtemplate>                                                                
                                                            </asp:TemplateColumn>
                                                            <asp:TemplateColumn HeaderText="科目名稱" HeaderStyle-Width="160" SortExpression="b.ACCNAME">
                                                                <itemtemplate><asp:Label ID="科目名稱" Text='<%# Container.DataItem("ACCNAME").ToString%>' runat="server" /></itemtemplate>                                                                
                                                            </asp:TemplateColumn>
                                                            <asp:TemplateColumn HeaderText="事由" HeaderStyle-Width="220" SortExpression="a.REMARK">
                                                                <itemtemplate><asp:Label ID="事由" Text='<%# Container.DataItem("REMARK").ToString%>' runat="server" /></itemtemplate>                                                                
                                                            </asp:TemplateColumn>                                                      
                                                            <asp:TemplateColumn HeaderText="請購金額" HeaderStyle-Width="100" ItemStyle-HorizontalAlign="Right" SortExpression="a.amt1">
                                                                <itemtemplate><asp:Label ID="請購金額" Text='<%# FormatNumber(Container.DataItem("amt1").ToString, 2)%>' runat="server" /></itemtemplate>                                                                
                                                            </asp:TemplateColumn>                                                    
                                                        </columns>
                                                    </asp:DataGrid>
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
