<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="PGM010.aspx.vb" Inherits="LAEACC.PGM010" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="AjaxToolkit" %>

<%@ MasterType VirtualPath="~/Site.master" %>
<%@ Register SRC="~/LAE10406/UserControl/UCBase.ascx" TagName="UCBase" TagPrefix="uc1" %>

<asp:Content ID="Head" ContentPlaceHolderID="MainHead" runat="server">
    <script type="text/javascript">
        function stringBytes(c) {
            var n = c.length, s;
            var len = 0;
            for (var i = 0; i < n; i++) {
                s = c.charCodeAt(i);
                while (s > 0) {
                    len++;
                    s = s >> 8;
                }
            }
            return len;
        }
        function substr(str, len) {
            if (!str || !len) { return ''; }
            var a = 0;
            var i = 0;
            var temp = '';

            for (i = 0; i < str.length; i++) {
                if (str.charCodeAt(i) > 255) {
                    a += 2;
                }
                else {
                    a++;
                }
                if (a > len) { return temp; }
                temp += str.charAt(i);
            }
            return str;
        }
        function ismaxlength(obj) {
            if (stringBytes(obj.value) > 10)
                obj.value = substr(obj.value, 10)
            obj.value = obj.value.trim()
        }
        function ismaxlength2(obj) {
            if (stringBytes(obj.value) > 5)
                obj.value = substr(obj.value, 5)
            obj.value = obj.value.trim()
        }
    </script>
</asp:content>


<asp:Content ID="Content" ContentPlaceHolderID="MainContent" runat="server">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>        
            <!--關鍵值隱藏區-->
            <asp:Label ID="txtKey1" Text="" Visible="false" runat="server" />
            <table id="table-serch" rules="all">                                            
                <tr>
                    <td style="text-align:center;">
                        財務編號
                    </td>
                    <td style="text-align:center;">
                        <asp:TextBox ID="txtQueryPrNo"  Width="120" runat="server" AutoPostBack="true"  />
                    </td>
                    <td style="text-align:center;">
                        <asp:TextBox ID="txtQueryPrNo2"  Width="120" runat="server" AutoPostBack="true" />
                    </td>
                        <!--#自動完成-->
                        <AjaxToolkit:AutoCompleteExtender 
                            ID="AutoCompleteExtender1" runat="server" ServicePath="~/active/WebService.asmx"               
                            TargetControlID="txtQueryPrNo"  ServiceMethod="GetPGMKindNo" 
                            MinimumPrefixLength="0"   CompletionInterval="100" CompletionSetCount="12" BehaviorID="_content_AutoCompleteExtender1" DelimiterCharacters="" />
                        <AjaxToolkit:AutoCompleteExtender 
                            ID="AutoCompleteExtender2" runat="server" ServicePath="~/active/WebService.asmx"               
                            TargetControlID="txtQueryPrNo2"  ServiceMethod="GetPGMKindNo"
                            MinimumPrefixLength="0"   CompletionInterval="100" CompletionSetCount="12" BehaviorID="_content_AutoCompleteExtender1" DelimiterCharacters="" />
                    <td style="text-align:center;">
                        報廢日期
                    </td>
                    <td style="text-align:center;">
                       <asp:TextBox ID="txtQueryEndDate" Width="80px"  onClick="WdatePicker({dateFmt:'yyy-MM-dd',skin:'whyGreen'})" runat="server" />
                    </td>
                    <td style="text-align:center;">
                        <asp:TextBox ID="txtQueryEndDate2" Width="80px"  onClick="WdatePicker({dateFmt:'yyy-MM-dd',skin:'whyGreen'})" runat="server" />
                    </td>   

                   <td style="text-align:center;">
                        財物名稱
                    </td>
                    <td style="text-align:center;">
                        <asp:TextBox ID="txtQueryName"  Width="100" runat="server"  />
                    </td>

                    <td style="text-align:center;">
                        用途
                    </td>                               
                    <td style="text-align:center;">
                        <asp:TextBox ID="txtQueryUses"  Width="80" runat="server"  />
                    </td>
                    <td style="text-align:center;">
                        <asp:Button ID="btnQuery" Text="搜尋" runat="server" />
                    </td>
                    
                </tr>
                <tr>
                    <td style="text-align:center;">
                        會計科目
                    </td>
                    <td style="text-align:center;">
                        <asp:TextBox ID="txtQueryAcNo"  Width="120" runat="server"   />
                    </td>
                    <td style="text-align:center;">
                        <asp:TextBox ID="txtQueryAcNo2"  Width="120" runat="server" />
                    </td> 

                    <td style="text-align:center;">
                       修改日期
                    </td>
                    <td style="text-align:center;">
                       <asp:TextBox ID="txtQueryRevisedDate" Width="80px"  onClick="WdatePicker({dateFmt:'yyy-MM-dd',skin:'whyGreen'})" runat="server" />
                    </td>
                    <td style="text-align:center;">
                        <asp:TextBox ID="txtQueryRevisedDate2" Width="80px"  onClick="WdatePicker({dateFmt:'yyy-MM-dd',skin:'whyGreen'})" runat="server" />
                    </td>
                    <td style="text-align:center;">
                        補助單位
                    </td>
                    <td style="text-align:center;">
                        <asp:TextBox ID="txtQueryComeFrom"  Width="100" runat="server"  />
                    </td>

                    <td style="text-align:center;">
                        使用年限
                    </td>
                    <td style="text-align:center;">
                       <asp:DropDownList ID="cboQueryUseyear" runat="server" />
                    </td>
                    <td style="text-align:center;">
                        <asp:DropDownList ID="cboQueryUseyear2" runat="server" />
                    </td>

                    
                </tr>
                <tr>
                    <td style="text-align:center;">
                        預算科目
                    </td>
                    <td style="text-align:center;">
                        <asp:TextBox ID="txtQueryBgAcNo"  Width="120" runat="server"   />
                    </td>
                    <td style="text-align:center;">
                        <asp:TextBox ID="txtQueryBgAcNo2"  Width="120" runat="server" />
                    </td>

                    <td style="text-align:center;">
                        保管人
                    </td>
                    <td style="text-align:center;">
                        <asp:TextBox ID="txtQueryEmpNo"  Width="80" runat="server"   />
                    </td>
                    <td style="text-align:center;">
                        <asp:TextBox ID="txtQueryEmpNo2"  Width="80" runat="server" />
                    </td>

                    <td style="text-align:center;">
                        廠牌型號
                    </td>
                    <td style="text-align:center;">
                        <asp:TextBox ID="txtQueryModel"  Width="100" runat="server"  />
                    </td>

                    <td style="text-align:center;">
                       原值
                    </td>
                    <td style="text-align:center;">
                        <asp:TextBox ID="txtQueryAmt"  Width="80" runat="server"   />
                    </td>
                    <td style="text-align:center;">
                        <asp:TextBox ID="txtQueryAmt2"  Width="80" runat="server" />
                    </td>
                    
                </tr>
                <tr> 
                    <td style="text-align:center;">
                        購入日期
                    </td>
                    <td style="text-align:center;">
                         <asp:TextBox ID="txtQueryPDate" Width="80px"  onClick="WdatePicker({dateFmt:'yyy-MM-dd',skin:'whyGreen'})" runat="server" />
                    </td>
                    <td style="text-align:center;">
                        <asp:TextBox ID="txtQueryPDate2" Width="80px"  onClick="WdatePicker({dateFmt:'yyy-MM-dd',skin:'whyGreen'})" runat="server" />
                    </td> 
                    
                    <td style="text-align:center;">
                        保管單位
                    </td>
                    <td style="text-align:center;">
                        <asp:TextBox ID="txtQueryKeepUnit"  Width="80" runat="server"   />
                    </td>
                    <td style="text-align:center;">
                        <asp:TextBox ID="txtQueryKeepUnit2"  Width="80" runat="server" />
                    </td>

                    

                    <td style="text-align:center;">
                        規格備註
                    </td>
                    <td style="text-align:center;">
                        <asp:TextBox ID="txtQuerySpecRemark"  Width="100" runat="server"  />
                    </td>
                           
                    
                    
                    
                    
                    <td style="text-align:center;">
                        報廢情形
                    </td>
                    <td colspan="2" style="text-align:center;">
                        <asp:DropDownList ID="cboQueryDiscardMode" runat="server" />
                    </td>

                    <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                        <ContentTemplate></ContentTemplate>
                        <Triggers>
                            <asp:PostBackTrigger ControlID="btnQuery" />
                        </Triggers>
                    </asp:UpdatePanel> 

                </tr>
                
            </table>
   
            <!--主項目區-->
            <div style="margin: 10px 0px 0px 10px;">
                <section id="widget-grid" style="width:98%;">
                    <div class="row">
                        <article class="col-sm-12 col-md-12 col-lg-12">
                            <!--控制項-->
                            <div style="margin:5px 0px 5px 0px;">
                                <uc1:UCBase ID="UCBase1" runat="server" />

                                <asp:UpdatePanel ID="UpdatePanel4" runat="server">
                                    <ContentTemplate></ContentTemplate>
                                    <Triggers>
                                        <asp:PostBackTrigger ControlID="UCBase1" />
                                    </Triggers>
                                </asp:UpdatePanel></td>
                            </div>
                            <div style="clear:both; height:5px;"></div> 

                            <!--詳細內容顯示區-->                           
                            <AjaxToolkit:TabContainer ID="TabContainer1" Width="100%" CssClass="Tab" runat="server" ActiveTabIndex="1">
                                <AjaxToolkit:TabPanel ID="TabPanel1" runat="server">
                                    <HeaderTemplate>多筆瀏灠</HeaderTemplate>
                                    <ContentTemplate>
                                        <div style="font-size:14px; OVERFLOW:auto; height:450px;">                                            
                                            <div style="float:left; padding-right:10px;">
                                                共<asp:Label ID="lbl_GrdCount" ForeColor="Red" Font-Size="14" Font-Bold="true" Text="0" runat="server" />筆符合&nbsp;                                                
                                                <asp:Label ID="lbl_sort" runat="server" />
                                            </div>
                                            &nbsp;&nbsp;
                                            <asp:Label ID="Label1" ForeColor="Red" Font-Size="14" Font-Bold="true" Text="0" runat="server" />
                                            <asp:DataGrid ID="DataGridView" Width="100%" AllowSorting="true" AllowPaging="false" PageSize="100" style="font-size:14px;" CssClass="table table-bordered table-condensed smart-form" runat="server" >
                                                <columns>
                                                    <asp:TemplateColumn HeaderText="管理" HeaderStyle-Width="40px" ItemStyle-HorizontalAlign="Center">
                                                        <itemtemplate>                                                                                                            
                                                            <asp:ImageButton ID="Show" AlternateText="查閱" ImageUrl="~/active/images/icon/items/zoom.png" CommandName="btnShow" runat="server" />
                                                            <asp:Label ID="id" Text='<%# Container.DataItem("PRNO").ToString%>' Visible="false" runat="server" />
                                                            <asp:UpdatePanel ID="DataGridView_UpdatePanel" runat="server">                                                                
                                                                <Triggers><asp:AsyncPostBackTrigger ControlID="Show" EventName="Click" /></Triggers>
                                                            </asp:UpdatePanel>
                                                        </itemtemplate>                                                         
                                                    </asp:TemplateColumn>
                                                                            
                                                    <asp:TemplateColumn HeaderText="財物編號" SortExpression="KindNo" HeaderStyle-Width="100" ItemStyle-HorizontalAlign="left">
                                                        <itemtemplate><asp:Label ID="財物編號" Text='<%# Container.DataItem("PRNO").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>                                                   
                                                    <asp:TemplateColumn HeaderText="財產名稱" SortExpression="Name" HeaderStyle-Width="200" ItemStyle-HorizontalAlign="left">
                                                        <itemtemplate><asp:Label ID="財產名稱" Text='<%# Container.DataItem("Name").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>                                                    
                                                    <asp:TemplateColumn HeaderText="會計科目" SortExpression="Unit" HeaderStyle-Width="100" ItemStyle-HorizontalAlign="Center">
                                                        <itemtemplate><asp:Label ID="會計科目" Text='<%# Container.DataItem("ACNO").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="購入日期" SortExpression="Material" HeaderStyle-Width="200" ItemStyle-HorizontalAlign="Center">
                                                        <itemtemplate><asp:Label ID="購入日期" Text='<%# Container.DataItem("PDate").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="原值" SortExpression="UseYear" HeaderStyle-Width="100" ItemStyle-HorizontalAlign="Center">
                                                        <itemtemplate><asp:Label ID="原值" Text='<%# Container.DataItem("AMT").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>
                                                                                                                                                                                                                
                                                </columns>
                                            </asp:DataGrid>
                                        </div>
                                    </ContentTemplate>
                                </AjaxToolkit:TabPanel>

                                <AjaxToolkit:TabPanel ID="TabPanel2" runat="server">
                                    <HeaderTemplate>新增/修改</HeaderTemplate>
                                    <ContentTemplate>
                                        <table id="table-data" rules="all">
                                            <tr>
                                                <th>*財物編號：</th>
                                                <td>
                                                    <asp:TextBox ID="txtPrNo" CssClass="form-control" Width="200px" runat="server" AutoPostBack="True" OnTextChanged="txtPrNo_TextChanged" />
                                                    <asp:Label ID="lblkey" ForeColor="Blue" Font-Size="12pt" Font-Bold="True" Visible="False" runat="server" />
                                                    <AjaxToolkit:AutoCompleteExtender 
                                                        ID="AutoCompleteExtender3" runat="server" ServicePath="~/active/WebService.asmx"               
                                                        TargetControlID="txtPrNo"  ServiceMethod="GetPGMKindNo"
                                                        MinimumPrefixLength="0"   CompletionInterval="100" CompletionSetCount="12" BehaviorID="_content_AutoCompleteExtender1" DelimiterCharacters="" />
                                                    <asp:UpdatePanel ID="UpdatePanel3" runat="server">
                                                        <ContentTemplate></ContentTemplate>
                                                        <Triggers>
                                                            <asp:PostBackTrigger ControlID="txtPrNo" />
                                                        </Triggers>
                                                    </asp:UpdatePanel> 
                                                </td>
                                                <th>新增(件)：</th>
                                                <td><asp:DropDownList ID="cboAddCount" runat="server" />
                                                </td>
                                                <th>*序號：</th>
                                                <td>
                                                    <asp:TextBox ID="txtNo" CssClass="form-control" Width="100px" runat="server" />
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="txtNo2" CssClass="form-control" Width="100px" runat="server" />
                                                </td>
                                                <td>
                                                    <asp:Button ID="btnGetPrNo" Text="取得序號" runat="server" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <th>*名稱：</th>
                                                <td><asp:TextBox ID="txtName" CssClass="form-control" Width="200px" runat="server" /></td>
                                                <th>單位：</th>
                                                <td><asp:TextBox ID="txtUnit" CssClass="form-control" Width="100px" runat="server" /></td>
                                                <th>材質：</th>
                                                <td><asp:TextBox ID="txtMaterial" CssClass="form-control" Width="100px" runat="server" /></td>
                                                <th>*使用年限：</th>
                                                <td><asp:DropDownList ID="cboUseYear" runat="server" /></td>
                                            </tr>
                                            <tr>
                                                <th>會計科目：</th>
                                                <td><asp:TextBox ID="txtAcNo" CssClass="form-control" Width="200px" runat="server" /></td>
                                                <th>*數量：</th>
                                                <td><asp:TextBox ID="txtQty" CssClass="form-control" Width="100px" runat="server" /></td>
                                                <th>*原值：</th>
                                                <td><asp:TextBox ID="txtAmt" CssClass="form-control" Width="100px" runat="server" /></td>
                                                <th>*預估殘值：</th>
                                                <td><asp:TextBox ID="txtEndAmt" CssClass="form-control" Width="100px" runat="server" /></td>
                                            </tr>
                                            <tr>
                                                <th>*購入日期：</th>
                                                <td><asp:TextBox ID="txtPDate" Width="80px"  onClick="WdatePicker({dateFmt:'yyy-MM-dd',skin:'whyGreen'})" runat="server" /></td>
                                                <th>預算科目：</th>
                                                <td><asp:TextBox ID="txtBgAcNo" CssClass="form-control" Width="100px" runat="server" /></td>
                                                <th>補助單位：</th>
                                                <td><asp:TextBox ID="txtComeFrom" CssClass="form-control" Width="100px" runat="server" /></td>
                                                <th>製造日期：</th>
                                                <td><asp:TextBox ID="txtMadeDate" Width="80px"  onClick="WdatePicker({dateFmt:'yyy-MM-dd',skin:'whyGreen'})" runat="server" /></td>
                                            </tr>
                                            <tr>
                                                <th>預算名稱：</th>
                                                <td><asp:TextBox ID="txtBgAcName" CssClass="form-control" Width="200px" runat="server" /></td>
                                                <th>廠牌型號：</th>
                                                <td><asp:TextBox ID="txtModel" CssClass="form-control" Width="100px" runat="server" /></td>
                                                <th>放置地點：</th>
                                                <td><asp:TextBox ID="txtPlace" CssClass="form-control" Width="100px" runat="server" /></td>
                                                <th>用途：</th>
                                                <td><asp:TextBox ID="txtUses" CssClass="form-control" Width="100px" runat="server" /></td>
                                            </tr>
                                            <tr>
                                                <th>*保管：</th>
                                                <td><asp:DropDownList ID="cboWhoKeep" runat="server" AutoPostBack="true"/>
                                                <asp:TextBox ID="txtWhoKeep" CssClass="form-control" Width="150px" runat="server" /></td>
                                                    <AjaxToolkit:AutoCompleteExtender 
                                                        ID="AutoCompleteExtender4" runat="server" ServicePath="~/active/WebService.asmx"               
                                                        TargetControlID="txtWhoKeep"  ServiceMethod="GetPGMWhoKeep"
                                                        MinimumPrefixLength="0"   CompletionInterval="100" CompletionSetCount="12" BehaviorID="_content_AutoCompleteExtender1" DelimiterCharacters="" />
                                                <th>借用單位：</th>
                                                <td><asp:TextBox ID="txtBorrow" CssClass="form-control" Width="100px" runat="server" /></td>
                                                <th>報廢日期：</th>
                                                <td><asp:TextBox ID="txtEndDate" Width="80px"  onClick="WdatePicker({dateFmt:'yyy-MM-dd',skin:'whyGreen'})" runat="server" /></td>
                                                <th>報廢原因：</th>
                                                <td><asp:DropDownList ID="cboEndRemark" runat="server" /></td>
                                            </tr>
                                            <tr>
                                                <th>規格備註：</th>
                                                <td colspan="3"><asp:TextBox ID="txtSpecRemark" CssClass="form-control" Width="400px" TextMode="MultiLine" Height="100px"  runat="server" /></td>
                                                <th>備註：</th>
                                                <td colspan="3"><asp:TextBox ID="txtRemark" CssClass="form-control" Width="400px" TextMode="MultiLine" Height="100px" runat="server" /></td>
                                            </tr>
                                            
                                        </table>  
                                        <asp:Label ID="lblNetAmt" CssClass="td-right" ForeColor="Blue" Font-Size="12pt" Font-Bold="True" runat="server" />                                                                                         
                                    </ContentTemplate>
                                </AjaxToolkit:TabPanel> 
                                
                                <AjaxToolkit:TabPanel ID="TabPanel3" runat="server">
                                    <HeaderTemplate>財務交接作業</HeaderTemplate>
                                    <ContentTemplate>
                                        <table id="table-data" rules="all">
                                            <tr>
                                                <th>*財物編號：</th>
                                                <td>
                                                    <asp:TextBox ID="txtTraPrNo" runat="server" />
                                                    <asp:Button ID="btnTransGetPrNo" Text="查詢財物" runat="server" />
                                                </td>
                                                <th>財物名稱：</th>
                                                <td>
                                                    <asp:Label ID="txtTraName" ForeColor="Blue" Font-Size="12pt" Font-Bold="True" runat="server" />
                                                </td>
                                                
                                            </tr>
                                            <tr>
                                                <th>保管者：</th>
                                                <td><asp:Label ID="txtTraWhoKeep" ForeColor="Blue" Font-Size="12pt" Font-Bold="True" runat="server" /></td>
                                                <th>購入日期：</th>
                                                <td><asp:Label ID="txtTraPDate" ForeColor="Blue" Font-Size="12pt" Font-Bold="True" runat="server" /></td>
                                            </tr>
                                            <tr>
                                                <th>報廢日期：</th>
                                                <td><asp:Label ID="txtTraEndDate" ForeColor="Blue" Font-Size="12pt" Font-Bold="True" runat="server" /></td>
                                                <th>報廢原因：</th>
                                                <td><asp:Label ID="lblTraEndRemark" ForeColor="Blue" Font-Size="12pt" Font-Bold="True" runat="server" /></td>
                                            </tr>
                                            <tr>
                                                <th>*交接日期：</th>
                                                <td><asp:TextBox ID="txtTransferDate" Width="80px"  onClick="WdatePicker({dateFmt:'yyy-MM-dd',skin:'whyGreen'})" runat="server" /></td>

                                                <th>*保管：</th>
                                                <td><asp:DropDownList ID="cboTraNewWhoKeep" runat="server" />
                                                <asp:TextBox ID="txtTraNewWhoKeep" runat="server" /></td>
                                                <AjaxToolkit:AutoCompleteExtender 
                                                        ID="AutoCompleteExtender5" runat="server" ServicePath="~/active/WebService.asmx"               
                                                        TargetControlID="txtTraNewWhoKeep"  ServiceMethod="GetPGMWhoKeep"
                                                        MinimumPrefixLength="0"   CompletionInterval="100" CompletionSetCount="12" BehaviorID="_content_AutoCompleteExtender1" DelimiterCharacters="" />

                                            </tr>
                                            <tr>
                                                <td colspan="4">
                                                    <asp:Button ID="btnTransferQuery" Text="查詢交接紀錄" runat="server" />
                                                
                                                    <asp:Button ID="btnTransfer" Text="執行交接作業" runat="server" />
                                                </td>
                                            </tr>
                                            
                                        </table>  
                                        
                                    </ContentTemplate>
                                </AjaxToolkit:TabPanel>   

                                <AjaxToolkit:TabPanel ID="TabPanel4" runat="server">
                                    <HeaderTemplate>財務報廢作業</HeaderTemplate>
                                    <ContentTemplate>
                                        <table id="table-data" rules="all">
                                            <tr>
                                                <th>*財物編號：</th>
                                                <td>
                                                    <asp:TextBox ID="txtDisPrNo" runat="server" />
                                                    <asp:Button ID="btnDisGetPrNo" Text="查詢財物" runat="server" />
                                                </td>
                                                </td>
                                                <th>財物名稱：</th>
                                                <td>
                                                    <asp:Label ID="txtDisName" ForeColor="Blue" Font-Size="12pt" Font-Bold="True" runat="server" />
                                                </td>
                                                
                                            </tr>
                                            <tr>
                                                <th>保管者：</th>
                                                <td><asp:Label ID="txtDisWhoKeep" ForeColor="Blue" Font-Size="12pt" Font-Bold="True" runat="server" /></td>
                                                <th>購入日期：</th>
                                                <td><asp:Label ID="txtDisPDate" ForeColor="Blue" Font-Size="12pt" Font-Bold="True" runat="server" /></td>
                                            </tr>
                                            <tr>
                                                <th>報廢日期：</th>
                                                <td><asp:Label ID="txtDisEndDate2" ForeColor="Blue" Font-Size="12pt" Font-Bold="True" runat="server" /></td>
                                                <th>報廢原因：</th>
                                                <td><asp:Label ID="lblDisEndRemark" ForeColor="Blue" Font-Size="12pt" Font-Bold="True" runat="server" /></td>
                                            </tr>
                                            <tr>
                                                <th>*報廢日期：</th>
                                                <td><asp:TextBox ID="txtDisEndDate" Width="80px"  onClick="WdatePicker({dateFmt:'yyy-MM-dd',skin:'whyGreen'})" runat="server" /></td>

                                                <th>*報廢原因：</th>
                                                <td><asp:DropDownList ID="cboDisEndRemark" runat="server" /></td>

                                            </tr>
                                            <tr>
                                                <td colspan="4">
                                                    <asp:Button ID="btnDiscardQuery" Text="查詢當日報廢清單" runat="server" />

                                                    <asp:Button ID="btnDiscard" Text="執行報廢作業" runat="server" />
                                                </td>
                                            </tr>
                                            
                                        </table>  
                                        
                                    </ContentTemplate>
                                </AjaxToolkit:TabPanel> 

                                <AjaxToolkit:TabPanel ID="TabPanel5" runat="server">
                                    <HeaderTemplate>財務報廢作業</HeaderTemplate>
                                    <ContentTemplate>
                                        <table id="table-data" rules="all">
                                            
                                            <tr>
                                                <td>
                                                    <asp:DropDownList ID="cboCanDiscard" runat="server" />
                                                </td>
                                                 <td>
                                                    <asp:Button ID="btnQueryCanDiscard" Text="查詢" runat="server" />
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
