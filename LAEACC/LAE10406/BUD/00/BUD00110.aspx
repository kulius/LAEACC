<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="BUD00110.aspx.vb" Inherits="LAEACC.BUD00110" %>
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
                                    <h2>
                                        查詢條件值
                                        <span style="margin-left:50px;"><asp:CheckBox ID="btnAdvSerach" Text="進階查詢" AutoPostBack="true" runat="server" /></span>                                        
                                    </h2>
                                </header>
                                <div>
                                    <div class="widget-body form-horizontal">
                                        <fieldset>
                                            <!-- 項目查詢 -->
                                            <legend>
                                                <asp:CheckBox ID="I_S_ACCNO" Text="過濾預算專屬科目" runat="server" />
                                            </legend>

                                            <!-- 常用查詢 -->
								            <div class="form-group">
									            <label class="col-lg-1 control-label">科目編號</label>
									            <div class="col-lg-2">
                                                    <asp:TextBox ID="S_ACCNO_START" CssClass="form-control" Text="1" runat="server" />
                                                    <AjaxToolkit:MaskedEditExtender ID="ACCNO_START_MASK" runat="server"
                                                        TargetControlID="S_ACCNO_START"
                                                        MaskType="None" Mask="?-????-??-??-???????-?"
                                                        InputDirection="LeftToRight" />
									            </div>
									            <div class="col-lg-2">
                                                    <asp:TextBox ID="S_ACCNO_END" CssClass="form-control" Text="9" runat="server" />
                                                    <AjaxToolkit:MaskedEditExtender ID="ACCNO_EN_MASK" runat="server"
                                                        TargetControlID="S_ACCNO_END"
                                                        MaskType="None" Mask="?-????-??-??-???????-?"
                                                        InputDirection="LeftToRight" />
									            </div>

                                                <label class="col-lg-1 control-label">科目名稱</label>
									            <div class="col-lg-3"><asp:TextBox ID="S_ACCNAME" CssClass="form-control" placeholder="科目名稱" runat="server" /></div>
								            </div>

                                            <!-- 進階查詢 -->
                                            <asp:Panel ID="divAdvSerach" Visible="false" runat="server">
                                                <div class="form-group">
									                <label class="col-lg-1 control-label">預算單位</label>                                       
									                <div class="col-lg-2"><asp:DropDownList ID="S_unit_id" CssClass="form-control" AutoPostBack="true" runat="server" /></div>
                                                    <div class="col-lg-2"><asp:DropDownList ID="S_u_user_id" CssClass="form-control" runat="server" /></div>
								                </div>                                             
                                                <div class="form-group">
									                <label class="col-lg-1 control-label">主計審核</label>									            
                                                    <div class="col-lg-2"><asp:DropDownList ID="S_a_user_id" CssClass="form-control" runat="server" /></div>
								                </div>
                                            </asp:Panel>
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
                                            <label class="col-lg-2 control-label">科目編號</label>
                                            <div class="col-lg-2">
                                                <asp:TextBox ID="ACCNO" CssClass="form-control" placeholder="科目編號" runat="server" />
                                                <AjaxToolkit:MaskedEditExtender ID="ACCNO_MASK" runat="server"
                                                    TargetControlID="ACCNO"
                                                    MaskType="None" Mask="?-????-??-??-???????-?"
                                                    InputDirection="LeftToRight" />
                                            </div>
                                            <label class="col-lg-2 control-label">科目名稱</label>
                                            <div class="col-lg-3"><asp:TextBox ID="ACCNAME" CssClass="form-control" placeholder="科目名稱" runat="server" /></div>
                                        </div>
                                    </div>
                                </div>
                            </div>

                            <!--詳細內容顯示區-->                           
                            <AjaxToolkit:TabContainer ID="TabContainer1" Width="100%" CssClass="Tab" runat="server">
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
                                                            <asp:Label ID="id" Text='<%# Container.DataItem("ACCNO").ToString%>' Visible="false" runat="server" />
                                                            <asp:UpdatePanel ID="DataGridView_UpdatePanel" runat="server">                                                                
                                                                <Triggers><asp:AsyncPostBackTrigger ControlID="Show" EventName="Click" /></Triggers>
                                                            </asp:UpdatePanel>
                                                        </itemtemplate>                                                         
                                                    </asp:TemplateColumn>                                                                                
                                                    <asp:TemplateColumn HeaderText="會計科目" HeaderStyle-Width="140" SortExpression="ACCNO">
                                                        <itemtemplate><asp:Label ID="會計科目" Text='<%# Container.DataItem("ACCNO").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="科目名稱" HeaderStyle-Width="180" SortExpression="ACCNAME">
                                                        <itemtemplate><asp:Label ID="科目名稱" Text='<%# Container.DataItem("ACCNAME").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="歸屬系統" HeaderStyle-Width="80" SortExpression="BELONG">
                                                        <itemtemplate><asp:Label ID="歸屬系統" Text='<%# Container.DataItem("BELONG").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="往來銀行" HeaderStyle-Width="80" SortExpression="BANK">
                                                        <itemtemplate><asp:Label ID="往來銀行" Text='<%# Container.DataItem("BANK").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="預算單位" HeaderStyle-Width="80" ItemStyle-HorizontalAlign="Center" SortExpression="UNIT">
                                                        <itemtemplate><asp:Label ID="預算單位" Text='<%# Master.ACC.strUnitName(Container.DataItem("UNIT").ToString)%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="預算控制" HeaderStyle-Width="80" ItemStyle-HorizontalAlign="Center" SortExpression="STAFF_NO">
                                                        <itemtemplate><asp:Label ID="預算控制" Text='<%# Master.ACC.strStaffName(Container.DataItem("STAFF_NO").ToString)%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="主計審核" HeaderStyle-Width="80" ItemStyle-HorizontalAlign="Center" SortExpression="ACCOUNT_NO">
                                                        <itemtemplate><asp:Label ID="主計審核" Text='<%# Master.ACC.strStaffName(Container.DataItem("ACCOUNT_NO").ToString)%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="計帳科目" HeaderStyle-Width="140" SortExpression="BOOKACCNO">
                                                        <itemtemplate><asp:Label ID="計帳科目" Text='<%# Container.DataItem("BOOKACCNO").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>                                                            
                                                </columns>
                                            </asp:DataGrid>
                                        </div>
                                    </ContentTemplate>
                                </AjaxToolkit:TabPanel>

                                <AjaxToolkit:TabPanel ID="TabPanel2" runat="server">
                                    <HeaderTemplate>會計科目明細（D）</HeaderTemplate>
                                    <ContentTemplate>
                                        <div class="widget-body form-horizontal">
                                            <div style="background-color:#71eb8a; font-size:16px; padding-top:20px;">
                                                <fieldset>                                                    
                                                    <div class="form-group">
                                                        <label class="col-lg-2 control-label">歸屬系統</label>
                                                        <div class="col-lg-2"><asp:TextBox ID="BELONG" CssClass="form-control" placeholder="系統代號" runat="server" /></div>
                                                    </div>
                                                    <div class="form-group">
                                                        <label class="col-lg-2 control-label">往來銀行</label>
                                                        <div class="col-lg-2"><asp:TextBox ID="BANK" CssClass="form-control" placeholder="銀行代號" runat="server" /></div>
                                                    </div>
                                                    <div class="form-group">
                                                        <label class="col-lg-2 control-label">預算單位</label>
                                                        <div class="col-lg-2"><asp:DropDownList ID="unit_id" CssClass="form-control" AutoPostBack="true" runat="server" /></div>
                                                        <div class="col-lg-2"><asp:DropDownList ID="u_user_id" CssClass="form-control" runat="server" /></div>
                                                    </div>
                                                    <div class="form-group">
                                                        <label class="col-lg-2 control-label">主計審核</label>
                                                        <div class="col-lg-2"><asp:DropDownList ID="a_user_id" CssClass="form-control" runat="server" /></div>
                                                    </div>
                                                    <div class="form-group">
                                                        <label class="col-lg-2 control-label">記帳科目編號</label>
                                                        <div class="col-lg-2">
                                                            <asp:TextBox ID="BOOKACCNO" CssClass="form-control" placeholder="記帳科目編號" runat="server" />
                                                            <AjaxToolkit:MaskedEditExtender ID="BOOKACCNO_MASK" runat="server"
                                                                TargetControlID="BOOKACCNO"
                                                                MaskType="None" Mask="?-????-??-??-???????-?"
                                                                InputDirection="LeftToRight" />
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
