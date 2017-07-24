<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="BG888.aspx.vb" Inherits="LAEACC.BG888" %>
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
                            <!--詳細內容顯示區-->                           
                            <AjaxToolkit:TabContainer ID="TabContainer1" Width="100%" CssClass="Tab" runat="server" ActiveTabIndex="0">
                                <AjaxToolkit:TabPanel ID="TabPanel1" runat="server">
                                    <HeaderTemplate>未開支清單</HeaderTemplate>
                                    <ContentTemplate>
                                        <div class="table-responsive">
                                            <div class="col-lg-10">
                                                請購編號
                                                <asp:Label ID="lblNoYear" ForeColor="Blue" Font-Size="12" Font-Bold="true" runat="server" />
                                                <asp:TextBox ID="txtNO"  runat="server" />
                                                <asp:Label ID="lblSeason" ForeColor="Red" Font-Size="12" Font-Bold="true" runat="server" />
                                                <asp:Button ID="btnNo" Text="確定" CssClass="btn btn-primary" runat="server" />
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

                                                    <asp:TemplateColumn HeaderText="請購編號" HeaderStyle-Width="80px">
                                                        <itemtemplate><asp:Label ID="請購編號" Text='<%# Container.DataItem("BGNO").ToString%>' runat="server" /></itemtemplate>                                                                                                               
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="請購日期" HeaderStyle-Width="80" ItemStyle-HorizontalAlign="Center">
                                                        <itemtemplate><asp:Label ID="請購日期" Text='<%#Master.Models.strDateADToChiness(Container.DataItem("DATE1").ToShortDateString.ToString)%>' runat="server" /></itemtemplate>                                                        
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="年度" HeaderStyle-Width="40" ItemStyle-HorizontalAlign="Center">
                                                        <itemtemplate><asp:Label ID="年度" Text='<%# Container.DataItem("accyear").ToString%>' runat="server" /></itemtemplate>                                                    
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="科目" HeaderStyle-Width="100">
                                                        <itemtemplate><asp:Label ID="科目" Text='<%# Container.DataItem("ACCNO").ToString%>' runat="server" /></itemtemplate>                                                        
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="科目名稱" HeaderStyle-Width="160">
                                                        <itemtemplate><asp:Label ID="科目名稱" Text='<%# Container.DataItem("ACCNAME").ToString%>' runat="server" /></itemtemplate>                                                        
                                                    </asp:TemplateColumn>                                                     
                                                    <asp:TemplateColumn HeaderText="請購事由" HeaderStyle-Width="220">
                                                        <itemtemplate><asp:Label ID="請購事由" Text='<%# Container.DataItem("REMARK").ToString%>' runat="server" /></itemtemplate>                                                        
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="請購金額" HeaderStyle-Width="100" ItemStyle-HorizontalAlign="Right">                                                    
                                                        <itemtemplate><asp:Label ID="請購金額" Text='<%# FormatNumber(Container.DataItem("AMT1").ToString,2)%>' runat="server" /></itemtemplate>                                                        
                                                    </asp:TemplateColumn>                                                                                                                                                                                                                                                                                                                         
                                                </columns>
                                            </asp:DataGrid>
                                        </div>
                                    </ContentTemplate>
                                </AjaxToolkit:TabPanel>

                                <AjaxToolkit:TabPanel ID="TabPanel2" runat="server">
                                    <HeaderTemplate>單筆明細開支</HeaderTemplate>
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
                                                    <asp:Label ID="lblautono" ForeColor="Blue" Font-Size="12pt" Font-Bold="True" Visible="False" runat="server" />
                                                </td>
                                                <th>請購科目：</th>
                                                <td colspan="3">
                                                    <asp:Label ID="lblAccno" ForeColor="Blue" Font-Size="12pt" Font-Bold="True" runat="server" />
                                                    <asp:Label ID="lblAccname" ForeColor="Blue" Font-Size="12pt" Font-Bold="True"  runat="server" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <th>請購金額：</th>
                                                <td><asp:Label ID="lblAmt1"  runat="server" /></td>
                                                <th>單位請購日期：</th>
                                                <td colspan="3"><asp:Label ID="lblDate1"  runat="server" /></td>
                                            </tr>
                                            <tr>
                                                <th>發包金額：</th>
                                                <td><asp:Label ID="lblAmt2"  runat="server" /></td>
                                                <th>主計審核日期：</th>
                                                <td colspan="3"><asp:Label ID="lblDate2"  runat="server" /></td>
                                            </tr>
                                            <tr>
                                                <th>變更金額：</th>
                                                <td><asp:Label ID="lblAmt3"  runat="server" /></td>
                                                <th>預算餘額=</th>
                                                <td colspan="3"><asp:Label ID="lblBgamt"  runat="server" /></td>
                                            </tr>
                                            <tr>
                                                <th>可支金額：</th>
                                                <td><asp:Label ID="lblUseableAmt"  runat="server" /></td>
                                                <th>開支餘額=</th>
                                                <td colspan="3"><asp:Label ID="lblunUseamt"  runat="server" /></td>
                                            </tr>
                                            <tr>
                                                <th>轉入應付科目：</th>
                                                <td colspan="5"><asp:DropDownList ID="cboAccno" CssClass="form-control" AutoPostBack="True" runat="server" /></td>                                                
                                            </tr>
                                            <tr>
                                                <th>摘要：</th>
                                                <td colspan="5"><asp:TextBox ID="txtRemark" CssClass="form-control"  runat="server" /></td>                                                
                                            </tr>
                                            <tr>
                                                <th>保留金額：</th>
                                                <td style="width:20%;"><asp:TextBox ID="txtUseAmt" CssClass="form-control" runat="server" /></td>
                                                <th>權責發生日：</th>
                                                <td style="width:20%;"><asp:TextBox ID="txtDateopen" CssClass="form-control" Width="100" onClick="WdatePicker({dateFmt:'yyy-MM-dd',skin:'whyGreen'})" runat="server" /></td>
                                                <th>保留原因：</th>
                                                <td style="width:20%;"><asp:TextBox ID="txtReason" CssClass="form-control" runat="server" /></td>
                                            </tr>
                                            <tr>
                                                <th>受款人：</th>
                                                <td>
                                                    <asp:TextBox ID="txtSubject" CssClass="form-control" runat="server" />
                                                    <asp:Label ID="lblSubject" ForeColor="Blue" Font-Size="12pt" Font-Bold="True"  runat="server" />
                                                </td>
                                                <th>預計完工日：</th>
                                                <td colspan="3"><asp:TextBox ID="txtDateclose" CssClass="form-control" Width="100" onClick="WdatePicker({dateFmt:'yyy-MM-dd',skin:'whyGreen'})" runat="server" /></td>
                                            </tr>
                                            <tr>
                                                <th>新請購編號：</th>
                                                <td colspan="5"><asp:Label ID="lblNewBgno"  runat="server" /></td>                                                
                                            </tr>
                                            <tr>                                                
                                                <td align="center" colspan="6">
                                                    <asp:Button ID="btnSure" Text="轉入應付費用" CssClass="btn btn-primary" runat="server" />
                                                    <asp:Button ID="btnBack" Text="返回清單" CssClass="btn btn-danger" runat="server" />
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
