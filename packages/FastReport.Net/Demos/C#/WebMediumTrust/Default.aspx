<%@ Page Language="C#" AutoEventWireup="true"  CodeFile="Default.aspx.cs" Inherits="_Default" %>


<%@ Register Assembly="FastReport" Namespace="FastReport.Web" TagPrefix="cc1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>FastReport Medium Trust Demo</title>
</head>
<body background="images/background.jpg">
    <form id="form1" runat="server">
    <div>
        <table style="width: 795px">
            <tr>
                <td style="width: 106px; height: 44px; text-align: center">
                    <asp:Image ID="Image1" runat="server" ImageUrl="~/images/demo_logo.png" EnableViewState="False" /></td>
                <td style="width: 768px; height: 44px">
                    <span style="font-size: 16pt; font-family: Tahoma"><strong>FastReport .NET Demo<br />
                        <asp:Label ID="Version" runat="server" Font-Size="Small" Text="ver. 1.0.0" Width="188px" EnableViewState="False"></asp:Label>&nbsp;<br />
                        <span style="font-size: 8pt">Medium Trust</span></strong></span></td>
            </tr>
        </table>
        <hr />
        <table style="width: 941px">
            <tr>
                <td style="width: 300px; height: 826px" valign="top">
                    
                    <asp:Menu ID="LeftMenu" runat="server" BackColor="GhostWhite" DynamicHorizontalOffset="2"
                        Font-Names="Tahoma" Font-Size="8pt" ForeColor="Black" Height="169px" MaximumDynamicDisplayLevels="2"
                        OnInit="LeftMenu_Init" OnMenuItemClick="LeftMenu_MenuItemClick" StaticDisplayLevels="2"
                        StaticSubMenuIndent="10px" Width="240px">
                        <StaticSelectedStyle BackColor="LightSteelBlue" ForeColor="DarkBlue" />
                        <StaticMenuItemStyle HorizontalPadding="5px" VerticalPadding="2px" />
                        <DynamicHoverStyle BackColor="#990000" ForeColor="White" />
                        <DynamicMenuStyle BackColor="#FFFBD6" />
                        <DynamicItemTemplate>
                            <%# Eval("Text") %>
                        </DynamicItemTemplate>
                        <DynamicSelectedStyle BackColor="Chocolate" />
                        <DynamicMenuItemStyle HorizontalPadding="5px" VerticalPadding="2px" />
                        <StaticHoverStyle BackColor="Lavender" ForeColor="DarkBlue" />
                        <Items>
                            <asp:MenuItem Selectable="False" Text="Simple reports" Value="Simple reports">
                                <asp:MenuItem Selected="True" Text="Report1" Value="Report1"></asp:MenuItem>
                                <asp:MenuItem Text="Report2" Value="Report2"></asp:MenuItem>
                                <asp:MenuItem Text="Report3" Value="Report3"></asp:MenuItem>
                            </asp:MenuItem>
                            <asp:MenuItem Selectable="False" Text="Matrix" Value="Matrix">
                                <asp:MenuItem Text="Matrix1" Value="Matrix1"></asp:MenuItem>
                                <asp:MenuItem Text="Matrix2" Value="Matrix2"></asp:MenuItem>
                                <asp:MenuItem Text="Matrix3" Value="Matrix3"></asp:MenuItem>
                            </asp:MenuItem>
                        </Items>
                        <LevelMenuItemStyles>
                            <asp:MenuItemStyle Font-Bold="True" Font-Underline="False" ForeColor="Black" />
                            <asp:MenuItemStyle Font-Size="8pt" Font-Underline="False" />
                        </LevelMenuItemStyles>
                    </asp:Menu>
                    <br />
                    <span style="font-size: 8pt; font-family: Tahoma"><strong>Report Description</strong><span
                        style="font-size: 4pt; font-family: Times New Roman">&nbsp;<br />
                        &nbsp; </span></span>
                    <br />
                        <asp:Label ID="Label2" runat="server" BackColor="GhostWhite" 
                        Font-Bold="False" Font-Names="Tahoma"
                            Font-Size="8pt" Height="319px" Text="Report desc" Width="230px"></asp:Label></td>
                <td style="width: 641px; height: 826px" valign="top">
                    <span style="font-size: 10pt; font-family: Tahoma"><strong>
                        <span style="font-size: 4pt"> </span>
                        <cc1:WebReport ID="WebReport1" runat="server" BackColor="White" ButtonsPath="images\buttons1"
                            Font-Bold="False" Height="756px" OnStartReport="WebReport1_StartReport"
                            ToolbarColor="Lavender" Width="668px" Zoom="0.65" />
                    </strong></span>
                </td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>
