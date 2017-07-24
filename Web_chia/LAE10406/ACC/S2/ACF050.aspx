<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="ACF050.aspx.vb" Inherits="LAEACC.ACF050" %>
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
            <table id="table-serch" rules="all">                                            
                <tr>
                    <td style="text-align:center;">
                        年度：<asp:TextBox ID="nudYear"  Width="80" TextMode="Number" runat="server" min="0"  step="1" />
                    </td>     
                    <td style="text-align:center;">
                        月份：<asp:TextBox ID="nudMM"  Width="80" TextMode="Number" runat="server" min="" max="12"  step="1" />
                    </td>                                                                                             
                    <td style="text-align:center;">
                        <asp:TextBox ID="vxtStartNo" CssClass="form-control" runat="server" />
                                            <AjaxToolkit:MaskedEditExtender ID="vxtStartNo_Mask" runat="server"
                                                TargetControlID="vxtStartNo"
                                                MaskType="None" Mask="?-????-??-??-???????-?"
                                                InputDirection="LeftToRight" />
                    </td>
                    <td style="text-align:center;">
                        <asp:TextBox ID="vxtEndNo" CssClass="form-control" runat="server" />
                                            <AjaxToolkit:MaskedEditExtender ID="MaskedEditExtender1" runat="server"
                                                TargetControlID="vxtEndNo"
                                                MaskType="None" Mask="?-????-??-??-???????-?"
                                                InputDirection="LeftToRight" />
                    </td>
                                         
                    <td style="text-align:center;">
                        <asp:Button ID="btnSearch" Text="查詢" CssClass="btn btn-primary" runat="server" />                                                   
                    </td>
                    <td style="text-align:center;">
                        <asp:Button ID="btnExport" Text="匯出" CssClass="btn btn-primary" runat="server" />                                                  
                    </td>
                    <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                        <ContentTemplate></ContentTemplate>
                        <Triggers>
                            <asp:PostBackTrigger ControlID="btnExport" />
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
                            <AjaxToolkit:TabContainer ID="TabContainer1" Width="100%" CssClass="Tab" runat="server" ActiveTabIndex="2">
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
                                            <asp:DataGrid ID="DataGridView" Width="100%" AllowSorting="false" AllowPaging="true" style="font-size:14px;" CssClass="table table-bordered table-condensed smart-form" runat="server" >
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
                                                                            
                                                    <asp:TemplateColumn HeaderText="年度" HeaderStyle-Width="32" ItemStyle-HorizontalAlign="Center">
                                                        <itemtemplate><asp:Label ID="年度" Text='<%# Container.DataItem("accyear").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>                                                   
                                                    <asp:TemplateColumn HeaderText="會計科目" HeaderStyle-Width="140" >
                                                        <itemtemplate><asp:Label ID="會計科目" Text='<%# Container.DataItem("accno").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>                                                    
                                                    <asp:TemplateColumn HeaderText="會計科目名稱" HeaderStyle-Width="300" ItemStyle-HorizontalAlign="left">
                                                        <itemtemplate><asp:Label ID="會計科目名稱" Text='<%# Container.DataItem("accname").ToString%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="年初餘額" HeaderStyle-Width="160" ItemStyle-HorizontalAlign="right">
                                                        <itemtemplate><asp:Label ID="年初餘額" Text='<%# FormatNumber(Container.DataItem("begamt").ToString, 2)%>' runat="server" /></itemtemplate>                                                       
                                                    </asp:TemplateColumn> 
                                                    
                                                    <asp:TemplateColumn HeaderText="該月餘額" HeaderStyle-Width="160" ItemStyle-HorizontalAlign="right">
                                                        <itemtemplate><asp:Label ID="該月餘額" Text='<%# FormatNumber(Container.DataItem("netamt").ToString, 2)%>' runat="server" /></itemtemplate>                                                       
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
                                                <th>年度：</th>
                                                <td>
                                                    <asp:TextBox ID="txtAccYear" CssClass="form-control td-left" Width="80px" TextMode="Number" runat="server" min="0" max="999" step="1" runat="server" />
                                                </td>
                                                <th>科目代號：</th>
                                                <td>
                                                    <asp:TextBox ID="vxtAccno" CssClass="form-control td-left" Width="160px" runat="server"  runat="server" />
                                                    <asp:Label ID="lblAccname" CssClass="td-right" ForeColor="Blue" Font-Size="12pt" Font-Bold="True"  runat="server" />
                                                    <asp:Label ID="lblkey" Visible="False"  runat="server" />
                                                </td>                                                    
                                            </tr>
                                            <tr>
                                                <th>月份</th>
                                                <th>各月借方總額</th>
                                                <th>各月貸方總額</th>
                                                <th>各月淨額</th>                                                    
                                            </tr>
                                            <tr>
                                                <th>年初</th>
                                                <td><asp:TextBox ID="txtBeg_debit" CssClass="form-control" Width="160px" runat="server"  runat="server" /></td>
                                                <td><asp:TextBox ID="txtBeg_credit" CssClass="form-control" Width="160px" runat="server"  runat="server" /></td>
                                                <td><asp:Label ID="lblNet00" CssClass="form-control" Width="160px" ForeColor="Blue" Font-Size="12pt" Font-Bold="True"  runat="server" /></td>                                                    
                                            </tr>
                                            <tr>
                                                <th> 1月</th>
                                                <td><asp:TextBox ID="txtDeamt01" CssClass="form-control" Width="160px" runat="server"  runat="server" /></td>
                                                <td><asp:TextBox ID="txtCramt01" CssClass="form-control" Width="160px" runat="server"  runat="server" /></td>
                                                <td><asp:Label ID="lblNet01" CssClass="form-control" Width="160px" ForeColor="Blue" Font-Size="12pt" Font-Bold="True"  runat="server" /></td>                                                    
                                            </tr>
                                            <tr>
                                                <th> 2月</th>
                                                <td><asp:TextBox ID="txtDeamt02" CssClass="form-control" Width="160px" runat="server"  runat="server" /></td>
                                                <td><asp:TextBox ID="txtCramt02" CssClass="form-control" Width="160px" runat="server"  runat="server" /></td>
                                                <td><asp:Label ID="lblNet02" CssClass="form-control" Width="160px" ForeColor="Blue" Font-Size="12pt" Font-Bold="True"  runat="server" /></td>                                                    
                                            </tr>
                                            <tr>
                                                <th> 3月</th>
                                                <td><asp:TextBox ID="txtDeamt03" CssClass="form-control" Width="160px" runat="server"  runat="server" /></td>
                                                <td><asp:TextBox ID="txtCramt03" CssClass="form-control" Width="160px" runat="server"  runat="server" /></td>
                                                <td><asp:Label ID="lblNet03" CssClass="form-control" Width="160px" ForeColor="Blue" Font-Size="12pt" Font-Bold="True"  runat="server" /></td>                                                    
                                            </tr>
                                            <tr>
                                                <th> 4月</th>
                                                <td><asp:TextBox ID="txtDeamt04" CssClass="form-control" Width="160px" runat="server"  runat="server" /></td>
                                                <td><asp:TextBox ID="txtCramt04" CssClass="form-control" Width="160px" runat="server"  runat="server" /></td>
                                                <td><asp:Label ID="lblNet04" CssClass="form-control" Width="160px" ForeColor="Blue" Font-Size="12pt" Font-Bold="True"  runat="server" /></td>                                                    
                                            </tr>
                                            <tr>
                                                <th> 5月</th>
                                                <td><asp:TextBox ID="txtDeamt05" CssClass="form-control" Width="160px" runat="server"  runat="server" /></td>
                                                <td><asp:TextBox ID="txtCramt05" CssClass="form-control" Width="160px" runat="server"  runat="server" /></td>
                                                <td><asp:Label ID="lblNet05" CssClass="form-control" Width="160px" ForeColor="Blue" Font-Size="12pt" Font-Bold="True"  runat="server" /></td>                                                    
                                            </tr>
                                            <tr>
                                                <th> 6月</th>
                                                <td><asp:TextBox ID="txtDeamt06" CssClass="form-control" Width="160px" runat="server"  runat="server" /></td>
                                                <td><asp:TextBox ID="txtCramt06" CssClass="form-control" Width="160px" runat="server"  runat="server" /></td>
                                                <td><asp:Label ID="lblNet06" CssClass="form-control" Width="160px" ForeColor="Blue" Font-Size="12pt" Font-Bold="True"  runat="server" /></td>                                                    
                                            </tr>
                                            <tr>
                                                <th> 7月</th>
                                                <td><asp:TextBox ID="txtDeamt07" CssClass="form-control" Width="160px" runat="server"  runat="server" /></td>
                                                <td><asp:TextBox ID="txtCramt07" CssClass="form-control" Width="160px" runat="server"  runat="server" /></td>
                                                <td><asp:Label ID="lblNet07" CssClass="form-control" Width="160px" ForeColor="Blue" Font-Size="12pt" Font-Bold="True"  runat="server" /></td>                                                    
                                            </tr>
                                            <tr>
                                                <th> 8月</th>
                                                <td><asp:TextBox ID="txtDeamt08" CssClass="form-control" Width="160px" runat="server"  runat="server" /></td>
                                                <td><asp:TextBox ID="txtCramt08" CssClass="form-control" Width="160px" runat="server"  runat="server" /></td>
                                                <td><asp:Label ID="lblNet08" CssClass="form-control" Width="160px" ForeColor="Blue" Font-Size="12pt" Font-Bold="True"  runat="server" /></td>                                                    
                                            </tr>
                                            <tr>
                                                <th> 9月</th>
                                                <td><asp:TextBox ID="txtDeamt09" CssClass="form-control" Width="160px" runat="server"  runat="server" /></td>
                                                <td><asp:TextBox ID="txtCramt09" CssClass="form-control" Width="160px" runat="server"  runat="server" /></td>
                                                <td><asp:Label ID="lblNet09" CssClass="form-control" Width="160px" ForeColor="Blue" Font-Size="12pt" Font-Bold="True"  runat="server" /></td>                                                    
                                            </tr>
                                            <tr>
                                                <th> 10月</th>
                                                <td><asp:TextBox ID="txtDeamt10" CssClass="form-control" Width="160px" runat="server"  runat="server" /></td>
                                                <td><asp:TextBox ID="txtCramt10" CssClass="form-control" Width="160px" runat="server"  runat="server" /></td>
                                                <td><asp:Label ID="lblNet10" CssClass="form-control" Width="160px" ForeColor="Blue" Font-Size="12pt" Font-Bold="True"  runat="server" /></td>                                                    
                                            </tr>
                                            <tr>
                                                <th> 11月</th>
                                                <td><asp:TextBox ID="txtDeamt11" CssClass="form-control" Width="160px" runat="server"  runat="server" /></td>
                                                <td><asp:TextBox ID="txtCramt11" CssClass="form-control" Width="160px" runat="server"  runat="server" /></td>
                                                <td><asp:Label ID="lblNet11" CssClass="form-control" Width="160px" ForeColor="Blue" Font-Size="12pt" Font-Bold="True"  runat="server" /></td>                                                    
                                            </tr>
                                            <tr>
                                                <th> 12月</th>
                                                <td><asp:TextBox ID="txtDeamt12" CssClass="form-control" Width="160px" runat="server"  runat="server" /></td>
                                                <td><asp:TextBox ID="txtCramt12" CssClass="form-control" Width="160px" runat="server"  runat="server" /></td>
                                                <td><asp:Label ID="lblNet12" CssClass="form-control" Width="160px" ForeColor="Blue" Font-Size="12pt" Font-Bold="True"  runat="server" /></td>                                                    
                                            </tr>
                                            
                                            
                                        </table>                                        
                                    </ContentTemplate>
                                </AjaxToolkit:TabPanel> 
                                
                                <AjaxToolkit:TabPanel ID="TabPanel3" runat="server">
                                    <HeaderTemplate>列印</HeaderTemplate>
                                    <ContentTemplate>
                                        <table id="table-data" rules="all">
                                            <tr>
                                                <td>列印起訖科目範圍之會計科目</td>
                                            </tr>
                                            <tr>
                                                <td><asp:Button ID="BtnPrint" Text="列印" CssClass="btn btn-primary" runat="server" /></td>
                                                <asp:UpdatePanel ID="UpdatePanel5" runat="server">
                                                                <ContentTemplate></ContentTemplate>
                                                                <Triggers>
                                                                    <asp:PostBackTrigger ControlID="BtnPrint" />
                                                                </Triggers>
                                                            </asp:UpdatePanel>   
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
