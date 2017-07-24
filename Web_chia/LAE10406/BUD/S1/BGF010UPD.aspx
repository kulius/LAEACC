<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="BGF010UPD.aspx.vb" Inherits="LAEACC.BGF010UPD" %>
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
                            <div class="easyui-panel" style="width:100%;" title="查詢條件值" data-options="iconCls:'icon-search'">
                                <table id="table-serch" rules="all">                                                                 
                                    <tr>   
                                        <th>年度：</th>                                             
                                        <td><asp:TextBox ID="nudYear" Width="80" TextMode="Number" runat="server" min="0" max="999" step="1"/></td>
                                    </tr>
                                    <tr>   
                                        <th>請輸入科目起值：</th>                                             
                                        <td>
                                            <asp:TextBox ID="vxtStartNo" MaxLength="17" runat="server" />
                                            <AjaxToolkit:MaskedEditExtender ID="vxtStartNo_Mask" runat="server"
                                                TargetControlID="vxtStartNo"
                                                MaskType="None" Mask="?-????-??-??-???????-?"
                                                InputDirection="LeftToRight" />
                                            ～
                                            <asp:TextBox ID="vxtEndNo" MaxLength="17" runat="server" />
                                            <AjaxToolkit:MaskedEditExtender ID="MaskedEditExtender1" runat="server"
                                                TargetControlID="vxtEndNo"
                                                MaskType="None" Mask="?-????-??-??-???????-?"
                                                InputDirection="LeftToRight" />
                                        </td>
                                    </tr>                                                                         
                                    <tr>                                                
                                        <td colspan="2" align="center">
                                            <asp:Button ID="BtnSearch" Text="確認" CssClass="btn btn-primary" runat="server" />

                                            <asp:Label ID="lblFinish1" Text="" Font-Size="12pt" Font-Bold="True"  runat="server" />
                                            <asp:Label ID="lblFinish2" Text="" Font-Size="12pt" Font-Bold="True"  runat="server" />
                                        </td>
                                    </tr>                                
                                </table>
                            </div>                            
                                                                                               
                            <div style="padding-bottom:1px;">&nbsp;</div>                            
                        </article>
                    </div>
                </section>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
