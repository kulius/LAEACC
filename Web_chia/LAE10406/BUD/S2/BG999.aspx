<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="BG999.aspx.vb" Inherits="LAEACC.BG999" %>
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
                            <!--詳細內容顯示區-->                           
                            <AjaxToolkit:TabContainer ID="TabContainer1" Width="100%" CssClass="Tab" runat="server" ActiveTabIndex="0">
                                <AjaxToolkit:TabPanel ID="TabPanel1" runat="server">
                                    <HeaderTemplate>瀏灠</HeaderTemplate>
                                    <ContentTemplate>
                                        <div class="table-responsive">
                                            <div style="margin:3px 0px 0px 10px; float:left; font-size:16px; font-weight:bold; color:#ff0000;" >
                                                決算注記(Y)，即無法再請購
                                            </div>
                                            <div style="float:right; padding-right:10px;">
                                                共<asp:Label ID="lbl_GrdCount" ForeColor="Red" Font-Size="14" Font-Bold="true" Text="0" runat="server" />筆符合&nbsp;                                                
                                                <asp:Label ID="lbl_sort" runat="server" />
                                            </div>
                                            <asp:DataGrid ID="DataGridView" Width="100%" AllowSorting="True" AllowPaging="True" style="font-size:14px;" CssClass="table table-bordered table-condensed smart-form" runat="server" >
                                                <columns>
                                                    <asp:TemplateColumn HeaderText="管理" HeaderStyle-Width="40" ItemStyle-HorizontalAlign="Center">
                                                        <itemtemplate>                                                                                                            
                                                            <asp:ImageButton ID="Show" AlternateText="查閱" ImageUrl="~/active/images/icon/items/zoom.png" CommandName="btnShow" runat="server" />
                                                            <asp:Label ID="id" Text='<%# Container.DataItem("autono").ToString%>' Visible="false" runat="server" />
                                                            <asp:UpdatePanel ID="DataGridView_UpdatePanel" runat="server">                                                                
                                                                <Triggers><asp:AsyncPostBackTrigger ControlID="Show" EventName="Click" /></Triggers>
                                                            </asp:UpdatePanel>
                                                        </itemtemplate>                                                         
                                                    </asp:TemplateColumn>  
                                                         
                                                    <asp:TemplateColumn HeaderText="年度" HeaderStyle-Width="40" ItemStyle-HorizontalAlign="Center">
                                                        <itemtemplate><asp:Label ID="年度" Text='<%# Container.DataItem("ACCYEAR").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>  
                                                    <asp:TemplateColumn HeaderText="科目" HeaderStyle-Width="100">
                                                        <itemtemplate><asp:Label ID="科目" Text='<%# Container.DataItem("ACCNO").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="科目名稱" HeaderStyle-Width="160">
                                                        <itemtemplate><asp:Label ID="科目名稱" Text='<%# Container.DataItem("ACCNAME").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="單位" HeaderStyle-Width="80" ItemStyle-HorizontalAlign="Center">
                                                        <itemtemplate><asp:Label ID="單位" Text='<%# Container.DataItem("UNIT").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>                                                                           
                                                    <asp:TemplateColumn HeaderText="總預算" HeaderStyle-Width="100" ItemStyle-HorizontalAlign="right" >
                                                        <itemtemplate><asp:Label ID="總預算" Text='<%# FormatNumber(Container.DataItem("BGSUM").ToString, 2)%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="請購中" HeaderStyle-Width="100" ItemStyle-HorizontalAlign="right" >
                                                        <itemtemplate><asp:Label ID="請購中" Text='<%# FormatNumber(Container.DataItem("TOTPER").ToString, 2)%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="已開支" HeaderStyle-Width="100" ItemStyle-HorizontalAlign="right" >
                                                        <itemtemplate><asp:Label ID="已開支" Text='<%# FormatNumber(Container.DataItem("TOTUSE").ToString, 2)%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>                                                                                                        
                                                    <asp:TemplateColumn HeaderText="可溢支" HeaderStyle-Width="40" ItemStyle-HorizontalAlign="Center">
                                                        <itemtemplate><asp:Label ID="可溢支" Text='<%# Container.DataItem("FLOW").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>                                                       
                                                    <asp:TemplateColumn HeaderText="工程編號" HeaderStyle-Width="40" ItemStyle-HorizontalAlign="Center">
                                                        <itemtemplate><asp:Label ID="工程編號" Text='<%# Container.DataItem("ENGNO").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>                                                                                                           
                                                </columns>
                                            </asp:DataGrid>
                                        </div>
                                    </ContentTemplate>
                                </AjaxToolkit:TabPanel>

                                <AjaxToolkit:TabPanel ID="TabPanel2" runat="server">
                                    <HeaderTemplate>單筆明細決算</HeaderTemplate>
                                    <ContentTemplate>
                                        <table id="table-data" rules="all">
                                            <tr>
                                                <th>年度：</th>
                                                <td><asp:Label ID="lblAccyear" ForeColor="Blue" Font-Size="12pt" Font-Bold="True" runat="server" /></td>
                                                <th>請購科目：</th>
                                                <td>
                                                    <asp:Label ID="lblAccno" ForeColor="Blue" Font-Size="12pt" Font-Bold="True" runat="server" />
                                                    <asp:Label ID="lblAccname" ForeColor="Blue" Font-Size="12pt" Font-Bold="True"  runat="server" />

                                                    <asp:Label ID="lblkey" ForeColor="Blue" Font-Size="12pt" Font-Bold="True" Visible="False" runat="server" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <th>單位：</th>
                                                <td><asp:Label ID="lblUnit" ForeColor="Blue" Font-Size="12pt" Font-Bold="True" runat="server" /></td>
                                                <th>工程編號：</th>
                                                <td><asp:Label ID="lblEngno" ForeColor="Blue" Font-Size="12pt" Font-Bold="True" runat="server" /></td>
                                            </tr>
                                            <tr>
                                                <th>預算金額：</th>
                                                <td><asp:Label ID="lblBgsum" ForeColor="Blue" Font-Size="12pt" Font-Bold="True" runat="server" /></td>
                                                <th>可溢支：</th>
                                                <td><asp:Label ID="lblFlow" ForeColor="Blue" Font-Size="12pt" Font-Bold="True"  runat="server" /></td>
                                            </tr>
                                            <tr>
                                                <th>請購金額：</th>
                                                <td colspan="3"><asp:Label ID="lblTotper" ForeColor="Blue" Font-Size="12pt" Font-Bold="True" runat="server" /></td>
                                            </tr>
                                            <tr>
                                                <th>開支金額：</th>
                                                <td colspan="3"><asp:Label ID="lblTotuse" ForeColor="Blue" Font-Size="12pt" Font-Bold="True"  runat="server" /></td>
                                            </tr>
                                            <tr>
                                                <th>預算餘額</th>
                                                <td colspan="3"><asp:Label ID="lblUnUseAmt" ForeColor="Blue" Font-Size="12pt" Font-Bold="True"  runat="server" /></td>
                                            </tr>
                                            <tr>
                                                <td colspan="4" align="center">
                                                    <asp:Button ID="btnSure" Text="決算" CssClass="btn btn-primary" runat="server" />
                                                    <asp:Button ID="btnBack" Text="返回清單" CssClass="btn btn-danger" runat="server" />
                                                </td>
                                            </tr>
                                        </table>
                                        <!--#伺服器驗證-->
                                        <AjaxToolkit:ConfirmButtonExtender ID="ConfirmButtonExtender1" runat="server"
                                            TargetControlID="btnSure" ConfirmText="確定此科目不再開支，要辦理決算嗎? 是 / 否 ">
                                        </AjaxToolkit:ConfirmButtonExtender>
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
