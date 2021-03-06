﻿<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="AC220.aspx.vb" Inherits="LAEACC.AC220" %>
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
                                    <td><asp:RadioButton id="rdbKind1" Text="收入" GroupName="rdbKind" runat="server"/><asp:RadioButton id="rdbKind2" Text="支出" Checked="true" GroupName="rdbkind" runat="server"/></td>
                                    <th>銀行：</th>
                                    <td><asp:TextBox ID="txtBank"  Width="80px"  runat="server" /></td>
                                    <th>支票號</th>
                                    <td><asp:TextBox ID="txtChkNo"  Width="80px"  runat="server" /></td>
                                    <td><asp:Button ID="btnSureNo" Text="調出傳票" CssClass="btn btn-primary" runat="server" /> </td>
                                </tr>
                           
                            </table>
                            <div style="padding-bottom:1px;">&nbsp;</div>     
                            <table id="table-data" rules="all">
                                <tr>
                                    <th>收付款日：</th>
                                    <td><asp:Label ID="lblDate_2" ForeColor="Blue" Font-Size="12pt" Font-Bold="True" runat="server" /></td>
                                </tr>
                                <tr>
                                    <th>支票金額：</th>
                                    <td><asp:Label ID="lblAmt" ForeColor="Blue" Font-Size="12pt" Font-Bold="True" runat="server" /></td>
                                </tr>
                                <tr>
                                    <th>受款人：</th>
                                    <td><asp:Label ID="lblChkname" ForeColor="Blue" Font-Size="12pt" Font-Bold="True" runat="server" /></td>
                                </tr>
                                <tr>
                                    <th>摘要：</th>
                                    <td><asp:Label ID="lblRemark" ForeColor="Blue" Font-Size="12pt" Font-Bold="True" runat="server" /></td>
                                </tr>
                                <tr>
                                    <th>傳票起訖號：</th>
                                    <td><asp:Label ID="lblNo2" ForeColor="Blue" Font-Size="12pt" Font-Bold="True" runat="server" /></td>
                                </tr>
                                
                                <tr>
                                    <th>修正銀行：</th>
                                    <td><asp:TextBox ID="txtNewBank"  Width="160px"  runat="server" /></td>
                                </tr>
                                <tr>
                                    <th>修正支票號：</th>
                                    <td><asp:TextBox ID="txtNewChkNo"  Width="160px"  runat="server" /></td>
                                </tr>

                                <tr>
                                    <td><asp:Button ID="btnFinish" Text="確定" CssClass="btn btn-primary" runat="server" /> </td>
                                    <td><asp:Button ID="btnGiveUp" Text="放棄" CssClass="btn btn-primary" runat="server" /> </td>
                                </tr>
                               
                           
                            </table>                       
                        </article>
                    </div>
                </section>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>

</asp:Content>
