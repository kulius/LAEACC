<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="BGP050.aspx.vb" Inherits="LAEACC.BGP050" %>
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
                                    <td colspan="2" style="text-align:center;">
                                        <asp:Button ID="BtnPrint" Text="列印" CssClass="btn btn-primary" runat="server" />
                                    </td>
                                </tr>                                
                            </table>
                            <div style="padding-bottom:1px;">&nbsp;</div>                            
                        </article>

                        <!--詳細內容顯示區-->                           
                            <AjaxToolkit:TabContainer ID="TabContainer1" Width="100%" CssClass="Tab" runat="server" >
                                
                            </AjaxToolkit:TabContainer>
                    </div>
                </section>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
