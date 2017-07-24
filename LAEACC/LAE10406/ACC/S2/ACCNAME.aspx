<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="ACCNAME.aspx.vb" Inherits="LAEACC.ACCNAME" %>
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
                                              <td>科目：</td>
                                              <td>
                                                  <asp:TextBox ID="vxtStartNo" runat="server" />～                                                  
                                                  <asp:TextBox ID="vxtEndNo" runat="server" />                                                  
                                              </td>
                                          </tr>
                                          <tr>
                                              <td colspan="2" align="center">
                                                  <asp:Button ID="BtnSearch" Text="查詢" CssClass="btn btn-primary" runat="server" />
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
							    </fieldset> 
                            </div>
                                                                                                                                               
                            <!--控制項-->
                            <div style="margin:5px 0px 5px 0px;">
                                <uc1:UCBase ID="UCBase1" runat="server" />
                            </div>
                            <div style="clear:both; height:5px;"></div>

                            <!--詳細內容顯示區-->                           
                            <AjaxToolkit:TabContainer ID="TabContainer1" Width="100%" CssClass="Tab" runat="server" ActiveTabIndex="6">
                                <AjaxToolkit:TabPanel ID="TabPanel1" runat="server">
                                    <HeaderTemplate>多筆瀏灠</HeaderTemplate>
                                    <ContentTemplate>
                                        <div style="font-size:14px; height:450px; OVERFLOW:auto;">
                                            <div style="float:left; padding-right:10px;">
                                                共<asp:Label ID="lbl_GrdCount" ForeColor="Red" Font-Size="14" Font-Bold="true" Text="0" runat="server" />筆符合&nbsp;                                                
                                                <asp:Label ID="lbl_sort" runat="server" />
                                            </div>
                                            <asp:DataGrid ID="DataGridView" Width = "2048px" AllowSorting="false" AllowPaging="true" style="font-size:14px;" CssClass="table table-bordered table-condensed smart-form" runat="server" >
                                                <columns>
                                                    <asp:TemplateColumn HeaderText="管理" HeaderStyle-Width="40" ItemStyle-HorizontalAlign="Center">
                                                        <itemtemplate>                                                                                                            
                                                            <asp:ImageButton ID="Show" AlternateText="查閱" ImageUrl="~/active/images/icon/items/zoom.png" CommandName="btnShow" runat="server" />
                                                            <asp:Label ID="id" Text='<%# Container.DataItem("accno").ToString%>' Visible="false" runat="server" />
                                                            <asp:UpdatePanel ID="DataGridView_UpdatePanel" runat="server">                                                                
                                                                <Triggers><asp:AsyncPostBackTrigger ControlID="Show" EventName="Click" /></Triggers>
                                                            </asp:UpdatePanel>
                                                        </itemtemplate>                                                         
                                                    </asp:TemplateColumn>                                                                                
                                                    <asp:TemplateColumn HeaderText="會計科目" HeaderStyle-Width="100" >
                                                        <itemtemplate><asp:Label ID="會計科目" Text='<%# Container.DataItem("accno").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>                                                   
                                                    <asp:TemplateColumn HeaderText="科目名稱" HeaderStyle-Width="160"  >
                                                        <itemtemplate><asp:Label ID="科目名稱" Text='<%# Container.DataItem("accname").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="歸屬系統" HeaderStyle-Width="80" ItemStyle-HorizontalAlign="Center" >
                                                        <itemtemplate><asp:Label ID="歸屬系統" Text='<%# Container.DataItem("belong").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="往來銀行" HeaderStyle-Width="80" ItemStyle-HorizontalAlign="Center" >
                                                        <itemtemplate><asp:Label ID="往來銀行" Text='<%# Container.DataItem("bank").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="預算控制者" HeaderStyle-Width="80" ItemStyle-HorizontalAlign="Center" >
                                                        <itemtemplate><asp:Label ID="預算控制者" Text='<%# Container.DataItem("staff_no").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="主計審核者" HeaderStyle-Width="80" ItemStyle-HorizontalAlign="Center" >
                                                        <itemtemplate><asp:Label ID="主計審核者" Text='<%# Container.DataItem("account_no").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="計帳科目" HeaderStyle-Width="100" >
                                                        <itemtemplate><asp:Label ID="計帳科目" Text='<%# Container.DataItem("bookaccno").ToString%>' runat="server" /></itemtemplate>                                                       
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
                                                    <asp:Label ID="lblAccno" ForeColor="blue" Font-Size="12pt" Font-Bold="True"  runat="server" />
                                                    <asp:Label ID="lblkey" ForeColor="Blue" Font-Size="12pt" Font-Bold="True" Visible="False" runat="server" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <th>科目代號：</th>
                                                <td><asp:TextBox ID="vxtAccno" CssClass="form-control" runat="server" /></td>
                                            </tr>
                                            <tr>
                                                <th>科目名稱：</th>
                                                <td><asp:TextBox ID="txtAccname" CssClass="form-control" runat="server" /></td>
                                            </tr>
                                            <tr>
                                                <th>歸屬單位：</th>
                                                <td>
                                                    <asp:TextBox ID="txtBelong" CssClass="form-control" Width="40%" runat="server" />
                                                    <asp:Label ID="lblUseAmt" ForeColor="Blue" Font-Size="12pt" Font-Bold="True" runat="server" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <th>往來銀行：</th>
                                                <td>
                                                    <asp:TextBox ID="txtBank" CssClass="form-control" Width="40%" runat="server" />
                                                    <asp:Label ID="Label2" ForeColor="Blue" Font-Size="12pt" Font-Bold="True" runat="server" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <th>預算單位：</th>
                                                <td>
                                                    <asp:TextBox ID="txtUnit" CssClass="form-control" Width="25%" runat="server" />
                                                    <asp:Label ID="Label3" ForeColor="Blue" Font-Size="12pt" Font-Bold="True" runat="server" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <th>單位預算控制者：</th>
                                                <td>
                                                    <asp:TextBox ID="txtStaff_no" CssClass="form-control" Width="25%" runat="server" />
                                                    <asp:Label ID="Label4" ForeColor="Blue" Font-Size="12pt" Font-Bold="True" runat="server" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <th>主計預算審核者：</th>
                                                <td>
                                                    <asp:TextBox ID="txtAccount_no" CssClass="form-control" Width="25%" runat="server" />
                                                    <asp:Label ID="Label5" ForeColor="Blue" Font-Size="12pt" Font-Bold="True" runat="server" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <th>記帳科目：</th>
                                                <td><asp:TextBox ID="vxtBook" CssClass="form-control" runat="server" /></td>
                                            </tr>                                            
                                        </table>
                                        <div style="display:none;">
                                            管理處
                                            <asp:TextBox ID="txtArea" CssClass="form-control" runat="server" />
                                            <asp:Label ID="Label7" ForeColor="Blue" Font-Size="12pt" Font-Bold="True" runat="server" />
                                        </div>
                                    </ContentTemplate>
                                </AjaxToolkit:TabPanel>

                                <AjaxToolkit:TabPanel ID="TabPanel3" runat="server">
                                    <HeaderTemplate>條件設定</HeaderTemplate>
                                    <ContentTemplate>
                                        <table id="table-data" rules="all">
                                            <tr>
                                                <th>科目名稱：</th>
                                                <td><asp:TextBox ID="txtQaccname" CssClass="form-control" runat="server" /></td>
                                            </tr>
                                            <tr>
                                                <th>預算單位：</th>
                                                <td>
                                                    <asp:TextBox ID="txtQunit" CssClass="form-control" Width="40%" runat="server" />
                                                    <asp:Label ID="Label6" ForeColor="Blue" Font-Size="12pt" Font-Bold="True" runat="server" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <th>單位預算控制者：</th>
                                                <td>
                                                    <asp:TextBox ID="txtQstaff_no" CssClass="form-control" Width="25%" runat="server" />
                                                    <asp:Label ID="Label10" ForeColor="Blue" Font-Size="12pt" Font-Bold="True" runat="server" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <th>主計預算審核者：</th>
                                                <td>
                                                    <asp:TextBox ID="txtQaccount_no" CssClass="form-control" Width="25%" runat="server" />
                                                    <asp:Label ID="Label11" ForeColor="Blue" Font-Size="12pt" Font-Bold="True" runat="server" />
                                                </td>
                                            </tr>
                                            <tr>                                                
                                                <td colspan="2"><asp:CheckBox ID="ChkBelong" Text="要過濾掉預算專屬科目(B)" runat="server" /></td>
                                            </tr>
                                        </table>
                                        <div style="display:none;">
                                            管理處
                                            <asp:TextBox ID="txtQArea" CssClass="form-control" runat="server" />
                                            <asp:Label ID="Label12" ForeColor="Blue" Font-Size="12pt" Font-Bold="True" runat="server" />
                                        </div>
                                    </ContentTemplate>
                                </AjaxToolkit:TabPanel>

                                <AjaxToolkit:TabPanel ID="TabPanel4" runat="server">
                                    <HeaderTemplate>由工程名稱轉入會計科目</HeaderTemplate>
                                    <ContentTemplate>
                                        <table id="table-data" rules="all">
                                            <tr>   
                                                <th>工程代號：</th>                                             
                                                <td colspan="3">
                                                    <asp:TextBox ID="txtSEngno" CssClass="form-control td-left" Width="25%" runat="server" />
                                                    <div class="td-left">～</div>
                                                    <asp:TextBox ID="txtEEngno" CssClass="form-control td-right" Width="25%" runat="server" />
                                                </td>
                                            </tr>
                                            <tr>   
                                                <th>單位預算控制者：</th>                                             
                                                <td><asp:TextBox ID="txtStaffno" CssClass="form-control" Width="25%" runat="server" /></td>
                                                <th>主計預算審核者：</th>                                             
                                                <td><asp:TextBox ID="txtAccountno" CssClass="form-control" Width="25%" runat="server" /></td>
                                            </tr>
                                            <tr>   
                                                <th>會計科目前9碼：</th>                                             
                                                <td colspan="3"><asp:TextBox ID="txtEaccno" CssClass="form-control" Width="40%" runat="server" /></td>
                                            </tr>                                            
                                            <tr>                                                
                                                <td colspan="4" align="center">
                                                    <asp:Button ID="btnAutoAdd" Text="確定" CssClass="btn btn-primary" runat="server" />
                                                    <asp:Button ID="btnAutoExit" Text="取消" CssClass="btn btn-danger" runat="server" />
                                                </td>
                                            </tr>
                                            <tr>                                                                                           
                                                <td colspan="4" style="color:#f84444;">
                                                    <ul>
                                                        <li>本作業將自動新增會計科目請留意使用</li>
                                                    </ul>
                                                </td>
                                            </tr>
                                        </table>
                                    </ContentTemplate>
                                </AjaxToolkit:TabPanel>

                                <AjaxToolkit:TabPanel ID="TabPanel5" runat="server">
                                    <HeaderTemplate>列印</HeaderTemplate>
                                    <ContentTemplate>
                                        <table id="table-data" rules="all">
                                            <tr>                                                
                                                <td align="center">
                                                    <asp:Button ID="btnPrint" Text="確定" CssClass="btn btn-primary" runat="server" />                                               
                                                </td>
                                            </tr>                                            
                                        </table>
                                    </ContentTemplate>
                                </AjaxToolkit:TabPanel>

                                <AjaxToolkit:TabPanel ID="TabPanel6" runat="server">
                                    <HeaderTemplate>更新記帳科目</HeaderTemplate>
                                    <ContentTemplate>                                        
                                        <table id="table-data" rules="all">
                                            <tr>
                                                <th>會計科目：</th>
                                                <td>
                                                    <asp:DropDownList ID="cboAccno" CssClass="form-control" AutoPostBack="True" runat="server" />
                                                    <asp:Label ID="Label8" ForeColor="Blue" Font-Size="12pt" Font-Bold="True"  runat="server" />

                                                    <asp:Label ID="lblFinish" ForeColor="Blue" Font-Size="12pt" Font-Bold="True" runat="server" />
                                                    <asp:Label ID="lblError" ForeColor="Blue" Font-Size="12pt" Font-Bold="True" runat="server" />
                                                </td>
                                            </tr>
                                            <tr>                                               
                                                <td colspan="2" align="center">
                                                    <asp:Button ID="btnBook" Text="確定" CssClass="btn btn-primary" runat="server" />
                                                </td>
                                            </tr>
                                            <tr>                                                                                           
                                                <td colspan="4" style="color:#f84444;">
                                                    <ul>
                                                        <li>本作業在將每一預算控制科目填上記帳科目</li>
                                                    </ul>
                                                </td>
                                            </tr>
                                        </table>
                                    </ContentTemplate>
                                </AjaxToolkit:TabPanel>
                                
                                <AjaxToolkit:TabPanel ID="TabPanel7" runat="server">
                                    <HeaderTemplate>人員異動</HeaderTemplate>
                                    <ContentTemplate>
                                        <table id="table-data" rules="all">
                                            <tr>
                                                <th>請選擇異動項目：</th>
                                                <td>
                                                    <asp:RadioButton id="rdoStaff" Text="單位預算控制者異動" GroupName="rdbKind" runat="server"/>
                                                    <asp:RadioButton id="rdoAccount" Text="主計預算審核者異動" GroupName="rdbKind" runat="server"/>
                                                </td>
                                            </tr>
                                            <tr>
                                                <th>移交者：</th>
                                                <td>
                                                    <asp:TextBox ID="txtOldNo" CssClass="form-control td-left" Width="25%" runat="server" />
                                                    <asp:Label ID="labInfo1" CssClass="td-right" Font-Size="10" ForeColor="red" Text="請輸入正確員工代碼" runat="server" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <th>接交者：</th>
                                                <td>
                                                    <asp:TextBox ID="txtNewNo" CssClass="form-control td-left" Width="25%" runat="server" />                                                    
                                                    <asp:Label ID="labInfo2" CssClass="td-right" Font-Size="10" ForeColor="red" Text="請輸入正確員工代碼" runat="server" />
                                                </td>
                                            </tr>
                                            <tr>                                               
                                                <td colspan="2" align="center">
                                                    <asp:Button ID="btbChange" Text="確定" CssClass="btn btn-primary" runat="server" />
                                                    <asp:Button ID="btnCancel" Text="取消" CssClass="btn btn-danger" runat="server" />
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
