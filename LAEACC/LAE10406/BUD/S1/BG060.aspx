<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="BG060.aspx.vb" Inherits="LAEACC.BG060" %>
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
                            <!-- 查詢項目值 --> 
                            <!--控制項-->
                            <div style="margin:5px 0px 5px 0px; display:none;">
                                <uc1:UCBase ID="UCBase1" runat="server" />
                            </div>                         

                            <!--詳細內容顯示區-->                           
                            <AjaxToolkit:TabContainer ID="TabContainer1" Width="100%" CssClass="Tab" runat="server" ActiveTabIndex="1">
                                <AjaxToolkit:TabPanel ID="TabPanel1" runat="server">
                                    <HeaderTemplate>多筆瀏灠</HeaderTemplate>
                                    <ContentTemplate>
                                        <table id="table-data" rules="all">
                                            <tr>
                                                <td>
                                                    <asp:Button ID="btnReflesh" Text="重新整理" CssClass="btn btn-success" runat="server" />
                                                    <asp:Button ID="btnNew" Text="新增轉帳" CssClass="btn btn-primary" runat="server" />                                                    
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <div style="float:right; padding-right:10px;">
                                                        共<asp:Label ID="lbl_GrdCount" ForeColor="Red" Font-Size="14pt" Font-Bold="True" Text="0" runat="server" />筆符合&nbsp;                                                
                                                        <asp:Label ID="lbl_sort" runat="server" />
                                                    </div>
                                                    <asp:DataGrid ID="DataGridView" Width = "100%" AllowSorting="True" AllowPaging="false" CssClass="table table-bordered table-condensed smart-form" runat="server" >
                                                        <columns>
                                                            <asp:TemplateColumn HeaderText="管理" HeaderStyle-Width="40" ItemStyle-HorizontalAlign="Center">
                                                                <itemtemplate>                                                                                                            
                                                                    <asp:ImageButton ID="Show" AlternateText="查閱" ImageUrl="~/active/images/icon/items/zoom.png" CommandName="btnShow" runat="server" />
                                                                    <asp:Label ID="id" Text='<%# Container.DataItem("BGCNO").ToString%>' Visible="false" runat="server" />
                                                                    <asp:UpdatePanel ID="DataGridView_UpdatePanel" runat="server">                                                                
                                                                        <Triggers><asp:AsyncPostBackTrigger ControlID="Show" EventName="Click" /></Triggers>
                                                                    </asp:UpdatePanel>
                                                                </itemtemplate>                                                                                                                         
                                                            </asp:TemplateColumn>                                                                                                                                            
                                                            <asp:TemplateColumn HeaderText="年度" HeaderStyle-Width="40" ItemStyle-HorizontalAlign="Center" SortExpression="ACCYEAR">
                                                                <itemtemplate><asp:Label ID="年度" Text='<%# Container.DataItem("ACCYEAR").ToString%>' runat="server" /></itemtemplate>                                                       
                                                            </asp:TemplateColumn>                                                   
                                                            <asp:TemplateColumn HeaderText="轉帳編號" HeaderStyle-Width="80" ItemStyle-HorizontalAlign="Center" SortExpression="BGCNO">
                                                                <itemtemplate><asp:Label ID="轉帳編號" Text='<%# Container.DataItem("BGCNO").ToString%>' runat="server" /></itemtemplate>
                                                            </asp:TemplateColumn>
                                                            <asp:TemplateColumn HeaderText="轉帳日期" HeaderStyle-Width="80" ItemStyle-HorizontalAlign="Center" SortExpression="DATEC">
                                                                <itemtemplate><asp:Label ID="轉帳日期" Text='<%# Container.DataItem("DATEC").ToString%>' runat="server" /></itemtemplate>
                                                            </asp:TemplateColumn>
                                                            <asp:TemplateColumn HeaderText="借貸" HeaderStyle-Width="40" ItemStyle-HorizontalAlign="Center" SortExpression="kind">
                                                                <itemtemplate><asp:Label ID="借貸" Text='<%# Container.DataItem("kind").ToString%>' runat="server" /></itemtemplate>                                                                
                                                            </asp:TemplateColumn>
                                                            <asp:TemplateColumn HeaderText="合計金額" HeaderStyle-Width="100" ItemStyle-HorizontalAlign="Right" SortExpression="useamt">
                                                                <itemtemplate><asp:Label ID="合計金額" Text='<%# FormatNumber(Container.DataItem("useamt").ToString, 2)%>' runat="server" /></itemtemplate>
                                                            </asp:TemplateColumn>                                                                                                                                                             
                                                        </columns>
                                                    </asp:DataGrid>
                                                </td>
                                            </tr>
                                        </table>
                                    </ContentTemplate>
                                </AjaxToolkit:TabPanel>

                                <AjaxToolkit:TabPanel ID="TabPanel2" runat="server">
                                    <HeaderTemplate>單筆明細</HeaderTemplate>
                                    <ContentTemplate>
                                        <table id="table-data" rules="all">
                                            <tr>
                                                <th>年度：</th>
                                                <td colspan="5"><asp:Label ID="lblYear" ForeColor="Blue" Font-Size="12pt" Font-Bold="True" runat="server" /></td>
                                            </tr>
                                            <tr>
                                                <th>轉帳編號：</th>
                                                <td style="width:20%;"><asp:Label ID="lblBgcno" ForeColor="Blue" Font-Size="12pt" Font-Bold="True"  runat="server" /></td>
                                                <th>請購編號：</th>
                                                <td style="width:20%;"><asp:Label ID="lblBgno" ForeColor="Blue" Font-Size="12pt" Font-Bold="True"  runat="server" /></td>
                                                <th>轉帳日期：</th>
                                                <td style="width:20%;"><asp:Label ID="lblDatec" ForeColor="Blue" Font-Size="12pt" Font-Bold="True"  runat="server" /></td>
                                            </tr>
                                            <tr>
                                                <th>傳票：</th>
                                                <td colspan="5">
                                                    <asp:RadioButton id="rdbKind1" Text="借方" GroupName="rdbKind" Checked="true" runat="server"/>
                                                    <asp:RadioButton id="rdbKind2" Text="貸方" GroupName="rdbKind" runat="server"/>

                                                    <asp:Label ID="lblKind" ForeColor="Blue" Font-Size="12pt" Font-Bold="True" Visible="False" runat="server" />
                                                    <asp:Label ID="lblaccyear" ForeColor="Blue" Font-Size="12pt" Font-Bold="True" Visible="False" runat="server" />
                                                    <asp:Label ID="lblkey" ForeColor="Blue" Font-Size="12pt" Font-Bold="True" Visible="False" runat="server" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <th>會計科目：</th>
                                                <td colspan="5">
                                                    <asp:DropDownList ID="cboAccno" CssClass="form-control td-left" Width="60%" AutoPostBack="True" runat="server" />
                                                    <asp:Label ID="lblAccno" CssClass="td-right" ForeColor="Blue" Font-Size="12pt" Font-Bold="True"  runat="server" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <th>摘要：</th>
                                                <td colspan="5"><asp:TextBox ID="txtRemark" CssClass="form-control" placeholder="摘要" runat="server" /></td>
                                            </tr>
                                            <tr>
                                                <th>金額：</th>
                                                <td colspan="5">
                                                    <asp:TextBox ID="txtUseAmt" CssClass="form-control td-left" Width="40%" placeholder="金額" runat="server" />
                                                    <asp:Label ID="lblUseAmt" CssClass="td-right" ForeColor="Blue" Font-Size="12pt" Font-Bold="True" runat="server" />
                                                </td>
                                            </tr>
                                            <tr>                                                
                                                <td colspan="6" align="center">
                                                    <asp:Button ID="btnInsert" Text="新增" CssClass="btn btn-primary" runat="server" />
                                                    <asp:Button ID="btnModify" Text="修改" CssClass="btn btn-primary" runat="server" />
                                                    <asp:Button ID="btnDelete" Text="刪除" CssClass="btn btn-primary" runat="server" />
                                                </td>
                                            </tr>
                                            <tr><td colspan="6">&nbsp;</td></tr>

                                            <tr>
                                                <td colspan="6">
                                                    <div style="float:right; padding-right:10px;">
                                                        共<asp:Label ID="lbl_GrdCount2" ForeColor="Red" Font-Size="14pt" Font-Bold="True" Text="0" runat="server" />筆符合&nbsp;                                                
                                                        <asp:Label ID="Label2" runat="server" />
                                                    </div>
                                                    <asp:DataGrid ID="DataGrid2" Width = "100%" AllowPaging="True" runat="server" >
                                                        <columns>
                                                            <asp:TemplateColumn HeaderText="管理" HeaderStyle-Width="40" ItemStyle-HorizontalAlign="Center">
                                                                <itemtemplate>                                                                                                            
                                                                    <asp:ImageButton ID="Show" AlternateText="查閱" ImageUrl="~/active/images/icon/items/zoom.png" CommandName="btnShow" runat="server" />
                                                                    <asp:Label ID="id" Text='<%# Container.DataItem("BGNO").ToString%>' Visible="false" runat="server" />
                                                                    <asp:UpdatePanel ID="DataGridView_UpdatePanel" runat="server">                                                                
                                                                        <Triggers><asp:AsyncPostBackTrigger ControlID="Show" EventName="Click" /></Triggers>
                                                                    </asp:UpdatePanel>
                                                                </itemtemplate>
                                                            </asp:TemplateColumn>   
                                                                                                                                                                                             
                                                            <asp:TemplateColumn HeaderText="借貸" HeaderStyle-Width="40" ItemStyle-HorizontalAlign="Center">
                                                                <itemtemplate><asp:Label ID="借貸" Text='<%# Container.DataItem("KIND").ToString%>' runat="server" /></itemtemplate>                                                                
                                                            </asp:TemplateColumn>
                                                            <asp:TemplateColumn HeaderText="請購編號" HeaderStyle-Width="80" ItemStyle-HorizontalAlign="Center">
                                                                <itemtemplate><asp:Label ID="請購編號" Text='<%# Container.DataItem("BGNO").ToString%>' runat="server" /></itemtemplate>                                                                
                                                            </asp:TemplateColumn>
                                                            <asp:TemplateColumn HeaderText="年度" HeaderStyle-Width="40" ItemStyle-HorizontalAlign="Center">
                                                                <itemtemplate><asp:Label ID="年度" Text='<%# Container.DataItem("ACCYEAR").ToString%>' runat="server" /></itemtemplate>                                                       
                                                            </asp:TemplateColumn>
                                                            <asp:TemplateColumn HeaderText="科目" HeaderStyle-Width="100">
                                                                <itemtemplate><asp:Label ID="科目" Text='<%# Container.DataItem("ACCNO").ToString%>' runat="server" /></itemtemplate>                                                                
                                                            </asp:TemplateColumn>
                                                            <asp:TemplateColumn HeaderText="科目名稱" HeaderStyle-Width="160">
                                                                <itemtemplate><asp:Label ID="科目名稱" Text='<%# Container.DataItem("ACCNAME").ToString%>' runat="server" /></itemtemplate>                                                                
                                                            </asp:TemplateColumn>
                                                            <asp:TemplateColumn HeaderText="金額" HeaderStyle-Width="100" ItemStyle-HorizontalAlign="Right">
                                                                <itemtemplate><asp:Label ID="金額" Text='<%# FormatNumber(Container.DataItem("useamt").ToString, 2)%>' runat="server" /></itemtemplate>                                                                
                                                            </asp:TemplateColumn>
                                                             <asp:TemplateColumn HeaderText="摘要" HeaderStyle-Width="250">
                                                                <itemtemplate><asp:Label ID="摘要" Text='<%# Container.DataItem("remark").ToString%>' runat="server" /></itemtemplate>
                                                            </asp:TemplateColumn>                                                                                                        
                                                        </columns>
                                                    </asp:DataGrid>
                                                </td>
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
