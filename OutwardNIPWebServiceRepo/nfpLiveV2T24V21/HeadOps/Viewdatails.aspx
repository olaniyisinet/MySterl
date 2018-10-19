<%@ Page Language="C#" MasterPageFile="~/HeadOps/trans.master" AutoEventWireup="true" CodeFile="Viewdatails.aspx.cs" Inherits="HeadOps_Viewdatails" Title="NIBBS | View details" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">

<script type="text/javascript">
setCookie("scrh",screen.availHeight,1);
</script>
<wpt:infoBar ID="ibox" runat="server" msg="" />
<table width="100%" border="0" class="mytable">
  <tr>
    <td class="mytabletd" colspan="2"><h2>Transaction Details</h2>
        <asp:Label ID="lblmsg" runat="server" Font-Bold="True" Font-Italic="True" ForeColor="Blue"></asp:Label></td>
  </tr>
    <tr>
        <td class="mytabletd" style="width: 383px">
            <strong>Teller Details</strong></td>
        <td class="mytabletd">
        </td>
    </tr>
    <tr>
        <td class="mytabletd" style="width: 383px">
            &nbsp;Originating Branch Code:</td>
        <td class="mytabletd">
            &nbsp;<asp:Label ID="lblbranchcode" runat="server" Font-Bold="True"></asp:Label></td>
    </tr>
    <tr>
        <td class="mytabletd" style="width: 383px">
            &nbsp;Inputer Name:</td>
        <td class="mytabletd">
            &nbsp;<asp:Label ID="lblinputername" runat="server" Font-Bold="True"></asp:Label></td>
    </tr>
    <tr>
        <td class="mytabletd" colspan="2">
            <hr />
        </td>
    </tr>
    <tr>
        <td class="mytabletd" style="width: 383px">
            <strong>Customer Details</strong></td>
        <td class="mytabletd">
        </td>
    </tr>
    <tr>
        <td class="mytabletd" style="width: 383px">
            &nbsp;Customer Name:</td>
        <td class="mytabletd">
            &nbsp;<asp:Label ID="lblcusname" runat="server" Font-Bold="True"></asp:Label></td>
    </tr>
    <tr>
        <td class="mytabletd" style="width: 383px">
            &nbsp;Customer Account:</td>
        <td class="mytabletd">
            &nbsp;<asp:Label ID="lblcusacct" runat="server" Font-Bold="True"></asp:Label></td>
    </tr>
    <tr>
        <td class="mytabletd" style="width: 383px">
            &nbsp;Current Balance:</td>
        <td class="mytabletd">
            &nbsp;<asp:Label ID="lblcurbal" runat="server" Font-Bold="True" ></asp:Label></td>
    </tr>
    <tr>
        <td class="mytabletd" colspan="2">
            <hr />
        </td>
    </tr>
    <tr>
        <td class="mytabletd" style="width: 383px">
            <strong>Beneficiary Details</strong></td>
        <td class="mytabletd">
        </td>
    </tr>
    <tr>
        <td class="mytabletd" style="width: 383px">
            &nbsp;Beneficiary Name:</td>
        <td class="mytabletd">
            &nbsp;<asp:Label ID="lblbenname" runat="server" Font-Bold="True"></asp:Label></td>
    </tr>
  <tr>
    <td class="mytabletd" style="width: 383px">&nbsp;Beneficiary Account Number:</td>
    <td class="mytabletd">&nbsp;<asp:Label ID="lblbenacctnum" runat="server" Font-Bold="True"></asp:Label></td>
  </tr>
  <tr>
    <td class="mytabletd" style="width: 383px">&nbsp;Beneficiary Bank</td>
    <td class="mytabletd">&nbsp;<asp:Label ID="Lblbenbank" runat="server" Font-Bold="True"></asp:Label></td>
  </tr>
    <tr>
        <td class="mytabletd" colspan="2">
            <hr />
        </td>
    </tr>
    <tr>
        <td class="mytabletd" style="width: 383px">
            <strong>Transaction Details</strong></td>
        <td class="mytabletd">
        </td>
    </tr>
    <tr>
        <td class="mytabletd" style="width: 383px">
            &nbsp;Document Number:</td>
        <td class="mytabletd">
            &nbsp;<asp:Label ID="lbldocument" runat="server" Font-Bold="True"></asp:Label></td>
    </tr>
  <tr>
    <td class="mytabletd" style="width: 383px">&nbsp;Amount:</td>
    <td class="mytabletd">&nbsp;<asp:Label ID="lblamt" runat="server" Font-Bold="True"></asp:Label></td>
  </tr>
  <tr>
    <td class="mytabletd" style="width: 383px">&nbsp;Amount in Words:</td>
    <td class="mytabletd">&nbsp;<asp:Label ID="lblamtinword" runat="server" Font-Bold="True"></asp:Label></td>
  </tr>
    <tr>
        <td class="mytabletd" style="width: 383px">
            &nbsp;Transaction Fee:</td>
        <td class="mytabletd">
            &nbsp;<asp:Label ID="lblFee" runat="server" Font-Bold="True"></asp:Label></td>
    </tr>
  <tr>
    <td class="mytabletd" style="width: 383px">&nbsp;Transaction Date:</td>
    <td class="mytabletd">&nbsp;<asp:Label ID="lbltransdate" runat="server" Font-Bold="True"></asp:Label></td>
  </tr>
  <tr>
    <td class="mytabletd" style="width: 383px">&nbsp;</td>
    <td class="mytabletd">&nbsp;<asp:HyperLink ID="hypblob1" runat="server" Font-Bold="True">View NIBBS Transfer Instruction</asp:HyperLink></td>
  </tr>
  <tr>
    <td class="mytabletd" style="width: 383px"><a href="javascript:history.go(-1)">Back</a></td>
    <td class="mytabletd">&nbsp;<asp:Button ID="btn_approve" runat="server" Text="Approve" OnClick="btn_approve_Click" />
        |
        <asp:Button ID="btn_reject" runat="server" Text="Reject" OnClick="btn_reject_Click" /></td>
  </tr>
</table>
</asp:Content>

