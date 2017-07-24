<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="BG020.aspx.vb" Inherits="LAEACC.BG020" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="AjaxToolkit" %>

<%@ MasterType VirtualPath="~/Site.master" %>
<%@ Register SRC="~/LAE10406/UserControl/UCBase.ascx" TagName="UCBase" TagPrefix="uc1" %>

<asp:Content ID="Head" ContentPlaceHolderID="MainHead" runat="server">
    <script type="text/javascript">
    function changeColor() {
        var color = "orange|gray"; color = color.split("|"); document.getElementById("MainContent_TabContainer1_TabPanel3_lblMsg").style.color = color[parseInt(Math.random() * color.length)];
    }
    setInterval("changeColor()", 500);
 </script>
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
                            <AjaxToolkit:TabContainer ID="TabContainer1" Width="100%" CssClass="Tab" runat="server" ActiveTabIndex="2">
                                <AjaxToolkit:TabPanel ID="TabPanel1" runat="server">
                                    <HeaderTemplate>請購中清單</HeaderTemplate>
                                    <ContentTemplate>
                                        <div class="table-responsive">
                                            <div style="float:right; padding-right:10px;">
                                                共<asp:Label ID="lbl_DataGrid0Count" ForeColor="Red" Font-Size="14" Font-Bold="true" Text="0" runat="server" />筆符合&nbsp;                                                
                                                <asp:Label ID="bm0accyear" runat="server" /><asp:Label ID="bm0accno" runat="server" />
                                            </div>
                                            <asp:DataGrid ID="DataGrid0" Width = "100%" AllowSorting="False" AllowPaging="False" style="font-size:14px;" CssClass="table table-bordered table-condensed smart-form" runat="server" >
                                                <columns>
                                                    <asp:TemplateColumn HeaderText="管理" HeaderStyle-Width="40" ItemStyle-HorizontalAlign="Center">
                                                        <itemtemplate>                                                                                                            
                                                            <asp:ImageButton ID="Show" AlternateText="查閱" ImageUrl="~/active/images/icon/items/zoom.png" CommandName="btnShow1" runat="server" />
                                                            <asp:Label ID="id" Text='<%# Container.DataItem("ACCNO").ToString%>' Visible="false" runat="server" />
                                                            <asp:UpdatePanel ID="DataGridView_UpdatePanel" runat="server">                                                                
                                                                <Triggers><asp:AsyncPostBackTrigger ControlID="Show" EventName="Click" /></Triggers>
                                                            </asp:UpdatePanel>
                                                        </itemtemplate>                                                         
                                                    </asp:TemplateColumn>                                                    
                                                    <asp:TemplateColumn HeaderText="年度" HeaderStyle-Width="40px" ItemStyle-HorizontalAlign="Center" SortExpression="a.ACCYEAR">
                                                        <itemtemplate><asp:Label ID="年度" Text='<%# Container.DataItem("ACCYEAR").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="科目" HeaderStyle-Width="100px" SortExpression="a.ACCNO">
                                                        <itemtemplate><asp:Label ID="科目" Text='<%# Container.DataItem("ACCNO").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="科目名稱" HeaderStyle-Width="500px" SortExpression="b.ACCNAME">
                                                        <itemtemplate><asp:Label ID="科目名稱" Text='<%# Container.DataItem("ACCNAME").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>                                                     
                                                </columns>
                                            </asp:DataGrid>
                                        </div>
                                    </ContentTemplate>
                                </AjaxToolkit:TabPanel>

                                <AjaxToolkit:TabPanel ID="TabPanel2" runat="server">
                                    <HeaderTemplate>單筆明細審核</HeaderTemplate>
                                    <ContentTemplate>
                                        <div class="table-responsive">
                                            <div class="col-lg-10">
                                                請購編號
                                                <asp:Label ID="lblNoYear" ForeColor="Blue" Font-Size="12pt" Font-Bold="True" runat="server" />
                                                <asp:TextBox ID="txtNO" runat="server" /> 
                                                <asp:Button ID="btnNo" Text="確定" CssClass="btn btn-primary" runat="server" />
                                            </div>                                                   
             
                                            <div style="float:right; padding-right:10px;">
                                                <asp:Label ID="lbl_GrdCount" ForeColor="Red" Font-Size="14pt" Font-Bold="True" Text="0" visible="false" runat="server" />                                                
                                                <asp:Label ID="lbl_sort" runat="server" />
                                            </div>
                                            <asp:DataGrid ID="DataGridView" Width = "100%" AllowSorting="false" AllowPaging="false" CssClass="table table-bordered table-condensed smart-form" runat="server" >
                                                <columns>
                                                    <asp:TemplateColumn HeaderText="管理">
                                                        <itemtemplate>                                                                                                            
                                                            <asp:ImageButton ID="Show" AlternateText="查閱" ImageUrl="~/active/images/icon/items/zoom.png" CommandName="btnShow" runat="server" />
                                                            <asp:Label ID="id" Text='<%# Container.DataItem("BGNO").ToString%>' Visible="false" runat="server" />
                                                            <asp:UpdatePanel ID="DataGridView_UpdatePanel" runat="server">                                                                
                                                                <Triggers><asp:AsyncPostBackTrigger ControlID="Show" EventName="Click" /></Triggers>
                                                            </asp:UpdatePanel>
                                                        </itemtemplate>
                                                        <HeaderStyle Width="40px" />
                                                        <ItemStyle HorizontalAlign="Center" />
                                                    </asp:TemplateColumn>                                                                                
                                                    <asp:TemplateColumn HeaderText="請購編號" SortExpression="a.BGNO">
                                                        <itemtemplate><asp:Label ID="請購編號" Text='<%# Container.DataItem("BGNO").ToString%>' runat="server" /></itemtemplate>                                                       
                                                        <HeaderStyle Width="80px" />
                                                        <ItemStyle HorizontalAlign="Center" />
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="請購日期" SortExpression="a.DATE1">
                                                        <itemtemplate><asp:Label ID="請購日期" Text='<%# Master.Models.strDateADToChiness(Container.DataItem("DATE1").ToShortDateString.ToString)%>' runat="server" /></itemtemplate>
                                                        <HeaderStyle Width="90px" />
                                                        <ItemStyle HorizontalAlign="Center" />
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="年度" SortExpression="a.ACCYEAR">
                                                        <itemtemplate><asp:Label ID="年度" Text='<%# Container.DataItem("ACCYEAR").ToString%>' runat="server" /></itemtemplate>                                                       
                                                        <HeaderStyle Width="40px" />
                                                        <ItemStyle HorizontalAlign="Center" />
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="科目" SortExpression="a.ACCNO">
                                                        <itemtemplate><asp:Label ID="科目" Text='<%# Container.DataItem("ACCNO").ToString%>' runat="server" /></itemtemplate>                                                       
                                                        <HeaderStyle Width="100px" />
                                                    </asp:TemplateColumn>
                                                    
                                                    <asp:TemplateColumn HeaderText="事由" SortExpression="a.REMARK">
                                                        <itemtemplate><asp:Label ID="事由" Text='<%# Container.DataItem("REMARK").ToString%>' runat="server" /></itemtemplate>                                                       
                                                        <HeaderStyle Width="220px" />
                                                    </asp:TemplateColumn>   
                                                    <asp:TemplateColumn HeaderText="金額" SortExpression="a.AMT1">
                                                        <itemtemplate><asp:Label ID="金額" Text='<%# FormatNumber(Container.DataItem("AMT1").ToString, 0)%>' runat="server" /></itemtemplate>                                                       
                                                        <HeaderStyle Width="100px" />
                                                        <ItemStyle HorizontalAlign="Right" />
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="種" SortExpression="a.kind">
                                                        <itemtemplate><asp:Label ID="種" Text='<%# Container.DataItem("kind").ToString%>' runat="server" /></itemtemplate>                                                       
                                                        <HeaderStyle Width="40px" />
                                                    </asp:TemplateColumn> 
                                                    <asp:TemplateColumn HeaderText="受款人" SortExpression="a.subject">
                                                        <itemtemplate><asp:Label ID="受款人" Text='<%# Container.DataItem("subject").ToString%>' runat="server" /></itemtemplate>                                                       
                                                        <HeaderStyle Width="100px" />
                                                    </asp:TemplateColumn>     
                                                    <asp:TemplateColumn HeaderText="科目名稱" SortExpression="b.ACCNAME">
                                                        <itemtemplate><asp:Label ID="科目名稱" Text='<%# Container.DataItem("ACCNAME").ToString%>' runat="server" /></itemtemplate>                                                       
                                                        <HeaderStyle Width="160px" />
                                                    </asp:TemplateColumn>                                                     
                                                </columns>
                                            </asp:DataGrid>
                                        </div>
                                        <table id="table-data" rules="all">
                                            <tr>
                                                <td colspan="4" style="text-align:center">
                                                    <asp:Button ID="btnBack_tab1" Text="返回" CssClass="btn btn-danger" runat="server" />
                                                </td>
                                            </tr>
                                        </table>
                                    </ContentTemplate>
                                </AjaxToolkit:TabPanel>

                                <AjaxToolkit:TabPanel ID="TabPanel3" runat="server">
                                    <HeaderTemplate>單筆明細</HeaderTemplate>
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
                                                <th>六級餘額查詢：</th>
                                                <td>
                                                    <asp:Label ID="lblGrade6" ForeColor="Blue" Font-Size="12pt" Font-Bold="True" runat="server" />
                                                    <asp:Button ID="btnGrade6" Text="六級餘額查詢" CssClass="btn btn-info" runat="server" />
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
                                                <td><asp:Label ID="lblDate1"  runat="server" /></td>                                                
                                            </tr>
                                            <tr>
                                                <th>請購事由：</th>
                                                <td colspan="3">
                                                    <asp:TextBox ID="txtRemark" CssClass="form-control" Font-Size="12pt" Font-Bold="True" ForeColor="Blue" runat="server" />
                                                    <asp:Label ID="lblRemark" ForeColor="Blue" Visible="False" Font-Size="12pt" Font-Bold="True"  runat="server" />
                                                </td>                                                
                                            </tr>
                                            <tr>
                                                <th>請購審核金額：</th>
                                                <td>
                                                    <asp:TextBox ID="txtAmt1" CssClass="form-control td-left" Width="200px"  Font-Size="12pt" Font-Bold="True" ForeColor="Blue" runat="server" />
                                                    <asp:Label ID="lblUseableAmt" CssClass="td-right" ForeColor="Blue" Font-Size="12pt" Font-Bold="True" runat="server" Visible="False" />
                                                </td>
                                                <th>受款人：</th>
                                                <td>
                                                    <asp:TextBox ID="txtSubject" CssClass="form-control td-left" Width="300px" Font-Size="12pt" ForeColor="Blue" placeholder="直接開支才填受款人" runat="server" />
                                                    <asp:Label ID="lblSubject" ForeColor="Blue" Font-Size="12pt" Font-Bold="True" runat="server" Visible="False" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <th>預算餘額：</th>
                                                <td><asp:Label ID="lblBgamt" ForeColor="Blue" Font-Size="12pt" runat="server" /></td>
                                                <th>開支餘額：</th>
                                                <td><asp:Label ID="lblunUseamt" ForeColor="Blue" Font-Size="12pt" runat="server" /></td>
                                            </tr>
                                            <tr>
                                                <th>憑證應補事項：</th>
                                                <td colspan="3"><asp:Label ID="lblMark" runat="server" class="col-lg-2"></asp:Label><asp:TextBox ID="txtMark" CssClass="form-control" runat="server" /><asp:DropDownList ID="cboMark" CssClass="form-control" AutoPostBack="True" runat="server" /></td>
                                            </tr>
                                            <tr>
                                                <td colspan="4" style="text-align:center">
                                                    <asp:Label ID="lblMsg" ForeColor="Red" Font-Size="14pt" Font-Bold="True" runat="server" />                                                   
                                                    <asp:Button ID="btnSure" Text="請購登記" CssClass="btn btn-primary" runat="server" />&nbsp;&nbsp;&nbsp; 
                                                    <asp:Button ID="btnDirect" Text="直接開支" CssClass="btn btn-success" runat="server" />&nbsp;&nbsp;&nbsp; 
                                                    <asp:Button ID="btnBack_tab2" Text="返回" CssClass="btn btn-danger" runat="server" />
                                                </td>
                                            </tr>
                                        </table>
                                        <!--#伺服器驗證-->
                                        <AjaxToolkit:ConfirmButtonExtender ID="ConfirmButtonExtender1" runat="server"
                                            TargetControlID="btnDirect" ConfirmText="是否確定直接開支??" Enabled="True">
                                        </AjaxToolkit:ConfirmButtonExtender>
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
