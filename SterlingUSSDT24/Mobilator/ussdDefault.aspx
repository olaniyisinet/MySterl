<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ussdDefault.aspx.cs" Inherits="ussdDefault" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <title></title>
    <script type="text/javascript" src="jquery.min.js"></script>

 
</head>
<body>
    <form id="form1" runat="server">
    <div style="padding:0px; width:320px; margin:0 auto;">
        <h2>USSD Service Simulator</h2>
        <table bgcolor="#FFCC66" border="1" cellpadding="4" cellspacing="0">
            <tr>
                <td align="right">
                    <asp:Button ID="btnNewSession" runat="server" Text="New Session" OnClick="btnNewSession_Click" />
                </td>
                <td align="right">
                    &nbsp;<asp:TextBox ID="txtSessionID" runat="server" 
                        Width="150px"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td align="right" colspan="2">
                    Mobile: <asp:TextBox ID="txtMobile" runat="server" Width="150px" Text="234"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td align="right" colspan="2">
                    Network:  
                    <asp:DropDownList ID="txtNetwork" runat="server" Width="150px">
                        <asp:ListItem Value="1">ETISALAT</asp:ListItem>
                        <asp:ListItem Value="2">GLO</asp:ListItem>
                        <asp:ListItem Value="3">AIRTEL</asp:ListItem>
                        <asp:ListItem Value="4">MTN</asp:ListItem>
                    </asp:DropDownList>
 
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    Screen: 
                    <asp:Literal ID="lblCnt" runat="server"></asp:Literal> xter(s)</td>
            </tr>
            <tr>
                <td align="center" colspan="2">
                    <asp:TextBox ID="txtScreen" TextMode="MultiLine" Rows="11" runat="server" Width="250px" Enabled="false"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td align="right" colspan="2">Input:
                    <asp:TextBox ID="lblinput" runat="server" ></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <div style="cursor:pointer" >
                    <table bgcolor="#CCFFFF" border="1" cellpadding="3" cellspacing="3" 
                        style="width:100%;">
                        <tr>
                            <td align="center" onclick="addToText('1');">
                                1</td>
                            <td align="center" onclick="addToText('2');">
                                2</td>
                            <td align="center" onclick="addToText('3');">
                                3</td>
                        </tr>
                        <tr>
                            <td align="center" onclick="addToText('4');">
                                4</td>
                            <td align="center" onclick="addToText('5');">
                                5</td>
                            <td align="center" onclick="addToText('6');">
                                6</td>
                        </tr>
                        <tr>
                            <td align="center" onclick="addToText('7');">
                                7</td>
                            <td align="center" onclick="addToText('8');">
                                8</td>
                            <td align="center" onclick="addToText('9');">
                                9</td>
                        </tr>
                        <tr>
                            <td align="center" onclick="addToText('*');">
                                *</td>
                            <td align="center" onclick="addToText('0');">
                                0</td>
                            <td align="center" onclick="addToText('#');">
                                #</td>
                        </tr>
                    </table>
                        </div>
                </td>
            </tr>
            <tr>
                <td align="center" colspan="2">
                    <asp:Button ID="btnDial" runat="server" Text="Dial" onclick="btnDial_Click"  /> - 
                    <asp:Button ID="btnSend" runat="server" Text="Send" onclick="btnSend_Click" /> - 
                    <asp:Button ID="btnClear" runat="server" Text="Clear" OnClick="btnClear_Click" /> - 
                    <asp:Button ID="btnEnd" runat="server" Text="End" onclick="btnEnd_Click" />
                </td>
            </tr>
        </table>
    </div>
    </form>
</body>
   <script type="text/javascript">
       function addToText(itm) {
           var txt = $("#lblinput").val();
           $("#lblinput").val(txt + itm);
       }

       $("#btnClear").click(function () {
           $("#lblinput").val("");
       });
    </script>
</html>


