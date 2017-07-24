<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="ACF070.aspx.vb" Inherits="LAEACC.ACF070" %>
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
            <table id="table-serch" rules="all">                                            
                <tr>
                    <td style="text-align:center;">
                        起：<asp:TextBox ID="dtpStartDate" Width="80px"  onClick="WdatePicker({dateFmt:'yyy-MM-dd',skin:'whyGreen'})" runat="server" />
                    </td>     
                    <td style="text-align:center;">
                        迄：<asp:TextBox ID="dtpEndDate" Width="80px"  onClick="WdatePicker({dateFmt:'yyy-MM-dd',skin:'whyGreen'})" runat="server" />
                    </td>                                                                                             
                    <td style="text-align:center;">
                        <asp:TextBox ID="vxtStartNo" CssClass="form-control" runat="server" />
                                            <AjaxToolkit:MaskedEditExtender ID="vxtStartNo_Mask" runat="server"
                                                TargetControlID="vxtStartNo"
                                                MaskType="None" Mask="?-????-??-??-???????-?"
                                                InputDirection="LeftToRight" />
                    </td>
                    <td style="text-align:center;">
                        <asp:TextBox ID="vxtEndNo" CssClass="form-control" runat="server" />
                                            <AjaxToolkit:MaskedEditExtender ID="MaskedEditExtender1" runat="server"
                                                TargetControlID="vxtEndNo"
                                                MaskType="None" Mask="?-????-??-??-???????-?"
                                                InputDirection="LeftToRight" />
                    </td>
                    <td style="text-align:center;">
                        排序<asp:RadioButton id="rdbdate" Text="日期" GroupName="rdbsort"  Checked="true" runat="server"/><asp:RadioButton id="RadioButton1" Text="科目" GroupName="rdbsort" runat="server"/>
                    </td>                     
                    <td style="width:120px; text-align:center;">
                        <asp:Button ID="btnSearch" Text="查詢" CssClass="btn btn-primary" runat="server" />                                                   
                    </td>

                </tr>
            </table>
   
            <!--主項目區-->
            <div style="margin: 10px 0px 0px 10px;">
                <section id="widget-grid" style="width:98%;">
                    <div class="row">
                        <article class="col-sm-12 col-md-12 col-lg-12">
                            <!--控制項-->
                            <div style="margin:5px 0px 5px 0px;">
                                <uc1:UCBase ID="UCBase1" runat="server" />
                            </div>
                            <div style="clear:both; height:5px;"></div> 

                            <!--詳細內容顯示區-->                           
                            <AjaxToolkit:TabContainer ID="TabContainer1" Width="100%" CssClass="Tab" runat="server" ActiveTabIndex="2">
                                <AjaxToolkit:TabPanel ID="TabPanel1" runat="server">
                                    <HeaderTemplate>多筆瀏灠</HeaderTemplate>
                                    <ContentTemplate>
                                        <div class="table-responsive">
                                            &nbsp;&nbsp;
                                            <asp:Label ID="Label1" ForeColor="Red" Font-Size="14" Font-Bold="true" Text="0" runat="server" />
                                            <div style="float:right; padding-right:10px;">
                                                共<asp:Label ID="lbl_GrdCount" ForeColor="Red" Font-Size="14" Font-Bold="true" Text="0" runat="server" />筆符合&nbsp;                                                
                                                <asp:Label ID="lbl_sort" runat="server" />
                                            </div>
                                            <asp:DataGrid ID="DataGridView" Width="100%" AllowSorting="false" AllowPaging="true" style="font-size:14px;" CssClass="table table-bordered table-condensed smart-form" runat="server" >
                                                <columns>
                                                    <asp:TemplateColumn HeaderText="管理" HeaderStyle-Width="40" ItemStyle-HorizontalAlign="Center">
                                                        <itemtemplate>                                                                                                            
                                                            <asp:ImageButton ID="Show" AlternateText="查閱" ImageUrl="~/active/images/icon/items/zoom.png" CommandName="btnShow" runat="server" />
                                                            <asp:Label ID="id" Text='<%# Container.DataItem("autono").ToString%>' Visible="false" runat="server" />
                                                            <asp:UpdatePanel ID="DataGridView_UpdatePanel" runat="server">                                                                
                                                                <Triggers><asp:AsyncPostBackTrigger ControlID="Show" EventName="Click" /></Triggers>
                                                            </asp:UpdatePanel>
                                                        </itemtemplate>                                                         
                                                    </asp:TemplateColumn>
                                                                            
                                                    <asp:TemplateColumn HeaderText="日期" HeaderStyle-Width="75" ItemStyle-HorizontalAlign="Center">                                                        
                                                        <itemtemplate><asp:Label ID="日期" Text='<%# Master.Models.strDateADToChiness(Container.DataItem("Date_2").ToShortDateString.ToString)%>' runat="server" /></itemtemplate>  
                                                    </asp:TemplateColumn>                                                   
                                                    <asp:TemplateColumn HeaderText="科目" HeaderStyle-Width="50" >
                                                        <itemtemplate><asp:Label ID="科目" Text='<%# Container.DataItem("accno").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>                                                    
                                                    <asp:TemplateColumn HeaderText="會計科目名稱" HeaderStyle-Width="110" ItemStyle-HorizontalAlign="left">
                                                        <itemtemplate><asp:Label ID="會計科目名稱" Text='<%# Container.DataItem("accname").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="借方(銀行存款)" HeaderStyle-Width="110" ItemStyle-HorizontalAlign="right">
                                                        <itemtemplate><asp:Label ID="借方銀行存款" Text='<%# FormatNumber(Container.DataItem("DeAmt1").ToString, 0)%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn> 
                                                    
                                                    <asp:TemplateColumn HeaderText="借方(專戶存款)" HeaderStyle-Width="110" ItemStyle-HorizontalAlign="right">
                                                        <itemtemplate><asp:Label ID="借方專戶存款" Text='<%# FormatNumber(Container.DataItem("DeAmt2").ToString, 0)%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn> 
                                                    <asp:TemplateColumn HeaderText="借方(轉帳)" HeaderStyle-Width="110" ItemStyle-HorizontalAlign="right">
                                                        <itemtemplate><asp:Label ID="借方轉帳" Text='<%# FormatNumber(Container.DataItem("DeAmt3").ToString, 0)%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn> 
                                                    <asp:TemplateColumn HeaderText="貸方(銀行存款)" HeaderStyle-Width="110" ItemStyle-HorizontalAlign="right">
                                                        <itemtemplate><asp:Label ID="貸方銀行存款" Text='<%# FormatNumber(Container.DataItem("CrAmt1").ToString, 0)%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn> 
                                                    <asp:TemplateColumn HeaderText="貸方(專戶存款)" HeaderStyle-Width="110" ItemStyle-HorizontalAlign="right">
                                                        <itemtemplate><asp:Label ID="貸方專戶存款" Text='<%# FormatNumber(Container.DataItem("CrAmt2").ToString, 0)%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn> 
                                                    <asp:TemplateColumn HeaderText="貸方(轉帳)" HeaderStyle-Width="110" ItemStyle-HorizontalAlign="right">
                                                        <itemtemplate><asp:Label ID="貸方轉帳" Text='<%# FormatNumber(Container.DataItem("CrAmt3").ToString, 0)%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn> 
                                                     
                                                    
                                                                                                                                                                
                                                </columns>
                                            </asp:DataGrid>
                                        </div>
                                    </ContentTemplate>
                                </AjaxToolkit:TabPanel>

                                <AjaxToolkit:TabPanel ID="TabPanel2" runat="server">
                                    <HeaderTemplate>單筆明細</HeaderTemplate>
                                    <ContentTemplate>
                                        <table id="table-data" rules="all">
                                            <tr>
                                                <th>日期：</th>
                                                <td>
                                                    <asp:TextBox ID="dtpDate" Width="100px" CssClass="form-control td-left" onClick="WdatePicker({dateFmt:'yyy-MM-dd',skin:'whyGreen'})" runat="server" />
                                                </td>
                                                <th>科目代號：</th>
                                                <td>
                                                    <asp:TextBox ID="vxtAccno" CssClass="form-control td-left" Width="160px" runat="server"  runat="server" />
                                                    <asp:Label ID="lblAccname" CssClass="td-right" ForeColor="Blue" Font-Size="12pt" Font-Bold="True"  runat="server" />
                                                    <asp:Label ID="lblkey" Visible="False"  runat="server" />
                                                </td>                                                    
                                            </tr>
                                        </table>
                                        <table id="table-data" rules="all">
                                            <tr>
                                                <td></td>
                                                <td>當日借方總額</td>
                                                <td>當日貸方總額</td>
                                            </tr>
                                            <tr>
                                                <td>銀行存款</td>
                                                <td><asp:TextBox ID="txtDeAmt1" CssClass="form-control" Width="160px" runat="server"  runat="server" /></td>
                                                <td><asp:TextBox ID="txtCrAmt1" CssClass="form-control" Width="160px" runat="server"  runat="server" /></td>
                                            </tr>
                                            <tr>
                                                <td>專戶存款</td>
                                                <td><asp:TextBox ID="txtDeAmt2" CssClass="form-control" Width="160px" runat="server"  runat="server" /></td>
                                                <td><asp:TextBox ID="txtCrAmt2" CssClass="form-control" Width="160px" runat="server"  runat="server" /></td>
                                            </tr>
                                            <tr>
                                                <td>轉    帳</td>
                                                <td><asp:TextBox ID="txtDeAmt3" CssClass="form-control" Width="160px" runat="server"  runat="server" /></td>
                                                <td><asp:TextBox ID="txtCrAmt3" CssClass="form-control" Width="160px" runat="server"  runat="server" /></td>
                                            </tr>
                                            <tr>
                                                <td>合    計</td>
                                                <td><asp:Label ID="lblSumDebit" CssClass="form-control" width="160px"  runat="server" /></td>
                                                <td><asp:Label ID="lblSumCredit" CssClass="form-control" width="160px"  runat="server" /></td>
                                            </tr>
                                            
                                            
                                            
                                        </table>                                        
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
