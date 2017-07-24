<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="AC050.aspx.vb" Inherits="LAEACC.AC050" %>
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
                                <tr><td colspan="2" style="color:#f84444;font-size:16px;font-weight:bold;text-align:center;">過       帳</td></tr>                                 
                                <tr><td colspan="2" style="color:#f84444;font-size:16px;font-weight:bold;text-align:center;">請輸入過帳至：<asp:TextBox ID="dtpDate" Width="80px" onClick="WdatePicker({dateFmt:'yyy-MM-dd',skin:'whyGreen'})" runat="server" />日前之傳票</td></tr>                                 
                                 <tr>                                                
                                    <td colspan="2" align="center">
                                        <asp:UpdatePanel ID="UpdatePanel2" runat="server">             
                                            <ContentTemplate>
                                                <asp:Button ID="btnSure" Text="確定" CssClass="btn btn-primary" runat="server" /> 
                                            </ContentTemplate>
                                        </asp:UpdatePanel>
                                    </td>                                   
                                </tr>
                                <tr>
                                    <td colspan="2" align="center">
                                        <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="UpdatePanel1">
                                             <ProgressTemplate>
                                                 處理中...
                                             </ProgressTemplate>
                                         </asp:UpdateProgress>
                                         <asp:UpdateProgress ID="UpdateProgress2" runat="server" AssociatedUpdatePanelID="UpdatePanel1">
                                             <ProgressTemplate>
                                                 <img src="../../../active/images/connecting.gif" />
                                             </ProgressTemplate>
                                         </asp:UpdateProgress>
                                    </td>
                                </tr>     
                                <tr>   
                                    <td colspan="2" align="center"><asp:Label ID="lblNo" CssClass="td-right" Text="" runat="server" /></td>                                   
                                </tr>
                                <tr>   
                                    <th>收入傳票張數=</th>                                             
                                    <td>
                                        <asp:Label ID="lblKind1" CssClass="td-right" Text="" runat="server" />
                                    </td>
                                </tr>
                                <tr>   
                                    <th>支出傳票張數=</th>                                             
                                    <td>
                                        <asp:Label ID="lblKind2" CssClass="td-right" Text="" runat="server" />
                                    </td>
                                </tr>                                
                                <tr>   
                                    <th>轉帳傳票張數=</th>                                             
                                    <td>
                                        <asp:Label ID="lblKind3" CssClass="td-right" Text="" runat="server" />
                                    </td>
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
