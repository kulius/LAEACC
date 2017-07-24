<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="PAY030.aspx.vb" Inherits="LAEACC.PAY030" %>
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
                                                  
                                       
                            <!--詳細內容顯示區-->                           
                            <AjaxToolkit:TabContainer ID="TabContainer1" Width="100%" CssClass="Tab" runat="server" >
                                <AjaxToolkit:TabPanel ID="TabPanel1" runat="server">
                                    <HeaderTemplate>收入傳票記錄</HeaderTemplate>
                                    <ContentTemplate>
                                        <div class="widget-body form-horizontal">
                                            <div style="background-color:#71eb8a; font-size:16px; padding-top:20px;">
                                                <fieldset>
                                                    <div class="form-group">
                                                        <div class="col-lg-2 control-label" ><asp:Label ID="lblDate_2" ForeColor="Blue" Font-Size="12pt" Font-Bold="True"  runat="server" /></div>  
                                                        <label class="col-lg-4 control-label"><asp:Label ID="lblFile" Text="請輸入製票編號:" ForeColor="Blue" Font-Bold="True"  runat="server" /><asp:TextBox ID="txtNo1" runat="server" /></label>  
                                                        <div class="col-lg-1 control-label"><asp:Button ID="btnSureNo" Text="確定" CssClass="btn btn-primary" runat="server" /></div>
                                                        <div class="col-lg-1 control-label"><asp:Button ID="btnFinish" Text="記入帳冊" CssClass="btn btn-primary" runat="server" /></div>
                                                        <div class="col-lg-2 control-label" ><asp:Label ID="lblMsg" ForeColor="Blue" Font-Size="12pt" Font-Bold="True"  runat="server" /></div>  
                                                    </div>
                                            
                                                </fieldset>
                                            </div>
                                        </div>

                                                                                       
                                            <asp:DataGrid ID="DataGridView" Width = "100%" runat="server" >
                                                <columns>
                                                    <asp:TemplateColumn HeaderText="管理">
                                                        <itemtemplate>        
                                                            <asp:ImageButton ID="Delete" AlternateText="刪除" ImageUrl="~/active/images/icon/items/delete.png" CommandName="btnDelete" runat="server" />
                                                            <asp:Label ID="id" Text='<%# Container.DataItem("no_1_no").ToString%>' Visible="false" runat="server" />
                                                            <asp:UpdatePanel ID="DataGridView_UpdatePanel" runat="server">                                                                
                                                                <Triggers><asp:AsyncPostBackTrigger ControlID="Delete" EventName="Click" /></Triggers>
                                                            </asp:UpdatePanel>
                                                        </itemtemplate>                                                         

                                                        <HeaderStyle Width="40px" />
                                                        <ItemStyle HorizontalAlign="Center" />

                                                    </asp:TemplateColumn>   
                                                    <asp:TemplateColumn HeaderText="傳票號">
                                                        <itemtemplate><asp:Label ID="傳票號" Text='<%# Container.DataItem("no_1_no").ToString%>' runat="server" /></itemtemplate>                                                       
                                                        <HeaderStyle Width="60px" />
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="摘要" >
                                                        <itemtemplate><asp:Label ID="摘要" Text='<%# Container.DataItem("REMARK").ToString%>' runat="server" /></itemtemplate>                                                       
                                                        <HeaderStyle Width="120px" />
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="實付數">
                                                        <itemtemplate><asp:Label ID="實付數" Text='<%# FormatNumber(Container.DataItem("act_amt").ToString, 0)%>' runat="server" /></itemtemplate>                                                       
                                                        <HeaderStyle Width="120px" />
                                                        <ItemStyle HorizontalAlign="Right" />
                                                    </asp:TemplateColumn>
                                                    
                                                    <asp:TemplateColumn HeaderText="銀行" >
                                                        <itemtemplate><asp:Label ID="銀行" Text='<%# Container.DataItem("bank").ToString%>' runat="server" /></itemtemplate>                                                       
                                                        <HeaderStyle Width="50px" />
                                                    </asp:TemplateColumn>  
                                                    <asp:TemplateColumn HeaderText="支款編號">
                                                        <itemtemplate><asp:Label ID="支款編號" Text='<%# Container.DataItem("no_2_no").ToString%>' runat="server" /></itemtemplate>                                                       
                                                        <HeaderStyle Width="60px" />
                                                    </asp:TemplateColumn>                         
                                                </columns>
                                            </asp:DataGrid>
                                            <div class="widget-body form-horizontal">
                                                <div style="background-color:#71eb8a; font-size:16px; padding-top:20px;">

                                                    <fieldset>
                                                    <div class="form-group">
                                                        <div class="col-lg-2 control-label" ><asp:Label ID="lblBankName" ForeColor="Blue" Font-Size="12pt" Font-Bold="True"  runat="server" />	</div>
                                                        <div class="col-lg-2 control-label" >總額<asp:Label ID="lblTotAmt" ForeColor="Blue" Font-Size="12pt" Font-Bold="True"  runat="server" />	</div>
                                                    </div>
                                                    <div class="form-group">
                                                        <label class="col-lg-2 control-label">支票號碼：</label>
                                                        <label class="col-lg-2 control-label"><asp:TextBox ID="txtChkNo" runat="server" /></label> 
                                                        <div class="col-lg-2 control-label" ><asp:Label ID="lblChkTR" ForeColor="Blue" Font-Size="12pt" Font-Bold="True"  runat="server" />	</div>
                                                    </div>
                                                    <div class="form-group">
                                                        <label class="col-lg-2 control-label">受款人：</label>
                                                        <label class="col-lg-5 control-label"><asp:TextBox ID="txtChkname" Width="520px" runat="server" /></label>
                                                        <AjaxToolkit:AutoCompleteExtender 
                                                            ID="AutoCompleteExtender1"                             
                                                            runat="server"                
                                                            TargetControlID="txtChkname"
                                                            ServicePath="~/active/WebService.asmx"
                                                            ServiceMethod="GetPayRemark"
                                                            MinimumPrefixLength="0" 
                                                            CompletionInterval="100"
                                                            CompletionSetCount="12" DelimiterCharacters="" Enabled="True" />
                                                    </div>
                                                    <div class="form-group">
                                                        <label class="col-lg-2 control-label">摘要：</label>
                                                        <label class="col-lg-5 control-label"><asp:TextBox ID="txtRemark" Width="520px" runat="server" /></label> 
                                                    </div>
                                                      <div class="form-group">
                                                        <div class="col-lg-2 control-label" ><asp:Label ID="lblNo1" ForeColor="Blue" Font-Size="12pt" Font-Bold="True"  runat="server" />	</div>
                                                        <div class="col-lg-2 control-label" ><asp:Label ID="lblRemark" ForeColor="Blue" Font-Size="12pt" Font-Bold="True"  runat="server" /></div>
                                                        <div class="col-lg-2 control-label" ><asp:Label ID="lblAct_amt" ForeColor="Blue" Font-Size="12pt" Font-Bold="True"  runat="server" /></div>
                                                        <div class="col-lg-2 control-label" ><asp:Label ID="lblBank" ForeColor="Blue" Font-Size="12pt" Font-Bold="True"  runat="server" /></div>
                                                    </div>  
                                                    </fieldset>
                                                </div>
                                            </div>
                                        </div>
                                    </ContentTemplate>
                                </AjaxToolkit:TabPanel>                               
                                <AjaxToolkit:TabPanel ID="TabPanel2" runat="server">
                                    <HeaderTemplate>收款日期</HeaderTemplate>
                                    <ContentTemplate>
                                        <div class="widget-body form-horizontal">
                                            <div style="background-color:#71eb8a; font-size:16px; padding-top:20px;">
                                                <fieldset>
                                                    <div class="form-group">
                                                        <label class="col-lg-2 control-label">收款日期：</label>
                                                        <div class="col-lg-2 control-label" ><asp:TextBox ID="dtpDate_2"  onClick="WdatePicker({dateFmt:'yyy-MM-dd',skin:'whyGreen'})" runat="server" />	</div>
                                                    </div>
                                                    
                                                    <div class="form-group">
                                                        <div class="col-lg-2 control-label"><asp:Button ID="btnDate_2" Text="確定" CssClass="btn btn-primary" runat="server" /></div>
                                                    </div>

                                                    <div class="form-group">
                                                        <div class="col-lg-4 control-label" ><asp:Label ID="lblMsgDate" ForeColor="Blue" Font-Size="18pt" Font-Bold="True"  runat="server" /></div>  
                                                
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
