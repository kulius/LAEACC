<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="BG050.aspx.vb" Inherits="LAEACC.BG050" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="AjaxToolkit" %>

<%@ MasterType VirtualPath="~/Site.master" %>
<%@ Register SRC="~/LAE10406/UserControl/UCBase.ascx" TagName="UCBase" TagPrefix="uc1" %>

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
                            <!--控制項-->
                            <div style="margin:5px 0px 5px 0px; display:none;">
                                <uc1:UCBase ID="UCBase1" runat="server" />
                            </div>

                            <!--詳細內容顯示區-->                           
                            <AjaxToolkit:TabContainer ID="TabContainer1" Width="100%" CssClass="Tab" runat="server" ActiveTabIndex="1">
                                <AjaxToolkit:TabPanel ID="TabPanel1" runat="server">
                                    <HeaderTemplate>未開支清單</HeaderTemplate>
                                    <ContentTemplate>
                                        <div class="table-responsive">
                                            <div class="col-lg-10">
                                                請購編號
                                                <asp:Label ID="lblNoYear" ForeColor="Blue" Font-Size="12pt" Font-Bold="True" runat="server" />
                                                <asp:TextBox ID="txtNO" runat="server" /> 
                                                <asp:Button ID="btnNo" Text="確定" CssClass="btn btn-primary" runat="server" />
                                                <asp:Label ID="lblSeason"  runat="server" />
                                            </div>                                                   
             
                                            <div style="float:right; padding-right:10px;">
                                                共<asp:Label ID="lbl_GrdCount" ForeColor="Red" Font-Size="14" Font-Bold="true" Text="0" runat="server" />筆符合&nbsp;                                                
                                                <asp:Label ID="lbl_sort" runat="server" />
                                            </div>
                                            <asp:DataGrid ID="DataGridView" Width = "100%" AllowSorting="True" AllowPaging="True" style="font-size:14px;" CssClass="table table-bordered table-condensed smart-form" runat="server" >
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
                                                    <asp:TemplateColumn HeaderText="請購編號" HeaderStyle-Width="80" ItemStyle-HorizontalAlign="Center" SortExpression="a.BGNO">
                                                        <itemtemplate><asp:Label ID="請購編號" Text='<%# Container.DataItem("BGNO").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="請購日期" HeaderStyle-Width="90" ItemStyle-HorizontalAlign="Center" SortExpression="a.DATE1">
                                                        <itemtemplate><asp:Label ID="請購日期" Text='<%# Master.Models.strDateADToChiness(Container.DataItem("DATE1").ToShortDateString.ToString)%>' runat="server" /></itemtemplate>
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="年度" HeaderStyle-Width="40" ItemStyle-HorizontalAlign="Center" SortExpression="a.ACCYEAR">
                                                        <itemtemplate><asp:Label ID="年度" Text='<%# Container.DataItem("ACCYEAR").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="科目" HeaderStyle-Width="100" SortExpression="a.ACCNO">
                                                        <itemtemplate><asp:Label ID="科目" Text='<%# Container.DataItem("ACCNO").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="科目名稱" HeaderStyle-Width="160" SortExpression="b.ACCNAME">
                                                        <itemtemplate><asp:Label ID="科目名稱" Text='<%# Container.DataItem("ACCNAME").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="事由" HeaderStyle-Width="220" SortExpression="a.REMARK">
                                                        <itemtemplate><asp:Label ID="事由" Text='<%# Container.DataItem("REMARK").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>   
                                                    <asp:TemplateColumn HeaderText="開支金額"  HeaderStyle-Width="100" ItemStyle-HorizontalAlign="Right" SortExpression="a.useamt">
                                                        <itemtemplate><asp:Label ID="開支金額" Text='<%# FormatNumber(Container.DataItem("useamt").ToString, 0)%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="請購金額"  HeaderStyle-Width="100" ItemStyle-HorizontalAlign="Right" SortExpression="a.amt1">
                                                        <itemtemplate><asp:Label ID="請購金額" Text='<%# FormatNumber(Container.DataItem("amt1").ToString, 0)%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>                                                    
                                                </columns>
                                            </asp:DataGrid>
                                        </div>
                                    </ContentTemplate>
                                </AjaxToolkit:TabPanel>

                                <AjaxToolkit:TabPanel ID="TabPanel2" runat="server">
                                    <HeaderTemplate>單筆明細審核</HeaderTemplate>
                                    <ContentTemplate>
                                        <table id="table-data" rules="all">
                                            <tr>
                                                <th>年度：</th>
                                                <td colspan="5"><asp:Label ID="lblYear" ForeColor="Blue" Font-Size="12pt" Font-Bold="True" runat="server" /></td>
                                            </tr>
                                            <tr>
                                                <th>請購編號：</th>
                                                <td>
                                                    <asp:Label ID="lblBgno" ForeColor="Blue" Font-Size="12pt" Font-Bold="True" runat="server" />
                                                    <asp:Label ID="lblkey" ForeColor="Blue" Font-Size="12pt" Font-Bold="True" Visible="False" runat="server" />
                                                </td>
                                                <th>六級餘額查詢：</th>
                                                <td colspan="3">
                                                    <asp:Label ID="lblGrade6" ForeColor="Blue" Font-Size="12pt" Font-Bold="True" runat="server" />
                                                    <asp:Button ID="btnGrade6" Text="六級餘額查詢" CssClass="btn btn-info" runat="server" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <th>請購科目：</th>
                                                <td colspan="5">
                                                    <asp:Label ID="lblAccno" ForeColor="Blue" Font-Size="12pt" Font-Bold="True" runat="server" />
                                                    <asp:Label ID="lblAccname" ForeColor="Blue" Font-Size="12pt" Font-Bold="True"  runat="server" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <th>傳票：</th>
                                                <td colspan="5">
                                                    <asp:RadioButton id="rdbKind1" Text="收入" GroupName="rdbKind" runat="server"/>
                                                    <asp:RadioButton id="rdbKind2" Text="支出" GroupName="rdbKind" runat="server"/>
                                                </td>
                                            </tr>
                                            <tr>
                                                <th>請購金額：</th>
                                                <td style="width:20%;"><asp:Label ID="lblAmt1" ForeColor="Blue" Font-Size="12pt" Font-Bold="True" runat="server" /></td>
                                                <th>可支用額：</th>
                                                <td style="width:20%;"><asp:Label ID="lblUseableAmt" ForeColor="Blue" Font-Size="12pt" Font-Bold="True" runat="server" /></td>
                                                <th>單位請購日期：</th>
                                                <td style="width:20%;"><asp:Label ID="lblDate1"  runat="server" /></td>                                                
                                            </tr>
                                            <tr>
                                                <th>發包金額：</th>
                                                <td><asp:Label ID="lblAmt2" ForeColor="Blue" Font-Size="12pt" Font-Bold="True" runat="server" /></td>
                                                <th>變更金額：</th>
                                                <td><asp:Label ID="lblAmt3" ForeColor="Blue" Font-Size="12pt" Font-Bold="True" runat="server" /></td>
                                                <th>主計審核日期：</th>
                                                <td><asp:Label ID="lblDate2"  runat="server" /></td>
                                            </tr>
                                            <tr>           
                                                <th>單位開支金額：</th>
                                                <td><asp:Label ID="lblUseAmt" ForeColor="Blue" Font-Size="12pt" Font-Bold="True" runat="server" /></td>                                     
                                                <th>單位開支日期：</th>
                                                <td colspan="3"><asp:Label ID="lblDate3"  runat="server" /></td>                                                
                                            </tr>
                                            <tr>
                                                <th>開支事由：</th>
                                                <td colspan="5">
                                                    <asp:TextBox ID="txtRemark" CssClass="form-control td-left"  Font-Bold="True" Font-Size="12pt" ForeColor="Blue" runat="server" />
                                                    <asp:Label ID="lblRemark" CssClass="td-right" ForeColor="Blue" Font-Size="12pt" Font-Bold="True" runat="server" Visible="False" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <th>開支金額：</th>
                                                <td>
                                                    <asp:TextBox ID="txtUseAmt" CssClass="form-control" placeholder="開支金額"  Font-Size="12pt" ForeColor="Blue" runat="server" />
                                                    <asp:Label ID="lblAMT" ForeColor="Blue" Font-Size="12pt" Font-Bold="True" runat="server" Visible="False" />
                                                </td>
                                                <th>本筆已開支額=</th>
                                                <td colspan="3"><asp:Label ID="lblUsedAmt" ForeColor="Blue" Font-Size="12pt" Font-Bold="True"  runat="server" /></td>
                                            </tr>
                                            <tr>
                                                <th>受款人：</th>
                                                <td colspan="5">
                                                    <asp:TextBox ID="txtSubject" CssClass="form-control" placeholder="直接開支才填受款人" Font-Bold="True"  Font-Size="12pt" ForeColor="Blue" runat="server" />
                                                    <asp:Label ID="lblSubject" ForeColor="Blue" Font-Size="12pt" Font-Bold="True"  runat="server" Visible="False" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <th>預算科目餘額=</th>
                                                <td><asp:Label ID="lblBgamt" ForeColor="Blue" Font-Size="12pt"  runat="server" /></td>
                                                <th>科目開支餘額=</th>
                                                <td colspan="3"><asp:Label ID="lblUnUseamt" ForeColor="Blue" Font-Size="12pt"  runat="server" /></td>
                                            </tr>
                                            <tr>                                                
                                                <td colspan="6" align="center">
                                                    <asp:Button ID="btnSure" Text="核准開支" CssClass="btn btn-primary" runat="server" />&nbsp;&nbsp;&nbsp; 
                                                    <asp:Button ID="btnReturn" Text="退回業務單位" CssClass="btn btn-success" runat="server" />&nbsp;&nbsp;&nbsp; 
                                                    <asp:Button ID="btnBack" Text="返回" CssClass="btn btn-danger" runat="server" />
                                                </td>
                                            </tr>
                                        </table>
                                        <div style="display:none;">
                                            憑證應補事項
                                            <asp:Label ID="lblMark" runat="server" />
                                            <asp:TextBox ID="txtMark" CssClass="form-control" runat="server" />
                                            <asp:DropDownList ID="cboMark" CssClass="form-control" AutoPostBack="True" runat="server" />
                                        </div>
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
