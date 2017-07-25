<%@ Page Title="WEB版主計行政整合作業系統" Language="vb" AutoEventWireup="false" CodeBehind="index.aspx.vb" Inherits="LAEACC.index" %>

<!DOCTYPE html>

<html id="extr-page">
<head runat="server">
    <meta http-equiv="Content-Type" />
    <meta content="text/html" />
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0, maximum-scale=1.0, user-scalable=no" />
    <title><%: Page.Title %></title>	

    <webopt:bundlereference runat="server" path="/Content/css" />

    <!-- FAVICONS -->
    <link rel="shortcut icon" href="/active/images/favicon.ico" type="image/x-icon" />
    <link rel="icon" href="/active/images/favicon.ico" type="image/x-icon" />


    <!-- #GOOGLE FONT -->
    <link rel="stylesheet" href="http://fonts.googleapis.com/css?family=Open+Sans:400italic,700italic,300,400,700" />


    <!-- PACE LOADER -->
    <script src="/active/js/plugin/pace/pace.min.js"></script>

    <!-- Link to Google CDN's jQuery + jQueryUI -->
	<script src="//ajax.googleapis.com/ajax/libs/jquery/2.1.1/jquery.min.js"></script>
	<script> if (!window.jQuery) { document.write('<script src="js/libs/jquery-2.1.1.min.js"><\/script>'); } </script>

	<script src="//ajax.googleapis.com/ajax/libs/jqueryui/1.10.3/jquery-ui.min.js"></script>
	<script> if (!window.jQuery.ui) { document.write('<script src="js/libs/jquery-ui-1.10.3.min.js"><\/script>'); } </script>

	<!-- IMPORTANT: APP CONFIG -->
	<script src="/active/js/app.config.js"></script>

    <!-- BOOTSTRAP JS -->		
	<script src="/active/js/bootstrap/bootstrap.min.js"></script>

    <!-- JQUERY VALIDATE -->
	<script src="/active/js/plugin/jquery-validate/jquery.validate.min.js"></script>
		
	<!-- JQUERY MASKED INPUT -->
	<script src="/active/js/plugin/masked-input/jquery.maskedinput.min.js"></script>

	<!-- MAIN APP JS FILE -->
	<script src="/active/js/app.min.js"></script>

    <!--[if IE 8]>			
	    <h1>您的瀏覽器已不支援該網頁，請至http://www.microsoft.com/download更新您的瀏覽器</h1>			
    <![endif]-->

    <!-- Other -->
    <!-- #日期選擇控件 -->
    <script type="text/javascript" src="/active/js/My97DatePicker/WdatePicker.js"></script>
    
    <!-- #latest[跑馬燈] -->
    <script type="text/javascript" src="/active/jQuery/latest/js/jquery-latest.js"></script>
    <link type="text/css" href="/active/jQuery/latest/css/latest.css" rel="stylesheet" media="screen" />	    
    <!-- #end latest -->
</head>
<body class="animated fadeInDown">          
    <header id="header">
        <!-- Logo -->
		<div id="logo-group">            
			<span id="logo"><asp:Label ID="txtLogo" Text="" runat="server" /></span>
		</div>
	</header>                

    <div id="main" role="main">        
        <div id="content" class="container">
            <div class="row">
				<div class="col-xs-12 col-sm-12 col-md-7 col-lg-8 hidden-xs hidden-sm" style="height:200px;">
                    <div id="abgne_marquee" style="width:100%; text-align:center; font-size:14px;">
                        <div class="marquee_btn" id="marquee_message_txt"><span style="color:#f63; font-size:16px; font-weight:bold;">訊息公告：</span></div>
                        <div class="marquee_btn" id="marquee_next_btn"><img src="/active/jQuery/latest/marquee_next_btn.jpg" alt="圖片" /></div>                                    
                            <asp:Label ID="txtMessage" runat="server" />
                        <div class="marquee_btn" id="marquee_prev_btn"><img src="/active/jQuery/latest/marquee_prev_btn.jpg" alt="圖片" /></div>
                    </div>
                    <div style="clear:both; padding-bottom:5px;"></div>

                    <div style="background-image:url(active/images/main-1.png); width:100%; height:120px;"></div>                                      

                    <h1 class="txt-color-red login-header-big">系統模組</h1>                                                            
                    <div class="hero">
                        <div class="pull-left login-desc-box-l">
                            <h4 class="paragraph-header">
                                <span style="line-height:25px;"><span style="color:#950029;">■</span> 預算系統</span><br>
                                <span style="line-height:25px;"><span style="color:#950029;">■</span> 會計系統</span><br>
                                <span style="line-height:25px;"><span style="color:#950029;">■</span> 出納系統</span><br>
                                <span style="line-height:25px;"><span style="color:#950029;">■</span> 決算系統</span><br>
                                <span style="line-height:25px;"><span style="color:#950029;">■</span> 收入與轉戶</span><br>
                                <span style="line-height:25px;"><span style="color:#950029;">■</span> 保證金系統</span>
                                
                            </h4>                           
                        </div>
                        <div class="pull-left login-desc-box-l">
                            <h4 class="paragraph-header">                                
                                <span style="line-height:25px;"><span style="color:#950029;">■</span> 財務管理系統</span><br>
                                <span style="line-height:25px;"><span style="color:#950029;">■</span> 收據系統</span><br>
                                <span style="line-height:25px;"><span style="color:#950029;">■</span> 退撫基金會計出納系統</span><br>
                                <span style="line-height:25px;"><span style="color:#950029;">■</span> 撫建基金會計出納系統</span><br>
                                <span style="line-height:25px;"><span style="color:#950029;">■</span> 系統管理</span>
                            </h4>                           
                        </div>
                    </div>                   
                </div>
                <div class="col-xs-12 col-sm-12 col-md-5 col-lg-4">
                    <div class="well no-padding">
                        <form id="frmLogin" class="smart-form client-form" defaultbutton="btnSubmit" runat="server">
                            <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
                            <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                                <ContentTemplate>
                                    <header><%:Page.Title%> - 系統登入</header>
                                    <fieldset>
                                        <section>
									        <label class="label">帳號：</label>
									        <label class="input"><i class="icon-append fa fa-user"></i>
										        <asp:TextBox ID="txtUserName" runat="server" />
										        <b class="tooltip tooltip-top-right"><i class="fa fa-user txt-color-teal"></i>請輸入系統帳號</b>
									        </label>
								        </section>
                                        <section>
									        <label class="label">密碼：</label>
									        <label class="input"><i class="icon-append fa fa-lock"></i>
										        <asp:TextBox ID="txtPassword" type="password" runat="server" />
										        <b class="tooltip tooltip-top-right"><i class="fa fa-lock txt-color-teal"></i>請輸入密碼</b>
									        </label>
								        </section>
                                        <section>
									        <label class="label">日期：</label>
									        <label class="input"><i class="icon-append fa fa-calendar"></i>
										        <asp:TextBox ID="txtNowDate" Text="" onClick="WdatePicker({dateFmt:'yyy-MM-dd',skin:'whyGreen'})" runat="server" />										
									        </label>
								        </section>
                                        <section>
									        <label class="label">系統：</label>
									        <label class="input"><i class="icon-append fa fa-bookmark "></i>
										        <asp:DropDownList ID="cbos_system_id" Width="100%" CssClass="form-control"  runat="server" />  
										        
									        </label>
								        </section>
                                        <section>
                                            <label class="checkbox">                                        
                                                <asp:CheckBox ID="txtRemember" runat="server" />
                                                <i></i>記住我的帳號及預設系統
                                            </label>
                                        </section>
                                    </fieldset>
                                    <footer>
                                        <asp:Button ID="btnCreateReportTable" Text="" CssClass="btn btn-primary" runat="server" />	  
                                        <asp:Button ID="btnReset" Text="重填" CssClass="btn btn-primary" runat="server" />
                                        <asp:Button ID="btnSubmit" Text="登入" CssClass="btn btn-primary" runat="server" />
                                    </footer>
                                </ContentTemplate>
                                <Triggers>
                                    <asp:AsyncPostBackTrigger ControlID="btnReset" EventName="Click" />
                                    <asp:AsyncPostBackTrigger ControlID="btnSubmit" EventName="Click" />
                                </Triggers>
                            </asp:UpdatePanel>                             
                        </form>                        
                    </div>
                </div>
                <p class="note text-center">
                    建議螢幕解析度：1024 x 768以上　Design  by  本會資訊室 © 2015
                </p>
            </div>
        </div>            
    </div>    
</body>
</html>
