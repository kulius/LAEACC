<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="BGQ020.aspx.vb" Inherits="LAEACC.BGQ020" %>
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
                    
                                       
                            <!--詳細內容顯示區-->                           
                            <AjaxToolkit:TabContainer ID="TabContainer1" Width="100%" CssClass="Tab" runat="server" ActiveTabIndex="0">
                                <AjaxToolkit:TabPanel ID="TabPanel1" runat="server">
                                    <HeaderTemplate>資料來源</HeaderTemplate>
                                    <ContentTemplate>
                                        <div style="font-size:14px;">
                                            
                                            <asp:Label ID="lbl_GrdCount" Visible="False" Text="0" runat="server" />                                            
                                            <asp:DataGrid ID="DataGrid1"  Width = "100%" runat="server" >
                                                <columns>
                                                    <asp:TemplateColumn HeaderText="員工姓名" HeaderStyle-Width="75">
                                                        <itemtemplate><asp:Label ID="員工姓名" Text='<%# Container.DataItem("user_name").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="單位" HeaderStyle-Width="35" >
                                                        <itemtemplate><asp:Label ID="單位" Text='<%# Container.DataItem("unit_id").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="單位名稱" HeaderStyle-Width="75" >
                                                        <itemtemplate><asp:Label ID="單位名稱" Text='<%# Container.DataItem("unit_name").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="薪點" HeaderStyle-Width="75" ItemStyle-HorizontalAlign="right">
                                                        <itemtemplate><asp:Label ID="薪點" Text='<%# Container.DataItem("sal").ToString%>' runat="server" /></itemtemplate>                                                       
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
