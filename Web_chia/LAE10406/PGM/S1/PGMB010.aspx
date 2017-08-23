<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="PGMB010.aspx.vb" Inherits="LAEACC.PGMB010" %>
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
                        <span>編號</span>
                        <asp:TextBox ID="txtQueryKindNo"  Width="200" runat="server" AutoPostBack="true"  />
                        <span>~</span>
                        <asp:TextBox ID="txtQueryKindNo2"  Width="200" runat="server" AutoPostBack="true" />
                        <!--#自動完成-->
                        <AjaxToolkit:AutoCompleteExtender 
                            ID="AutoCompleteExtender1" runat="server" ServicePath="~/active/WebService.asmx"               
                            TargetControlID="txtQueryKindNo"  ServiceMethod="GetPGMKindNo" 
                            MinimumPrefixLength="0"   CompletionInterval="100" CompletionSetCount="12" BehaviorID="_content_AutoCompleteExtender1" DelimiterCharacters="" />
                        <AjaxToolkit:AutoCompleteExtender 
                            ID="AutoCompleteExtender2" runat="server" ServicePath="~/active/WebService.asmx"               
                            TargetControlID="txtQueryKindNo2"  ServiceMethod="GetPGMKindNo"
                            MinimumPrefixLength="0"   CompletionInterval="100" CompletionSetCount="12" BehaviorID="_content_AutoCompleteExtender1" DelimiterCharacters="" />

                    </td>   
                    <td style="text-align:center;">
                        單位<asp:DropDownList ID="cboQueryUnit" runat="server" />
                    </td>
                    <td style="text-align:center;">
                        使用年限<asp:DropDownList ID="cboQueryUseyear" runat="server" />
                    </td>
                    <td style="text-align:center;">
                        名稱<asp:TextBox ID="txtQueryName"  Width="100" runat="server"  />
                    </td>
                    <td style="text-align:center;">
                        材質<asp:DropDownList ID="cboQueryMaterial" runat="server" />
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
                                                            <asp:Label ID="id" Text='<%# Container.DataItem("KindNo").ToString%>' Visible="false" runat="server" />
                                                            <asp:UpdatePanel ID="DataGridView_UpdatePanel" runat="server">                                                                
                                                                <Triggers><asp:AsyncPostBackTrigger ControlID="Show" EventName="Click" /></Triggers>
                                                            </asp:UpdatePanel>
                                                        </itemtemplate>                                                         
                                                    </asp:TemplateColumn>
                                                                            
                                                    <asp:TemplateColumn HeaderText="財務編號" SortExpression="KindNo" HeaderStyle-Width="10" ItemStyle-HorizontalAlign="left">
                                                        <itemtemplate><asp:Label ID="財務編號" Text='<%# Container.DataItem("KindNo").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>                                                   
                                                    <asp:TemplateColumn HeaderText="財務名稱" SortExpression="Name" HeaderStyle-Width="20" ItemStyle-HorizontalAlign="left">
                                                        <itemtemplate><asp:Label ID="財務名稱" Text='<%# Container.DataItem("Name").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>                                                    
                                                    <asp:TemplateColumn HeaderText="單位" SortExpression="Unit" HeaderStyle-Width="10" ItemStyle-HorizontalAlign="Center">
                                                        <itemtemplate><asp:Label ID="單位" Text='<%# Container.DataItem("Unit").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="材質" SortExpression="Material" HeaderStyle-Width="20" ItemStyle-HorizontalAlign="Center">
                                                        <itemtemplate><asp:Label ID="材質" Text='<%# Container.DataItem("Material").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="使用年限" SortExpression="UseYear" HeaderStyle-Width="10" ItemStyle-HorizontalAlign="Center">
                                                        <itemtemplate><asp:Label ID="使用年限" Text='<%# Container.DataItem("UseYear").ToString%>' runat="server" /></itemtemplate>                                                       
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
                                                <th>編號：</th>
                                                <td>
                                                    <asp:TextBox ID="txtKindNo" CssClass="form-control" Width="100px" runat="server" />
                                                    <asp:Label ID="lblkey" ForeColor="Blue" Font-Size="12pt" Font-Bold="True" Visible="False" runat="server" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <th>名稱：</th>
                                                <td><asp:TextBox ID="txtName" CssClass="form-control" Width="200px" runat="server" /></td>
                                            </tr>
                                            <tr>
                                                <th>單位：</th>
                                                <td><asp:DropDownList ID="cboUnit" runat="server" /></td>
                                            </tr>
                                            <tr>
                                                <th>材質：</th>
                                                <td><asp:DropDownList ID="cboMaterial" runat="server" /></td>
                                            </tr>
                                            <tr>
                                                <th>使用年限：</th>
                                                <td><asp:DropDownList ID="cboUseYear" runat="server" /></td>
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
