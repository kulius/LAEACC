<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="PAY010.aspx.vb" Inherits="LAEACC.PAY010" %>
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
                            <!-- 查詢項目值 -->                             
                            <div class="widget-body form-horizontal" style="font-size:16px; background-color:#71eb8a;margin:10px 0px 10px 0px;" >
                                  <fieldset>
                                   <div class="form-group">
                                        <div class="col-lg-12" style="text-align:center"><asp:Label ID="Label2"  Text="每日開帳:(收支轉入昨日結存)" Font-Size="18pt" Font-Bold="True"  runat="server" /></div>
                                    </div>
                                    <div class="form-group">
                                        <div class="col-lg-2 control-label" >收付日期<asp:Label ID="lblDate2" Text="" Font-Size="12pt" Font-Bold="True"  runat="server" /></div>
                                        <div class="col-lg-4 control-label" ><asp:Label ID="lblMsg" Text="" Font-Size="12pt" Font-Bold="True"  runat="server" /></div>
                                        <div class="col-lg-2 control-label"><asp:Button ID="btnSure" Text="開帳作業" CssClass="btn btn-primary" runat="server" /></div>
                                        
                                    </div>
								    
							    </fieldset> 
                            </div>                          
                                       
                            <!--詳細內容顯示區-->                           
                            <AjaxToolkit:TabContainer ID="TabContainer1" Width="100%" CssClass="Tab" runat="server" ActiveTabIndex="0">
                                <AjaxToolkit:TabPanel ID="TabPanel1" runat="server">
                                    <HeaderTemplate>資料來源</HeaderTemplate>
                                    <ContentTemplate>
                                        <div style="font-size:14px;">
                                            
                                            <asp:Label ID="lbl_GrdCount" Visible="False" Text="0" runat="server" />                                            
                                            <asp:DataGrid ID="DataGridView" Width = "100%" runat="server" >
                                                <columns>
                                                    <asp:TemplateColumn HeaderText="銀行" HeaderStyle-Width="50">
                                                        <itemtemplate><asp:Label ID="銀行" Text='<%# Container.DataItem("BANK").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="帳戶" HeaderStyle-Width="60" >
                                                        <itemtemplate><asp:Label ID="帳戶" Text='<%# Container.DataItem("ACCOUNT").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="昨日結存" HeaderStyle-Width="120" ItemStyle-HorizontalAlign="right">
                                                        <itemtemplate><asp:Label ID="昨日結存" Text='<%# Container.DataItem("BALANCE").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="本日共收" HeaderStyle-Width="120" ItemStyle-HorizontalAlign="right">
                                                        <itemtemplate><asp:Label ID="本日共收" Text='<%# Container.DataItem("DAY_INCOME").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="本日共支" HeaderStyle-Width="120" ItemStyle-HorizontalAlign="right">
                                                        <itemtemplate><asp:Label ID="本日共支" Text='<%# Container.DataItem("DAY_PAY").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="收付日期" HeaderStyle-Width="85" >
                                                        <itemtemplate><asp:Label ID="收付日期" Text='<%# Container.DataItem("DATE_2").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="列印否" HeaderStyle-Width="60" >
                                                        <itemtemplate><asp:Label ID="列印否" Text='<%# Container.DataItem("PRT_CODE").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="名稱" HeaderStyle-Width="200" >
                                                        <itemtemplate><asp:Label ID="名稱" Text='<%# Container.DataItem("bankname").ToString%>' runat="server" /></itemtemplate>                                                       
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
