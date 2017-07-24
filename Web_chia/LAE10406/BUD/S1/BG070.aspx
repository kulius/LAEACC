<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="BG070.aspx.vb" Inherits="LAEACC.BG070" %>
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
                                    <td colspan="2" style="color:#f84444;font-size:16px;font-weight:bold;text-align:center;"><asp:Label ID="lblmsg"  Text="" Font-Size="18pt" Font-Bold="True"  runat="server" /></td>
                                </tr>                                 
                                <tr>   
                                    <th>業務單位可異動否?</th>                                             
                                    <td>
                                        <asp:RadioButton id="rdoYes" Text="允許異動" GroupName="rdbKind" runat="server"/>
                                        <asp:RadioButton id="rdoNo" Text="不可異動" GroupName="rdbKind" runat="server"/>
                                    </td>
                                </tr>                                                                       
                                <tr>                                                
                                    <td colspan="2" align="center">
                                        <asp:Button ID="BtnSearch" Text="確認" CssClass="btn btn-primary" runat="server" />
                                    </td>
                                </tr>
                                <tr>                                                                                           
                                    <td colspan="2" style="color:#f84444;">
                                        <ul>
                                            <li>在會計科目='5'的主計審核者欄標示 Y or N ,以控制使用者可異動預算分配檔</li>
                                        </ul>
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
