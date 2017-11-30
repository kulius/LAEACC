<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="BGF010.aspx.vb" Inherits="LAEACC.BGF010" %>
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
                            <div class="easyui-panel" style="width:100%;" title="查詢條件值" data-options="iconCls:'icon-search'">
                                <table id="table-serch" rules="all">
                                    <tr>
                                        <th>年度：</th>
                                        <td><asp:TextBox ID="nudYear" TextMode="Number" runat="server" min="0" max="999" step="1" /></td>
                                    </tr>
                                     <tr>
                                        <th>科目：</th>
                                        <td>
                                            <asp:TextBox ID="vxtStartNo" MaxLength="17" runat="server" />
                                            <AjaxToolkit:MaskedEditExtender ID="MaskedEditExtender2" runat="server"
                                                    TargetControlID="vxtStartNo"
                                                    MaskType="None" Mask="?-????-??-??-???????-?"
                                                    InputDirection="LeftToRight" />
                                            ～
                                            <asp:TextBox ID="vxtEndNo" MaxLength="17" runat="server" />
                                            <AjaxToolkit:MaskedEditExtender ID="MaskedEditExtender4" runat="server"
                                                    TargetControlID="vxtEndNo"
                                                    MaskType="None" Mask="?-????-??-??-???????-?"
                                                    InputDirection="LeftToRight" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="2" style="text-align:center;">
                                            <asp:Button ID="btnSave" Text="查詢" CssClass="btn btn-primary" runat="server" />
                                            <asp:Button ID="btnClear" Text="清除條件" CssClass="btn btn-primary" runat="server" />
                                            <br />
                                            <asp:Label ID="lblMsgMod" ForeColor="Blue" Font-Size="12pt" Font-Bold="True" runat="server" />
                                        </td>
                                    </tr>                                    
                                </table>
                            </div>                            
                                                  
                            <!--控制項-->
                            <div style="margin:5px 0px 5px 0px;">
                                <uc1:UCBase ID="UCBase1" runat="server" />
                            </div>
                            <div style="clear:both; height:5px;"></div>
                            
                            <!--詳細內容顯示區-->                           
                            <AjaxToolkit:TabContainer ID="TabContainer1" Width="100%" CssClass="Tab" runat="server" ActiveTabIndex="1">
                                <AjaxToolkit:TabPanel ID="TabPanel1" runat="server">
                                    <HeaderTemplate>多筆瀏灠</HeaderTemplate>
                                    <ContentTemplate>
                                        <div style="font-size:14px; OVERFLOW:auto;height:450px;">
                                            <div style="float:left; padding-right:10px;">
												<asp:Label ID="lbl_sort" runat="server" />&nbsp;
                                                共<asp:Label ID="lbl_GrdCount" ForeColor="Red" Font-Size="14" Font-Bold="true" Text="0" runat="server" />筆符合                                                                                                
                                            </div>
                                            <asp:DataGrid ID="DataGridView" Width="2048px" AllowSorting="false" AllowPaging="false" style="font-size:14px;" CssClass="table table-bordered table-condensed smart-form" runat="server" >
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
                                                                                                                                   
                                                    <asp:TemplateColumn HeaderText="年度" HeaderStyle-Width="40" ItemStyle-HorizontalAlign="Center" SortExpression="a.accyear">
                                                        <itemtemplate><asp:Label ID="年度" Text='<%# Container.DataItem("accyear").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>                                                   
                                                    <asp:TemplateColumn HeaderText="會計科目" HeaderStyle-Width="160" SortExpression="a.accno">
                                                        <itemtemplate><asp:Label ID="會計科目" Text='<%# Container.DataItem("accno").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="會計科目<br />名稱" HeaderStyle-Width="240" SortExpression="a.accname">
                                                        <itemtemplate><asp:Label ID="會計科目名稱" Text='<%# Container.DataItem("accname").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="單位" HeaderStyle-Width="40" ItemStyle-HorizontalAlign="Center" SortExpression="b.unit">
                                                        <itemtemplate><asp:Label ID="單位" Text='<%# Container.DataItem("unit").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="第一季<br />預算數" HeaderStyle-Width="100" ItemStyle-HorizontalAlign="right" SortExpression="a.bg1">
                                                        <itemtemplate><asp:Label ID="第一季預算數" Text='<%# FormatNumber(Container.DataItem("bg1").ToString, 2)%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>                                                    
                                                    <asp:TemplateColumn HeaderText="第二季<br />預算數" HeaderStyle-Width="100" ItemStyle-HorizontalAlign="right" SortExpression="a.bg2">
                                                        <itemtemplate><asp:Label ID="第二季預算數" Text='<%# FormatNumber(Container.DataItem("bg2").ToString, 2)%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>                                                      
                                                    <asp:TemplateColumn HeaderText="第三季<br />預算數" HeaderStyle-Width="100" ItemStyle-HorizontalAlign="right" SortExpression="a.bg3">
                                                        <itemtemplate><asp:Label ID="第三季預算數" Text='<%# FormatNumber(Container.DataItem("bg3").ToString, 2)%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>                                                      
                                                    <asp:TemplateColumn HeaderText="第四季<br />預算數" HeaderStyle-Width="100" ItemStyle-HorizontalAlign="right" SortExpression="a.bg4">
                                                        <itemtemplate><asp:Label ID="第四季預算數" Text='<%# FormatNumber(Container.DataItem("bg4").ToString, 2)%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>                                                      
                                                    <asp:TemplateColumn HeaderText="預算<br />保留數" HeaderStyle-Width="100" ItemStyle-HorizontalAlign="right" SortExpression="a.bg5">
                                                        <itemtemplate><asp:Label ID="預算保留數" Text='<%# FormatNumber(Container.DataItem("bg5").ToString, 2)%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>                                                      
                                                    <asp:TemplateColumn HeaderText="第一季<br />變動數" HeaderStyle-Width="100" ItemStyle-HorizontalAlign="right" SortExpression="a.UP1">
                                                        <itemtemplate><asp:Label ID="第一季變動數" Text='<%# FormatNumber(Container.DataItem("UP1").ToString, 2)%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="第二季<br />變動數" HeaderStyle-Width="100" ItemStyle-HorizontalAlign="right" SortExpression="a.UP2">
                                                        <itemtemplate><asp:Label ID="第二季變動數" Text='<%# FormatNumber(Container.DataItem("UP2").ToString, 2)%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="第三季<br />變動數" HeaderStyle-Width="100" ItemStyle-HorizontalAlign="right" SortExpression="a.UP3">
                                                        <itemtemplate><asp:Label ID="第三季變動數" Text='<%# FormatNumber(Container.DataItem("UP3").ToString, 2)%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="第四季<br />變動數" HeaderStyle-Width="100" ItemStyle-HorizontalAlign="right" SortExpression="a.UP4">
                                                        <itemtemplate><asp:Label ID="第四季變動數" Text='<%# FormatNumber(Container.DataItem("UP4").ToString, 2)%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="變動<br />保留數" HeaderStyle-Width="100" ItemStyle-HorizontalAlign="right" SortExpression="a.UP5">
                                                        <itemtemplate><asp:Label ID="變動保留數" Text='<%# FormatNumber(Container.DataItem("UP5").ToString, 2)%>' runat="server" /></itemtemplate>                                                       
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
                                                <th>年度：</th>
                                                <td>
                                                    <asp:TextBox ID="txtAccYear" CssClass="form-control td-left" Width="80" TextMode="Number" runat="server" min="0" max="999" step="1" runat="server" />
                                                    <asp:Label ID="lblYear" CssClass="td-right" ForeColor="blue" Font-Size="12pt" Font-Bold="True"  runat="server" />
                                                </td>
                                                <th>科目代號：</th>
                                                <td colspan="3">
                                                    <asp:DropDownList ID="cboAccno" CssClass="form-control td-left" Width="50%" AutoPostBack="True" runat="server" />
                                                    <asp:Label ID="lblAccno" CssClass="td-right" ForeColor="blue" Font-Size="12pt" Font-Bold="True"  runat="server" />

                                                    <asp:Button ID="btnCboAccno" Text="科目重新整理" CssClass="btn btn-primary td-right" runat="server" />
                                                    <asp:Label ID="lblkey" Visible="false"  runat="server" />
                                                </td>                                                    
                                            </tr>
                                            <tr>
                                                <th>此科目可否溢支：</th>
                                                <td>
                                                    <asp:TextBox ID="txtFlow" CssClass="form-control td-left" Width="80" runat="server" />
                                                    <asp:Label ID="labYN1" CssClass="td-right" Text="(Y/N)" runat="server" />
                                                </td>
                                                <th>預算使用單位：</th>
                                                <td><asp:TextBox ID="txtUnit" CssClass="form-control" Width="80" runat="server" /></td>
                                                <th>工程編號：<br />(工程經費時)</th>
                                                <td><asp:TextBox ID="txtEngno" CssClass="form-control" Width="80" runat="server" /></td>
                                            </tr>
                                            <tr>
                                                <td colspan="6">
                                                    <table id="table-data" rules="all">
                                                        <tr>
                                                            <td colspan="4">
                                                                <asp:Button ID="btnFour" Text="四季平均" CssClass="btn btn-info" runat="server" />
                                                                <asp:Button ID="Button1" Text="清空" CssClass="btn btn-denger" runat="server" />
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <th>&nbsp;</th>
                                                            <th style="text-align:center;">原預算數</th>
                                                            <th style="text-align:center;">變動數</th>
                                                            <th style="text-align:center;">預算淨額</th>
                                                        </tr>
                                                        <tr>
                                                            <th style="text-align:center;">第一季</th>
                                                            <td><asp:TextBox ID="txtBg1" CssClass="form-control" runat="server" /></td>
                                                            <td><asp:TextBox ID="txtUp1" CssClass="form-control" runat="server" /></td>
                                                            <td style="text-align:right;"><asp:Label ID="lblNet1" ForeColor="blue" Font-Size="12pt" Font-Bold="True"  runat="server" /></td>
                                                        </tr>
                                                        <tr>
                                                            <th style="text-align:center;">第二季</th>
                                                            <td><asp:TextBox ID="txtBg2" CssClass="form-control" runat="server" /></td>
                                                            <td><asp:TextBox ID="txtUp2" CssClass="form-control" runat="server" /></td>
                                                            <td style="text-align:right;"><asp:Label ID="lblNet2" ForeColor="blue" Font-Size="12pt" Font-Bold="True"  runat="server" /></td>
                                                        </tr>
                                                        <tr>
                                                            <th style="text-align:center;">第三季</th>
                                                            <td><asp:TextBox ID="txtBg3" CssClass="form-control" runat="server" /></td>
                                                            <td><asp:TextBox ID="txtUp3" CssClass="form-control" runat="server" /></td>
                                                            <td style="text-align:right;"><asp:Label ID="lblNet3" ForeColor="blue" Font-Size="12pt" Font-Bold="True"  runat="server" /></td>
                                                        </tr>
                                                        <tr>
                                                            <th style="text-align:center;">第四季</th>
                                                            <td><asp:TextBox ID="txtBg4" CssClass="form-control" runat="server" /></td>
                                                            <td><asp:TextBox ID="txtUp4" CssClass="form-control" runat="server" /></td>
                                                            <td style="text-align:right;"><asp:Label ID="lblNet4" ForeColor="blue" Font-Size="12pt" Font-Bold="True"  runat="server" /></td>
                                                        </tr>
                                                        <tr>
                                                            <th style="text-align:center;">保留數</th>
                                                            <td><asp:TextBox ID="txtBg5" CssClass="form-control" runat="server" /></td>
                                                            <td><asp:TextBox ID="txtUp5" CssClass="form-control" runat="server" /></td>
                                                            <td style="text-align:right;"><asp:Label ID="lblNet5" ForeColor="blue" Font-Size="12pt" Font-Bold="True"  runat="server" /></td>
                                                        </tr>
                                                        <tr>
                                                            <th style="text-align:center;">合計</th>
                                                            <td style="text-align:right;"><asp:Label ID="lblSumBg" ForeColor="blue" Font-Size="12pt" Font-Bold="True"  runat="server" /></td>
                                                            <td style="text-align:right;"><asp:Label ID="lblSumUp" ForeColor="blue" Font-Size="12pt" Font-Bold="True"  runat="server" /></td>
                                                            <td style="text-align:right;"><asp:Label ID="lblSumNet" ForeColor="blue" Font-Size="12pt" Font-Bold="True"  runat="server" /></td>
                                                        </tr>
                                                    </table>
                                                </td>
                                            </tr>                                                
                                            <tr>
                                                <th>請購累計：</th>
                                                <td style="width:25%;"><asp:TextBox ID="txtTotper" CssClass="form-control" runat="server" /></td>
                                                <th>開支累計</th>
                                                <td style="width:25%;"><asp:TextBox ID="txtTotuse" CssClass="form-control" runat="server" /></td>
                                                <th>更動日期</th>
                                                <td style="width:25%;"><asp:Label ID="lbldate" ForeColor="blue" Font-Size="12pt" Font-Bold="True"  runat="server" /></td>
                                            </tr>
                                            <tr>
                                                <th>此科目已決算否：</th>
                                                <td colspan="5">
                                                    <asp:TextBox ID="txtCtrl" CssClass="form-control td-left" Width="80" runat="server" />
                                                    <asp:Label ID="labYN2" CssClass="td-right" Text="(Y/N)" runat="server" />
                                                </td>                                                    
                                            </tr>
                                        </table>                                        
                                    </ContentTemplate>
                                </AjaxToolkit:TabPanel>
                                <AjaxToolkit:TabPanel ID="TabPanel3" runat="server">
                                    <HeaderTemplate>條件篩選</HeaderTemplate>
                                    <ContentTemplate>                                       
                                        <table id="table-data" rules="all">
                                            <tr>
                                                <th>此科目可否溢支：</th>
                                                <td>
                                                    <asp:TextBox ID="TextBox3" CssClass="form-control td-left" Width="80" runat="server" />
                                                    <asp:Label ID="labYN3" CssClass="td-right" Text="(Y/N)" runat="server" />
                                                </td>                                                    
                                                <th>預算使用單位：</th>
                                                <td><asp:TextBox ID="TextBox1" CssClass="form-control td-left" Width="80" runat="server" /></td>
                                                <th>工程編號：<br />(工程經費時)</th>
                                                <td><asp:TextBox ID="TextBox2" runat="server" /></td>
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
        <asp:Label ID="Label1" runat="server" Text="Label"></asp:Label>

</asp:Content>
