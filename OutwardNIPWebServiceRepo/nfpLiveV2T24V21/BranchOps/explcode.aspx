<%@ Page Language="C#" MasterPageFile="~/BranchOps/all.master" AutoEventWireup="true" CodeFile="explcode.aspx.cs" Inherits="BranchOps_explcode" Title="Manage Explanation Code" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<script type="text/javascript">
function loadDetails()
{
    document.getElementById("CurrentExpcode").innerHTML = "loading...";
    url = "../sync/getcurrentExpcode.aspx?r=" + rando();
    $("#CurrentExpcode").load(url);    
}
</script>
<table border="1" class="nibbstable" style="width: 697px">
    <tr>
        <td class="nibbstabletd" colspan="2">
            <wpt:infoBar ID="InfoBar1" runat="server" msg="" />
        </td>
    </tr>
    <tr>
        <td class="nibbstabletd" colspan="2" style="width: 598px">
            <h2>
                Explanation Code Setup</h2> <div id="CurrentExpcode" style=" font-family:Verdana; font-size:12px; color:#000; margin-top:2px; font-weight:bold;"></div>
        </td>
    </tr>
    <tr>
        <td class="nibbstabletd" colspan="2" style="width: 598px">
          <a href="#" onclick="loadDetails();">  View existing Explanation Code</a>
         
          </td>
    </tr>
    <tr>
        <td class="nibbstabletd" colspan="2" style="width: 598px">
<strong>Explanation Code:<asp:TextBox ID="txtexpcode" runat="server" MaxLength="3" Width="50px"></asp:TextBox></strong></td>
    </tr>
    <tr>
        <td class="nibbstabletd" colspan="2" style="width: 598px">
        <asp:Button ID="btn_submit" runat="server" Text="Submit" OnClick="btn_submit_Click" /></td>
    </tr>
          </table> 
</asp:Content>

