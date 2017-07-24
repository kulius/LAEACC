<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="BUD_AC150.aspx.vb" Inherits="LAEACC.BUD_AC150" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="AjaxToolkit" %>

<%@ MasterType VirtualPath="~/Site.master" %>
<%@ Register SRC="~/LAE10406/UserControl/UCBase.ascx" TagName="UCBase" TagPrefix="uc1" %>

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
                                    <!-- 查詢項目值 -->
                                    <div class="easyui-panel" style="width:100%;" title="查詢條件值" data-options="iconCls:'icon-search'">
                                        <table id="table-serch" rules="all">
                                            <tr>
                                                <th>年度：</th>
                                                <td><asp:TextBox ID="nudYear" Width="80" TextMode="Number" runat="server" min="0" max="999" step="1" /></td>
                                            </tr>
                                            <tr>
                                                <th>科目：</th>
                                                <td>
                                                    <asp:DropDownList ID="cboAccno"  runat="server" />
                                                    <asp:Button ID="BtnSearch" Text="查詢" CssClass="btn btn-primary" runat="server" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <th>科目：</th>
                                                <td>
                                                    <asp:TextBox ID="vxtAccno" Width="300px"  AutoPostBack="True" runat="server" />
                                                    <AjaxToolkit:MaskedEditExtender ID="MaskedEditExtender1" runat="server"
                                                        TargetControlID="vxtAccno"
                                                        MaskType="None" Mask="?-????-??-??-???????-?"
                                                        InputDirection="LeftToRight" />
                                                    <asp:Button ID="BtnSearch1" Text="查詢" CssClass="btn btn-primary" runat="server" />
                                                </td>                                            
                                            </tr>
                                        </table>                                                                              
                                        <AjaxToolkit:AutoCompleteExtender 
                                            ID="AutoCompleteExtender"                             
                                            runat="server"                
                                            TargetControlID="vxtAccno"
                                            ServicePath="~/active/WebService.asmx"
                                            ServiceMethod="GetAC010Accno"
                                            MinimumPrefixLength="0" 
                                            CompletionInterval="100"
                                            CompletionSetCount="12" DelimiterCharacters="" Enabled="True" />
                                    </div>                                                                                                                                                								    
						        </fieldset> 
                            </div>                                       
                            <!--詳細內容顯示區-->                           
                            <AjaxToolkit:TabContainer ID="TabContainer1" Width="100%" CssClass="Tab" runat="server" ActiveTabIndex="0">
                                <AjaxToolkit:TabPanel ID="TabPanel1" runat="server">
                                    <HeaderTemplate>資料來源</HeaderTemplate>
                                    <ContentTemplate>
                                        <div class="table-responsive">                                            
                                            <asp:Label ID="lbl_GrdCount" Visible="False" Text="0" runat="server" />                                            
                                            <asp:DataGrid ID="DataGrid1" Width = "100%" AllowSorting="false" AllowPaging="false" style="font-size:14px;" CssClass="table table-bordered table-condensed smart-form" runat="server" >
                                                <columns>
                                                    <asp:TemplateColumn HeaderText="科目" HeaderStyle-Width="100">
                                                        <itemtemplate><asp:Label ID="科目" Text='<%# Container.DataItem("accno").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="名稱" HeaderStyle-Width="160" >
                                                        <itemtemplate><asp:Label ID="名稱" Text='<%# Container.DataItem("accname").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="期初借方" HeaderStyle-Width="80" ItemStyle-HorizontalAlign="right">
                                                        <itemtemplate><asp:Label ID="期初借方" Text='<%# FormatNumber(Container.DataItem("beg_DEBIT").ToString, 2)%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="期初貸方" HeaderStyle-Width="80" ItemStyle-HorizontalAlign="right">
                                                        <itemtemplate><asp:Label ID="期初貸方" Text='<%# FormatNumber(Container.DataItem("beg_credit").ToString, 2)%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="已過帳借方" HeaderStyle-Width="80" ItemStyle-HorizontalAlign="right">
                                                        <itemtemplate><asp:Label ID="已過帳借方" Text='<%# FormatNumber(Container.DataItem("deamt12").ToString, 2)%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="已過帳貸方" HeaderStyle-Width="80" ItemStyle-HorizontalAlign="right">
                                                        <itemtemplate><asp:Label ID="已過帳貸方" Text='<%# FormatNumber(Container.DataItem("cramt12").ToString, 2)%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="未過帳借方" HeaderStyle-Width="80" ItemStyle-HorizontalAlign="right">
                                                        <itemtemplate><asp:Label ID="未過帳借方" Text='<%# FormatNumber(Container.DataItem("acf020D").ToString, 2)%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="未過帳貸方" HeaderStyle-Width="80" ItemStyle-HorizontalAlign="right">
                                                        <itemtemplate><asp:Label ID="未過帳貸方" Text='<%# FormatNumber(Container.DataItem("acf020C").ToString, 2)%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="借方餘額" HeaderStyle-Width="80" ItemStyle-HorizontalAlign="right">
                                                        <itemtemplate><asp:Label ID="借方餘額" Text='<%# FormatNumber(Container.DataItem("BalanceD").ToString, 2)%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="貸方餘額" HeaderStyle-Width="80" ItemStyle-HorizontalAlign="right">
                                                        <itemtemplate><asp:Label ID="貸方餘額" Text='<%# FormatNumber(Container.DataItem("BalanceC").ToString, 2)%>' runat="server" /></itemtemplate>                                                       
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
