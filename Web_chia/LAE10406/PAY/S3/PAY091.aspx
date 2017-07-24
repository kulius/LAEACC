<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="PAY091.aspx.vb" Inherits="LAEACC.PAY091" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="AjaxToolkit" %>

<%@ MasterType VirtualPath="~/Site.master" %>
<asp:Content ID="Head" ContentPlaceHolderID="MainHead" runat="server">
</asp:content>


<asp:Content ID="Content" ContentPlaceHolderID="MainContent" runat="server">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>        
            <!--關鍵值隱藏區-->
            <asp:Label ID="txtKey1" Text="" Visible="false" runat="server" />
            <table id="table-serch" rules="all">                                            
                <tr>
                    <th>收付日期：</th>                                                                      
                    <td style="text-align:center;">
                        <asp:TextBox ID="lblDate2" Width="80px" onClick="WdatePicker({dateFmt:'yyy-MM-dd',skin:'whyGreen'})" runat="server" />
                    </td>
                    <th>頁次：</th>
                    <td>
                        <asp:TextBox ID="txtPageNo" runat="server"></asp:TextBox>
                    </td>
                    <td>
                        <asp:Label ID="lblMsg" runat="server" Font-Bold="True" Font-Size="12pt" ForeColor="Blue"></asp:Label>
                    </td>
                    <td style="width:120px; text-align:center;">
                        <asp:Button ID="btnSure" Text="確定" CssClass="btn btn-primary" runat="server" />                                                   
                    </td>
                    <asp:UpdatePanel ID="UpdatePanel4" runat="server">
                        <ContentTemplate>
                        </ContentTemplate>
                        <Triggers>
                            <asp:PostBackTrigger ControlID="btnSure" />
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
                                            <asp:DataGrid ID="DataGridView" Width="100%" AllowSorting="false" AllowPaging="false" style="font-size:14px;" CssClass="table table-bordered table-condensed smart-form" runat="server" >
                                                <columns>
                                                    
                                                    <asp:TemplateColumn HeaderText="銀行" HeaderStyle-Width="50" >
                                                        <itemtemplate><asp:Label ID="銀行" Text='<%# Container.DataItem("BANK").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn> 
                                                    <asp:TemplateColumn HeaderText="昨日結存" HeaderStyle-Width="75" ItemStyle-HorizontalAlign="left">
                                                        <itemtemplate><asp:Label ID="昨日結存" Text='<%# FormatNumber(Container.DataItem("BALANCE").ToString, 0)%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="本日共收" HeaderStyle-Width="75" ItemStyle-HorizontalAlign="left">
                                                        <itemtemplate><asp:Label ID="本日共收" Text='<%# FormatNumber(Container.DataItem("DAY_INCOME").ToString, 0)%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="本日共支" HeaderStyle-Width="75" ItemStyle-HorizontalAlign="left">
                                                        <itemtemplate><asp:Label ID="本日共支" Text='<%# FormatNumber(Container.DataItem("DAY_PAY").ToString, 0)%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="收付日期" HeaderStyle-Width="100" ItemStyle-HorizontalAlign="Center">
                                                        <itemtemplate><asp:Label ID="收付日期" Text='<%# Container.DataItem("DATE_2").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>

                                                    <asp:TemplateColumn HeaderText="名稱" HeaderStyle-Width="100" >
                                                        <itemtemplate><asp:Label ID="名稱" Text='<%# Container.DataItem("bankname").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="會計科目" HeaderStyle-Width="75" >
                                                        <itemtemplate><asp:Label ID="會計科目" Text='<%# Container.DataItem("accno").ToString%>' runat="server" /></itemtemplate>                                                       
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
