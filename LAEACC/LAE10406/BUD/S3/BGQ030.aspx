<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="BGQ030.aspx.vb" Inherits="LAEACC.BGQ030" %>
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
                                        <th>工程名稱：</th>
                                        <td><asp:TextBox ID="txtEngName" CssClass="form-control td-left" Width="300" runat="server" /></td>
                                    </tr>
                                    <tr>
                                        <td colspan="2" style="text-align:center;">
                                            <asp:Button ID="BtnSearch" Text="查詢" CssClass="btn btn-primary" runat="server" />    
                                        </td>
                                    </tr>
                                </table>
                            </div>                                                    
                                       
                            <!--詳細內容顯示區-->                           
                            <AjaxToolkit:TabContainer ID="TabContainer1" Width="100%" CssClass="Tab" runat="server" ActiveTabIndex="0">
                                <AjaxToolkit:TabPanel ID="TabPanel1" runat="server">
                                    <HeaderTemplate>資料來源</HeaderTemplate>
                                    <ContentTemplate>
                                        <div class="table-responsive">                                            
                                            <asp:Label ID="lbl_GrdCount" Visible="False" Text="0" runat="server" />                                            
                                            <asp:DataGrid ID="DataGridView" Width="100%" AllowSorting="True" AllowPaging="True" style="font-size:14px;" CssClass="table table-bordered table-condensed smart-form" runat="server" >
                                                <columns>
                                                    <asp:TemplateColumn HeaderText="工程代號" HeaderStyle-Width="100" ItemStyle-HorizontalAlign="Center">
                                                        <itemtemplate><asp:Label ID="工程代號" Text='<%# Container.DataItem("engno").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="工程名稱" HeaderStyle-Width="200">
                                                        <itemtemplate><asp:Label ID="工程名稱" Text='<%# Container.DataItem("engname").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="員工" HeaderStyle-Width="120">
                                                        <itemtemplate><asp:Label ID="員工" Text='<%# Container.DataItem("userid").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="廠商" HeaderStyle-Width="160">
                                                        <itemtemplate><asp:Label ID="廠商" Text='<%# Container.DataItem("cop").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>                                                                                                                                  
                                                </columns>
                                            </asp:DataGrid>
                                        </div>
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
