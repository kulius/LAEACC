<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="AC040.aspx.vb" Inherits="LAEACC.AC040" %>
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
                            <div class="widget-body form-horizontal" style="font-size:16px; background-color:#71eb8a;margin:10px 0px 10px 0px;" >
                                  <fieldset>
                                   <div class="form-group">
                                        <div class="col-lg-2 control-label">年度：<asp:Label ID="lblYear" Text="" Font-Size="12pt" Font-Bold="True"  runat="server" /></div>
                                        <div class="col-lg-2 control-label">上張轉帳編號：<asp:Label ID="lblUseNO" Text="" Font-Size="12pt" Font-Bold="True"  runat="server" /></div>
                                        
                                   </div>
								    
							    </fieldset> 
                            </div>                          
                                       
                            <!--詳細內容顯示區-->                           
                            <AjaxToolkit:TabContainer ID="TabContainer1" Width="100%" CssClass="Tab" runat="server" ActiveTabIndex="1">
                                <AjaxToolkit:TabPanel ID="TabPanel1" runat="server">
                                    <HeaderTemplate>資料來源</HeaderTemplate>
                                    <ContentTemplate>
                                        <div style="font-size:14px;">
                                            <asp:DataGrid ID="dtgSource" Width = "100%" runat="server" >
                                                <columns>
                                                    <asp:TemplateColumn HeaderText="管理">
                                                        <itemtemplate>                                                                                                            
                                                            <asp:ImageButton ID="Show" AlternateText="決裁" ImageUrl="~/active/images/icon/items/edit.png" CommandName="btnShow" runat="server" />
                                                            <asp:Label ID="id" Text='<%# Container.DataItem("no_1_no").ToString%>' Visible="false" runat="server" />
                                                            <asp:UpdatePanel ID="DataGridView_UpdatePanel" runat="server">                                                                
                                                                <Triggers><asp:AsyncPostBackTrigger ControlID="Show" EventName="Click" /></Triggers>
                                                            </asp:UpdatePanel>
                                                        </itemtemplate>                                                         
                                                        <HeaderStyle Width="40px" />
                                                        <ItemStyle HorizontalAlign="Center" />
                                                    </asp:TemplateColumn>                                                                                
                                                                                                                                 
                                                    <asp:TemplateColumn HeaderText="製票編號">
                                                        <itemtemplate><asp:Label ID="製票編號" Text='<%# Container.DataItem("no_1_no").ToString%>' runat="server" /></itemtemplate>                                                       
                                                        <HeaderStyle Width="80px" />
                                                    </asp:TemplateColumn>
                                                   
                                                    <asp:TemplateColumn HeaderText="會計科目"  >
                                                        <itemtemplate><asp:Label ID="會計科目" Text='<%# Container.DataItem("accno").ToString%>' runat="server" /></itemtemplate>                                                       
                                                        <HeaderStyle Width="140px" />
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="摘要" >
                                                        <itemtemplate><asp:Label ID="摘要" Text='<%# Container.DataItem("remark").ToString%>' runat="server" /></itemtemplate>                                                       
                                                        <HeaderStyle Width="300px" />
                                                    </asp:TemplateColumn>
                                                    
                                                    <asp:TemplateColumn HeaderText="金額">
                                                        <itemtemplate><asp:Label ID="金額" Text='<%# Container.DataItem("amt").ToString%>' runat="server" /></itemtemplate>                                                       
                                                        <HeaderStyle Width="120px" />
                                                        <ItemStyle HorizontalAlign="Right" />
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="" >
                                                        <itemtemplate><asp:Label ID="自動編號" Text='<%# Container.DataItem("autono").ToString%>' runat="server" /></itemtemplate>                                                       
                                                        <HeaderStyle Width="75px" />
                                                    </asp:TemplateColumn>
                         
                                                </columns>
                                            </asp:DataGrid>

                                            

                                        </div>
                                    </ContentTemplate>
                                </AjaxToolkit:TabPanel>

                                <AjaxToolkit:TabPanel ID="TabPanel2" runat="server">
                                    <HeaderTemplate>傳票決裁</HeaderTemplate>
                                    <ContentTemplate>
                                        <div class="widget-body form-horizontal">
                                            <div style="background-color:#71eb8a; font-size:16px; padding-top:20px;">
                                                <fieldset>
                                                    
                                                    <div class="form-group">
                                                        <label class="col-lg-2 control-label">製票日期：<asp:Label ID="lblDate_1" ForeColor="Blue" Font-Size="12pt" Font-Bold="True"  runat="server" /></label>
                                                        <label class="col-lg-2 control-label">決裁日期</label>
                                                        <label class="col-lg-1 control-label"><asp:TextBox ID="dtpDate" Width="100px" onClick="WdatePicker({dateFmt:'yyy-MM-dd',skin:'whyGreen'})" runat="server" /></label>
                                                        <label class="col-lg-1 control-label"><asp:Button ID="btnback" Text="回上頁" CssClass="btn btn-primary" runat="server" /></label>
                                                    </div>
                                                    <div class="form-group">
                                                        <label class="col-lg-2 control-label">製票編號：<asp:Label ID="lblNo_1_no" ForeColor="Blue" Font-Size="12pt" Font-Bold="True"  runat="server" /></label>
                                                        <label class="col-lg-2 control-label">轉帳編號：</label>
                                                        <label class="col-lg-1 control-label"><asp:Label ID="lblNo_2_no" ForeColor="Blue" Font-Size="12pt" Font-Bold="True"  runat="server" /></label>
                                                        <label class="col-lg-1 control-label"><asp:Button ID="btnFinish" Text="確定" CssClass="btn btn-primary" runat="server" /></label>
                                                        
                                                    </div>
                                                    
                                                    <div class="form-group">
                                                        <label class="col-lg-2 control-label" style="text-decoration:underline;Text-align: center">會計科目</label> 
                                                        <label class="col-lg-4 control-label" style="text-decoration:underline;Text-align: center">摘要</label> 
                                                        <label class="col-lg-1 control-label" style="text-decoration:underline;Text-align: center">金額</label> 
                                                        <label class="col-lg-1 control-label" style="text-decoration:underline;Text-align: center">內容別</label>
                                                        <label class="col-lg-2 control-label" style="text-decoration:underline;Text-align: center">其他</label>
                                                    </div>
                                                    <div class="form-group">
                                                        <label class="col-lg-2 control-label" style="Text-align: left"><asp:TextBox ID="vxtAccno1" Width="100px" runat="server" /><br /><asp:Label ID="lblAccName1" ForeColor="Blue" Font-Size="8pt" Font-Bold="True"  runat="server" /></label> 
                                                        <label class="col-lg-4 control-label"><asp:TextBox ID="txtRemark1" Width="380px"   runat="server" /></label> 
                                                        <label class="col-lg-1 control-label"><asp:TextBox ID="txtAmt1"  Width="100px" style="text-align:right" runat="server" /></label> 
                                                        <label class="col-lg-1 control-label"></label>
                                                        <label class="col-lg-2 control-label"></label>
                                                    </div>
                                                    <div class="form-group">
                                                        <label class="col-lg-2 control-label" style="Text-align: left"><asp:TextBox ID="vxtAccno2" Width="190px"  AutoPostBack="True" runat="server" /><asp:Label ID="lblAccName2" ForeColor="Blue" Font-Size="8pt" Font-Bold="True"  runat="server" /></label> 
                                                        <label class="col-lg-4 control-label"><asp:TextBox ID="txtRemark2" Width="380px"  runat="server" /></label> 
                                                        <label class="col-lg-1 control-label"><asp:TextBox ID="txtAmt2" Width="100px" style="text-align:right" runat="server" /></label> 
                                                        <label class="col-lg-1 control-label"><asp:TextBox ID="txtCode2" Width="40px" runat="server" /></label>
                                                        <label class="col-lg-2 control-label"><asp:TextBox ID="vxtOther2" runat="server" /><asp:Label ID="lblOtherName2" ForeColor="Blue" Font-Size="8pt" Font-Bold="True"  runat="server" /></label>
                                                    </div>
                                                    <div class="form-group">
                                                        <label class="col-lg-2 control-label" style="Text-align: left"><asp:TextBox ID="vxtAccno3" Width="190px"  AutoPostBack="True" runat="server" /><asp:Label ID="lblAccName3" ForeColor="Blue" Font-Size="8pt" Font-Bold="True"  runat="server" /></label> 
                                                        <label class="col-lg-4 control-label"><asp:TextBox ID="txtRemark3" Width="380px"  runat="server" /></label> 
                                                        <label class="col-lg-1 control-label"><asp:TextBox ID="txtAmt3" Width="100px" style="text-align:right" runat="server" /></label> 
                                                        <label class="col-lg-1 control-label"><asp:TextBox ID="txtCode3" Width="40px" runat="server" /></label>
                                                        <label class="col-lg-2 control-label"><asp:TextBox ID="vxtOther3" runat="server" /><asp:Label ID="lblOtherName3" ForeColor="Blue" Font-Size="8pt" Font-Bold="True"  runat="server" /></label>
                                                    </div>
                                                    <div class="form-group">
                                                        <label class="col-lg-2 control-label" style="Text-align: left"><asp:TextBox ID="vxtAccno4" Width="190px"  AutoPostBack="True" runat="server" /><asp:Label ID="lblAccName4" ForeColor="Blue" Font-Size="8pt" Font-Bold="True"  runat="server" /></label> 
                                                        <label class="col-lg-4 control-label"><asp:TextBox ID="txtRemark4" Width="380px"  runat="server" /></label> 
                                                        <label class="col-lg-1 control-label"><asp:TextBox ID="txtAmt4" Width="100px" style="text-align:right" runat="server" /></label> 
                                                        <label class="col-lg-1 control-label"><asp:TextBox ID="txtCode4" Width="40px" runat="server" /></label>
                                                        <label class="col-lg-2 control-label"><asp:TextBox ID="vxtOther4" runat="server" /><asp:Label ID="lblOtherName4" ForeColor="Blue" Font-Size="8pt" Font-Bold="True"  runat="server" /></label>
                                                    </div>
                                                    <div class="form-group">
                                                        <label class="col-lg-2 control-label" style="Text-align: left"><asp:TextBox ID="vxtAccno5" Width="190px"  AutoPostBack="True" runat="server" /><asp:Label ID="lblAccName5" ForeColor="Blue" Font-Size="8pt" Font-Bold="True"  runat="server" /></label> 
                                                        <label class="col-lg-4 control-label"><asp:TextBox ID="txtRemark5" Width="380px"  runat="server" /></label> 
                                                        <label class="col-lg-1 control-label"><asp:TextBox ID="txtAmt5" Width="100px" style="text-align:right" runat="server" /></label> 
                                                        <label class="col-lg-1 control-label"><asp:TextBox ID="txtCode5" Width="40px" runat="server" /></label>
                                                        <label class="col-lg-2 control-label"><asp:TextBox ID="vxtOther5" runat="server" /><asp:Label ID="lblOtherName5" ForeColor="Blue" Font-Size="8pt" Font-Bold="True"  runat="server" /></label>
                                                    </div>
                                                    <div class="form-group">
                                                        <label class="col-lg-2 control-label" style="Text-align: left"><asp:TextBox ID="vxtAccno6" Width="190px"  AutoPostBack="True" runat="server" /><asp:Label ID="lblAccName6" ForeColor="Blue" Font-Size="8pt" Font-Bold="True"  runat="server" /></label> 
                                                        <label class="col-lg-4 control-label"><asp:TextBox ID="txtRemark6" Width="380px"  runat="server" /></label> 
                                                        <label class="col-lg-1 control-label"><asp:TextBox ID="txtAmt6" Width="100px" style="text-align:right" runat="server" /></label> 
                                                        <label class="col-lg-1 control-label"><asp:TextBox ID="txtCode6" Width="40px" runat="server" /></label>
                                                        <label class="col-lg-2 control-label"><asp:TextBox ID="vxtOther6" runat="server" /><asp:Label ID="lblOtherName6" ForeColor="Blue" Font-Size="8pt" Font-Bold="True"  runat="server" /></label>
                                                    </div>
                                                   
                                                    <asp:Panel ID="gbxQty" GroupingText="材料數量23456項" runat="server">
                                                        <div class="form-group">
                                                            <div class="col-lg-1 control-label" ><asp:TextBox ID="txtQty2" Width="80px" runat="server" /></div>
                                                            <div class="col-lg-1 control-label" ><asp:TextBox ID="txtQty3" Width="80px" runat="server" /></div>
                                                            <div class="col-lg-1 control-label" ><asp:TextBox ID="txtQty4" Width="80px" runat="server" /></div>
                                                            <div class="col-lg-1 control-label" ><asp:TextBox ID="txtQty5" Width="80px" runat="server" /></div>
                                                            <div class="col-lg-1 control-label" ><asp:TextBox ID="txtQty6" Width="80px" runat="server" /></div>
                                                        </div>
                                                    </asp:Panel>
                                                    
                                                    <AjaxToolkit:AutoCompleteExtender 
                                                            ID="AutoCompleteExtender2"                             
                                                            runat="server"                
                                                            TargetControlID="vxtAccno2"
                                                            ServicePath="~/active/WebService.asmx"
                                                            ServiceMethod="GetAC010Accno"
                                                            MinimumPrefixLength="0" 
                                                            CompletionInterval="100"
                                                            CompletionSetCount="12" DelimiterCharacters="" Enabled="True" />
                                                    <AjaxToolkit:AutoCompleteExtender 
                                                            ID="AutoCompleteExtender3"                             
                                                            runat="server"                
                                                            TargetControlID="vxtAccno3"
                                                            ServicePath="~/active/WebService.asmx"
                                                            ServiceMethod="GetAC010Accno"
                                                            MinimumPrefixLength="0" 
                                                            CompletionInterval="100"
                                                            CompletionSetCount="12" DelimiterCharacters="" Enabled="True" />
                                                    <AjaxToolkit:AutoCompleteExtender 
                                                            ID="AutoCompleteExtender4"                             
                                                            runat="server"                
                                                            TargetControlID="vxtAccno4"
                                                            ServicePath="~/active/WebService.asmx"
                                                            ServiceMethod="GetAC010Accno"
                                                            MinimumPrefixLength="0" 
                                                            CompletionInterval="100"
                                                            CompletionSetCount="12" DelimiterCharacters="" Enabled="True" />
                                                    <AjaxToolkit:AutoCompleteExtender 
                                                            ID="AutoCompleteExtender5"                             
                                                            runat="server"                
                                                            TargetControlID="vxtAccno5"
                                                            ServicePath="~/active/WebService.asmx"
                                                            ServiceMethod="GetAC010Accno"
                                                            MinimumPrefixLength="0" 
                                                            CompletionInterval="100"
                                                            CompletionSetCount="12" DelimiterCharacters="" Enabled="True" />
                                                    <AjaxToolkit:AutoCompleteExtender 
                                                            ID="AutoCompleteExtender6"                             
                                                            runat="server"                
                                                            TargetControlID="vxtAccno6"
                                                            ServicePath="~/active/WebService.asmx"
                                                            ServiceMethod="GetAC010Accno"
                                                            MinimumPrefixLength="0" 
                                                            CompletionInterval="100"
                                                            CompletionSetCount="12" DelimiterCharacters="" Enabled="True" />

                                                    <AjaxToolkit:AutoCompleteExtender 
                                                            ID="AutoCompleteExtender1"                             
                                                            runat="server"                
                                                            TargetControlID="txtRemark2"
                                                            ServicePath="~/active/WebService.asmx"
                                                            ServiceMethod="GetAC010Remark"
                                                            MinimumPrefixLength="0" 
                                                            CompletionInterval="100"
                                                            CompletionSetCount="12" DelimiterCharacters="" Enabled="True" />
                                                    <AjaxToolkit:AutoCompleteExtender 
                                                            ID="AutoCompleteExtender7"                             
                                                            runat="server"                
                                                            TargetControlID="txtRemark3"
                                                            ServicePath="~/active/WebService.asmx"
                                                            ServiceMethod="GetAC010Remark"
                                                            MinimumPrefixLength="0" 
                                                            CompletionInterval="100"
                                                            CompletionSetCount="12" DelimiterCharacters="" Enabled="True" />
                                                    <AjaxToolkit:AutoCompleteExtender 
                                                            ID="AutoCompleteExtender8"                             
                                                            runat="server"                
                                                            TargetControlID="txtRemark4"
                                                            ServicePath="~/active/WebService.asmx"
                                                            ServiceMethod="GetAC010Remark"
                                                            MinimumPrefixLength="0" 
                                                            CompletionInterval="100"
                                                            CompletionSetCount="12" DelimiterCharacters="" Enabled="True" />
                                                    <AjaxToolkit:AutoCompleteExtender 
                                                            ID="AutoCompleteExtender9"                             
                                                            runat="server"                
                                                            TargetControlID="txtRemark5"
                                                            ServicePath="~/active/WebService.asmx"
                                                            ServiceMethod="GetAC010Remark"
                                                            MinimumPrefixLength="0" 
                                                            CompletionInterval="100"
                                                            CompletionSetCount="12" DelimiterCharacters="" Enabled="True" />
                                                    <AjaxToolkit:AutoCompleteExtender 
                                                            ID="AutoCompleteExtender10"                             
                                                            runat="server"                
                                                            TargetControlID="txtRemark6"
                                                            ServicePath="~/active/WebService.asmx"
                                                            ServiceMethod="GetAC010Remark"
                                                            MinimumPrefixLength="0" 
                                                            CompletionInterval="100"
                                                            CompletionSetCount="12" DelimiterCharacters="" Enabled="True" />
                                                    <AjaxToolkit:AutoCompleteExtender 
                                                            ID="AutoCompleteExtender11"                             
                                                            runat="server"                
                                                            TargetControlID="vxtOther2"
                                                            ServicePath="~/active/WebService.asmx"
                                                            ServiceMethod="GetAC010Accno"
                                                            MinimumPrefixLength="0" 
                                                            CompletionInterval="100"
                                                            CompletionSetCount="12" DelimiterCharacters="" Enabled="True" />
                                                    <AjaxToolkit:AutoCompleteExtender 
                                                            ID="AutoCompleteExtender12"                             
                                                            runat="server"                
                                                            TargetControlID="vxtOther3"
                                                            ServicePath="~/active/WebService.asmx"
                                                            ServiceMethod="GetAC010Accno"
                                                            MinimumPrefixLength="0" 
                                                            CompletionInterval="100"
                                                            CompletionSetCount="12" DelimiterCharacters="" Enabled="True" />
                                                    <AjaxToolkit:AutoCompleteExtender 
                                                            ID="AutoCompleteExtender13"                             
                                                            runat="server"                
                                                            TargetControlID="vxtOther4"
                                                            ServicePath="~/active/WebService.asmx"
                                                            ServiceMethod="GetAC010Accno"
                                                            MinimumPrefixLength="0" 
                                                            CompletionInterval="100"
                                                            CompletionSetCount="12" DelimiterCharacters="" Enabled="True" />
                                                    <AjaxToolkit:AutoCompleteExtender 
                                                            ID="AutoCompleteExtender14"                             
                                                            runat="server"                
                                                            TargetControlID="vxtOther5"
                                                            ServicePath="~/active/WebService.asmx"
                                                            ServiceMethod="GetAC010Accno"
                                                            MinimumPrefixLength="0" 
                                                            CompletionInterval="100"
                                                            CompletionSetCount="12" DelimiterCharacters="" Enabled="True" />
                                                    <AjaxToolkit:AutoCompleteExtender 
                                                            ID="AutoCompleteExtender15"                             
                                                            runat="server"                
                                                            TargetControlID="vxtOther6"
                                                            ServicePath="~/active/WebService.asmx"
                                                            ServiceMethod="GetAC010Accno"
                                                            MinimumPrefixLength="0" 
                                                            CompletionInterval="100"
                                                            CompletionSetCount="12" DelimiterCharacters="" Enabled="True" />
                                                                                                 
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
