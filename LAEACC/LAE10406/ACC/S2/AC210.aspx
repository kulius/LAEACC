<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="AC210.aspx.vb" Inherits="LAEACC.AC210" %>
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
                                    <td><asp:RadioButton id="rdbKind1" Text="收入傳票" GroupName="rdbKind" runat="server"/><asp:RadioButton id="rdbKind2" Text="支出傳票" Checked="true" GroupName="rdbkind" runat="server"/></td>
                                    <th>收支款編號 </th>
                                    <td><asp:TextBox ID="txtNo2"  Width="80px"  runat="server" /></td>
                                    <td><asp:Button ID="btnSureNo" Text="調出傳票" CssClass="btn btn-primary" runat="server" /> </td>
                                </tr>
                           
                            </table>
                            <div style="padding-bottom:1px;">&nbsp;</div>     
                            <table id="table-data" rules="all">
                                <tr>
                                    <th>銀行：</th>
                                    <td><asp:Label ID="lblBank" ForeColor="Blue" Font-Size="12pt" Font-Bold="True" runat="server" /></td>
                                </tr>
                                <tr>
                                    <th>支票號碼：</th>
                                    <td><asp:Label ID="lblChkno" ForeColor="Blue" Font-Size="12pt" Font-Bold="True" runat="server" /></td>
                                </tr>
                                <tr>
                                    <th>摘要：</th>
                                    <td><asp:Label ID="lblRemark" ForeColor="Blue" Font-Size="12pt" Font-Bold="True" runat="server" /></td>
                                </tr>
                                <tr>
                                    <th>傳票起訖號：</th>
                                    <td><asp:Label ID="lblNo1" ForeColor="Blue" Font-Size="12pt" Font-Bold="True" runat="server" /></td>
                                </tr>
                                <tr>
                                    <th>總帳金額：</th>
                                    <td><asp:Label ID="lblAmt" ForeColor="Blue" Font-Size="12pt" Font-Bold="True" runat="server" /></td>
                                </tr>
                                <tr>
                                    <th>沖收付額：</th>
                                    <td><asp:Label ID="lblSubAmt" ForeColor="Blue" Font-Size="12pt" Font-Bold="True" runat="server" /></td>
                                </tr>
                                <tr>
                                    <th>實收付額：</th>
                                    <td><asp:Label ID="lblActAmt" ForeColor="Blue" Font-Size="12pt" Font-Bold="True" runat="server" /></td>
                                </tr>
                                <tr>
                                    <th>修正沖收付額：</th>
                                    <td><asp:TextBox ID="txtSubAmt2"  Width="160px"  runat="server" /></td>
                                </tr>
                                <tr>
                                    <th>新實收付額：</th>
                                    <td><asp:Label ID="lblActAmt2" ForeColor="Blue" Font-Size="12pt" Font-Bold="True" runat="server" /></td>
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
