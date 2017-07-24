<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="SYSGROUPUNIT.aspx.vb" Inherits="LAEACC.SYSGROUPUNIT" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="AjaxToolkit" %>

<%@ MasterType VirtualPath="~/Site.master" %>
<%@ Register SRC="~/LAE10406/UserControl/UCBase.ascx" TagName="UCBase" TagPrefix="uc1" %>
<%@ Register SRC="~/LAE10406/UserControl/AccText.ascx" TagName="AccText" TagPrefix="Acc1" %>

<asp:Content ID="Head" ContentPlaceHolderID="MainHead" runat="server">
    <style type="text/css">
        .auto-style1 {
            height: 50px;
        }
    </style>
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
                                                            <asp:Label ID="id" Text='<%# Container.DataItem("group_id").ToString%>' Visible="false" runat="server" />
                                                            <asp:UpdatePanel ID="DataGridView_UpdatePanel" runat="server">                                                                
                                                                <Triggers><asp:AsyncPostBackTrigger ControlID="Show" EventName="Click" /></Triggers>
                                                            </asp:UpdatePanel>
                                                        </itemtemplate>                                                         
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="群組名稱" HeaderStyle-Width="40" ItemStyle-HorizontalAlign="left">
                                                        <itemtemplate><asp:Label ID="群組名稱" Text='<%# Container.DataItem("group_name").ToString%>' runat="server" /></itemtemplate>                                                       
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
                                                <th>群組名稱：</th>
                                                <td>
                                                    <asp:TextBox ID="txtgroup_name" CssClass="form-control" Width="120px" runat="server" />
                                                    <asp:Label ID="lblkey" ForeColor="Blue" Font-Size="12pt" Font-Bold="True" Visible="False" runat="server" />
                                                </td>
                                            </tr>
                                             <tr>
                                                <th class="auto-style1">查詢：</th>
                                                <td class="auto-style1">
                                                   
                                                    <asp:Button ID="btnS" Text="查詢" CssClass="btn btn-warning td-right" runat="server" />  
                                                </td>
                                            </tr>
                                            <tr>
                                                <th>操作：</th>
                                                <td>
                                                   <asp:Button ID="btnSave" Text="存檔" CssClass="btn btn-warning" runat="server" />&nbsp;&nbsp;&nbsp;  
                                                   <asp:Button ID="CheckAll" runat="server" Text="全選" CssClass="btn btn-info"  />
                                                   <asp:Button ID="UncheckAll" runat="server" Text="取消" CssClass="btn btn-info"  />
                                                   
                                                </td>
                                            </tr>
                                            
                                        </table>

                                        <div class="table-responsive">
                                             <asp:DataGrid ID="DataGrid1" Width="100%"  style="font-size:14px;" CssClass="table table-bordered table-condensed smart-form" runat="server" >
                                                <columns>
                                                    <asp:TemplateColumn HeaderText="選取">
                                                        <itemtemplate>
                                                            <asp:CheckBox ID="objCheck" Checked='<%# Container.DataItem("checked")%>' runat="server"/> 
                                                            <asp:Label ID="id" Text='<%# Container.DataItem("unit_id").ToString%>' Visible="false" runat="server" />
                                                        </itemtemplate>                                                       
                                                        <HeaderStyle Width="40px" />
                                                        <ItemStyle HorizontalAlign="Left" />
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
