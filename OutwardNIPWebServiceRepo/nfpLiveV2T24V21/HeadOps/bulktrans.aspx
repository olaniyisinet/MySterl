<%@ Page Language="C#" MasterPageFile="~/HeadOps/trans.master" AutoEventWireup="true" CodeFile="bulktrans.aspx.cs" Inherits="HeadOps_bulktrans" Title="NFP Bulk" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<table border="0" class="mytable" style="width: 100%">
  <tr>
      <td class="mytabletd" colspan="2">
      <h2>HOP View Bulk Transactions</h2>
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
            <RowStyle BackColor="#FFFBD6" ForeColor="#333333" HorizontalAlign="Center" />
            <FooterStyle BackColor="#990000" Font-Bold="True" ForeColor="White" />
            <PagerStyle BackColor="#FFCC66" ForeColor="#333333" HorizontalAlign="Center" />
            <SelectedRowStyle BackColor="#FFCC66" Font-Bold="True" ForeColor="Navy" />
            <HeaderStyle BackColor="#990000" Font-Bold="True" ForeColor="White" />
            <AlternatingRowStyle BackColor="White" />
               <Columns>
                <asp:TemplateField HeaderText="S/N">
                 <ItemTemplate>
                      <%# Container.DataItemIndex + 1 %>
                 </ItemTemplate>
               </asp:TemplateField>
               
                          <asp:TemplateField HeaderText="Bulk Batch code">
                <ItemTemplate>
                
                <a href="bulkdetails.aspx?id=<%# DataBinder.Eval(Container, "DataItem.batchnumber")%>">Detail</a>

                </ItemTemplate>
              </asp:TemplateField>
            </Columns>
        </asp:GridView>
        <asp:Button ID="btn_export" runat="server" Text="Export to Excel" Enabled="False" OnClick="btn_export_Click" /></td>
  </tr>
</table>
</asp:Content>

