<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="UCBase.ascx.vb" Inherits="LAEACC.UCBase" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="AjaxToolkit" %>

<!--#功能項目集-->
<div style="float:left;">    
    <div id="Modify" style="float:left;" runat="server">
        <span style="color:#666; font-size:18px; font-weight:bold; padding:8px 0px 0px 0px;">|</span>
        <asp:Button ID="btnSave" Text="存檔" CommandName="Save" CssClass="btn btn-primary" OnClientClick="showBlockUI();" runat="server" />
        <asp:Button ID="btnCancelEdit" Text="取消" CommandName="CancelEdit" CssClass="btn btn-primary" runat="server" />
        <asp:Button ID="btnCopy" Text="複製" CommandName="Copy" CssClass="btn btn-primary" runat="server" />
        <asp:Button ID="btnAddNew" Text="新增" CommandName="AddNew" CssClass="btn btn-primary" runat="server" />
        <asp:Button ID="btnEdit" Text="修改" CommandName="Edit" CssClass="btn btn-primary" runat="server" />
        <asp:Button ID="btnDelete" Text="刪除" CommandName="Delete" CssClass="btn btn-primary" runat="server" />

        <asp:UpdatePanel ID="UpdatePanel1" Visible="false" runat="server">
            <Triggers>
                <asp:PostBackTrigger ControlID="btnSave" />
            </Triggers>
        </asp:UpdatePanel>
    </div>
    <div id="Navigation" style="float:left;" runat="server">
        <span style="color:#666; font-size:18px; font-weight:bold; padding:8px 0px 0px 0px;">|</span>
        <asp:Button ID="btnFirst" Text="首筆" CommandName="First" CssClass="btn btn-primary" runat="server" />
        <asp:Button ID="btnPrior" Text="上筆" CommandName="Prior" CssClass="btn btn-primary" runat="server" />
        <asp:Button ID="btnNext" Text="下筆" CommandName="Next" CssClass="btn btn-primary" runat="server" />
        <asp:Button ID="btnLast" Text="末筆" CommandName="Last" CssClass="btn btn-primary" runat="server" />
    </div>
    <div id="Other" style="float:left;" runat="server">
        <span style="color:#666; font-size:18px; font-weight:bold; padding:8px 0px 0px 0px;">|</span>
        <asp:Button ID="btnPrint" Text="列印" CommandName="Print" CssClass="btn btn-primary" runat="server" />
    </div>
</div>

<!--#伺服器驗證-->
<AjaxToolkit:ConfirmButtonExtender ID="ConfirmButtonExtender1" runat="server"
    TargetControlID="btnDelete" ConfirmText="您是否確定刪除??">
</AjaxToolkit:ConfirmButtonExtender>