<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="BGP020.aspx.vb" Inherits="LAEACC.BGP020" %>
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
                                    <th>年度：</th>
                                    <td><asp:TextBox ID="nudYear" Width="80" TextMode="Number" runat="server" min="0" max="999" step="1"/></td>
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
                                    <th>列印至幾級：</th>
                                    <td><asp:TextBox ID="nudGrade" Width="80" TextMode="Number" Text="6" runat="server" min="4" max="16" step="1"/></td>
                                </tr>
                                <tr>                                                
                                    <td colspan="2" style="text-align:center;">
                                        <asp:Button ID="BtnSearch" Text="1‧查詢並統計" CssClass="btn btn-primary" runat="server" />
                                        <asp:Button ID="BtnPrint" Text="2‧列印" CssClass="btn btn-primary" runat="server" />
                                        <asp:Button ID="BtnPrint1" Text="3‧列印(封面+內容)" CssClass="btn btn-primary" runat="server" />
                                        |
                                        <asp:Button ID="btnExport" Text="匯出" CssClass="btn btn-primary" runat="server" />
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
                                                    <asp:TemplateColumn HeaderText="名稱" HeaderStyle-Width="180" >
                                                        <itemtemplate><asp:Label ID="名稱" Text='<%# Container.DataItem("ACCNAME").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>

                                                    <asp:TemplateColumn HeaderText="核定預算數" HeaderStyle-Width="100" ItemStyle-HorizontalAlign="right">
                                                        <itemtemplate><asp:Label ID="核定" Text='<%# FormatNumber(Container.DataItem("bg0").ToString, 0)%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="第一季分配" HeaderStyle-Width="80" ItemStyle-HorizontalAlign="right">
                                                        <itemtemplate><asp:Label ID="第一季" Text='<%# FormatNumber(Container.DataItem("bg1").ToString, 0)%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="第二季分配" HeaderStyle-Width="80" ItemStyle-HorizontalAlign="right">
                                                        <itemtemplate><asp:Label ID="第二季" Text='<%# FormatNumber(Container.DataItem("bg2").ToString, 0)%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="第三季分配" HeaderStyle-Width="80" ItemStyle-HorizontalAlign="right">
                                                        <itemtemplate><asp:Label ID="第三季" Text='<%# FormatNumber(Container.DataItem("bg3").ToString, 0)%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="第四季分配數" HeaderStyle-Width="80" ItemStyle-HorizontalAlign="right">
                                                        <itemtemplate><asp:Label ID="第四季" Text='<%# FormatNumber(Container.DataItem("bg4").ToString, 0)%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="保留數" HeaderStyle-Width="60" ItemStyle-HorizontalAlign="right">
                                                        <itemtemplate><asp:Label ID="保留數" Text='<%# FormatNumber(Container.DataItem("bg5").ToString, 0)%>' runat="server" /></itemtemplate>                                                       
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
        <Triggers>
            <asp:PostBackTrigger ControlID="BtnPrint" />
            <asp:PostBackTrigger ControlID="BtnPrint1" />
            <asp:PostBackTrigger ControlID="btnExport" />
       </Triggers>
    </asp:UpdatePanel>
</asp:Content>
