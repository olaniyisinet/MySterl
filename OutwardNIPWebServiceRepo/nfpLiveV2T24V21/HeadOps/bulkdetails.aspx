<%@ Page Language="C#" MasterPageFile="~/HeadOps/trans.master" AutoEventWireup="true" CodeFile="bulkdetails.aspx.cs" Inherits="HeadOps_bulkdetails" Title="NFP | Bulk Details" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<script type="text/javascript">
function Reload()
{
  location.reload();
}
//setTimeout("Reload();",5000);
</script>
<table border="0" class="mytable" style="width: 84%">
  <tr>
    <td class="mytabletd" colspan="2"><h2>
        Bulk Transaction Details</h2>
        <asp:Label ID="lblmsg" runat="server" Font-Bold="True" Font-Italic="True" ForeColor="Blue"></asp:Label>
        <asp:Label ID="lblFee" runat="server"></asp:Label>

         <wpt:infoBar ID="ibox" runat="server" msg="" />

        </td>
  </tr>
  
  <tr>
  </tr>
  <tr>
  </tr>
  <tr>
  </tr>
  <tr>
    <td class="mytabletd" style="width: 605px"><a href="javascript:history.go(-1)"></a></td>
    <td class="mytabletd">
        <asp:Button ID="btn_verify" runat="server" Text="Verify from NIBSS" OnClick="btn_verify_Click" />
        | &nbsp;<asp:Button ID="btn_approve" runat="server" Text="Approve" OnClick="btn_approve_Click" />
        |
        <asp:Button ID="btn_reject" runat="server" Text="Reject" OnClick="btn_reject_Click" />
        |<a href="#" onclick='Reload();'>Refresh</a> </td>
  </tr>
    <tr>
        <td class="mytabletd" colspan="2">
            <asp:GridView ID="grddisplay" runat="server" AutoGenerateColumns="False" CellPadding="4"
                ForeColor="#333333" Width="100%">
                <RowStyle BackColor="#FFFBD6" ForeColor="#333333" HorizontalAlign="Center" />
                <Columns>
                    <asp:BoundField DataField="Refid" HeaderText="Refid" />
                    <asp:BoundField DataField="BatchNumber" HeaderText="Batch Code" />
                    <asp:BoundField DataField="Orig_bra_code" HeaderText="OrigBranchcode" />
                    <asp:BoundField DataField="Benename" HeaderText="Beneficiary Name" />
                    <asp:BoundField DataField="Beneaccount" HeaderText="Beneficiary Account" />
                    <asp:BoundField DataField="Benebank" HeaderText="BENEBANK" />
                    <asp:BoundField DataField="accname" HeaderText="Originator Name" />
                    <asp:BoundField DataField="Remark" HeaderText="Narration" />
                    <asp:BoundField DataField="PaymentRef" HeaderText="Payment Reference" />
                    <asp:BoundField DataField="amt" HeaderText="Amount" />
                    <asp:BoundField DataField="inputedby" HeaderText="Addedby" />
                    <asp:BoundField DataField="inputdate" HeaderText="Dateadded" />  
                <asp:TemplateField>
               <HeaderTemplate>Status</HeaderTemplate>
                <ItemTemplate>
                <%#g.getStatus (Convert.ToString(DataBinder.Eval(Container, "DataItem.statusflag")))%>
                
                </ItemTemplate>
              </asp:TemplateField>
                </Columns>
                <FooterStyle BackColor="#990000" Font-Bold="True" ForeColor="White" />
                <PagerStyle BackColor="#FFCC66" ForeColor="#333333" HorizontalAlign="Center" />
                <SelectedRowStyle BackColor="#FFCC66" Font-Bold="True" ForeColor="Navy" />
                <HeaderStyle BackColor="#990000" Font-Bold="True" ForeColor="White" />
                <AlternatingRowStyle BackColor="White" />
            </asp:GridView>
        </td>
    </tr>
    <tr>
        <td class="mytabletd" style="width: 605px">
            <a href="javascript:history.go(-1)">Back</a></td>
        <td class="mytabletd">
        </td>
    </tr>
</table>
</asp:Content>

