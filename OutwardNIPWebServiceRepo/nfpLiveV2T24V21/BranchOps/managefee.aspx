<%@ Page Language="C#" MasterPageFile="~/BranchOps/all.master" AutoEventWireup="true" CodeFile="managefee.aspx.cs" Inherits="BranchOps_managefee" Title="Manage Fees" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<script type="text/javascript">
function loadDetails()
{
    document.getElementById("CurrentAmts").innerHTML = "loading...";
    url = "../sync/getCurrentFees.aspx?r=" + rando();
    $("#CurrentAmts").load(url);    
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
                Fees Setup</h2> <div id="CurrentAmts" style=" font-family:Verdana; font-size:12px; color:#000; margin-top:2px; font-weight:bold;"></div>
        </td>
    </tr>
    <tr>
        <td class="nibbstabletd" colspan="2" style="width: 598px">
          <a href="#" onclick="loadDetails();">  View existing Fees</a>
         
          </td>
    </tr>
    <tr>
        <td class="nibbstabletd" colspan="2" style="width: 598px">
<strong>Amt1:<asp:TextBox ID="txtamt1" runat="server" MaxLength="3" Width="50px"></asp:TextBox>&nbsp;
    Amt2</strong>:<asp:TextBox ID="txtamt2" runat="server"
                    MaxLength="7" Width="50px"></asp:TextBox>&nbsp; <strong>Amt3</strong>:<asp:TextBox ID="txtamt3"
                        runat="server" MaxLength="3" Width="50px"></asp:TextBox></td>
    </tr>
    <tr>
        <td class="nibbstabletd" colspan="2" style="width: 598px">
        <asp:Button ID="btn_submit" runat="server" Text="Submit" OnClick="btn_submit_Click" /></td>
    </tr>
          </table> 
</asp:Content>

