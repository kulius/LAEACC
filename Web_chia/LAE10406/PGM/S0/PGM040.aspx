<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="PGM040.aspx.vb" Inherits="LAEACC.PGM040" %>
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
                        會計科目
                    </td>
                    <td style="text-align:center;">
                        <asp:TextBox ID="txtQueryAcNo"  Width="120" runat="server"   />
                    </td>
                    <td style="text-align:center;">
                        <asp:TextBox ID="txtQueryAcNo2"  Width="120" runat="server" />
                    </td> 
                    <td style="text-align:center;">
                        財物名稱
                    </td>
                    <td style="text-align:center;">
                        <asp:TextBox ID="txtQueryName"  Width="100" runat="server"  />
                    </td>
                    <td style="text-align:center;">
                        <asp:Button ID="btnQuery" Text="查詢" runat="server" />
                    </td>

                </tr>
                <tr>
                    <td style="text-align:center;">
                        增減日期
                    </td>
                    <td style="text-align:center;">
                       <asp:TextBox ID="txtQueryAddDelDate" Width="80px"  onClick="WdatePicker({dateFmt:'yyy-MM-dd',skin:'whyGreen'})" runat="server" />
                    </td>
                    <td style="text-align:center;">
                        <asp:TextBox ID="txtQueryAddDelDate2" Width="80px"  onClick="WdatePicker({dateFmt:'yyy-MM-dd',skin:'whyGreen'})" runat="server" />
                    </td>
                    <td style="text-align:center;">
                       金額
                    </td>
                    <td style="text-align:center;">
                        <asp:TextBox ID="txtQueryAmt"  Width="80" runat="server"   />
                    </td>
                    <td style="text-align:center;">
                        <asp:TextBox ID="txtQueryAmt2"  Width="80" runat="server" />
                    </td>
                    <td style="text-align:center;">
                        規格
                    </td>
                    <td style="text-align:center;">
                        <asp:TextBox ID="txtQueryRemark"  Width="100" runat="server"  />
                    </td> 
                    <td style="text-align:center;">
                        <asp:Button ID="btnQueryClear" Text="清除" runat="server" />
                    </td>
                </tr>
                
            </table>
   
            <!--主項目區-->
            <div style="margin: 10px 0px 0px 10px;">
                <section id="widget-grid" style="width:98%;">
                    <div class="row">
                        <article class="col-sm-12 col-md-12 col-lg-12">
                            <!--控制項

                            -->
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
                                            <asp:DataGrid ID="DataGridView"  AllowSorting="true" AllowPaging="false" PageSize="100" style="font-size:14px;" CssClass="table table-bordered table-condensed smart-form" runat="server" >
                                                <columns>
                                                    <asp:TemplateColumn HeaderText="管理" HeaderStyle-Width="40" ItemStyle-HorizontalAlign="Center">
                                                        <itemtemplate>                                                                                                            
                                                            <asp:ImageButton ID="Show" AlternateText="查閱" ImageUrl="~/active/images/icon/items/zoom.png" CommandName="btnShow" runat="server" />
                                                            <asp:Label ID="id" Text='<%# Container.DataItem("ID").ToString%>' Visible="false" runat="server" />
                                                            <asp:UpdatePanel ID="DataGridView_UpdatePanel" runat="server">                                                                
                                                                <Triggers><asp:AsyncPostBackTrigger ControlID="Show" EventName="Click" /></Triggers>
                                                            </asp:UpdatePanel>
                                                        </itemtemplate>                                                         
                                                    </asp:TemplateColumn>
                                                                            
                                                    <asp:TemplateColumn HeaderText="財務編號" SortExpression="prno" HeaderStyle-Width="10%" ItemStyle-HorizontalAlign="left">
                                                        <itemtemplate><asp:Label ID="財務編號" Text='<%# Container.DataItem("prno").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>                                                   
                                                    <asp:TemplateColumn HeaderText="財務名稱" SortExpression="Name" HeaderStyle-Width="10%" ItemStyle-HorizontalAlign="left">
                                                        <itemtemplate><asp:Label ID="財務名稱" Text='<%# Container.DataItem("name").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>                                                    
                                                    <asp:TemplateColumn HeaderText="增減日期" SortExpression="pdate" HeaderStyle-Width="10%" ItemStyle-HorizontalAlign="Center">
                                                        <itemtemplate><asp:Label ID="增減日期" Text='<%# Container.DataItem("pdate").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="金額" SortExpression="amt" HeaderStyle-Width="10%" ItemStyle-HorizontalAlign="Center">
                                                        <itemtemplate><asp:Label ID="金額" Text='<%# Container.DataItem("amt").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="會計科目" SortExpression="acno" HeaderStyle-Width="10%" ItemStyle-HorizontalAlign="Center">
                                                        <itemtemplate><asp:Label ID="會計科目" Text='<%# Container.DataItem("acno").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="購入日期" SortExpression="purchaseDate" HeaderStyle-Width="10%" ItemStyle-HorizontalAlign="Center">
                                                        <itemtemplate><asp:Label ID="購入日期" Text='<%# Container.DataItem("purchaseDate").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="使用年限" SortExpression="UseYear" HeaderStyle-Width="10%" ItemStyle-HorizontalAlign="Center">
                                                        <itemtemplate><asp:Label ID="使用年限" Text='<%# Container.DataItem("UseYear").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="原值" SortExpression="originalAmt" HeaderStyle-Width="10%" ItemStyle-HorizontalAlign="Center">
                                                        <itemtemplate><asp:Label ID="原值" Text='<%# Container.DataItem("originalAmt").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="預估殘值" SortExpression="endamt" HeaderStyle-Width="10%" ItemStyle-HorizontalAlign="Center">
                                                        <itemtemplate><asp:Label ID="預估殘值" Text='<%# Container.DataItem("endamt").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="增減值" SortExpression="totalAddDel" HeaderStyle-Width="10%" ItemStyle-HorizontalAlign="Center">
                                                        <itemtemplate><asp:Label ID="增減值" Text='<%# Container.DataItem("totalAddDel").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="己提折舊" SortExpression="depreciation" HeaderStyle-Width="10%" ItemStyle-HorizontalAlign="Center">
                                                        <itemtemplate><asp:Label ID="己提折舊" Text='<%# Container.DataItem("depreciation").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="淨值" SortExpression="NetAmt" HeaderStyle-Width="10%" ItemStyle-HorizontalAlign="Center">
                                                        <itemtemplate><asp:Label ID="淨值" Text='<%# Container.DataItem("NetAmt").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="保管人" SortExpression="keepEmpName" HeaderStyle-Width="10%" ItemStyle-HorizontalAlign="Center">
                                                        <itemtemplate><asp:Label ID="保管人" Text='<%# Container.DataItem("keepEmpName").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="保管單位" SortExpression="keepUnitName" HeaderStyle-Width="10%" ItemStyle-HorizontalAlign="Center">
                                                        <itemtemplate><asp:Label ID="保管單位" Text='<%# Container.DataItem("keepUnitName").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="報廢日期" SortExpression="enddate" HeaderStyle-Width="10%" ItemStyle-HorizontalAlign="Center">
                                                        <itemtemplate><asp:Label ID="報廢日期" Text='<%# Container.DataItem("enddate").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="報廢原因" SortExpression="endremk" HeaderStyle-Width="10%" ItemStyle-HorizontalAlign="Center">
                                                        <itemtemplate><asp:Label ID="報廢原因" Text='<%# Container.DataItem("endremk").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="備註" SortExpression="remark" HeaderStyle-Width="10%" ItemStyle-HorizontalAlign="Center">
                                                        <itemtemplate><asp:Label ID="備註" Text='<%# Container.DataItem("remark").ToString%>' runat="server" /></itemtemplate>                                                       
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
                                                <th>*財物編號：</th>
                                                <td>
                                                    <asp:TextBox ID="txtPrNo" runat="server" />
                                                    <asp:Label ID="lblkey" ForeColor="Blue" Font-Size="12pt" Font-Bold="True" Visible="False" runat="server" />
                                                    <asp:Button ID="btnGetPrNo" Text="查詢財物" runat="server" />
                                                </td>
                                                <th>財物名稱：</th>
                                                <td>
                                                    <asp:Label ID="txtName" ForeColor="Blue" Font-Size="12pt" Font-Bold="True" runat="server" />
                                                </td>
                                                
                                            </tr>
                                            <tr>
                                                <th>保管者：</th>
                                                <td><asp:Label ID="txtWhoKeep" ForeColor="Blue" Font-Size="12pt" Font-Bold="True" runat="server" /></td>
                                                <th>購入日期：</th>
                                                <td><asp:Label ID="txtPurchaseDate" ForeColor="Blue" Font-Size="12pt" Font-Bold="True" runat="server" /></td>
                                            </tr>
                                            <tr>
                                                <th>原值：</th>
                                                <td><asp:Label ID="txtOriginalAmt" ForeColor="Blue" Font-Size="12pt" Font-Bold="True" runat="server" /></td>
                                                <th>預估殘值：</th>
                                                <td><asp:Label ID="txtEndAmt" ForeColor="Blue" Font-Size="12pt" Font-Bold="True" runat="server" /></td>
                                            </tr>
                                            <tr>
                                                <th>增減值：</th>
                                                <td><asp:Label ID="txtTotalAddDel" ForeColor="Blue" Font-Size="12pt" Font-Bold="True" runat="server" /></td>
                                                <th>淨值：</th>
                                                <td><asp:Label ID="txtNetAmt" ForeColor="Blue" Font-Size="12pt" Font-Bold="True" runat="server" /></td>
                                            </tr>
                                            <tr>
                                                <th>已提折舊：</th>
                                                <td><asp:Label ID="txtDepreciation" ForeColor="Blue" Font-Size="12pt" Font-Bold="True" runat="server" /></td>
                                                <th>使用年限：</th>
                                                <td><asp:Label ID="txtUseYear" ForeColor="Blue" Font-Size="12pt" Font-Bold="True" runat="server" /></td>
                                            </tr>
                                            <tr>
                                                <th>報廢日期：</th>
                                                <td><asp:Label ID="txtEndDate" ForeColor="Blue" Font-Size="12pt" Font-Bold="True" runat="server" /></td>
                                                <th>報廢原因：</th>
                                                <td><asp:Label ID="lblEndRemark" ForeColor="Blue" Font-Size="12pt" Font-Bold="True" runat="server" /></td>
                                            </tr>

                                            <tr>
                                                <th>*增減日期：</th>
                                                <td><asp:TextBox ID="txtAddDelDate" Width="80px"  onClick="WdatePicker({dateFmt:'yyy-MM-dd',skin:'whyGreen'})" runat="server" /></td>

                                                <th>*金額：</th>
                                                <td><asp:TextBox ID="txtAmt" runat="server" /></td>
                                                
                                            </tr>
                                            <tr>
                                                <th>備註：</th>
                                                <td colspan="3"><asp:TextBox ID="txtRemark" CssClass="form-control" Width="400px" TextMode="MultiLine" Height="60px"  runat="server" /></td>
                                            </tr>
                                            <tr>
                                                <td colspan="4">
                                                    <asp:Button ID="btnAddDelQuery" Text="查詢增減紀錄" runat="server" />
                                                
                                                    <asp:Button ID="btnAddReviseOK" Text="確定新增" runat="server" />
                                                    <asp:Button ID="btnDeleteOK" Text="刪除" runat="server" />
                                                    <asp:Button ID="btnAddReviseCancel" Text="放棄" runat="server" />
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
