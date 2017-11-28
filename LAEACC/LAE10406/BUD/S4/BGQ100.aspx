<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="BGQ100.aspx.vb" Inherits="LAEACC.BGQ100" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="AjaxToolkit" %>

<%@ MasterType VirtualPath="~/Site.master" %>
<%@ Register SRC="~/LAE10406/UserControl/UCBase.ascx" TagName="UCBase" TagPrefix="uc1" %>

<asp:Content ID="Head" ContentPlaceHolderID="MainHead" runat="server">
    
</asp:content>

<asp:Content ID="Content" ContentPlaceHolderID="MainContent" runat="server">
<asp:UpdatePanel ID="UpdatePanel1" runat="server" >
       <Triggers>
          <asp:PostBackTrigger ControlID="btnExcel" />
           <asp:PostBackTrigger ControlID="btnPrint1" />
       </Triggers>
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
                                        <td colspan="2" style="text-align:center;">
                                            <asp:Button ID="BtnSearch" Text="查詢" CssClass="btn btn-primary" runat="server" />
                                            &nbsp;|&nbsp;
                                            <asp:Button ID="btnPrint1" Text="推算簿列印" CssClass="btn btn-primary" runat="server" />
                                            <asp:Button ID="btnExcel" Text="科目開支EXCEL" CssClass="btn btn-primary" runat="server" />
                                        </td>
                                    </tr>                                    
                                </table>
                            </div>                            
                                            
                            <!--詳細內容顯示區-->                           
                            <AjaxToolkit:TabContainer ID="TabContainer1" Width="100%" CssClass="Tab" runat="server" ActiveTabIndex="0">
                                <AjaxToolkit:TabPanel ID="TabPanel1" runat="server">
                                    <HeaderTemplate>科目分類</HeaderTemplate>
                                    <ContentTemplate>
                                        <div class="table-responsive">
                                            <div style="float:right; padding-right:10px;">
                                                <asp:Label ID="lbl_DataGrid0Count" ForeColor="Red" Font-Size="14" Font-Bold="true" Text="0" Visible="false" runat="server" />                                                
                                                <asp:Label ID="bm0accyear" runat="server" /><asp:Label ID="bm0accno" runat="server" />
                                            </div>
                                            <asp:DataGrid ID="DataGrid1" Width = "100%" AllowSorting="False" AllowPaging="False" style="font-size:14px;" CssClass="table table-bordered table-condensed smart-form" runat="server" >
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

                                                    <asp:TemplateColumn HeaderText="年度" HeaderStyle-Width="40" ItemStyle-HorizontalAlign="Center">
                                                        <itemtemplate><asp:Label ID="年度" Text='<%# Container.DataItem("ACCYEAR").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="科目" HeaderStyle-Width="100">
                                                        <itemtemplate><asp:Label ID="科目" Text='<%# Container.DataItem("ACCNO").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="科目名稱" HeaderStyle-Width="160">
                                                        <itemtemplate><asp:Label ID="科目名稱" Text='<%# Container.DataItem("ACCNAME").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="預算總額" HeaderStyle-Width="100" ItemStyle-HorizontalAlign="right">
                                                        <itemtemplate><asp:Label ID="預算總額" Text='<%# FormatNumber(Container.DataItem("BGAMT").ToString, 2)%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="開支總計" HeaderStyle-Width="100" ItemStyle-HorizontalAlign="right">
                                                        <itemtemplate><asp:Label ID="開支總計" Text='<%# FormatNumber(Container.DataItem("TOTUSE").ToString, 2)%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="請購中" HeaderStyle-Width="100" ItemStyle-HorizontalAlign="right">
                                                        <itemtemplate><asp:Label ID="請購中" Text='<%# FormatNumber(Container.DataItem("TOTPER").ToString, 2)%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>                                                    
                                                    <asp:TemplateColumn HeaderText="可溢支" HeaderStyle-Width="40" ItemStyle-HorizontalAlign="Center">
                                                        <itemtemplate><asp:Label ID="可溢支" Text='<%# Container.DataItem("flow").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="決算" HeaderStyle-Width="40" ItemStyle-HorizontalAlign="Center">
                                                        <itemtemplate><asp:Label ID="決算" Text='<%# Container.DataItem("CTRL").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="單位" HeaderStyle-Width="40" ItemStyle-HorizontalAlign="Center">
                                                        <itemtemplate><asp:Label ID="單位" Text='<%# Container.DataItem("UNIT").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>                                                     
                                                </columns>
                                            </asp:DataGrid>
                                        </div>
                                    </ContentTemplate>
                                </AjaxToolkit:TabPanel>

                                <AjaxToolkit:TabPanel ID="TabPanel2" runat="server">
                                    <HeaderTemplate>科目開支清冊</HeaderTemplate>
                                    <ContentTemplate>
                                        <table id="table-data" rules="all">
                                            <tr>
                                                <th>預算科目</th>
                                                <td colspan="5">
                                                    <asp:Label ID="lblAccno1" ForeColor="Blue" Font-Size="12pt" Font-Bold="True" runat="server" />
                                                    <asp:Label ID="lblAccname1" ForeColor="Blue" Font-Size="12pt" Font-Bold="True" runat="server" />
                                                </td>                                                                                                
                                                <td align="center" colspan="2">
                                                    <asp:RadioButton id="Sum1" Text="摘要小計" GroupName="rdbRep" runat="server"/>
                                                    <asp:RadioButton id="Sum2" Text="月份小計" GroupName="rdbRep" runat="server"/>
                                                </td>
                                            </tr>
                                            <tr>
                                                <th>總預算：</th>
                                                <td style="width:15%;"><asp:Label ID="lblBgamt" ForeColor="Blue" Font-Size="12pt" Font-Bold="True" runat="server" /></td>
                                                <th>請購中：</th>
                                                <td style="width:15%;"><asp:Label ID="lblUnuse" ForeColor="Blue" Font-Size="12pt" Font-Bold="True" runat="server" /></td>
                                                <th>已開支：</th>
                                                <td style="width:15%;"><asp:Label ID="lblUsed" ForeColor="Blue" Font-Size="12pt" Font-Bold="True" runat="server" /></td>
                                                <th>餘額：</th>
                                                <td style="width:15%;"><asp:Label ID="lblNet" ForeColor="Blue" Font-Size="12pt" Font-Bold="True" runat="server" /></td>
                                            </tr>
                                            <tr>                                                
                                                <td align="center" colspan="8">
                                                    <asp:Button ID="btnPrint" Text="推算簿列印" Visible="false" CssClass="btn btn-primary" runat="server" />
                                                    
                                                    <asp:Button ID="btnBack1" Text="回上頁" CssClass="btn btn-danger" runat="server" />
                                                </td>
                                            </tr>
                                        </table>                                                                               
                                                                                      
                                        <div class="table-responsive">
                                            <div style="float:right; padding-right:10px;">
                                                <asp:Label ID="lbl_GrdCount" ForeColor="Red" Font-Size="14pt" Font-Bold="True" Text="0" Visible="false" runat="server" />                                               
                                                <asp:Label ID="lbl_sort" runat="server" />
                                            </div>                                            
                                            <asp:DataGrid ID="DataGrid2" Width="100%" AllowPaging="false" AllowSorting="true" style="font-size:12px;" CssClass="table table-bordered table-condensed smart-form" runat="server" >
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
                                                    <asp:TemplateColumn HeaderText="年度">
                                                        <itemtemplate><asp:Label ID="年度" Text='<%# Container.DataItem("ACCYEAR").ToString%>' runat="server" /></itemtemplate>                                                        
                                                        <HeaderStyle Width="30px" />
                                                        <ItemStyle HorizontalAlign="Center" />
                                                    </asp:TemplateColumn>                                                                                
                                                    <asp:TemplateColumn HeaderText="請購編號" SortExpression="BGF020.bgno">
                                                        <itemtemplate><asp:Label ID="請購編號" Text='<%# Container.DataItem("BGNO").ToString%>' runat="server" /></itemtemplate>                                                        
                                                        <HeaderStyle Width="80px" />
                                                        <ItemStyle HorizontalAlign="Center" />
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="摘要">
                                                        <itemtemplate><asp:Label ID="摘要" Text='<%# Container.DataItem("REMARK").ToString%>' runat="server" /></itemtemplate>                                                        
                                                        <HeaderStyle Width="220px" />
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="收支">
                                                        <itemtemplate><asp:Label ID="收支" Text='<%# Container.DataItem("KIND").ToString%>' runat="server" /></itemtemplate>                                                        
                                                        <HeaderStyle Width="20px" />
                                                        <ItemStyle HorizontalAlign="Center" />
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="請購金額" SortExpression="bgf020.amt1">
                                                        <itemtemplate><asp:Label ID="請購金額" Text='<%# FormatNumber(Container.DataItem("AMT1").ToString, 2)%>' runat="server" /></itemtemplate>                                                        
                                                        <HeaderStyle Width="80px" />
                                                        <ItemStyle HorizontalAlign="Right" />
                                                    </asp:TemplateColumn>   
                                                    <asp:TemplateColumn HeaderText="開支金額" SortExpression="bgf030.useamt">
                                                        <itemtemplate><asp:Label ID="開支金額" Text='<%# FormatNumber(Container.DataItem("USEAMT").ToString, 2)%>' runat="server" /></itemtemplate>
                                                        <HeaderStyle Width="80px" />
                                                        <ItemStyle HorizontalAlign="Right" />
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="受款人">
                                                        <itemtemplate><asp:Label ID="受款人" Text='<%# Container.DataItem("SUBJECT").ToString%>' runat="server" /></itemtemplate>
                                                        <HeaderStyle Width="40px" />
                                                        <ItemStyle HorizontalAlign="left" />
                                                    </asp:TemplateColumn>     
                                                    <asp:TemplateColumn HeaderText="請購日期" SortExpression="bgf020.date1">
                                                        <itemtemplate><asp:Label ID="請購日期" Text='<%# Master.Models.strDateADToChiness(Container.DataItem("DATE1").ToString)%>' runat="server" /></itemtemplate>                                                    
                                                        <HeaderStyle Width="80px" />
                                                        <ItemStyle HorizontalAlign="Center" />
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="主計審核" SortExpression="bgf020.date2">
                                                        <itemtemplate><asp:Label ID="主計審核" Text='<%# Master.Models.strDateADToChiness(Container.DataItem("DATE2").ToString)%>' runat="server" /></itemtemplate>                                                    
                                                        <HeaderStyle Width="80px" />
                                                        <ItemStyle HorizontalAlign="Center" />
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="單位開支" SortExpression="bgf030.date3">
                                                        <itemtemplate><asp:Label ID="單位開支" Text='<%# Master.Models.strDateADToChiness(Container.DataItem("DATE3").ToString)%>' runat="server" /></itemtemplate>                                                    
                                                        <HeaderStyle Width="80px" />
                                                        <ItemStyle HorizontalAlign="Center" />
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="主計開支" SortExpression="bgf030.date4">
                                                        <itemtemplate><asp:Label ID="主計開支" Text='<%# Master.Models.strDateADToChiness(Container.DataItem("date4").ToString)%>' runat="server" /></itemtemplate>                                                    
                                                        <HeaderStyle Width="80px" />
                                                        <ItemStyle HorizontalAlign="Center" />
                                                    </asp:TemplateColumn> 
                                                    <asp:TemplateColumn HeaderText="傳票號碼">
                                                        <itemtemplate><asp:Label ID="傳票號碼" Text='<%# Container.DataItem("no_1_no").ToString%>' runat="server" /></itemtemplate>                                                    
                                                        <HeaderStyle Width="40px" />
                                                        <ItemStyle HorizontalAlign="Center" />
                                                    </asp:TemplateColumn>                                                                                                                                                              
                                                </columns>
                                            </asp:DataGrid>
                                        </div>
                                    </ContentTemplate>
                                </AjaxToolkit:TabPanel>

                                <AjaxToolkit:TabPanel ID="TabPanel3" runat="server">
                                    <HeaderTemplate>開支明細</HeaderTemplate>
                                    <ContentTemplate>
                                        <table id="table-data" rules="all">
                                            <tr>
                                                <th>年度：</th>
                                                <td><asp:Label ID="lblYear" ForeColor="Blue" Font-Size="12pt" Font-Bold="True" runat="server" /></td>
                                                <th>序號：</th>
                                                <td>
                                                    <asp:Label ID="lblRel" ForeColor="Blue" Font-Size="12pt" Font-Bold="True" runat="server" />

                                                    <asp:Label ID="lblkey" ForeColor="Blue" Font-Size="12pt" Font-Bold="True" Visible="False" runat="server" />
                                                    <asp:Label ID="lblautono" ForeColor="Blue" Font-Size="12pt" Font-Bold="True" Visible="False" runat="server" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <th>請購編號：</th>
                                                <td><asp:Label ID="lblBgno" ForeColor="Blue" Font-Size="12pt" Font-Bold="True" runat="server" /></td>
                                                <th>請購科目：</th>
                                                <td>
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
                                                <td><asp:Label ID="lblAmt1" runat="server" /></td>
                                                <th>單位請購日期：</th>
                                                <td><asp:Label ID="lblDate1" runat="server" /></td>
                                            </tr>
                                            <tr>
                                                <th>發包金額：</th>
                                                <td><asp:Label ID="lblAmt2" runat="server" /></td>
                                                <th>主計審核日期：</th>
                                                <td><asp:Label ID="lblDate2" runat="server" /></td>
                                            </tr>
                                            <tr>
                                                <th>變更金額：</th>
                                                <td><asp:Label ID="lblAmt3" runat="server" /></td>
                                                <th>單位開支日期：</th>
                                                <td><asp:Label ID="lblDate3" runat="server" /></td>
                                            </tr>
                                            <tr>
                                                <th>開支金額：</th>
                                                <td><asp:Label ID="lblUseAmt" runat="server" /></td>
                                                <th>主計開支日期：</th>
                                                <td><asp:Label ID="lblDate4" runat="server" /></td>
                                            </tr>
                                            <tr>
                                                <th>請購事由：</th>
                                                <td colspan="3">
                                                    <asp:TextBox ID="txtRemark" CssClass="form-control td-left" Width="60%"  runat="server" />
                                                    <asp:Label ID="lblRemark" CssClass="td-right" ForeColor="Blue" Font-Size="12pt" Font-Bold="True"  runat="server" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <th>開支事由：</th>
                                                <td colspan="3">
                                                    <asp:TextBox ID="txtRemark3" CssClass="form-control" runat="server" />
                                                    <asp:Label ID="lblRemark3" ForeColor="Blue"  Font-Size="12pt" Font-Bold="True"  runat="server" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <th>受款人：</th>
                                                <td>
                                                    <asp:TextBox ID="txtSubject" CssClass="form-control td-left" Width="60%" runat="server" />
                                                    <asp:Label ID="lblSubject" CssClass="td-right" ForeColor="Blue" Font-Size="12pt" Font-Bold="True"  runat="server" />
                                                </td>
                                                <th>已開傳票號：</th>
                                                <td>
                                                    <asp:TextBox ID="txtNo_1_no" CssClass="form-control td-left" Width="60%" runat="server" />
                                                    <asp:Label ID="lblNo_1_no" CssClass="td-right" ForeColor="Blue"  Font-Size="12pt" Font-Bold="True"  runat="server" />
                                                </td>
                                            </tr>
                                            <tr>                                                
                                                <td align="center" colspan="4">
                                                    <asp:Button ID="btnEmptyDate2" Text="取消主計審核" CssClass="btn btn-primary" runat="server" />
                                                    <asp:Button ID="btnEmptyDate3" Text="取消單位開支" CssClass="btn btn-primary" runat="server" />
                                                    <asp:Button ID="btnEmptyDate4" Text="取消主計開支" CssClass="btn btn-primary" runat="server" />
                                                    <asp:Button ID="btnEmptyDate" Text="快速取消開支" CssClass="btn btn-primary" runat="server" />
                                                    <asp:Button ID="btnBack" Text="回上頁" CssClass="btn btn-danger" runat="server" />
                                                </td>                                                
                                            </tr>
                                            <tr>
                                                <td align="center" colspan="4">
                                                    <asp:Button ID="btnModAccno" Text="修改科目" CssClass="btn btn-info" runat="server" /> 
                                                    <asp:Button ID="btnModOther" Text="修正摘要等" CssClass="btn btn-info" runat="server" />
                                                    <asp:Button ID="btnModAmt" Text="修改金額" CssClass="btn btn-info" runat="server" />
                                                    <asp:Button ID="btnDelete" Text="整筆刪除" CssClass="btn btn-danger" runat="server" />
                                                </td>
                                            </tr>
                                        </table>
                                        <asp:Panel  id="gbxModAccno" Visible="false" runat="server">
                                            請購科目
                                            <asp:TextBox ID="txtAccno" Width="160px" CssClass="form-control" runat="server" />
                                            <asp:Button ID="btnSureAccno" Text="確定" CssClass="btn btn-primary" runat="server" />
                                            <asp:Button ID="btnCancelAccno" Text="取消" CssClass="btn btn-primary" runat="server" />
                                        </asp:Panel>
                                        <asp:Panel  id="gbxModAmt" Visible="false" runat="server">                                            
                                            開支金額
                                            <asp:TextBox ID="txtUseAmt" CssClass="form-control" runat="server" />
                                            <asp:Button ID="btnSure" Text="確定" CssClass="btn btn-primary" runat="server" />
                                            <asp:Button ID="btnCancel" Text="取消" CssClass="btn btn-primary" runat="server" />                                         
                                        </asp:Panel>
                                        <!--#伺服器驗證-->
                                        <AjaxToolkit:ConfirmButtonExtender ID="ConfirmButtonExtender1" runat="server"
                                            TargetControlID="btnEmptyDate2" ConfirmText="此筆要取消主計審核 確定嗎?">
                                        </AjaxToolkit:ConfirmButtonExtender>
                                        <AjaxToolkit:ConfirmButtonExtender ID="ConfirmButtonExtender3" runat="server"
                                            TargetControlID="btnEmptyDate3" ConfirmText="此筆要取消單位開支 確定嗎?">
                                        </AjaxToolkit:ConfirmButtonExtender> 
                                        <AjaxToolkit:ConfirmButtonExtender ID="ConfirmButtonExtender2" runat="server"
                                            TargetControlID="btnEmptyDate4" ConfirmText="此筆要取消主計開支 確定嗎?">
                                        </AjaxToolkit:ConfirmButtonExtender>                                            
                                        <AjaxToolkit:ConfirmButtonExtender ID="ConfirmButtonExtender4" runat="server"
                                            TargetControlID="btnDelete" ConfirmText="此筆要刪除 確定嗎?">
                                        </AjaxToolkit:ConfirmButtonExtender>
                                    </ContentTemplate>
                                </AjaxToolkit:TabPanel>

                                <AjaxToolkit:TabPanel ID="TabPanel4" runat="server">
                                    <HeaderTemplate>請購編號查詢</HeaderTemplate>
                                    <ContentTemplate>
                                        <table id="table-data" rules="all">
                                            <tr>
                                                <th>請購編號：</th>
                                                <td colspan="2"><asp:TextBox ID="txtQbgno" CssClass="form-control" runat="server" /></td>
                                                <td><asp:Button ID="btnQbgno" Text="確定" CssClass="btn btn-primary" runat="server" /><asp:Label ID="lblQautono" Visible="false" runat="server" /><asp:Label ID="lblQkey" Visible="false" runat="server" /></td>
                                            </tr>
                                            <tr>                                                
                                                <th>推算者：</th>
                                                <td colspan="3"><asp:Label ID="lblQUserId" ForeColor="Blue" Font-Size="12pt" Font-Bold="True" runat="server" /></td>
                                            </tr>
                                            <tr>
                                                <th>年度：</th>
                                                <td><asp:Label ID="lblQyear" runat="server" /></td> 
                                                <th>請購科目：</th>
                                                <td>
                                                    <asp:Label ID="lblQAccno" ForeColor="Blue" Font-Size="12pt" Font-Bold="True" runat="server" />
                                                    <asp:Label ID="lblQAccname" ForeColor="Blue" Font-Size="12pt" Font-Bold="True"  runat="server" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <th>請購金額：</th>
                                                <td><asp:Label ID="lblQAmt1" runat="server" /></td>
                                                <th>單位請購日期：</th>
                                                <td><asp:Label ID="lblQDate1" runat="server" /></td>
                                            </tr>
                                            <tr>
                                                <th>發包金額：</th>
                                                <td><asp:Label ID="lblQAmt2" runat="server" /></td>
                                                <th>主計審核日期：</th>
                                                <td><asp:Label ID="lblQDate2" runat="server" /></td>
                                            </tr>
                                            <tr>
                                                <th>變更金額：</th>
                                                <td><asp:Label ID="lblQAmt3" runat="server" /></td>
                                                <th>單位開支日期：</th>
                                                <td><asp:Label ID="lblQDate3" runat="server" /></td>
                                            </tr>
                                            <tr>
                                                <th>開支金額：</th>
                                                <td><asp:Label ID="lblQUseAmt" runat="server" /></td>
                                                <th>主計開支日期：</th>
                                                <td><asp:Label ID="lblQDate4" runat="server" /></td>
                                            </tr>
                                            <tr>
                                                <th>開支事由：</th>
                                                <td colspan="3">
                                                    <asp:TextBox ID="lblQRemark" CssClass="form-control" runat="server" />
                                                    <asp:Label ID="Label18" ForeColor="Blue" Visible="False" Font-Size="12pt" Font-Bold="True"  runat="server" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="center" colspan="4">                                                    
                                                    <asp:Button ID="btnQEmptyDate" Text="快速取消開支" CssClass="btn btn-primary" runat="server" />
                                                    <asp:Button ID="btnGoQuery" Text="回上頁" CssClass="btn btn-danger" runat="server" />
                                                </td>
                                            </tr>
                                        </table>
                                    </ContentTemplate>
                                </AjaxToolkit:TabPanel>

                                <AjaxToolkit:TabPanel ID="TabPanel5" runat="server">
                                    <HeaderTemplate>條件查詢</HeaderTemplate>
                                    <ContentTemplate>
                                        <div class="table-responsive">
                                            <div class="col-lg-10"> 
                                                年度<asp:TextBox ID="nudQyear" TextMode="Number" runat="server" min="0" max="999" step="1"/>
                                                開支事由<asp:TextBox ID="txtQremark" runat="server" />
                                                金額<asp:TextBox ID="txtQamt" runat="server"/>
                                                <asp:Button ID="btnQsearch" Text="搜尋" CssClass="btn btn-primary" runat="server" />
                                            </div>
                                            <div style="float:right; padding-right:10px;">
                                                共<asp:Label ID="Label9" ForeColor="Red" Font-Size="14pt" Font-Bold="True" Text="0" runat="server" />筆符合&nbsp;                                                
                                                <asp:Label ID="Label10" runat="server" />
                                            </div>                                            
                                            <asp:DataGrid ID="DataGridQuery" Width="100%" AllowSorting="False" AllowPaging="False" style="font-size:14px;" CssClass="table table-bordered table-condensed smart-form" runat="server" >
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
                                                                                                                                 
                                                    <asp:TemplateColumn HeaderText="請購編號" HeaderStyle-Width="80" ItemStyle-HorizontalAlign="Center">
                                                        <itemtemplate><asp:Label ID="請購編號" Text='<%# Container.DataItem("BGNO").ToString%>' runat="server" /></itemtemplate>                                                        
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
                                                        <itemtemplate><asp:Label ID="請購金額" Text='<%# FormatNumber(Container.DataItem("AMT1").ToString, 2)%>' runat="server" /></itemtemplate>
                                                    </asp:TemplateColumn>   
                                                    <asp:TemplateColumn HeaderText="開支金額" HeaderStyle-Width="100" ItemStyle-HorizontalAlign="Right">
                                                        <itemtemplate><asp:Label ID="開支金額" Text='<%# FormatNumber(Container.DataItem("USEAMT").ToString, 2)%>' runat="server" /></itemtemplate>                                                        
                                                    </asp:TemplateColumn>                                                                                                                                                                                                                                                                        
                                                </columns>
                                            </asp:DataGrid>
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
