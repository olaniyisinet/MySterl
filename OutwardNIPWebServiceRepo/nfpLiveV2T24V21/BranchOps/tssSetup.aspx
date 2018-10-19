<%@ Page Language="C#" MasterPageFile="~/BranchOps/all.master" AutoEventWireup="true" CodeFile="tssSetup.aspx.cs" Inherits="BranchOps_tssSetup" Title="NFP | Tss" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<script type="text/javascript">
function loadDetails()
{
    document.getElementById("CurrentTss").innerHTML = "loading...";
    url = "../sync/getcurrentTss.aspx?r=" + rando();
    $("#CurrentTss").load(url);    
}
</script>
<table border="1" class="nibbstable" style="width: 697px">
    <tr>
        <td class="nibbstabletd" colspan="2">
            <wpt:infoBar ID="InfoBar1" runat="server" />
        </td>
    </tr>
    <tr>
        <td class="nibbstabletd" colspan="2" style="width: 598px">
            <h2>
                TSS Setup</h2> <div id="CurrentTss" style=" font-family:Verdana; font-size:12px; color:#000; margin-top:2px; font-weight:bold;"></div>
        </td>
    </tr>
    <tr>
        <td class="nibbstabletd" colspan="2" style="width: 598px">
          <a href="#" onclick="loadDetails();">  View existing Tss Account</a>
         
          </td>
    </tr>
    <tr>
        <td class="nibbstabletd" colspan="2" style="width: 598px">
<strong>Bracode:<asp:TextBox ID="txtbracode" runat="server" MaxLength="3" Width="50px"></asp:TextBox>Cusnum</strong>:<asp:TextBox ID="txtcusnum" runat="server"
                    MaxLength="7" Width="74px"></asp:TextBox><strong>Currency</strong>:<asp:TextBox ID="txtcurrency"
                        runat="server" MaxLength="3" Width="50px"></asp:TextBox><strong>Ledger</strong>:<asp:TextBox
                            ID="txtledger" runat="server" MaxLength="3" Width="50px"></asp:TextBox><strong>Subaccount:</strong><asp:TextBox
                                ID="txtsubact" runat="server" MaxLength="3" Width="50px"></asp:TextBox></td>
    </tr>
    <tr>
        <td class="nibbstabletd" colspan="2" style="width: 598px">
        <asp:Button ID="btn_submit" runat="server" Text="Submit" OnClick="btn_submit_Click" /></td>
    </tr>
          </table> 
</asp:Content>

