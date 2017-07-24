<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="ACC01110.aspx.vb" Inherits="LAEACC.ACC01110" %>
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
                            <AjaxToolkit:TabContainer ID="TabContainer1" Width="100%" CssClass="Tab" runat="server" ActiveTabIndex="1">
                                <AjaxToolkit:TabPanel ID="TabPanel1" runat="server">
                                    <HeaderTemplate>查詢結果集（G）</HeaderTemplate>
                                    <ContentTemplate>
                                        <!-- 查詢項目值 -->
                                        <div class="jarviswidget" id="wid-id-0" data-widget-colorbutton="false" data-widget-editbutton="false" data-widget-custombutton="false">                        
                                            <div>
                                                <div class="widget-body form-horizontal">
                                                    <fieldset>
								                        <div class="form-group">
                                                            <label class="col-lg-2 control-label">年度</label>									            
                                                            <div class="col-lg-2"><asp:DropDownList ID="S_ACCYEAR" CssClass="form-control" runat="server" /></div>
                                                            <label class="col-lg-2 control-label">請購編號</label>
									                        <div class="col-lg-3"><asp:TextBox ID="S_BGNO" CssClass="form-control" placeholder="請購編號" runat="server" /></div>
								                        </div>
							                        </fieldset>                                        

                                                    <div class="form-actions" style="margin-top:-5px;">
								                        <div class="row">
									                        <div class="col-md-12">                                            
                                                                <asp:Button ID="btnSave" Text="查詢" CssClass="btn btn-primary" runat="server" />
                                                                <asp:Button ID="btnClear" Text="清除條件" CssClass="btn btn-primary" runat="server" />
									                        </div>
								                        </div>
							                        </div>
                                                </div>                            
                                            </div>                                               
                                        </div>
                                                                    <!--控制項-->
                                        <div style="margin:-20px 0px 10px 0px;">
                                            <!--#功能項目集-->
                                            <div style="float:left;">
                                                <div id="Other" style="float:left;" runat="server">
                                                    <asp:Button ID="btnChk" Text="碓認己選項目" CssClass="btn btn-primary" runat="server" />
                                                </div>
                                            </div>

                                            <!--#伺服器驗證-->
                                            <AjaxToolkit:ConfirmButtonExtender ID="ConfirmButtonExtender1" runat="server"
                                                TargetControlID="btnChk" ConfirmText="您是否確定己選項目??">
                                            </AjaxToolkit:ConfirmButtonExtender>
                                        </div>
                                        <div style="font-size:14px;">
                                            <div style="float:right; padding-right:10px;">
                                                共<asp:Label ID="lbl_GrdCount" ForeColor="Red" Font-Size="14" Font-Bold="true" Text="0" runat="server" />筆符合&nbsp;                                                
                                                <asp:Label ID="lbl_sort" runat="server" />
                                            </div>
                                            <asp:DataGrid ID="DataGridView" Width = "100%" AllowSorting="True" AllowPaging="True" runat="server" >
                                                <columns>
                                                    <asp:TemplateColumn HeaderText="管理" HeaderStyle-Width="40" ItemStyle-HorizontalAlign="Center">
                                                        <itemtemplate>                                                                                                            
                                                          
                                                            <asp:CheckBox ID="chkRow" runat="server" />
                                                            <asp:Label ID="id" Text='<%# Container.DataItem("BGNO").ToString%>' Visible="false" runat="server" />
                                                           
                                                        </itemtemplate>                                                         
                                                    </asp:TemplateColumn>                                                                                
                                                    <asp:TemplateColumn HeaderText="單號" HeaderStyle-Width="80" SortExpression="a.BGNO">
                                                        <itemtemplate><asp:Label ID="單號" Text='<%# Container.DataItem("BGNO").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="日期" HeaderStyle-Width="80" ItemStyle-HorizontalAlign="Center" SortExpression="a.DATE3">
                                                        <itemtemplate><asp:Label ID="日期" Text='<%# Master.Models.strDateADToChiness(Container.DataItem("DATE3").ToShortDateString.ToString)%>' runat="server" /></itemtemplate>
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="金額" HeaderStyle-Width="40" ItemStyle-HorizontalAlign="Center" SortExpression="a.ACCYEAR">
                                                        <itemtemplate><asp:Label ID="金額" Text='<%# Container.DataItem("USEAMT").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="科目" HeaderStyle-Width="100" SortExpression="a.ACCNO">
                                                        <itemtemplate><asp:Label ID="科目" Text='<%# Container.DataItem("ACCNO").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="摘要" HeaderStyle-Width="220" SortExpression="a.REMARK">
                                                        <itemtemplate><asp:Label ID="摘要" Text='<%# Container.DataItem("REMARK").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>                                                         
                                                </columns>
                                            </asp:DataGrid>
                                        </div>
                                    </ContentTemplate>
                                </AjaxToolkit:TabPanel>

                                <AjaxToolkit:TabPanel ID="TabPanel2" runat="server">
                                    <HeaderTemplate>傳票資料輸入（B）</HeaderTemplate>
                                    <ContentTemplate>
                                        <div class="form-group">
                                            <label class="col-lg-2 control-label">製票日期</label>
                                            <div class="col-lg-2"><asp:TextBox ID="DATE_1" CssClass="form-control" runat="server" /></div>
                                            <label class="col-lg-2 control-label">製票編號</label>
                                            <div class="col-lg-2"><asp:TextBox ID="NO_1_NO" CssClass="form-control" runat="server" /></div>
                                        </div>
                                        <br />
                                        <br />
                                       
                                       <table border="1">
                                           <tr>
                                               <td></td>
                                               <td>會計科目及符號</td>
                                               <td>摘要</td>
                                               <td>金額</td>
                                               <td>代號</td>
                                           </tr>
                                           <tr>
                                               <td>總帳科目</td>
                                               <td>
                                                   <asp:TextBox ID="ACCNO" CssClass="form-control" runat="server" />
                                                    <AjaxToolkit:MaskedEditExtender ID="ACCNO_MASK" runat="server"
                                                        TargetControlID="ACCNO"
                                                        MaskType="None" Mask="?-????"
                                                        InputDirection="LeftToRight" />
                                               </td>
                                               <td><asp:TextBox ID="REMARK" CssClass="form-control" runat="server" /></td>
                                               <td><asp:TextBox ID="AMT" CssClass="form-control" runat="server" /></td>
                                               <td></td>
                                           </tr>
                                           <tr>
                                               <td rowspan="5">明細科目</td>
                                               <td>
                                                   <asp:TextBox ID="ACCNO1" CssClass="form-control"  runat="server" />
                                                    <AjaxToolkit:MaskedEditExtender ID="ACCNO1_MASK" runat="server"
                                                        TargetControlID="ACCNO1"
                                                        MaskType="None" Mask="?-????-??-??-???????-?"
                                                        InputDirection="LeftToRight" />
                                               </td>
                                               <td><asp:TextBox ID="REMARK1" CssClass="form-control" runat="server" /></td>
                                               <td><asp:TextBox ID="AMT1" CssClass="form-control" runat="server" /></td>
                                               <td><asp:TextBox ID="COTN_CODE1" CssClass="form-control" runat="server" /></td>
                                           </tr>
                                           <tr>
                                               
                                               <td>
                                                   <asp:TextBox ID="ACCNO2" CssClass="form-control"  runat="server" />
                                                    <AjaxToolkit:MaskedEditExtender ID="ACCNO2_MASK" runat="server"
                                                        TargetControlID="ACCNO2"
                                                        MaskType="None" Mask="?-????-??-??-???????-?"
                                                        InputDirection="LeftToRight" />
                                               </td>
                                               <td><asp:TextBox ID="REMARK2" CssClass="form-control" runat="server" /></td>
                                               <td><asp:TextBox ID="AMT2" CssClass="form-control" runat="server" /></td>
                                               <td><asp:TextBox ID="COTN_CODE2" CssClass="form-control" runat="server" /></td>
                                           </tr>
                                           <tr>
                                               
                                               <td>
                                                   <asp:TextBox ID="ACCNO3" CssClass="form-control" runat="server" />
                                                    <AjaxToolkit:MaskedEditExtender ID="ACCNO3_MASK" runat="server"
                                                        TargetControlID="ACCNO3"
                                                        MaskType="None" Mask="?-????-??-??-???????-?"
                                                        InputDirection="LeftToRight" />
                                               </td>
                                               <td><asp:TextBox ID="REMARK3" CssClass="form-control" runat="server" /></td>
                                               <td><asp:TextBox ID="AMT3" CssClass="form-control" runat="server" /></td>
                                               <td><asp:TextBox ID="COTN_CODE3" CssClass="form-control" runat="server" /></td>
                                           </tr>
                                           <tr>
                                               
                                               <td>
                                                   <asp:TextBox ID="ACCNO4" CssClass="form-control"  runat="server" />
                                                    <AjaxToolkit:MaskedEditExtender ID="ACCNO4_MASK" runat="server"
                                                        TargetControlID="ACCNO4"
                                                        MaskType="None" Mask="?-????-??-??-???????-?"
                                                        InputDirection="LeftToRight" />
                                               </td>
                                               <td><asp:TextBox ID="REMARK4" CssClass="form-control" runat="server" /></td>
                                               <td><asp:TextBox ID="AMT4" CssClass="form-control" runat="server" /></td>
                                               <td><asp:TextBox ID="COTN_CODE4" CssClass="form-control" runat="server" /></td>
                                           </tr>
                                           <tr>
                                               
                                               <td>
                                                   <asp:TextBox ID="ACCNO5" CssClass="form-control"  runat="server" />
                                                    <AjaxToolkit:MaskedEditExtender ID="ACCNO5_MASK" runat="server"
                                                        TargetControlID="ACCNO5"
                                                        MaskType="None" Mask="?-????-??-??-???????-?"
                                                        InputDirection="LeftToRight" />
                                               </td>
                                               <td><asp:TextBox ID="REMARK5" CssClass="form-control" runat="server" /></td>
                                               <td><asp:TextBox ID="AMT5" CssClass="form-control" runat="server" /></td>
                                               <td><asp:TextBox ID="COTN_CODE5" CssClass="form-control" runat="server" /></td>
                                           </tr>
                                           <tr>
                                               <td>沖收(付)數</td>
                                               <td>實收(付)數</td>
                                               <td>銀行</td>
                                               <td>帳號</td>
                                               <td>支票號碼</td>
                                           </tr><tr>
                                               <td><asp:TextBox ID="TextBox1" CssClass="form-control" runat="server" /></td>
                                               <td><asp:TextBox ID="TextBox2" CssClass="form-control" runat="server" /></td>
                                               <td><asp:TextBox ID="TextBox3" CssClass="form-control" runat="server" /></td>
                                               <td><asp:TextBox ID="TextBox4" CssClass="form-control" runat="server" /></td>
                                               <td><asp:TextBox ID="TextBox5" CssClass="form-control" runat="server" /></td>
                                           </tr>
                                       </table>
                                       
                                           
                                        
                                       
                                            <!--#功能項目集-->
                                            <div style="float:left;">
                                                <div id="Div1" style="float:left;" runat="server">
                                                    <asp:Button ID="BtnSave1" Text="儲存" CssClass="btn btn-primary" runat="server" />
                                                    <asp:Button ID="BtnCancel1" Text="取消" CssClass="btn btn-primary" runat="server" />
                                                </div>
                                            </div>

                                            <!--#伺服器驗證-->
                                            <AjaxToolkit:ConfirmButtonExtender ID="ConfirmButtonExtender2" runat="server"
                                                TargetControlID="BtnSave1" ConfirmText="您是否確定儲存??">
                                            </AjaxToolkit:ConfirmButtonExtender>
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
