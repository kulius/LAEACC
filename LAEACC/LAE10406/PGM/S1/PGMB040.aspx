<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="PGMB040.aspx.vb" Inherits="LAEACC.PGMB040" %>
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
                        <asp:TextBox ID="txtQueryPrNo"  Width="200" runat="server" AutoPostBack="true"  />
                        <span>~</span>
                        <asp:TextBox ID="txtQueryPrNo2"  Width="200" runat="server" AutoPostBack="true" />
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
                        類別<asp:DropDownList ID="cboQueryKind" runat="server" />
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
                                            <asp:DataGrid ID="DataGridView" Width="2048px" AllowSorting="true" AllowPaging="false" PageSize="100" style="font-size:14px;" CssClass="table table-bordered table-condensed smart-form" runat="server" >
                                                <columns>
                                                    <asp:TemplateColumn HeaderText="管理" HeaderStyle-Width="40" ItemStyle-HorizontalAlign="Center">
                                                        <itemtemplate>                                                                                                            
                                                            <asp:ImageButton ID="Show" AlternateText="查閱" ImageUrl="~/active/images/icon/items/zoom.png" CommandName="btnShow" runat="server" />
                                                            <asp:Label ID="id" Text='<%# Container.DataItem("prno").ToString%>' Visible="false" runat="server" />
                                                            <asp:UpdatePanel ID="DataGridView_UpdatePanel" runat="server">                                                                
                                                                <Triggers><asp:AsyncPostBackTrigger ControlID="Show" EventName="Click" /></Triggers>
                                                            </asp:UpdatePanel>
                                                        </itemtemplate>                                                         
                                                    </asp:TemplateColumn>
                                                                            
                                                    <asp:TemplateColumn HeaderText="財物編號" SortExpression="prno" HeaderStyle-Width="100" ItemStyle-HorizontalAlign="left">
                                                        <itemtemplate><asp:Label ID="財物編號" Text='<%# Container.DataItem("prno").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>                                                   
                                                    <asp:TemplateColumn HeaderText="權狀字號" SortExpression="builno" HeaderStyle-Width="100" ItemStyle-HorizontalAlign="left">
                                                        <itemtemplate><asp:Label ID="權狀字號" Text='<%# Container.DataItem("builno").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>                                                    
                                                    <asp:TemplateColumn HeaderText="基地地段" SortExpression="land_no1" HeaderStyle-Width="100" ItemStyle-HorizontalAlign="Center">
                                                        <itemtemplate><asp:Label ID="基地地段" Text='<%# Container.DataItem("land_no1").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="基地地號" SortExpression="land_no2" HeaderStyle-Width="100" ItemStyle-HorizontalAlign="Center">
                                                        <itemtemplate><asp:Label ID="基地地號" Text='<%# Container.DataItem("land_no2").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="基地面積" SortExpression="land_area" HeaderStyle-Width="100" ItemStyle-HorizontalAlign="Center">
                                                        <itemtemplate><asp:Label ID="基地面積" Text='<%# Container.DataItem("land_area").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="持分" SortExpression="part" HeaderStyle-Width="50" ItemStyle-HorizontalAlign="Center">
                                                        <itemtemplate><asp:Label ID="持分" Text='<%# Container.DataItem("part").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="主物面積" SortExpression="area1" HeaderStyle-Width="100" ItemStyle-HorizontalAlign="Center">
                                                        <itemtemplate><asp:Label ID="主物面積" Text='<%# Container.DataItem("area1").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="附屬建物面積" SortExpression="area2" HeaderStyle-Width="150" ItemStyle-HorizontalAlign="Center">
                                                        <itemtemplate><asp:Label ID="附屬建物面積" Text='<%# Container.DataItem("area2").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="房屋坪數" SortExpression="area3" HeaderStyle-Width="100" ItemStyle-HorizontalAlign="Center">
                                                        <itemtemplate><asp:Label ID="房屋坪數" Text='<%# Container.DataItem("area3").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="房屋構造" SortExpression="stru" HeaderStyle-Width="100" ItemStyle-HorizontalAlign="Center">
                                                        <itemtemplate><asp:Label ID="房屋構造" Text='<%# Container.DataItem("stru").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="房屋稅籍牌號" SortExpression="tax" HeaderStyle-Width="150" ItemStyle-HorizontalAlign="Center">
                                                        <itemtemplate><asp:Label ID="房屋稅籍牌號" Text='<%# Container.DataItem("tax").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="房屋地址" SortExpression="addr" HeaderStyle-Width="200" ItemStyle-HorizontalAlign="Center">
                                                        <itemtemplate><asp:Label ID="房屋地址" Text='<%# Container.DataItem("addr").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="登記日期" SortExpression="pdate" HeaderStyle-Width="100" ItemStyle-HorizontalAlign="Center">
                                                        <itemtemplate><asp:Label ID="登記日期" Text='<%# Container.DataItem("pdate").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="原值" SortExpression="amt" HeaderStyle-Width="100" ItemStyle-HorizontalAlign="Center">
                                                        <itemtemplate><asp:Label ID="原值" Text='<%# Container.DataItem("amt").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="預估殘值" SortExpression="endamt" HeaderStyle-Width="100" ItemStyle-HorizontalAlign="Center">
                                                        <itemtemplate><asp:Label ID="預估殘值" Text='<%# Container.DataItem("endamt").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="增減值" SortExpression="depreciation" HeaderStyle-Width="100" ItemStyle-HorizontalAlign="Center">
                                                        <itemtemplate><asp:Label ID="增減值" Text='<%# Container.DataItem("depreciation").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="已提折舊" SortExpression="endamt" HeaderStyle-Width="100" ItemStyle-HorizontalAlign="Center">
                                                        <itemtemplate><asp:Label ID="已提折舊" Text='<%# Container.DataItem("endamt").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="淨值" SortExpression="netamt" HeaderStyle-Width="100" ItemStyle-HorizontalAlign="Center">
                                                        <itemtemplate><asp:Label ID="淨值" Text='<%# Container.DataItem("netamt").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="使用年限" SortExpression="useyear" HeaderStyle-Width="100" ItemStyle-HorizontalAlign="Center">
                                                        <itemtemplate><asp:Label ID="使用年限" Text='<%# Container.DataItem("useyear").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="房屋類別" SortExpression="kind" HeaderStyle-Width="100" ItemStyle-HorizontalAlign="Center">
                                                        <itemtemplate><asp:Label ID="房屋類別" Text='<%# Container.DataItem("kind").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="使用現況" SortExpression="usemode" HeaderStyle-Width="100" ItemStyle-HorizontalAlign="Center">
                                                        <itemtemplate><asp:Label ID="使用現況" Text='<%# Container.DataItem("usemode").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="會計科目" SortExpression="acno" HeaderStyle-Width="100" ItemStyle-HorizontalAlign="Center">
                                                        <itemtemplate><asp:Label ID="會計科目" Text='<%# Container.DataItem("acno").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="報廢日期" SortExpression="enddate" HeaderStyle-Width="100" ItemStyle-HorizontalAlign="Center">
                                                        <itemtemplate><asp:Label ID="報廢日期" Text='<%# Container.DataItem("enddate").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="借用單位" SortExpression="borrow" HeaderStyle-Width="100" ItemStyle-HorizontalAlign="Center">
                                                        <itemtemplate><asp:Label ID="借用單位" Text='<%# Container.DataItem("borrow").ToString%>' runat="server" /></itemtemplate>                                                       
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
                                                    <asp:TextBox ID="txtPrNo" CssClass="form-control" Width="200px" runat="server" />
                                                    <asp:Label ID="lblkey" ForeColor="Blue" Font-Size="12pt" Font-Bold="True" Visible="False" runat="server" />
                                                    <AjaxToolkit:AutoCompleteExtender 
                                                        ID="AutoCompleteExtender3" runat="server" ServicePath="~/active/WebService.asmx"               
                                                        TargetControlID="txtPrNo"  ServiceMethod="GetPGMBuildKindNo"
                                                        MinimumPrefixLength="0"   CompletionInterval="100" CompletionSetCount="12" BehaviorID="_content_AutoCompleteExtender1" DelimiterCharacters="" />
                                                    <asp:UpdatePanel ID="UpdatePanel3" runat="server">
                                                        <ContentTemplate></ContentTemplate>
                                                        <Triggers>
                                                            <asp:PostBackTrigger ControlID="txtPrNo" />
                                                        </Triggers>
                                                    </asp:UpdatePanel> 
                                                </td>
                                                <th>*序號：</th>
                                                <td>
                                                    <asp:TextBox ID="txtNo"  Width="100px" runat="server" />
                                                    <asp:Button ID="btnGetPrNo" Text="取得序號" runat="server" />
                                                </td>
                                                <th>權狀字號：</th>
                                                <td>
                                                    <asp:TextBox ID="txtBuilNo" CssClass="form-control" Width="100px" runat="server" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <th>基地地段：</th>
                                                <td><asp:TextBox ID="txtLandNo1" CssClass="form-control" runat="server" /></td>
                                                <th>基地地號：</th>
                                                <td><asp:TextBox ID="txtLandNo2" CssClass="form-control"  runat="server" /></td>
                                                <th>*基地面積：</th>
                                                <td><asp:TextBox ID="txtLandArea" CssClass="form-control"  runat="server" /></td>
                                            </tr>
                                            <tr>
                                                <th>會計科目：</th>
                                                <td><asp:TextBox ID="txtAcNo" CssClass="form-control"  runat="server" /></td>
                                                <th>*原值：</th>
                                                <td><asp:TextBox ID="txtAmt" CssClass="form-control" runat="server" /></td>
                                                <th>*預估殘值：</th>
                                                <td><asp:TextBox ID="txtEndAmt" CssClass="form-control"  runat="server" /></td>
                                            </tr>
                                            <tr>
                                                <th>持分：</th>
                                                <td><asp:TextBox ID="txtPart" CssClass="form-control" Width="200px" runat="server" /></td>
                                                <th>*地址：</th>
                                                <td colspan="3"><asp:TextBox ID="txtAddr" CssClass="form-control" Width="200px" runat="server" /></td>

                                            </tr>
                                            <tr>
                                                <th>*房屋構造：</th>
                                                <td><asp:TextBox ID="txtStru" CssClass="form-control"  runat="server" /></td>
                                                <th>*房屋坪數：</th>
                                                <td><asp:TextBox ID="txtArea3" CssClass="form-control"  runat="server" /></td>
                                                <th>房屋稅籍牌號：</th>
                                                <td><asp:TextBox ID="txtTax" CssClass="form-control"  runat="server" /></td>
                                            </tr>
                                            <tr>
                                                <th>*登記日期：</th>
                                                <td><asp:TextBox ID="txtPDate" Width="80px"  onClick="WdatePicker({dateFmt:'yyy-MM-dd',skin:'whyGreen'})" runat="server" /></td>
                                                <th>*使用年限：</th>
                                                <td><asp:TextBox ID="txtUseYear" CssClass="form-control"  runat="server" /></td>
                                                <th>*使用現況：</th>
                                                <td><asp:TextBox ID="txtUseMode" CssClass="form-control"  runat="server" /></td>
                                            </tr>
                                            <tr>
                                                <th>報廢日期：</th>
                                                <td><asp:TextBox ID="txtEndDate" Width="80px"  onClick="WdatePicker({dateFmt:'yyy-MM-dd',skin:'whyGreen'})" runat="server" /></td>
                                                <th>借用單位：</th>
                                                <td><asp:TextBox ID="txtBorrow" CssClass="form-control" Width="200px" runat="server" /></td>
                                                <th>*房屋類別：</th>
                                                <td><asp:DropDownList ID="cboKind" runat="server" AutoPostBack="true"/></td>
                                            </tr>
                                            <tr>
                                                <th>備註：</th>
                                                <td><asp:TextBox ID="txtRemark" CssClass="form-control" Width="200px" runat="server" /></td>
                                                <th>*主物面積(ha)：</th>
                                                <td><asp:TextBox ID="txtArea1" CssClass="form-control" Width="200px" runat="server" /></td>
                                                <th>*附屬建物面積(ha)：</th>
                                                <td><asp:TextBox ID="txtArea2" CssClass="form-control" Width="200px" runat="server" /></td>
                                            </tr>  
                                            <tr>
                                                <th>淨值：</th>
                                                <td colspan="3"><asp:Label ID="lblNetAmt" ForeColor="Blue" Font-Size="12pt" Font-Bold="True" runat="server" /></td>
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
