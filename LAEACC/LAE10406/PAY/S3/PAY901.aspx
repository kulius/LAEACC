<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="PAY901.aspx.vb" Inherits="LAEACC.PAY901" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="AjaxToolkit" %>

<%@ MasterType VirtualPath="~/Site.master" %>

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
                            <table id="table-data" rules="all">
                                <tr> 
                                    <th>支票日期：</th>                                             
                                    <td><asp:TextBox ID="txtDate2" Width="100" onClick="WdatePicker({dateFmt:'yyy-MM-dd',skin:'whyGreen'})" runat="server" /></td>
                                </tr>
                                <tr>                                                
                                    <td colspan="4" style="text-align:center;">
                                        <asp:Button ID="btnSearch" Text="查詢" CssClass="btn btn-primary" runat="server" />
                                        <asp:Button ID="btnExcel" Text="匯出" CssClass="btn btn-primary" runat="server" />
                                    </td>
                                </tr>                                
                            </table>          
                            
                            <div class="module_content">
				                <asp:DataGrid ID="DataGridView" Width = "100%"
                                    runat="server">

                                    <columns>                                        
                                        <asp:TemplateColumn HeaderText="領款日期" HeaderStyle-Width="80" ItemStyle-HorizontalAlign="Center">
                                            <itemtemplate><asp:Label ID="領款日期" Text='<%# Container.DataItem("領款日期").ToString%>' runat="server" /></itemtemplate>
                                        </asp:TemplateColumn> 
                                        <asp:TemplateColumn HeaderText="領款名稱" HeaderStyle-Width="300">
                                            <itemtemplate><asp:Label ID="領款名稱" Text='<%# Container.DataItem("領款名稱").ToString%>' runat="server" /></itemtemplate>
                                        </asp:TemplateColumn>
                                        <asp:TemplateColumn HeaderText="金額" HeaderStyle-Width="100" ItemStyle-HorizontalAlign="Right">
                                            <itemtemplate><asp:Label ID="金額" Text='<%# FormatNumber(Container.DataItem("金額").ToString, 0)%>' runat="server" /></itemtemplate>
                                        </asp:TemplateColumn>
                                        <asp:TemplateColumn HeaderText="支票號碼" HeaderStyle-Width="80" ItemStyle-HorizontalAlign="Center">
                                            <itemtemplate><asp:Label ID="支票號碼" Text='<%# Container.DataItem("支票號碼").ToString%>' runat="server" /></itemtemplate>
                                        </asp:TemplateColumn>
                                        <asp:TemplateColumn HeaderText="支票金額" HeaderStyle-Width="80" ItemStyle-HorizontalAlign="Right">
                                            <itemtemplate><asp:Label ID="支票金額" Text='<%# FormatNumber(Container.DataItem("支票金額").ToString, 0)%>' runat="server" /></itemtemplate>
                                        </asp:TemplateColumn>
                                        <asp:TemplateColumn HeaderText="領款人" HeaderStyle-Width="100">
                                            <itemtemplate><asp:Label ID="領款人" Text='<%# Container.DataItem("領款人").ToString %>' runat="server" /></itemtemplate>
                                        </asp:TemplateColumn>                                       
                                    </columns>
                                </asp:DataGrid>
			                </div>
                        </article>
                    </div>
                </section>
            </div>
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="btnSearch" />
            <asp:PostBackTrigger ControlID="btnExcel" />
        </Triggers>
    </asp:UpdatePanel>
</asp:Content>
