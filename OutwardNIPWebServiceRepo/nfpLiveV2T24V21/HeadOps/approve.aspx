<%@ Page Language="C#" MasterPageFile="~/HeadOps/trans.master" AutoEventWireup="true" CodeFile="approve.aspx.cs" Inherits="HeadOps_approve" Title="NIBBS | Hop Approve" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<table border="0" class="mytable" style="width: 100%">
  <tr>
      <td class="mytabletd" colspan="2">
      <h2>HOP Approval</h2>
      </td>
  </tr>
  <tr>
    <td class="mytabletd" style="width: 193px" valign="top">
        Status: &nbsp;&nbsp;<asp:DropDownList ID="ddlStatus" runat="server" Font-Size="11px">
            <asp:ListItem Selected="true" Text="All Pending" Value="0"></asp:ListItem>
            <asp:ListItem Text="approved" Value="1"></asp:ListItem>
            <asp:ListItem Text="rejected" Value="2"></asp:ListItem>
        </asp:DropDownList><br />
        Created:
        <asp:DropDownList ID="ddlBinop" runat="server" Font-Size="11px">
            <asp:ListItem Text=" " Value="any"></asp:ListItem>
            <asp:ListItem Text="on" Value="is"></asp:ListItem>
            <asp:ListItem Text="before" Value="is before"></asp:ListItem>
            <asp:ListItem Text="after" Value="is after"></asp:ListItem>
        </asp:DropDownList>
        <br />
        <asp:Calendar ID="Calendar2" runat="server" Width="149px">
            <NextPrevStyle ForeColor="White" />
            <TitleStyle BackColor="#A00008" ForeColor="White" />
        </asp:Calendar>
        <asp:Button ID="Button1" runat="server" OnClick="Button1_Click" Text="Search" />
    </td>
    <td class="mytabletd" valign="top" style="width: 1007px">&nbsp;<asp:Label ID="lblmsg" runat="server" Font-Bold="True" Font-Italic="True" ForeColor="Blue"></asp:Label>
        <asp:GridView ID="grddisplay" runat="server" AutoGenerateColumns="False" CellPadding="4"
            ForeColor="#333333" GridLines="None" Width="100%">
            <RowStyle BackColor="#FFFBD6" ForeColor="#333333" />
            <FooterStyle BackColor="#990000" Font-Bold="True" ForeColor="White" />
            <PagerStyle BackColor="#FFCC66" ForeColor="#333333" HorizontalAlign="Center" />
            <SelectedRowStyle BackColor="#FFCC66" Font-Bold="True" ForeColor="Navy" />
            <HeaderStyle BackColor="#990000" Font-Bold="True" ForeColor="White" />
            <AlternatingRowStyle BackColor="White" />
               <Columns>
               
                <asp:TemplateField>
      <HeaderTemplate>Status</HeaderTemplate>
                <ItemTemplate>
                <%# fm.getStatus(Convert.ToString(DataBinder.Eval(Container, "DataItem.APPROVEVALUE")))%>
                
                </ItemTemplate>
              </asp:TemplateField>
                <asp:BoundField DataField="FORMNUM" HeaderText="Document No" />
                <asp:BoundField HeaderText="Orig_Bra_Code" DataField="Orig_bra_code" />
                <asp:BoundField DataField="CustomerAccount" HeaderText="CustomerAccount" />
                <asp:BoundField HeaderText="Transaction Code" DataField="transactioncode" />
                <asp:BoundField HeaderText="Amount" DataField="amt" />
                <asp:BoundField HeaderText="Inputedby" DataField="inputedby" />
                <asp:BoundField HeaderText="Beneficiary Bank" DataField="Benebank" />
                <asp:BoundField HeaderText="Beneficiary Name" DataField="Benename" />
                <asp:BoundField HeaderText="Beneficary Account" DataField="Beneaccount" /> 
               
                <asp:TemplateField>
                <ItemTemplate>
                


                <a href="Viewdatails.aspx?id=<%# DataBinder.Eval(Container, "DataItem.Refid")%>" target="_blank">Detail</a>
                <br />

                <a href="../Sync/getImages.aspx?id=<%# DataBinder.Eval(Container, "DataItem.Refid")%>&blob=1" target="_blank">Image</a>
                </ItemTemplate>
              </asp:TemplateField>
            </Columns>
        </asp:GridView>
        <asp:Button ID="btn_export" runat="server" Text="Export to Excel" Enabled="False" OnClick="btn_export_Click" /></td>
  </tr>
</table>
</asp:Content>

