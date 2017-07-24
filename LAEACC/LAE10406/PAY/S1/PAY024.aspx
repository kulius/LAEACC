<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="PAY024.aspx.vb" Inherits="LAEACC.PAY024" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="AjaxToolkit" %>

<%@ MasterType VirtualPath="~/Site.master" %>
<%@ Register SRC="~/LAE10406/UserControl/UCBase.ascx" TagName="UCBase" TagPrefix="uc1" %>
<%@ Register SRC="~/LAE10406/UserControl/AccText.ascx" TagName="AccText" TagPrefix="Acc1" %>

<asp:Content ID="Head" ContentPlaceHolderID="MainHead" runat="server">
        <script type="text/javascript">
        function ismaxlength(obj) {
            var mlength = obj.getAttribute ? parseInt(obj.getAttribute("maxlength")) : ""
            if (obj.getAttribute && obj.value.length == mlength) {
                var radioObj = document.getElementById("<%=txtChkNo.ClientID%>");
                radioObj.focus();
            }
        }
    </script>
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
                                    <HeaderTemplate>支票作廢</HeaderTemplate>
                                    <ContentTemplate>
                                        <div class="widget-body form-horizontal">
                                            <div style="background-color:#71eb8a; font-size:16px; padding-top:20px;">
                                                <fieldset>
                                                    
                                                    <div class="form-group">
                                                        <label class="col-lg-2 control-label">銀行：</label>  
                                                        <div class="col-lg-2 control-label"><asp:TextBox ID="txtBank" runat="server" /></div>  
                                                        <div class="col-lg-2 control-label"><asp:Label ID="lblBankname" ForeColor="Blue" Font-Size="12pt" Font-Bold="True"  runat="server" /></div>  
                                                        
                                                    </div>
                                                    <div class="form-group">
                                                        <label class="col-lg-2 control-label">支票號碼：</label>  
                                                        <div class="col-lg-2 control-label"><asp:TextBox ID="txtChkNo" runat="server" /></div>  
                                                        <div class="col-lg-1 control-label"><asp:Button ID="btnChkno" Text="調出支票" CssClass="btn btn-primary" runat="server" /></div>
                                                        <label class="col-lg-2 control-label"><asp:Label ID="lblMsg" ForeColor="Blue" Font-Size="12pt" Font-Bold="True"  runat="server" /></label>
                                                    </div>
                                                    <div class="form-group">
                                                        <label class="col-lg-2 control-label">支票金額：</label>
                                                        <div class="col-lg-2 control-label" ><asp:Label ID="lblamt" ForeColor="Blue" Font-Size="12pt" Font-Bold="True"  runat="server" />	</div>
                                                    </div>
                                                    <div class="form-group">
                                                        <label class="col-lg-2 control-label">受款人</label>
                                                        <label class="col-lg-2"  ><asp:Label ID="lblChkname" ForeColor="Blue" Font-Size="12pt" Width="520px" Font-Bold="True"  runat="server" />	</label>
                                                    </div>
                                                    <div class="form-group">
                                                        <label class="col-lg-2 control-label">摘要</label>
                                                        <label class="col-lg-2 " ><asp:Label ID="lblRemark" ForeColor="Blue" Font-Size="12pt" Width="520px" Font-Bold="True"  runat="server" />	</label>
                                                    </div>
                                                    <div class="form-group">
                                                        <label class="col-lg-2 control-label">傳票：</label>
                                                        <div class="col-lg-2 control-label" ><asp:Label ID="lblNo1" ForeColor="Blue" Font-Size="12pt" Font-Bold="True"  runat="server" />	</div>
                                                    </div>
                                                    
                                                                                                     
                                                    
                                                    <div class="form-group">
                                                        
                                                        <div class="col-lg-2 control-label"><asp:Button ID="btnFinish" Text="確定作廢" CssClass="btn btn-primary" runat="server" /></div>
                                                        <div class="col-lg-2 control-label"><asp:Button ID="btnGiveUp" Text="放棄" CssClass="btn btn-primary" runat="server" /></div>
           
                                                    </div>
                                                   
                                                                                                 
                                                </fieldset>
                                            </div>
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
