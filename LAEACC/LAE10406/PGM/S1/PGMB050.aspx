<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="PGMB050.aspx.vb" Inherits="LAEACC.PGMB050" %>
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
                        <span>財物編號</span>
                        <asp:TextBox ID="txtQueryPrNo"  Width="100" runat="server" AutoPostBack="true"  />
                        <span>~</span>
                        <asp:TextBox ID="txtQueryPrNo2"  Width="100" runat="server" AutoPostBack="true" />
                        <!--#自動完成-->
                        <AjaxToolkit:AutoCompleteExtender 
                            ID="AutoCompleteExtender1" runat="server" ServicePath="~/active/WebService.asmx"               
                            TargetControlID="txtQueryPrNo"  ServiceMethod="GetPGMBuildKindNo" 
                            MinimumPrefixLength="0"   CompletionInterval="100" CompletionSetCount="12" BehaviorID="_content_AutoCompleteExtender1" DelimiterCharacters="" />
                        <AjaxToolkit:AutoCompleteExtender 
                            ID="AutoCompleteExtender2" runat="server" ServicePath="~/active/WebService.asmx"               
                            TargetControlID="txtQueryPrNo2"  ServiceMethod="GetPGMBuildKindNo"
                            MinimumPrefixLength="0"   CompletionInterval="100" CompletionSetCount="12" BehaviorID="_content_AutoCompleteExtender1" DelimiterCharacters="" />

                    </td>   
                   
                    <td style="text-align:center;">
                        <span>提列日期</span>
                        <asp:TextBox ID="txtQueryDepreciationDate" Width="80px"  onClick="WdatePicker({dateFmt:'yyy-MM-dd',skin:'whyGreen'})" runat="server" />~
                        <asp:TextBox ID="txtQueryDepreciationDate2" Width="80px"  onClick="WdatePicker({dateFmt:'yyy-MM-dd',skin:'whyGreen'})" runat="server" />
                    </td>

                    <td style="text-align:center;">
                        <span>財物名稱</span>
                        <asp:TextBox ID="txtQueryName"  Width="120" runat="server"   />
                    </td>
                    <td style="text-align:center;">
                        <span>備註</span>
                        <asp:TextBox ID="txtQueryRemark"  Width="120" runat="server"   />
                    </td>
                </tr>
                <tr>
                    <td style="text-align:center;">
                        <span>會計科目</span>
                        <asp:TextBox ID="txtQueryAcNo"  Width="120" runat="server"   />~
                        <asp:TextBox ID="txtQueryAcNo2"  Width="120" runat="server"   />
                    </td>
                    <td style="text-align:center;">
                        <span>金額</span>
                        <asp:TextBox ID="txtQueryAmt"  Width="120" runat="server"   />~
                        <asp:TextBox ID="txtQueryAmt2"  Width="120" runat="server"   />
                    </td>
                    <td style="text-align:center;">
                        <span>小計每級會計科目</span>
                        <asp:CheckBox ID="chkIsSumAllGrade"  runat="server" />
                    </td>
                                                          
                    <td style="text-align:center;">
                        <asp:Button ID="btnQuery" Text="搜尋" runat="server" />
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
                            </div>
                            <div style="clear:both; height:5px;"></div> 

                            <!--詳細內容顯示區-->                           
                            <AjaxToolkit:TabContainer ID="TabContainer1" Width="100%" CssClass="Tab" runat="server" ActiveTabIndex="3">
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
                                            <asp:DataGrid ID="DataGridView" Width="2048px" AllowSorting="true" AllowPaging="false" PageSize="100" style="font-size:14px;" CssClass="table table-bordered table-condensed smart-form" runat="server" >
                                                <columns>
                                                    <asp:TemplateColumn HeaderText="管理" HeaderStyle-Width="40" ItemStyle-HorizontalAlign="Center">
                                                        <itemtemplate>                                                                                                            
                                                            <asp:ImageButton ID="Show" AlternateText="查閱" ImageUrl="~/active/images/icon/items/zoom.png" CommandName="btnShow" runat="server" />
                                                            <asp:Label ID="id" Text='<%# Container.DataItem("id").ToString%>' Visible="false" runat="server" />
                                                            <asp:UpdatePanel ID="DataGridView_UpdatePanel" runat="server">                                                                
                                                                <Triggers><asp:AsyncPostBackTrigger ControlID="Show" EventName="Click" /></Triggers>
                                                            </asp:UpdatePanel>
                                                        </itemtemplate>                                                         
                                                    </asp:TemplateColumn>
                                                                            
                                                    <asp:TemplateColumn HeaderText="財物編號" SortExpression="prno" HeaderStyle-Width="100" ItemStyle-HorizontalAlign="left">
                                                        <itemtemplate><asp:Label ID="財物編號" Text='<%# Container.DataItem("prno").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>                                                   
                                                    <asp:TemplateColumn HeaderText="財物名稱" SortExpression="name" HeaderStyle-Width="100" ItemStyle-HorizontalAlign="left">
                                                        <itemtemplate><asp:Label ID="財物名稱" Text='<%# Container.DataItem("name").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="會計科目" SortExpression="acno" HeaderStyle-Width="100" ItemStyle-HorizontalAlign="Center">
                                                        <itemtemplate><asp:Label ID="會計科目" Text='<%# Container.DataItem("acno").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="金額" SortExpression="amt" HeaderStyle-Width="100" ItemStyle-HorizontalAlign="Center">
                                                        <itemtemplate><asp:Label ID="金額" Text='<%# Container.DataItem("amt").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>                                                    
                                                    <asp:TemplateColumn HeaderText="提列日期" SortExpression="pdate" HeaderStyle-Width="100" ItemStyle-HorizontalAlign="Center">
                                                        <itemtemplate><asp:Label ID="提列日期" Text='<%# Container.DataItem("pdate").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="購入日期" SortExpression="purchaseDate" HeaderStyle-Width="100" ItemStyle-HorizontalAlign="Center">
                                                        <itemtemplate><asp:Label ID="購入日期" Text='<%# Container.DataItem("purchaseDate").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="使用年限" SortExpression="useyear" HeaderStyle-Width="100" ItemStyle-HorizontalAlign="Center">
                                                        <itemtemplate><asp:Label ID="使用年限" Text='<%# Container.DataItem("useyear").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="原值" SortExpression="originalAmt" HeaderStyle-Width="100" ItemStyle-HorizontalAlign="Center">
                                                        <itemtemplate><asp:Label ID="原值" Text='<%# Container.DataItem("originalAmt").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>
                                                    
                                                    <asp:TemplateColumn HeaderText="預估殘值" SortExpression="endamt" HeaderStyle-Width="100" ItemStyle-HorizontalAlign="Center">
                                                        <itemtemplate><asp:Label ID="預估殘值" Text='<%# Container.DataItem("endamt").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="增減值" SortExpression="totalAddDel" HeaderStyle-Width="100" ItemStyle-HorizontalAlign="Center">
                                                        <itemtemplate><asp:Label ID="增減值" Text='<%# Container.DataItem("totalAddDel").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="已提折舊" SortExpression="depreciation" HeaderStyle-Width="100" ItemStyle-HorizontalAlign="Center">
                                                        <itemtemplate><asp:Label ID="已提折舊" Text='<%# Container.DataItem("depreciation").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="淨值" SortExpression="netamt" HeaderStyle-Width="100" ItemStyle-HorizontalAlign="Center">
                                                        <itemtemplate><asp:Label ID="淨值" Text='<%# Container.DataItem("netamt").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="保管人" SortExpression="keepempname" HeaderStyle-Width="100" ItemStyle-HorizontalAlign="Center">
                                                        <itemtemplate><asp:Label ID="保管人" Text='<%# Container.DataItem("keepempname").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="保管單位" SortExpression="keepunitname" HeaderStyle-Width="100" ItemStyle-HorizontalAlign="Center">
                                                        <itemtemplate><asp:Label ID="保管單位" Text='<%# Container.DataItem("keepunitname").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="報廢日期" SortExpression="enddate" HeaderStyle-Width="100" ItemStyle-HorizontalAlign="Center">
                                                        <itemtemplate><asp:Label ID="報廢日期" Text='<%# Container.DataItem("enddate").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="報廢原因" SortExpression="endremk" HeaderStyle-Width="100" ItemStyle-HorizontalAlign="Center">
                                                        <itemtemplate><asp:Label ID="報廢原因" Text='<%# Container.DataItem("endremk").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="備註" SortExpression="remark" HeaderStyle-Width="100" ItemStyle-HorizontalAlign="Center">
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
                                                <th>*提列日期：</th>
                                                <td><asp:TextBox ID="txtDepreciationDate" Width="80px"  onClick="WdatePicker({dateFmt:'yyy-MM-dd',skin:'whyGreen'})" runat="server" /></td>

                                                <th>*金額：</th>
                                                <td><asp:TextBox ID="txtAmt" runat="server" /></td>
                                                
                                            </tr>
                                            <tr>
                                                <th>備註：</th>
                                                <td colspan="3"><asp:TextBox ID="txtRemark" CssClass="form-control" Width="400px" TextMode="MultiLine" Height="60px"  runat="server" /></td>
                                            </tr>
                                            <tr>
                                                <td colspan="4">
                                                    <asp:Button ID="btnDepreciationQuery" Text="查詢折舊紀錄" runat="server" />
                                                </td>
                                            </tr>
                                            
                                        </table>                                          
                                    </ContentTemplate>
                                </AjaxToolkit:TabPanel>  
                                
                                <AjaxToolkit:TabPanel ID="TabPanel3" runat="server">
                                    <HeaderTemplate>自動提列折舊</HeaderTemplate>
                                    <ContentTemplate>
                                        <table id="table-data" rules="all">
                                            
                                            <tr>
                                                <th>*提列資料</th>
                                                <td>
                                                    <asp:DropDownList ID="cboAutoDepreciationData" runat="server" />
                                                </td>
                                                <th>*提列方式</th>
                                                <td>
                                                    <asp:DropDownList ID="cboAutoDepreciationMode" runat="server" />
                                                    <br>
                                                    同一會計年度請勿使用不同的方式來提列
                                                </td>
                                                <th>*提列日期：</th>
                                                <td><asp:TextBox ID="txtAutoDepreciationDate" Width="100px"  onClick="WdatePicker({dateFmt:'yyy-MM-dd',skin:'whyGreen'})" runat="server" /></td>

                                                 <td>
                                                    <asp:Button ID="btnAutoDepreciation" Text="執行" runat="server" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td colspan="7">
                                                    <ul>
                                                        <li>自動提列的規則如下</li>
                                                        <li>1.使用年限 = 0 者，不予提列</li>
                                                        <li>2.已經報廢者，不予提列</li>
                                                        <li>3.會計科目編號在 131~136 之間才予以提列</li>
                                                        <li>4.按年提列者，財物的購入年份要小於提列年份才予以提列，也就是購入當年不提列</li>
                                                        <li>5.按月提列者，財物的購入年月要小於或等於提列年月才予以提列，若等於時，當月15日以前購入者才須提列</li>
                                                        <li>6.按年提列者，同一財物若同年度已經提列過就不再提列折舊</li>
                                                        <li>7.按月提列者，同一財物若同年同月份已經提列過就不再提列折舊</li>
                                                        <li>8.剩餘折舊值小於或等於 0 者不提列折舊 (剩餘折舊值 = 原值 - 殘值 + 增減值 - 已提折舊值)</li>

                                                    </ul>
                                                </td>
                                            </tr>
                                            
                                        </table>  
                                        
                                    </ContentTemplate>
                                </AjaxToolkit:TabPanel>     
                                
                                 <AjaxToolkit:TabPanel ID="TabPanel4" runat="server">
                                    <HeaderTemplate>預提下年度折舊</HeaderTemplate>
                                    <ContentTemplate>
                                        <table id="table-data" rules="all">
                                           <tr>
                                                <td colspan="4">
                                                    預提下年度折舊一律按年度提列，且一般財物和建物會同時提列，
                                                    請注意必須確實提列完今年的折舊之後再執行此功能才可得到準確之數據，
                                                    預提折舊之結果是放在暫存資料表中，不會影響到系統本身的折舊資料
                                                </td>
                                            </tr>
                                            
                                            <tr>
                                                <th>下個年度是</th>
                                                 <td><asp:Label ID="lblNextYear" ForeColor="Blue" Font-Size="12pt" Font-Bold="True" runat="server" /></td>
                                                 <td>
                                                    <asp:Button ID="btnPreAutoDepreciation" Text="執行" runat="server" />
                                                </td>
                                                <td>
                                                    <asp:Button ID="btnCopyPreDepreciation" Enabled="false" Text="複製預提折舊資料" runat="server" />
                                                </td>
                                                <asp:UpdatePanel ID="UpdatePanel3" runat="server">
                                                    <ContentTemplate></ContentTemplate>
                                                    <Triggers>
                                                        <asp:PostBackTrigger ControlID="btnCopyPreDepreciation" />
                                                    </Triggers>
                                                </asp:UpdatePanel> 
                                            </tr>
                                        </table>  
                                        <asp:Label ID="Label2" ForeColor="Red" Font-Size="14pt" Font-Bold="True" Text="0" runat="server" />
                                            <asp:DataGrid ID="DataGrid1" Width="2048px"  style="font-size:14px;" CssClass="table table-bordered table-condensed smart-form" runat="server" >
                                                <Columns>

                                                </Columns>
                                            </asp:DataGrid>
                                        
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
