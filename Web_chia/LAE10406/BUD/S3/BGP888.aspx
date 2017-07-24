<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="BGP888.aspx.vb" Inherits="LAEACC.BGP888" %>
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
                            <table id="table-data" rules="all">                                
                                <tr>   
                                    <th>請輸入列印範圍：</th>                                             
                                    <td><asp:DropDownList ID="cboUser" AutoPostBack="True" runat="server" /></td>
                                </tr>                                
                                <tr>   
                                    <th>請輸入科目：</th>                                             
                                    <td>
                                        <asp:TextBox ID="vxtStartNo" MaxLength="17" runat="server" />                                        
                                        <AjaxToolkit:MaskedEditExtender ID="MaskedEditExtender2" runat="server"
                                                TargetControlID="vxtStartNo"
                                                MaskType="None" Mask="?-????-??-??-???????-?"
                                                InputDirection="LeftToRight" />
                                        ～
                                        <asp:TextBox ID="vxtEndNo" MaxLength="17" runat="server" />
                                        <AjaxToolkit:MaskedEditExtender ID="MaskedEditExtender4" runat="server"
                                                TargetControlID="vxtEndNo"
                                                MaskType="None" Mask="?-????-??-??-???????-?"
                                                InputDirection="LeftToRight" />
                                    </td>
                                </tr>
                                <tr>                                                
                                    <td colspan="2" style="text-align:center;">
                                        <asp:Button ID="BtnPrint" Text="列印" CssClass="btn btn-primary" runat="server" />
                                        <asp:Button ID="BtnExcel" Text="匯出EXCEL" CssClass="btn btn-primary" runat="server" />
                                    </td>
                                </tr>                                
                            </table>

                            <!--詳細內容顯示區-->                           
                            <AjaxToolkit:TabContainer ID="TabContainer1" Width="100%" CssClass="Tab" runat="server" ActiveTabIndex="0">
                                <AjaxToolkit:TabPanel ID="TabPanel1" runat="server">
                                    <HeaderTemplate>資料來源</HeaderTemplate>
                                    <ContentTemplate>
                                        <div style="font-size:14px;">
                                            
                                            <asp:Label ID="lbl_GrdCount" Visible="False" Text="0" runat="server" />                                            
                                            <asp:DataGrid ID="DataGridView" Width = "100%" runat="server" >
                                                <columns>
                                                    <asp:TemplateColumn HeaderText="預算科目" HeaderStyle-Width="100">
                                                        <itemtemplate><asp:Label ID="預算科目" Text='<%# Container.DataItem("ACCNO").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="名稱" HeaderStyle-Width="120" >
                                                        <itemtemplate><asp:Label ID="名稱" Text='<%# Container.DataItem("ACCNAME").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="BG1" HeaderStyle-Width="100" ItemStyle-HorizontalAlign="right">
                                                        <itemtemplate><asp:Label ID="BG1" Text='<%# Container.DataItem("BG1").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="bg2" HeaderStyle-Width="100" ItemStyle-HorizontalAlign="right">
                                                        <itemtemplate><asp:Label ID="bg2" Text='<%# Container.DataItem("bg2").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="bg3" HeaderStyle-Width="100" ItemStyle-HorizontalAlign="right">
                                                        <itemtemplate><asp:Label ID="bg3" Text='<%# Container.DataItem("bg3").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="bg4" HeaderStyle-Width="100" ItemStyle-HorizontalAlign="right">
                                                        <itemtemplate><asp:Label ID="bg4" Text='<%# Container.DataItem("bg4").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="bg5" HeaderStyle-Width="100" ItemStyle-HorizontalAlign="right">
                                                        <itemtemplate><asp:Label ID="bg5" Text='<%# Container.DataItem("bg5").ToString%>' runat="server" /></itemtemplate>                                                       
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
