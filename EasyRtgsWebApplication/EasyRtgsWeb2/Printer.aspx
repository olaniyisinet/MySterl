<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Printer.aspx.vb" Inherits="Printer" title="Print SWIFT" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
   
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
        <asp:Panel ID="pnlEncrypted" runat="server">
            <asp:Button ID="btnPrint" runat="server" Text="Print Telex" 
    Visible="True" />
            <br />
            <asp:Label ID="lblError" runat="server" Font-Bold="True" 
                Text="There is no data to print here." Visible="False"></asp:Label>
            <br />
            <asp:TextBox ID="txtPrintArea" runat="server" Height="1900px" 
                TextMode="MultiLine" Width="1000px"></asp:TextBox>
        </asp:Panel>
        <asp:Panel ID="pnlExpanded" runat="server">
            <asp:Button ID="btnPrintTelex" runat="server" Text="Print Expanded" 
    Visible="True"/>
            <br />
            <asp:Label ID="lblErrorExpanded" runat="server" Font-Bold="True" 
                Text="There is no data to print here." Visible="False"></asp:Label>
            <br />
            <asp:TextBox ID="txtPrintAreaExt" runat="server" Height="1900px" 
                TextMode="MultiLine" Width="1000px"></asp:TextBox>
        </asp:Panel>
        <br />
        <br />
    
        <br />
        <br />
        <br />
    
        <br />
        <br />
    
    </div>
    </form>
</body>
</html>
