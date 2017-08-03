<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="BG040.aspx.vb" Inherits="LAEACC.BG040" %>
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
                            <div style="margin:5px 0px 5px 0px;">
                                請購編號
                                <asp:Label ID="lblNoYear" ForeColor="Blue" Font-Size="12pt" Font-Bold="True" runat="server" />
                                <asp:TextBox ID="txtNO" runat="server" /> 
                                <asp:Button ID="btnNo" Text="確定" CssClass="btn btn-primary" runat="server" />
                                <asp:Label ID="lblSeason"  runat="server" />
                            </div>
                                                        
                            <!--詳細內容顯示區-->                           
                            <AjaxToolkit:TabContainer ID="TabContainer1" Width="100%" CssClass="Tab" runat="server" ActiveTabIndex="1">
                                <AjaxToolkit:TabPanel ID="TabPanel1" runat="server">
                                    <HeaderTemplate>請購中清單</HeaderTemplate>
                                    <ContentTemplate>
                                        <div class="table-responsive">             
                                            <div style="float:right; padding-right:10px;">
                                                共<asp:Label ID="lbl_GrdCount" ForeColor="Red" Font-Size="14" Font-Bold="true" Text="0" runat="server" />筆符合&nbsp;                                                
                                                <asp:Label ID="lbl_sort" runat="server" />
                                            </div>
                                            <asp:DataGrid ID="DataGridView" Width = "100%" AllowSorting="True" AllowPaging="True" style="font-size:14px;" CssClass="table table-bordered table-condensed smart-form" runat="server" >
                                                <columns>
                                                    <asp:TemplateColumn HeaderText="管理" HeaderStyle-Width="40" ItemStyle-HorizontalAlign="Center">
                                                        <itemtemplate>                                                                                                            
                                                            <asp:ImageButton ID="Show" AlternateText="查閱" ImageUrl="~/active/images/icon/items/edit.png" CommandName="btnShow" runat="server" />
                                                            <asp:Label ID="id" Text='<%# Container.DataItem("BGNO").ToString%>' Visible="false" runat="server" />
                                                            <asp:UpdatePanel ID="DataGridView_UpdatePanel" runat="server">                                                                
                                                                <Triggers><asp:AsyncPostBackTrigger ControlID="Show" EventName="Click" /></Triggers>
                                                            </asp:UpdatePanel>
                                                        </itemtemplate>                                                         
                                                    </asp:TemplateColumn>   
                                                                                                                                 
                                                    <asp:TemplateColumn HeaderText="請購編號" HeaderStyle-Width="80" ItemStyle-HorizontalAlign="Center" SortExpression="a.BGNO">
                                                        <itemtemplate><asp:Label ID="請購編號" Text='<%# Container.DataItem("BGNO").ToString%>' runat="server" /></itemtemplate>
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="請購日期" HeaderStyle-Width="80" ItemStyle-HorizontalAlign="Center" SortExpression="a.DATE1">
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
                                                    <asp:TemplateColumn HeaderText="金額" HeaderStyle-Width="100" ItemStyle-HorizontalAlign="Right" SortExpression="a.AMT1">
                                                        <itemtemplate><asp:Label ID="金額" Text='<%# FormatNumber(Container.DataItem("AMT1").ToString, 2)%>' runat="server" /></itemtemplate>
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
                                                <td colspan="3"><asp:Label ID="lblYear" ForeColor="Blue" Font-Size="12pt" Font-Bold="True" runat="server" /></td>
                                            </tr>
                                            <tr>
                                                <th>請購編號：</th>
                                                <td>
                                                    <asp:Label ID="lblBgno" ForeColor="Blue" Font-Size="12pt" Font-Bold="True" runat="server" />
                                                    <asp:Label ID="lblkey" ForeColor="Blue" Font-Size="12pt" Font-Bold="True" Visible="False" runat="server" />
                                                </td>
                                                <th>六級科目餘額：</th>
                                                <td>
                                                    <asp:Button ID="btnGrade6" Text="六級餘額查詢" CssClass="btn btn-info" runat="server" />
                                                    <asp:Label ID="lblGrade6" ForeColor="Blue" Font-Size="12pt" Font-Bold="True" runat="server" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <th>請購科目：</th>
                                                <td colspan="3">
                                                    <asp:Label ID="lblAccno" ForeColor="Blue" Font-Size="12pt" Font-Bold="True" runat="server" />
                                                    <asp:Label ID="lblAccname" ForeColor="Blue" Font-Size="12pt" Font-Bold="True"  runat="server" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <th>傳票：</th>
                                                <td colspan="3">
                                                    <asp:RadioButton id="rdbKind1" Text="收入" GroupName="rdbKind" runat="server"/>
                                                    <asp:RadioButton id="rdbKind2" Text="支出" GroupName="rdbKind" runat="server"/>
                                                </td>
                                            </tr>
                                            <tr>
                                                <th>請購金額：</th>
                                                <td><asp:Label ID="lblAmt1" ForeColor="Blue" Font-Size="12pt" Font-Bold="True" runat="server" /></td>
                                                <th>單位請購日期：</th>
                                                <td><asp:Label ID="lblDate1" runat="server" /></td>
                                            </tr>
                                            <tr>
                                                <th>發包金額：</th>
                                                <td><asp:Label ID="lblAmt2" ForeColor="Blue" Font-Size="12pt" Font-Bold="True" runat="server" /></td>
                                                <th>主計審核日期：</th>
                                                <td><asp:Label ID="lblDate2" runat="server" /></td>
                                            </tr>
                                            <tr>
                                                <th>變更金額：</th>
                                                <td><asp:Label ID="lblAmt3" ForeColor="Blue" Font-Size="12pt" Font-Bold="True" runat="server" /></td>
                                                <th>預算科目餘額=</th>
                                                <td><asp:Label ID="lblBgamt" ForeColor="Blue" Font-Size="12pt" Font-Bold="True"  runat="server" /></td>
                                            </tr>
                                            <tr>
                                                <th>可支用：</th>
                                                <td><asp:Label ID="lblUseableAmt" ForeColor="Blue" Font-Size="12pt" Font-Bold="True"  runat="server" /></td>
                                                <th>科目開支餘額=</th>
                                                <td><asp:Label ID="lblUnUseamt" ForeColor="Blue" Font-Size="12pt" Font-Bold="True"  runat="server" /></td>
                                            </tr>
                                            <tr>
                                                <th>開支事由：</th>
                                                <td colspan="3"><asp:TextBox ID="txtRemark" CssClass="form-control" placeholder="開支事由" runat="server" /></td>
                                            </tr>
                                            <tr>
                                                <th>開支金額：</th>
                                                <td>
                                                    <asp:TextBox ID="txtUseAmt" CssClass="form-control" Width="40%" placeholder="開支金額" runat="server" />
                                                    <asp:Label ID="lblAMT" ForeColor="Blue" Font-Size="12pt" Font-Bold="True" runat="server" />
                                                </td>
                                                <th>本次己分次開支：</th>
                                                <td><asp:Label ID="lblUsedAmt" ForeColor="Blue" Font-Size="12pt" Font-Bold="True"  runat="server" /></td>
                                            </tr>
                                            <tr>
                                                <th>受款人：</th>
                                                <td colspan="3">
                                                    <asp:TextBox ID="txtSubject" CssClass="form-control td-left" Width="40%" placeholder="直接開支才填受款人" runat="server" />
                                                    <asp:Button ID="btnAddSubject" Text="將受款人增入片語" CssClass="btn btn-warning td-right" runat="server" />
                                                    <asp:Label ID="lblSubject" ForeColor="Blue" Font-Size="12pt" Font-Bold="True"  runat="server" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td colspan="4" align="center">
                                                    <asp:Button ID="btnSure" Text="開支或最後一次開支" CssClass="btn btn-primary" runat="server" />
                                                    <asp:Button ID="btnSure2" Text="分次開支" CssClass="btn btn-primary" runat="server" />
                                                    <asp:Button ID="btnZero" Text="取消開支" CssClass="btn btn-primary" runat="server" />
                                                    <asp:Button ID="btnBack" Text="返回" CssClass="btn btn-danger" runat="server" />
                                                </td>
                                            </tr>
                                        </table>
                                        <div style="display:none;">
                                            管理處
                                            <asp:Label ID="gbxArea"  runat="server" Visible="False" />
                                            <asp:TextBox ID="txtArea" CssClass="form-control" runat="server" />
                                        </div>

                                        <!--#伺服器驗證-->
                                        <AjaxToolkit:ConfirmButtonExtender ID="ConfirmButtonExtender1" runat="server"
                                            TargetControlID="btnSure" ConfirmText="確定本筆將全部開支或最後一次開支??" BehaviorID="_content_ConfirmButtonExtender1">
                                        </AjaxToolkit:ConfirmButtonExtender>

                                        <!--#自動完成-->
                                        <AjaxToolkit:AutoCompleteExtender 
                                            ID="AutoCompleteExtender1"
                                            runat="server"                
                                            TargetControlID="txtSubject"
                                            ServicePath="~/active/WebService.asmx"
                                            ServiceMethod="GetCompletionListSubject"
                                            MinimumPrefixLength="0" 
                                            CompletionInterval="100"
                                            CompletionSetCount="12" BehaviorID="_content_AutoCompleteExtender2" DelimiterCharacters="" />
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
