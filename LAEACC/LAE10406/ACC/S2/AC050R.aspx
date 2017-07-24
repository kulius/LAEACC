<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="AC050R.aspx.vb" Inherits="LAEACC.AC050R" %>
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
                                <tr><td colspan="2" style="color:#f84444;font-size:16px;font-weight:bold;text-align:center;">重新過帳(回復過帳前)</td></tr>                                 
                                <tr><td colspan="2" style="color:#f84444;font-size:16px;font-weight:bold;text-align:center;">請輸入清除過帳資料自<asp:Label ID="lblYear"  runat="server" />年<asp:TextBox ID="nudMonth"  Width="80" TextMode="Number" runat="server" min="" max="12"  step="1" />月</td></tr>                                 
                                 <tr>                                                
                                    <td colspan="2" align="center"><asp:Button ID="btnSure" Text="確定" CssClass="btn btn-primary" runat="server" /> </td>                                   
                                </tr>     
                                <tr>   
                                    <td colspan="2" align="center"><asp:Label ID="lblMsg" CssClass="td-right" Text="" runat="server" /></td>                                   
                                </tr>
                                                                        
                           
                            </table>
                            <div style="padding-bottom:1px;">&nbsp;</div>                            
                        </article>
                    </div>
                </section>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>

</asp:Content>
