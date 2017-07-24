<%@ Page Language="VB" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="main.aspx.vb" Inherits="LAEACC.main" %>

<%@ MasterType VirtualPath="~/Site.master" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
<div id="content">
	<div class="row">
		<!-- 單位名稱1 -->
        <div class="col-xs-12 col-sm-7 col-md-7 col-lg-4">
			<h1 class="page-title txt-color-blueDark"><i class="fa-fw fa fa-home"></i><asp:Label ID="txtOrgName" font-size="22px" runat="server" /></h1>
		</div>
        <!-- end 單位名稱 -->

        <!-- 主計室執行情況 -->
		<div class="col-xs-12 col-sm-5 col-md-5 col-lg-8">
			<ul id="sparks" class="">
				<li class="sparks-info">
					<h5> 年度預算數 <span class="txt-color-blue">$47,171</span></h5>								
				</li>
                <li class="sparks-info">
					<h5> 年度開支數 <span class="txt-color-red">$47,171</span></h5>								
				</li>
                <li class="sparks-info">
					<h5> 與上年度執行率 <span class="txt-color-purple"><i class="fa fa-arrow-circle-down"></i>&nbsp;45%</span></h5>
				</li>							
			</ul>
		</div>
        <!-- end 主計室執行情況 -->
	</div>

	<!-- widget grid 控件 -->
	<section id="widget-grid" class="">
        <div class="row">
            <article class="col-sm-12 col-md-12 col-lg-6">
			    <div class="jarviswidget" id="wid-id-1" data-widget-editbutton="false" data-widget-custombutton="false">				
				    <header>
					    <span class="widget-icon"> <i class="fa fa-edit"></i> </span>
					    <h2>權限操作系統</h2>
				    </header>

                    <div>
                        <div class="widget-body no-padding">
                            <div style="margin:10px 0px 0px 10px;">
                                <asp:Label ID="txtSystemButton" Text="" runat="server" />
                                <asp:Button ID="btnCreateReportTable" Text="重建TABLE" CssClass="btn btn-primary" runat="server" />	
                            </div>
                        </div>
                    </div>
                </div>
            </article>
         
            <article class="col-sm-12 col-md-12 col-lg-6">
			    <div class="jarviswidget" id="wid-id-2" data-widget-editbutton="false" data-widget-custombutton="false">				
				    <header>
					    <span class="widget-icon"> <i class="fa fa-edit"></i> </span>
					    <h2>帳號群組</h2>
				    </header>

                    <div>
                        <div class="widget-body no-padding">
                            <div style="margin:10px 0px 0px 10px;">
                                帳號群組
                            </div>
                        </div>
                    </div>
                </div>
            </article>
        </div>

		<!-- 分頁訊息 -->
		<div class="row">
			<article class="col-sm-12">
				<!-- new widget -->
				<div class="jarviswidget" id="wid-id-0" data-widget-togglebutton="false" data-widget-editbutton="false" data-widget-fullscreenbutton="false" data-widget-colorbutton="false" data-widget-deletebutton="false">								
					<header>
						<span class="widget-icon"> <i class="glyphicon glyphicon-stats txt-color-darken"></i> </span>
						<h2>訊息通知</h2>

						<ul class="nav nav-tabs pull-right in" id="myTab">
							<li class="active">
								<a data-toggle="tab" href="#s1"><i class="fa fa-dollar"></i> <span class="hidden-mobile hidden-tablet">狀態圖</span></a>
							</li>
							<li>
								<a data-toggle="tab" href="#s2"><i class="fa fa-clock-o"></i> <span class="hidden-mobile hidden-tablet">近<asp:Label ID="txtCount" runat="server" />筆系統登出入記錄</span></a>
							</li>										
						</ul>
					</header>

					<!-- widget div-->
					<div class="no-padding">
						<!-- widget edit box -->
						<div class="jarviswidget-editbox">test</div>
						<!-- end widget edit box -->

						<div class="widget-body">
							<!-- content -->
							<div id="myTabContent" class="tab-content">
                                <!-- s1 tab pane -->
								<div class="tab-pane fade active in padding-10 no-padding-bottom" id="s1">
									<div class="row no-space">
										<div class="col-xs-12 col-sm-12 col-md-8 col-lg-8" style="width:50%; padding-right:10px;">
											<div class="jarviswidget" id="wid-id-1-1" data-widget-editbutton="false">
                                                <header>
									                <span class="widget-icon"> <i class="fa fa-bar-chart-o"></i> </span>
									                <h2>各單位預算支出統計</h2>
								                </header>
                                                <div>									                
									                <div class="widget-body no-padding">
										                <div id="normal-bar-graph" class="chart no-padding"></div>
									                </div>									                
								                </div>
                                            </div>
										</div>
										<div class="col-xs-12 col-sm-12 col-md-8 col-lg-8" style="width:50%;">
                                            <div class="jarviswidget" id="wid-id-1-2" data-widget-editbutton="false">
                                                <header>
									                <span class="widget-icon"> <i class="fa fa-bar-chart-o"></i> </span>
									                <h2>預算分配圖</h2>
								                </header>
                                                <div>									                
									                <div class="widget-body no-padding">
										                <div id="donut-graph" class="chart no-padding"></div>
									                </div>									                
								                </div>
                                            </div>													
										</div>
									</div>
								</div>
								<!-- end s1 tab pane -->

                                <!-- s2 tab pane -->
                                <div class="tab-pane fade" id="s2">
                                    <div class="widget-body-toolbar bg-color-white">
                                        <asp:DataGrid ID="DataGridView"  
                                            Width="100%"
                                            class="table table-bordered table-striped table-condensed table-hover smart-form has-tickbox"
                                            runat="server">

                                            <columns>                                       
                                                
                                                <asp:TemplateColumn HeaderText="系統訊息" HeaderStyle-Width="80" ItemStyle-HorizontalAlign="Center" SortExpression="message_id">
                                                    <itemtemplate><asp:Label ID="系統訊息" Text='<%# Me.Master.Models.strSFColor(Container.DataItem("message_id").ToString)%>' runat="server" /></itemtemplate>
                                                </asp:TemplateColumn>
                                                <asp:TemplateColumn HeaderText="概述" SortExpression="operate_note">
                                                    <itemtemplate><asp:Label ID="概述" Text='<%# Container.DataItem("operate_note").ToString %>' runat="server" /></itemtemplate>
                                                </asp:TemplateColumn>
                                                <asp:TemplateColumn HeaderText="IP" HeaderStyle-Width="100" ItemStyle-HorizontalAlign="Center" SortExpression="ip">
                                                    <itemtemplate><asp:Label ID="IP" Text='<%# Container.DataItem("ip").ToString %>' runat="server" /></itemtemplate>
                                                </asp:TemplateColumn>
                                                <asp:TemplateColumn HeaderText="日期" HeaderStyle-Width="100" ItemStyle-HorizontalAlign="Center" SortExpression="date">
                                                    <itemtemplate><asp:Label ID="日期" Text='<%# Me.Master.Models.strStrToDate(Container.DataItem("date").ToString)%>' runat="server" /></itemtemplate>
                                                </asp:TemplateColumn>
                                                <asp:TemplateColumn HeaderText="時間" HeaderStyle-Width="100" ItemStyle-HorizontalAlign="Center" SortExpression="time">
                                                    <itemtemplate><asp:Label ID="時間" Text='<%# Container.DataItem("time").ToString %>' runat="server" /></itemtemplate>
                                                </asp:TemplateColumn>
                                            </columns>
                                        </asp:DataGrid>
                                    </div>									
								</div>
							</div>
							<!-- end content -->
						</div>
					</div>
					<!-- end widget div -->
				</div>
				<!-- end widget -->
			</article>
		</div>
		<!-- end row -->
	</section>
	<!-- end widget grid 控件 -->

    <!-- ENHANCEMENT PLUGINS : NOT A REQUIREMENT -->
	<!-- SmartChat UI : plugin -->
	<script src="/active/js/smart-chat-ui/smart.chat.ui.min.js"></script>
	<script src="/active/js/smart-chat-ui/smart.chat.manager.min.js"></script>

	<!-- PAGE RELATED PLUGIN(S) -->

	<!-- Morris Chart Dependencies -->
	<script src="/active/js/plugin/morris/raphael.min.js"></script>
	<script src="/active/js/plugin/morris/morris.min.js"></script>

    <script type="text/javascript">
        $(document).ready(function () {
            //bar
            if ($('#normal-bar-graph').length) {
                Morris.Bar({
                    element: 'normal-bar-graph',
                    data: [{
                        x: '2011 Q1',
                        y: 3,
                        z: 2
                    }, {
                        x: '2011 Q2',
                        y: 2,
                        z: null
                    }, {
                        x: '2011 Q3',
                        y: 0,
                        z: 2
                    }, {
                        x: '2011 Q4',
                        y: 2,
                        z: 4
                    }],
                    xkey: 'x',
                    ykeys: ['y', 'z'],
                    labels: ['Y', 'Z']
                });
            }

            // donut
            if ($('#donut-graph').length) {
                Morris.Donut({
                    element: 'donut-graph',
                    data: [{
                        value: 70,
                        label: '管理組'
                    }, {
                        value: 15,
                        label: '主計室'
                    }, {
                        value: 10,
                        label: '工務組'
                    }, {
                        value: 5,
                        label: '其他'
                    }],
                    formatter: function (x) {
                        return x + "%"
                    }
                });
            }
        });
    </script>
</div>
</asp:Content>
