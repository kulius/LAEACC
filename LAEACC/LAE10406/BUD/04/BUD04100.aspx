<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="BUD04100.aspx.vb" Inherits="LAEACC.BUD04100" %>
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
                            <!-- 查詢項目值 -->
                            <div class="jarviswidget" id="wid-id-0" data-widget-colorbutton="false" data-widget-editbutton="false" data-widget-custombutton="false">                        
                                <header>
                                    <span class="widget-icon"><i class="fa fa-edit"></i></span>
                                    <h2>查詢條件值</h2>
                                </header>
                                <div>
                                    <div class="widget-body form-horizontal">
                                        <fieldset>
                                            <!-- 項目查詢 -->

                                            <!-- 常用查詢 -->
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
                                <uc1:UCBase ID="UCBase1" runat="server" />
                            </div>

                            <!--主內容顯示區-->
                            <div style="margin-top:65px; background-color:#f8f38f;">
                                <div class="widget-body form-horizontal">
                                    <div style="padding:10px 0px 1px 0px;">
                                        <div class="form-group">
                                            <label class="col-lg-2 control-label"><span style="color:#e96f63;">異動人員</span></label>
                                            <div class="col-lg-2"><asp:TextBox ID="UPDATE_ID" Enabled="false" CssClass="form-control" runat="server" /></div>
                                            <label class="col-lg-2 control-label"><span style="color:#e96f63;">異動日期</span></label>
                                            <div class="col-lg-2"><asp:TextBox ID="UPDATE_DATE" Enabled="false" CssClass="form-control" runat="server" /></div>
                                        </div>
                                        <div class="form-group">
                                            <label class="col-lg-2 control-label">會計年度</label>
                                            <div class="col-lg-2 control-label" style="margin-left:-100px;"><asp:Label ID="ACCYEAR" ForeColor="Blue" Font-Size="12" Font-Bold="true" runat="server" /></div>
                                            <label class="col-lg-2 control-label">單位代碼</label>
                                            <div class="col-lg-2 control-label" style="margin-left:-100px;"><asp:Label ID="UNIT" ForeColor="Blue" Font-Size="12" Font-Bold="true" runat="server" /></div>
                                            <label class="col-lg-2 control-label">單位名稱</label>
                                            <div class="col-lg-2 control-label" style="margin-left:-100px;"><asp:Label ID="UNITNAME" ForeColor="Blue" Font-Size="12" Font-Bold="true" runat="server" /></div>
                                        </div>
                                    </div>
                                </div>
                            </div>

                            <!--詳細內容顯示區-->                           
                            <AjaxToolkit:TabContainer ID="TabContainer1" Width="100%" CssClass="Tab" runat="server" ActiveTabIndex="1">
                                <AjaxToolkit:TabPanel ID="TabPanel1" runat="server">
                                    <HeaderTemplate>查詢結果集（G）</HeaderTemplate>
                                    <ContentTemplate>
                                        <div style="font-size:14px;">
                                            <div style="float:right; padding-right:10px;">
                                                共<asp:Label ID="lbl_GrdCount" ForeColor="Red" Font-Size="14" Font-Bold="true" Text="0" runat="server" />筆符合&nbsp;                                                
                                                <asp:Label ID="lbl_sort" runat="server" />
                                            </div>
                                            <asp:DataGrid ID="DataGridView" Width = "100%" AllowSorting="True" AllowPaging="True" runat="server" >
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
                                                    <asp:TemplateColumn HeaderText="請購編號" HeaderStyle-Width="80" SortExpression="a.BGNO">
                                                        <itemtemplate><asp:Label ID="請購編號" Text='<%# Container.DataItem("BGNO").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="請購日期" HeaderStyle-Width="80" ItemStyle-HorizontalAlign="Center" SortExpression="a.DATE1">
                                                        <itemtemplate><asp:Label ID="請購日期" Text='<%# Master.Models.strDateADToChiness(Container.DataItem("DATE1").ToShortDateString.ToString)%>' runat="server" /></itemtemplate>
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="年" HeaderStyle-Width="40" ItemStyle-HorizontalAlign="Center" SortExpression="a.ACCYEAR">
                                                        <itemtemplate><asp:Label ID="年" Text='<%# Container.DataItem("ACCYEAR").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="科目" HeaderStyle-Width="100" SortExpression="a.ACCNO">
                                                        <itemtemplate><asp:Label ID="科目" Text='<%# Container.DataItem("ACCNO").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="科目名稱" HeaderStyle-Width="180" SortExpression="b.ACCNAME">
                                                        <itemtemplate><asp:Label ID="科目名稱" Text='<%# Container.DataItem("ACCNAME").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="事由" HeaderStyle-Width="220" SortExpression="a.REMARK">
                                                        <itemtemplate><asp:Label ID="事由" Text='<%# Container.DataItem("REMARK").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>                                                         
                                                </columns>
                                            </asp:DataGrid>
                                        </div>
                                    </ContentTemplate>
                                </AjaxToolkit:TabPanel>

                                <AjaxToolkit:TabPanel ID="TabPanel2" runat="server">
                                    <HeaderTemplate>單位請購資料輸入（B）</HeaderTemplate>
                                    <ContentTemplate>
                                        <div class="widget-body form-horizontal">
                                            <div style="background-color:#71eb8a; font-size:16px; padding-top:20px;">
                                                <fieldset>
                                                    <div class="form-group">
                                                        <label class="col-lg-2 control-label">請購日期</label>
                                                        <div class="col-lg-2 control-label" style="margin-left:-50px;"><asp:Label ID="DATE1" ForeColor="Blue" Font-Size="12" Font-Bold="true" runat="server" /></div>
                                                        <label class="col-lg-2 control-label">請購編號</label>
                                                        <div class="col-lg-2 control-label" style="margin-left:-50px;"><asp:Label ID="BGNO" ForeColor="Blue" Font-Size="12" Font-Bold="true" runat="server" /></div>                                                    
                                                    </div>
                                                    <div class="form-group">
                                                        <label class="col-lg-2 control-label">請購科目</label>
                                                        <div class="col-lg-6"><asp:DropDownList ID="ACCNO" CssClass="form-control" AutoPostBack="true" runat="server" /></div>                                                        
                                                    </div>
                                                    <div class="form-group">
                                                        <label class="col-lg-2 control-label">總經費</label>
                                                        <div class="col-lg-2 control-label" style="margin-left:-30px;"><asp:Label ID="BUD1" ForeColor="Red" Font-Size="12" Font-Bold="true" Text="0" runat="server" />元</div>
                                                        <label class="col-lg-2 control-label">季餘額</label>
                                                        <div class="col-lg-2 control-label" style="margin-left:-30px;"><asp:Label ID="BUD2" ForeColor="Red" Font-Size="12" Font-Bold="true" Text="0" runat="server" />元</div>
                                                    </div>
                                                    <div class="form-group">
                                                        <label class="col-lg-2 control-label">己開支數</label>
                                                        <div class="col-lg-2 control-label" style="margin-left:-30px;"><asp:Label ID="BUD3" ForeColor="Red" Font-Size="12" Font-Bold="true" Text="0" runat="server" />元</div>
                                                        <label class="col-lg-2 control-label">年度餘額</label>
                                                        <div class="col-lg-2 control-label" style="margin-left:-30px;"><asp:Label ID="BUD4" ForeColor="Red" Font-Size="12" Font-Bold="true" Text="0" runat="server" />元</div>
                                                    </div>
                                                    <div class="form-group">                                                        
                                                        <label class="col-lg-2 control-label">請購金額</label>
                                                        <div class="col-lg-2"><asp:TextBox ID="USEABLEAMT" CssClass="form-control" placeholder="請購金額" runat="server" /></div>
                                                    </div>
                                                    <div class="form-group">
                                                        <label class="col-lg-2 control-label">請購事由</label>
                                                        <div class="col-lg-8">
                                                            <asp:TextBox ID="REMARK" CssClass="form-control" placeholder="請購事由" runat="server" />
                                                        </div>
                                                    </div>                                                                                                     
                                                </fieldset>
                                            </div>
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
