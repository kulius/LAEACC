<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="SYSUSER.aspx.vb" EnableEventValidation="false" Inherits="LAEACC.SYSUSER" %>
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
                            <div class="widget-body form-horizontal" style="font-size:16px; margin:10px 0px 10px 0px;" >
                                <fieldset>
                                    <table id="table-data" rules="all">
                                        <tr>
                                            <th >所屬單位：</th>
                                            <td >
                                                <asp:DropDownList ID="cbounit_id" CssClass="form-control td-left" Width="40%" runat="server" />  
                                                <asp:Button ID="btnS" Text="查詢" CssClass="btn btn-warning td-right" runat="server" />  
                                            </td>
                                            <th >匯入使用者及所屬單位：</th>
                                            <td><asp:Button ID="BtnImport" Text="匯入" CssClass="btn btn-primary td-left" runat="server" /></td>
                                        </tr>
                                    </table>                                    
                                </fieldset>
                            </div>
                            <!--控制項-->
                            <div style="margin:5px 0px 5px 0px;">
                                <uc1:UCBase ID="UCBase1" runat="server" />
                            </div>
                            <div style="clear:both; height:5px;"></div>

                            <!--詳細內容顯示區-->                           
                            <AjaxToolkit:TabContainer ID="TabContainer1" Width="100%" CssClass="Tab" runat="server" ActiveTabIndex="0">
                                <AjaxToolkit:TabPanel ID="TabPanel1" runat="server">
                                    <HeaderTemplate>多筆瀏灠</HeaderTemplate>
                                    <ContentTemplate>
                                        <div class="table-responsive">
                                            &nbsp;&nbsp;
                                            <asp:Label ID="Label1" ForeColor="Red" Font-Size="14" Font-Bold="true" Text="0" runat="server" />
                                            <div style="float:right; padding-right:10px;">
                                                共<asp:Label ID="lbl_GrdCount" ForeColor="Red" Font-Size="14" Font-Bold="true" Text="0" runat="server" />筆符合&nbsp;                                                
                                                <asp:Label ID="lbl_sort" runat="server" />
                                            </div>
                                            <asp:DataGrid ID="DataGridView" Width="100%" AllowSorting="True" AllowPaging="True" style="font-size:14px;" CssClass="table table-bordered table-condensed smart-form" runat="server" >
                                                <columns>
                                                    <asp:TemplateColumn HeaderText="管理" HeaderStyle-Width="40" ItemStyle-HorizontalAlign="Center">
                                                        <itemtemplate>                                                                                                            
                                                            <asp:ImageButton ID="Show" AlternateText="查閱" ImageUrl="~/active/images/icon/items/zoom.png" CommandName="btnShow" runat="server" />
                                                            <asp:Label ID="id" Text='<%# Container.DataItem("user_id").ToString%>' Visible="false" runat="server" />
                                                            <asp:UpdatePanel ID="DataGridView_UpdatePanel" runat="server">                                                                
                                                                <Triggers><asp:AsyncPostBackTrigger ControlID="Show" EventName="Click" /></Triggers>
                                                            </asp:UpdatePanel>
                                                        </itemtemplate>                                                         
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="使用者代號" HeaderStyle-Width="40" ItemStyle-HorizontalAlign="left">
                                                        <itemtemplate><asp:Label ID="使用者代號" Text='<%# Container.DataItem("user_id").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>                                                                            
                                                    <asp:TemplateColumn HeaderText="使用者名稱" HeaderStyle-Width="40" ItemStyle-HorizontalAlign="Center">
                                                        <itemtemplate><asp:Label ID="使用者名稱" Text='<%# Container.DataItem("name").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>                                                   
                                                    <asp:TemplateColumn HeaderText="職員工編號" HeaderStyle-Width="45" ItemStyle-HorizontalAlign="left">
                                                        <itemtemplate><asp:Label ID="職員工編號" Text='<%# Container.DataItem("employee_id").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>                                                    
                                                    <asp:TemplateColumn HeaderText="所屬單位" HeaderStyle-Width="40">
                                                        <itemtemplate><asp:Label ID="所屬單位" Text='<%# Container.DataItem("unit_id").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="單位名稱" HeaderStyle-Width="40">
                                                       <itemtemplate><asp:Label ID="單位名稱" Text='<%# Container.DataItem("unit_name").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="密碼" HeaderStyle-Width="40">
                                                        <itemtemplate><asp:Label ID="密碼" Text='<%# Container.DataItem("password").ToString%>' runat="server" /></itemtemplate>                                                       
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
                                                <th>使用者代號：</th>
                                                <td>
                                                    <asp:TextBox ID="txtuser_id" CssClass="form-control" Width="120" runat="server" />
                                                    <asp:Label ID="lblkey" ForeColor="Blue" Font-Size="12pt" Font-Bold="True" Visible="False" runat="server" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <th>使用者名稱：</th>
                                                <td><asp:TextBox ID="txtname" CssClass="form-control" Width="200" runat="server" /></td>
                                            </tr>
                                            <tr>
                                                <th>職員工編號：</th>
                                                <td><asp:TextBox ID="txtemployee_id" CssClass="form-control" Width="80" runat="server" /></td>
                                            </tr>
                                            <tr>
                                                <th>所屬單位：</th>
                                                <td><asp:DropDownList ID="txtunit_id" CssClass="form-control" runat="server" /></td>
                                            </tr>
                                            <tr>
                                                <th>密碼：</th>
                                                <td><asp:TextBox ID="txtpassword" CssClass="form-control" Width="80" runat="server" /></td>
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
        <Triggers>
            <asp:PostBackTrigger ControlID="BtnImport" />
        </Triggers>
    </asp:UpdatePanel> 
</asp:Content>
