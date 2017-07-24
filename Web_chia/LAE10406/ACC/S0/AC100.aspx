<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="AC100.aspx.vb" Inherits="LAEACC.AC100" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="AjaxToolkit" %>

<%@ MasterType VirtualPath="~/Site.master" %>

<asp:Content ID="Head" ContentPlaceHolderID="MainHead" runat="server">
</asp:content>


<asp:Content ID="Content" ContentPlaceHolderID="MainContent" runat="server">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
       <Triggers>
          <asp:PostBackTrigger ControlID="BtnPrint" />
       </Triggers>

        <ContentTemplate>        
            <!--關鍵值隱藏區-->
            <asp:Label ID="txtKey1" Text="" Visible="false" runat="server" />
   
            <!--主項目區-->
            <div style="margin: 10px 0px 0px 10px;">
                <section id="widget-grid" style="width:98%;">
                    <div class="row">
                        <article class="col-sm-12 col-md-12 col-lg-12">
                            <table id="table-data" rules="all">
                                <tr><td colspan="2" style="color:#f84444;font-size:16px;font-weight:bold;text-align:center;">列印傳票交付簿 AC100 new</td></tr>                                 
                                
                                <tr>   
                                    <th>年度</th>                                             
                                    <td>
                                        <asp:TextBox ID="nudYear" CssClass="form-control" Width="80" TextMode="Number" AutoPostBack="true" runat="server" min="0" max="999" step="1"/>
                                    </td>
                                </tr>
                                <tr>   
                                    <th>收入傳票自</th>                                             
                                    <td>
                                         <asp:TextBox ID="txtSno1"  Width="100px" runat="server" />至<asp:TextBox ID="txtEno1" Width="100px" runat="server" />
                                    </td>
                                </tr>                                
                                <tr>   
                                    <th>支出傳票自</th>                                             
                                    <td>
                                       <asp:TextBox ID="txtSno2" Width="100px" runat="server" />至<asp:TextBox ID="txtEno2" Width="100px"  runat="server" />
                                    </td>
                                </tr>
                                <tr>   
                                    <th>轉帳傳票自</th>                                             
                                    <td>
                                       <asp:TextBox ID="txtSno3" Width="100px"  runat="server" />至<asp:TextBox ID="txtEno3" Width="100px" runat="server" />
                                    </td>
                                </tr>
                               
                                <tr>                                                
                                    <td colspan="2" align="center"><asp:Button ID="BtnPrint" Text="列印" CssClass="btn btn-primary" runat="server" /> </td>
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
