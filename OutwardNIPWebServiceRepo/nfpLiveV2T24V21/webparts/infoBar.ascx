<%@ Control Language="C#" AutoEventWireup="true" CodeFile="infoBar.ascx.cs" Inherits="webparts_infoBar" %>
<asp:Panel ID="infoBox" runat="server">
<table width="100%" cellpadding="2" cellspacing="0" border="0">
<tr>
<td valign="top"><asp:Literal ID="lblMsg" runat="server"></asp:Literal>
</td>
<td valign="top">
<img alt="close" src="../images/close.png" onclick="closeInfo();" style=" float:right; cursor:pointer;" /></td>
</tr>
</table>
</asp:Panel>