<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="ACCBG.aspx.vb" Inherits="LAEACC.ACCBG" %>
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
                            <div class="widget-body form-horizontal" style="margin:10px 0px 10px 0px;">
                                  <fieldset>
								      <table id="table-serch" rules="all">
                                          <tr>
                                              <td>年度：</td>
                                              <td><asp:TextBox ID="nudYear" CssClass="form-control" Width="80" TextMode="Number" runat="server" min="0" max="999" step="1" /></td>
                                          </tr>
                                          <tr>
                                              <td>科目：</td>
                                              <td>
                                                  <asp:TextBox ID="vxtStartNo" runat="server" />
                                                  <AjaxToolkit:MaskedEditExtender ID="vxtStartNo_Mask" runat="server"
                                                        TargetControlID="vxtStartNo"
                                                        MaskType="None" Mask="?-????-??-??-???????-?"
                                                        InputDirection="LeftToRight" />～
                                                  <asp:TextBox ID="vxtEndNo" runat="server" />
                                                  <AjaxToolkit:MaskedEditExtender ID="MaskedEditExtender1" runat="server"
                                                        TargetControlID="vxtEndNo"
                                                        MaskType="None" Mask="?-????-??-??-???????-?"
                                                        InputDirection="LeftToRight" />
                                              </td>
                                          </tr>
                                          <tr>
                                              <td colspan="2" align="center">
                                                  <asp:Button ID="btnSave" Text="查詢" CssClass="btn btn-primary" runat="server" />
                                                  <asp:Button ID="btnClear" Text="清除條件" CssClass="btn btn-primary" runat="server" />
                                                  <asp:Button ID="btnExport" Text="匯出" CssClass="btn btn-primary" runat="server" />
                                              </td>
                                          </tr>
								      </table>
                                      <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                                          <ContentTemplate></ContentTemplate>
                                          <Triggers>
                                              <asp:PostBackTrigger ControlID="btnExport" />
                                          </Triggers>
                                      </asp:UpdatePanel>
                                      <div class="form-group">
                                          <asp:Label ID="lblMsgMod" ForeColor="Blue" Font-Size="12pt" Font-Bold="True" runat="server" />
                                      </div>                                     
							    </fieldset> 
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
                                                共<asp:Label ID="lbl_GrdCount" ForeColor="Red" Font-Size="14" Font-Bold="true" Text="0" runat="server" />筆符合&nbsp;                                                
                                                <asp:Label ID="lbl_sort" runat="server" />
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
                                                    <asp:TemplateColumn HeaderText="會計科目" HeaderStyle-Width="100" SortExpression="a.accno">
                                                        <itemtemplate><asp:Label ID="會計科目" Text='<%# Container.DataItem("accno").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="會計科目名稱" HeaderStyle-Width="160" SortExpression="a.accname">
                                                        <itemtemplate><asp:Label ID="會計科目名稱" Text='<%# Container.DataItem("accname").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="第一季預算數" HeaderStyle-Width="80" ItemStyle-HorizontalAlign="right" SortExpression="a.bg1">
                                                        <itemtemplate><asp:Label ID="第一季預算數" Text='<%# FormatNumber(Container.DataItem("bg1").ToString, 2)%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="第二季預算數" HeaderStyle-Width="80" ItemStyle-HorizontalAlign="right" SortExpression="a.bg2">
                                                        <itemtemplate><asp:Label ID="第二季預算數" Text='<%# FormatNumber(Container.DataItem("bg2").ToString, 2)%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>  
                                                    <asp:TemplateColumn HeaderText="第三季預算數" HeaderStyle-Width="80" ItemStyle-HorizontalAlign="right" SortExpression="a.bg3">
                                                        <itemtemplate><asp:Label ID="第三季預算數" Text='<%# FormatNumber(Container.DataItem("bg3").ToString, 2)%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>  
                                                    <asp:TemplateColumn HeaderText="第四季預算數" HeaderStyle-Width="80" ItemStyle-HorizontalAlign="right" SortExpression="a.bg4">
                                                        <itemtemplate><asp:Label ID="第四季預算數" Text='<%# FormatNumber(Container.DataItem("bg4").ToString, 2)%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>  
                                                    <asp:TemplateColumn HeaderText="預算保留數" HeaderStyle-Width="80" ItemStyle-HorizontalAlign="right" SortExpression="a.bg5">
                                                        <itemtemplate><asp:Label ID="預算保留數" Text='<%# FormatNumber(Container.DataItem("bg5").ToString, 2)%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>  
                                                    <asp:TemplateColumn HeaderText="第一季變動數" HeaderStyle-Width="80" ItemStyle-HorizontalAlign="right" SortExpression="a.bg1">
                                                        <itemtemplate><asp:Label ID="第一季變動數" Text='<%# FormatNumber(Container.DataItem("UP1").ToString, 2)%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="第二季變動數" HeaderStyle-Width="80" ItemStyle-HorizontalAlign="right" SortExpression="a.bg2">
                                                        <itemtemplate><asp:Label ID="第二季變動數" Text='<%# FormatNumber(Container.DataItem("UP2").ToString, 2)%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>  
                                                    <asp:TemplateColumn HeaderText="第三季變動數" HeaderStyle-Width="80" ItemStyle-HorizontalAlign="right" SortExpression="a.bg3">
                                                        <itemtemplate><asp:Label ID="第三季變動數" Text='<%# FormatNumber(Container.DataItem("UP3").ToString, 2)%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>  
                                                    <asp:TemplateColumn HeaderText="第四季變動數" HeaderStyle-Width="80" ItemStyle-HorizontalAlign="right" SortExpression="a.bg4">
                                                        <itemtemplate><asp:Label ID="第四季變動數" Text='<%# FormatNumber(Container.DataItem("UP4").ToString, 2)%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>  
                                                    <asp:TemplateColumn HeaderText="保留變動數" HeaderStyle-Width="80" ItemStyle-HorizontalAlign="right" SortExpression="a.bg5">
                                                        <itemtemplate><asp:Label ID="保留變動數" Text='<%# FormatNumber(Container.DataItem("UP5").ToString, 2)%>' runat="server" /></itemtemplate>                                                       
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
                                                    <asp:TextBox ID="txtAccYear" CssClass="form-control td-left" Width="80px" TextMode="Number" runat="server" min="0" max="999" step="1" runat="server" />
                                                </td>
                                                <th>科目代號：</th>
                                                <td>
                                                    <asp:TextBox ID="vxtAccno" CssClass="form-control td-left" Width="80px" runat="server"  runat="server" />
                                                    <asp:Label ID="lblAccname" CssClass="td-right" ForeColor="Blue" Font-Size="12pt" Font-Bold="True"  runat="server" />
                                                    <asp:Label ID="lblkey" Visible="False"  runat="server" />
                                                </td>                                                    
                                            </tr>

                                            <tr>
                                                <td colspan="4">
                                                    <table id="table-data" rules="all">
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
                                                            <td style="text-align:right;"><asp:Label ID="lblNet1" ForeColor="Blue" Font-Size="12pt" Font-Bold="True"  runat="server" /></td>
                                                        </tr>
                                                        <tr>
                                                            <th style="text-align:center;">第二季</th>
                                                            <td><asp:TextBox ID="txtBg2" CssClass="form-control" runat="server" /></td>
                                                            <td><asp:TextBox ID="txtUp2" CssClass="form-control" runat="server" /></td>
                                                            <td style="text-align:right;"><asp:Label ID="lblNet2" ForeColor="Blue" Font-Size="12pt" Font-Bold="True"  runat="server" /></td>
                                                        </tr>
                                                        <tr>
                                                            <th style="text-align:center;">第三季</th>
                                                            <td><asp:TextBox ID="txtBg3" CssClass="form-control" runat="server" /></td>
                                                            <td><asp:TextBox ID="txtUp3" CssClass="form-control" runat="server" /></td>
                                                            <td style="text-align:right;"><asp:Label ID="lblNet3" ForeColor="Blue" Font-Size="12pt" Font-Bold="True"  runat="server" /></td>
                                                        </tr>
                                                        <tr>
                                                            <th style="text-align:center;">第四季</th>
                                                            <td><asp:TextBox ID="txtBg4" CssClass="form-control" runat="server" /></td>
                                                            <td><asp:TextBox ID="txtUp4" CssClass="form-control" runat="server" /></td>
                                                            <td style="text-align:right;"><asp:Label ID="lblNet4" ForeColor="Blue" Font-Size="12pt" Font-Bold="True"  runat="server" /></td>
                                                        </tr>
                                                        <tr>
                                                            <th style="text-align:center;">保留數</th>
                                                            <td><asp:TextBox ID="txtBg5" CssClass="form-control" runat="server" /></td>
                                                            <td><asp:TextBox ID="txtUp5" CssClass="form-control" runat="server" /></td>
                                                            <td style="text-align:right;"><asp:Label ID="lblNet5" ForeColor="Blue" Font-Size="12pt" Font-Bold="True"  runat="server" /></td>
                                                        </tr>
                                                        <tr>
                                                            <th style="text-align:center;">合計</th>
                                                            <td style="text-align:right;"><asp:Label ID="lblSumBg" ForeColor="Blue" Font-Size="12pt" Font-Bold="True"  runat="server" /></td>
                                                            <td style="text-align:right;"><asp:Label ID="lblSumUp" ForeColor="Blue" Font-Size="12pt" Font-Bold="True"  runat="server" /></td>
                                                            <td style="text-align:right;"><asp:Label ID="lblSumNet" ForeColor="Blue" Font-Size="12pt" Font-Bold="True"  runat="server" /></td>
                                                        </tr>
                                                    </table>
                                                </td>
                                            </tr>                                                
                                            <tr>
                                                <th>預計借方增加數：</th>
                                                <td style="width:25%;"><asp:TextBox ID="txtDeamt" CssClass="form-control" runat="server" /></td>
                                                <th>預計貸方增加數</th>
                                                <td style="width:25%;"><asp:TextBox ID="txtCramt" CssClass="form-control" runat="server" /></td>
                                            </tr>
                                        </table>                                        
                                    </ContentTemplate>
                                </AjaxToolkit:TabPanel>
                                <AjaxToolkit:TabPanel ID="TabPanel3" runat="server">
                                    <HeaderTemplate>由預算系統轉入</HeaderTemplate>
                                    <ContentTemplate>        
                                                                       
                                        <table id="table-data" rules="all">
                                            <tr><td colspan="2" style="color:#f84444;font-size:16px;font-weight:bold;text-align:center;">由預算系統轉入</td></tr>                                 
                                            <tr><td colspan="2" style="color:#f84444;font-size:16px;font-weight:bold;text-align:center;">此程式將會先刪除accbg,在由bgp020轉入,因此你必須先執行預算系統預算分配表(全部)</td></tr>                                 
                                            <tr>
                                                <th>年度：</th>
                                                <td>
                                                    <asp:TextBox ID="txtYear" CssClass="form-control td-left" Width="80px" runat="server" />
                                                </td>                                                    
                                            </tr>
                                            <tr>                                                
                                                <td colspan="2" align="center"><asp:Button ID="btnTrans" Text="執行" CssClass="btn btn-primary" runat="server" /> </td>
                                            </tr>  
                                        </table> 
                                    </ContentTemplate>
                                </AjaxToolkit:TabPanel>
                                <AjaxToolkit:TabPanel ID="TabPanel4" runat="server">
                                    <HeaderTemplate>自5級累計至4.3.2級</HeaderTemplate>
                                    <ContentTemplate>        
                                                                       
                                        <table id="table-data" rules="all">
                                            <tr><td colspan="2" style="color:#f84444;font-size:16px;font-weight:bold;text-align:center;">各項預算取五級資料自動累計至四級三級二級</td></tr>                                 
                                            <tr><td colspan="2" style="color:#f84444;font-size:16px;font-weight:bold;text-align:center;">此程式將會先刪除 四三二級</td></tr>                                 
                                            <tr><td colspan="2" style="color:#f84444;font-size:16px;font-weight:bold;text-align:center;">再由五級資料累計(請確定你的五級六級是正確資料)</td></tr>                                 
                                            <tr>
                                                <th>年度：</th>
                                                <td>
                                                    <asp:TextBox ID="txtAcuYear" CssClass="form-control td-left" Width="80px" runat="server" />
                                                </td>                                                    
                                            </tr>
                                            <tr>                                                
                                                <td colspan="2" align="center"><asp:RadioButton id="rdoR" Text="收入" GroupName="rdbKind" runat="server"/><asp:RadioButton id="rdoP" Text="支出" Checked="True" GroupName="rdbKind" runat="server"/><asp:RadioButton id="rdoRP" Text="收入及支出" GroupName="rdbKind" runat="server"/> </td>
                                            </tr> 
                                            <tr>                                                
                                                <td colspan="2" align="center"><asp:Button ID="btnRunAcu" Text="執行" CssClass="btn btn-primary" runat="server" /> </td>
                                            </tr> 
                                        </table> 
                                    </ContentTemplate>
                                </AjaxToolkit:TabPanel>
                                <AjaxToolkit:TabPanel ID="TabPanel5" runat="server">
                                    <HeaderTemplate>預算寫入帳簿分配檔</HeaderTemplate>
                                    <ContentTemplate>        
                                                                       
                                        <table id="table-data" rules="all">
                                            <tr><td colspan="2" style="color:#f84444;font-size:16px;font-weight:bold;text-align:center;">預算寫入帳簿分配檔</td></tr>                                 
                                            <tr><td colspan="2" style="color:#f84444;font-size:16px;font-weight:bold;text-align:center;">此程式將會先刪除accbgbook(同年度之收支),再由accbg轉入</td></tr>                                 
                                                                        
                                            <tr>
                                                <th>年度：</th>
                                                <td>
                                                    <asp:TextBox ID="txtYearTo" CssClass="form-control td-left" Width="80px" runat="server" />
                                                </td>                                                    
                                            </tr>
                                            <tr>                                                
                                                <td colspan="2" align="center"><asp:RadioButton id="rdbRev" Text="收入" GroupName="rdbKind" runat="server"/><asp:RadioButton id="rdbPay" Text="支出" Checked="True" GroupName="rdbKind" runat="server"/> </td>
                                            </tr> 
                                            <tr>                                                
                                                <td colspan="2" align="center">
                                                    <asp:Button ID="btnTransTo" Text="執行" CssClass="btn btn-primary" runat="server" /> 
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
