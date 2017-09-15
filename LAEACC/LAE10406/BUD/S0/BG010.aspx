<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="BG010.aspx.vb" Inherits="LAEACC.BG010" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="AjaxToolkit" %>

<%@ MasterType VirtualPath="~/Site.master" %>
<%@ Register SRC="~/LAE10406/UserControl/UCBase.ascx" TagName="UCBase" TagPrefix="uc1" %>

<asp:Content ID="Head" ContentPlaceHolderID="MainHead" runat="server">
<script type="text/javascript">
    function ismaxlength(obj) {
        debugger;
        var mlength = obj.getAttribute ? parseInt(obj.getAttribute("maxlength")) : "";
        if (obj.getAttribute && obj.value.length > mlength) {
            obj.value = obj.value.substring(0, mlength);
        }
    }
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
                            <div style="margin:5px 0px 5px 0px;">
                                <uc1:UCBase ID="UCBase1" runat="server" />
                            </div>
                            <div style="clear:both; height:5px;"></div>                            

                            <!--詳細內容顯示區-->                           
                            <AjaxToolkit:TabContainer ID="TabContainer1" Width="100%" CssClass="Tab" runat="server" ActiveTabIndex="1">
                                <AjaxToolkit:TabPanel ID="TabPanel1" runat="server">
                                    <HeaderTemplate>請購中清單</HeaderTemplate>
                                    <ContentTemplate>                                        
                                        <div class="table-responsive">
                                            <div class="col-lg-10">
                                                <asp:Button ID="btnPay000" Text="新增請購(由差勤轉入)" CssClass="btn btn-primary" runat="server" />
                                            </div> 
                                            <div style="float:right; padding-right:10px;">
                                                共<asp:Label ID="lbl_GrdCount" ForeColor="Red" Font-Size="14pt" Font-Bold="True" Text="0" runat="server" />筆符合&nbsp;                                                
                                                <asp:Label ID="lbl_sort" runat="server" />
                                            </div>
                                            <asp:DataGrid ID="DataGridView" Width="100%" AllowSorting="True" AllowPaging="True" style="font-size:16px; font-weight:bold;" CssClass="table table-bordered table-condensed smart-form" runat="server" >
                                                <columns>
                                                    <asp:TemplateColumn HeaderText="管理">
                                                        <itemtemplate>                                                                                                            
                                                            <asp:ImageButton ID="Show" AlternateText="查閱" ImageUrl="~/active/images/icon/items/edit.png" CommandName="btnShow" runat="server" />
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
                                                        <HeaderStyle Width="80px" />
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
                                                    <asp:TemplateColumn HeaderText="科目名稱" SortExpression="b.ACCNAME">
                                                        <itemtemplate><asp:Label ID="科目名稱" Text='<%# Container.DataItem("ACCNAME").ToString%>' runat="server" /></itemtemplate>                                                       
                                                        <HeaderStyle Width="160px" />
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
                                                </columns>
                                            </asp:DataGrid>
                                        </div>
                                    </ContentTemplate>
                                </AjaxToolkit:TabPanel>

                                <AjaxToolkit:TabPanel ID="TabPanel2" runat="server">
                                    <HeaderTemplate>請購明細</HeaderTemplate>
                                    <ContentTemplate>
                                        <table id="table-data" rules="all">
                                            <tr>
                                                <th>年度：</th>
                                                <td><asp:Label ID="lblYear" ForeColor="Blue" Font-Size="12pt" Font-Bold="True" runat="server" /></td>
                                                <th>請購日期：</th>
                                                <td>
                                                    <asp:TextBox ID="dtpDate1a" Width="100px" CssClass="form-control td-left" onClick="WdatePicker({dateFmt:'yyy-MM-dd',skin:'whyGreen'})" runat="server" />
                                                    <asp:Label ID="lblkind" ForeColor="Blue" Font-Size="12pt" Font-Bold="True" Visible="False" runat="server" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <th style="color:#00f;">請購編號：</th>
                                                <td>
                                                    <asp:Label ID="lblNo" ForeColor="Red" Font-Size="14pt" Font-Bold="True" Text="　　　　　" runat="server" />                                                    
                                                    <asp:Label ID="lblkey" ForeColor="Blue" Font-Size="12pt" Font-Bold="True" Visible="False" runat="server" />                                                    
                                                </td>
                                                <th>六級科目餘額：</th>
                                                <td>                                                    
                                                    <asp:Button ID="btnGrade6" Text="六級餘額查詢" CssClass="btn btn-info" runat="server" />
                                                    <asp:Label ID="lblGrade6" ForeColor="Blue" Font-Size="12pt" Font-Bold="True" runat="server" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <th>請購科目：</th>
                                                <td colspan="3">
                                                    <asp:Label ID="lblAccno" ForeColor="Blue" Font-Size="12pt" Font-Bold="True"  runat="server" />
                                                    <asp:Label ID="lblAccname" ForeColor="Blue" Font-Size="12pt" Font-Bold="True"  runat="server" />
                                                    <asp:DropDownList ID="cboAccno" CssClass="form-control td-left" Width="800px" AutoPostBack="True" runat="server" />                                                    
                                                </t>                                               
                                            </tr>
                                            <tr>
                                                <th>請購事由：</th>
                                                <td colspan="3">                                                    
                                                    <asp:TextBox ID="txtRemarka" placeholder="長度不得超出50個字" CssClass="form-control td-left" Width="70%" runat="server" />
                                                    <asp:Button ID="btnAddRemark" Text="增入片語" CssClass="btn btn-warning td-right" runat="server" />                                                    
                                                </td>                                                                                               
                                            </tr>
                                            <tr>
                                                <th>請購金額：</th>
                                                <td> 
                                                    <asp:TextBox ID="txtAmta" CssClass="form-control td-left" Width="100px" placeholder="請購金額" runat="server" /> 
                                                    <asp:Label ID="lblAMT" CssClass="td-right" ForeColor="Blue" Font-Size="12pt" Font-Bold="True" runat="server" />                                                   
                                                </td>
                                                <th>受款人：</th>
                                                <td> 
                                                    <asp:TextBox ID="txtSubjecta" CssClass="form-control td-left" Width="300px" placeholder="直接開支才填受款人" runat="server" />
                                                    <asp:Button ID="btnAddSubject" Text="增入片語" CssClass="btn btn-warning td-right" runat="server" />                                                                                                        
                                                </td>
                                            </tr>
                                            <tr>
                                                <th>編號：</th>
                                                <td colspan="3">                                                    
                                                    <asp:TextBox ID="txtUserid" CssClass="form-control td-left" Width="25%" placeholder="以員工編號取姓名,置請購事由" runat="server" />
                                                    <asp:Button ID="btnUserid" Text="找尋" CssClass="btn btn-success td-right" runat="server" />
                                                </td>                                                
                                            </tr>                                            
                                        </table>                                        
                                        <div style="display:none;">
                                            <asp:TextBox ID="txtAreaA" CssClass="form-control" placeholder="匯管理處" runat="server" />
                                            <asp:DropDownList ID="cboAreaA" CssClass="form-control" AutoPostBack="True" runat="server" />
                                        </div>

                                        <!--#伺服器驗證-->
                                        <AjaxToolkit:ConfirmButtonExtender ID="ConfirmButtonExtender1" runat="server"
                                                    TargetControlID="btnAddRemark" ConfirmText="您是否將摘要新增至片語檔??" BehaviorID="_content_ConfirmButtonExtender1">
                                        </AjaxToolkit:ConfirmButtonExtender>
                                        <AjaxToolkit:ConfirmButtonExtender ID="ConfirmButtonExtender2" runat="server"
                                                    TargetControlID="btnAddSubject" ConfirmText="您是否將受款人新增至片語檔??" BehaviorID="_content_ConfirmButtonExtender2">
                                        </AjaxToolkit:ConfirmButtonExtender>

                                        <!--#自動完成-->
                                        <AjaxToolkit:AutoCompleteExtender 
                                                    ID="AutoCompleteExtender1"                             
                                                    runat="server"                
                                                    TargetControlID="txtRemarka"
                                                    ServicePath="~/active/WebService.asmx"
                                                    ServiceMethod="GetCompletionListRemark"
                                                    MinimumPrefixLength="0" 
                                                    CompletionInterval="100"
                                                    CompletionSetCount="12" BehaviorID="_content_AutoCompleteExtender1" DelimiterCharacters="" />
                                        <AjaxToolkit:AutoCompleteExtender 
                                                    ID="AutoCompleteExtender2"                             
                                                    runat="server"                
                                                    TargetControlID="txtSubjecta"
                                                    ServicePath="~/active/WebService.asmx"
                                                    ServiceMethod="GetCompletionListSubject"
                                                    MinimumPrefixLength="0" 
                                                    CompletionInterval="100"
                                                    CompletionSetCount="12" BehaviorID="_content_AutoCompleteExtender2" DelimiterCharacters="" />                                                                                                                                                              
                                    </ContentTemplate>
                                </AjaxToolkit:TabPanel>
                                <AjaxToolkit:TabPanel ID="TabPanel3" runat="server">
                                    <HeaderTemplate>差勤</HeaderTemplate>
                                    <ContentTemplate>
                                        <div class="table-responsive">
                                            <div class="col-lg-10">
                                                年度
                                                <asp:TextBox ID="txtByear" runat="server" /> 
                                                批號
                                                <asp:TextBox ID="txtBatNo" runat="server" /> 
                                                <asp:Button ID="btnPayno" Text="確定" CssClass="btn btn-primary" runat="server" />
                                                 <asp:Button ID="btnPaynoC" Text="取消" CssClass="btn btn-primary" runat="server" />
                                            </div>  
                                            <div style="float:right; padding-right:10px;">
                                                共<asp:Label ID="lbl_dtgPay000GrdCount" ForeColor="Red" Font-Size="14pt" Font-Bold="True" Text="0" runat="server" />筆符合&nbsp;                                                
                                                <asp:Label ID="Label2" runat="server" />
                                            </div>
                                            <asp:DataGrid ID="dtgPay000" Width = "100%" AllowSorting="false" AllowPaging="false" CssClass="table table-bordered table-condensed smart-form" runat="server" >
                                                <columns>
                                                    <asp:TemplateColumn HeaderText="管理">
                                                        <itemtemplate>                                                                                                            
                                                            <asp:ImageButton ID="Show" AlternateText="查閱" ImageUrl="~/active/images/icon/items/zoom.png" CommandName="btnShow" runat="server" />
                                                            <asp:Label ID="id" Text='<%# Container.DataItem("BATNO").ToString%>' Visible="false" runat="server" />
                                                            <asp:UpdatePanel ID="dtgPay000_UpdatePanel" runat="server">                                                                
                                                                <Triggers><asp:AsyncPostBackTrigger ControlID="Show" EventName="Click" /></Triggers>
                                                            </asp:UpdatePanel>
                                                        </itemtemplate>                                                         
                                                        <HeaderStyle Width="40px" />
                                                        <ItemStyle HorizontalAlign="Center" />
                                                    </asp:TemplateColumn>                                                                                
                                                    <asp:TemplateColumn HeaderText="年度" >
                                                        <itemtemplate><asp:Label ID="年度" Text='<%# Container.DataItem("ACCYEAR").ToString%>' runat="server" /></itemtemplate>                                                       
                                                        <HeaderStyle Width="40px" />
                                                        <ItemStyle HorizontalAlign="Center" />
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="批號">
                                                        <itemtemplate><asp:Label ID="批號" Text='<%# Container.DataItem("BATNO").ToString%>' runat="server" /></itemtemplate>
                                                        <HeaderStyle Width="40px" />
                                                        <ItemStyle HorizontalAlign="Center" />
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="開支科目" >
                                                        <itemtemplate><asp:Label ID="開支科目" Text='<%# Container.DataItem("ACCNO").ToString%>' runat="server" /></itemtemplate>                                                       
                                                        <HeaderStyle Width="100px" />
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="科目名稱" >
                                                        <itemtemplate><asp:Label ID="科目名稱" Text='<%# Container.DataItem("accname").ToString%>' runat="server" /></itemtemplate>                                                       
                                                        <HeaderStyle Width="160px" />
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="受款人" >
                                                        <itemtemplate><asp:Label ID="受款人" Text='<%# Container.DataItem("NAME1").ToString%>' runat="server" /></itemtemplate>                                                       
                                                        <HeaderStyle Width="100px" />
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="摘要" >
                                                        <itemtemplate><asp:Label ID="摘要" Text='<%# Container.DataItem("REMARK").ToString%>' runat="server" /></itemtemplate>                                                       
                                                        <HeaderStyle Width="120px" />
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="金額" >
                                                        <itemtemplate><asp:Label ID="金額" Text='<%# FormatNumber(Container.DataItem("AMT").ToString, 2)%>' runat="server" /></itemtemplate>                                                       
                                                        <HeaderStyle Width="100px" />
                                                        <ItemStyle HorizontalAlign="Right" />
                                                    </asp:TemplateColumn>                                                                                                              
                                                    <asp:TemplateColumn HeaderText="組室">
                                                        <itemtemplate><asp:Label ID="組室" Text='<%# Container.DataItem("UNIT").ToString%>' runat="server" /></itemtemplate>                                                       
                                                        <HeaderStyle Width="40px" />
                                                        <ItemStyle HorizontalAlign="Center" />
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


