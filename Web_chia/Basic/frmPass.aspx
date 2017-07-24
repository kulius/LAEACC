<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Schild.Master" CodeBehind="frmPass.aspx.vb" Inherits="LAEACC.frmPass" %>

<%@ MasterType VirtualPath="~/Schild.master" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">    
<section id="widget-grid" style="width:95%;">
    <div class="row">		
		<article class="col-sm-12 col-md-12 col-lg-6">			
			<div class="jarviswidget" id="wid-id-1" data-widget-editbutton="false" data-widget-custombutton="false">				
				<header>
					<span class="widget-icon"> <i class="fa fa-edit"></i> </span>
					<h2>個人化</h2>					
				</header>

				<!-- widget div-->
				<div>								
					<div class="widget-body no-padding">						
						<form id="frmPage" class="smart-form" novalidate runat="server">
                            <asp:ScriptManager runat="server"></asp:ScriptManager>
                            <asp:UpdatePanel runat="server">
                                <ContentTemplate>
                                    <fieldset>
                                        <section class="col col-6">
										    <label class="input"> <i class="icon-append fa fa-lock"></i>											                                                    
                                                <asp:TextBox ID="O_PASS_WORD" TextMode="Password" placeholder="舊密碼" runat="server" />
										        <b class="tooltip tooltip-bottom-right">請輸入您的舊密碼</b>
										    </label>
									    </section>
                                    </fieldset>
                                    <fieldset>
                                        <section class="col col-6">
										    <label class="input"> <i class="icon-append fa fa-lock"></i>											                                                    
                                                <asp:TextBox ID="N_PASS_WORD" TextMode="Password" placeholder="新密碼" runat="server" />
										        <b class="tooltip tooltip-bottom-right">請輸入您的新密碼</b>
										    </label>
									    </section>
                                    </fieldset>
                                    <fieldset>
                                        <section class="col col-6">
										    <label class="input"> <i class="icon-append fa fa-lock"></i>											                                                    
                                                <asp:TextBox ID="C_PASS_WORD" TextMode="Password" placeholder="確認密碼" runat="server" />
										        <b class="tooltip tooltip-bottom-right">請再確認一次您的新密碼</b>
										    </label>
									    </section>
                                    </fieldset>
                                    <footer>
                                        <asp:Button ID="btnSave" Text="儲存" class="btn btn-primary" runat="server" />                                        
							        </footer>
                                </ContentTemplate>
                                <Triggers>
                                    <asp:AsyncPostBackTrigger ControlID="btnSave" EventName="Click" />                                    
                                </Triggers>
                            </asp:UpdatePanel>													
						</form>
					</div>
				</div>
			</div>
        </article>
    </div>
</section>
</asp:Content>
