<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Default.aspx.vb" Inherits="_Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>easiRTGS</title>
    <style type="text/css">
        .style1
        {
            width: 535px;
            border-collapse: collapse;
        }
        .style2
        {
            height: 21px;
        }
        .style3
        {
            width: 600px;
            height: 100px;
        }
        .style4
        {
            width: 600px;
            height: 150px;
        }
        .style5
        {
            height: 228px;
        }
        .style6
        {
            font-size: small;
        }
        .style7
        {
            font-size: small;
            color: #800000;
        }
        .style8
        {
            font-family: "Segoe UI Light";
            font-size: medium;
        }
    </style>
</head>
<body bgproperties="fixed" background="f.png" 
    style="left:10px; right:10px; background-repeat:repeat-x; font-size: 11pt; font-family: Verdana;">
    <form id="form1" runat="server">


    <div>
    
        <table class="style1" align="center">
            <tr>
                <td class="style5">
                    <br />
                    <br />
                    <br />
                    <br />
                    <br />
                    <br />
                    <br />
                    <img alt="" class="style3" src="images/header.png" /></td>
            </tr>
            <tr>
                <td bgcolor="#FFFFCC" class="style6">
                    &nbsp;
                    <asp:Label ID="Label1" runat="server" Font-Bold="True" ForeColor="#FF3300" 
                        Font-Names="Segoe UI Light" Font-Size="12pt"></asp:Label>
                    <br />
                </td>
            </tr>
            <tr>
                <td bgcolor="#FFFFCC" class="style7">
                    <strong>&nbsp;</strong><span class="style8">Username</span></td>
            </tr>
            <tr>
                <td bgcolor="#FFFFCC">
                    &nbsp;
                    <asp:TextBox  AutoComplete="off" ID="TextBox1" runat="server" BorderStyle="Solid" BorderWidth="1px" 
                        Height="35px" Width="302px" CssClass="style6" Font-Size="12pt" 
                        Font-Names="Segoe UI Light"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td bgcolor="#FFFFCC" class="style7">
                    <strong>&nbsp;</strong><span class="style8">Password</span></td>
            </tr>
            <tr>
                <td bgcolor="#FFFFCC">
                    &nbsp;
                    <asp:TextBox  AutoComplete="off" ID="TextBox2" runat="server" BorderStyle="Solid" BorderWidth="1px" 
                        Height="35px" Width="302px" CssClass="style6" Font-Size="12pt" 
                        TextMode="Password" Font-Names="Segoe UI Light"></asp:TextBox>
                    <br />
                </td>
            </tr>
            <tr>
                <td bgcolor="#FFFFCC">
                    &nbsp;
                    <asp:Button ID="Button1" runat="server" Height="37px" Text="Sign In" 
                        Width="131px" BackColor="#990000" BorderStyle="Solid" BorderWidth="1px" 
                        CssClass="style6" ForeColor="White" Font-Names="Segoe UI Light" 
                        Font-Size="12pt" />
                </td>
            </tr>
             <tr>
                <td bgcolor="#FFFFCC">
                    &nbsp;
                    <asp:Button ID="btnGen103" runat="server" Height="37px" Text="Test GenMt103" 
                        Width="131px" BackColor="#990000" BorderStyle="Solid" BorderWidth="1px" 
                        CssClass="style6" ForeColor="White" Font-Names="Segoe UI Light" 
                        Font-Size="12pt" Visible="False" />
                &nbsp;<asp:Button ID="btnTestNfiu" runat="server" Height="37px" Text="Test Nfiu" 
                        Width="131px" BackColor="#990000" BorderStyle="Solid" BorderWidth="1px" 
                        CssClass="style6" ForeColor="White" Font-Names="Segoe UI Light" 
                        Font-Size="12pt" Visible="False" />
                    <br />
                    <asp:TextBox ID="txtMtResult" runat="server" Height="53px" TextMode="MultiLine" 
                        Visible="False" Width="521px"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td bgcolor="#FFFFCC">
                    &nbsp;</td>
            </tr>
            <tr>
                <td class="style2">
                    <img alt="" class="style4" src="images/footer.png" /></td>
            </tr>
        </table>
    
    </div>
    </form>
</body>
</html>
