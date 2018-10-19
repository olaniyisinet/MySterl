<%@ Page Language="C#" MasterPageFile="~/Domops/all.master" AutoEventWireup="true" CodeFile="trnxSummary.aspx.cs" Inherits="Domops_trnxSummary" Title="NFP | Transaction Summary" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<table width="100%" class="mySearch">
<tr>
<td>
<table style="background-color:#999; width:100%; border:1px solid #333;">
<tr>
    <td>
<strong>TRANSACTION SUMMARY SEARCH</strong>
    </td>
    <td>
DATE INITIATED START:</td>
<td>
DATE INITIATED END:</td>
    <td>
    </td>
</tr>
<tr>
    <td>&nbsp;
    </td>
    <td>
<wpt:calendar id="calendar1" runat="server"></wpt:calendar>
    </td>
<td>
<wpt:calendar id="calendar2" runat="server"></wpt:calendar></td>
    <td>
            <asp:Button ID="btnSearch" runat="server" Text="Search" OnClick="btnSearch_Click" /></td>
</tr>
</table>

</td>
</tr>
<tr>
<td valign="top">
    <asp:Label ID="lblmsg" runat="server" Font-Bold="True" ForeColor="Blue"></asp:Label><br />
<asp:GridView ID="gvTransactions" runat="server" CellPadding="2"
     AutoGenerateColumns="False" Width="100%">
    <Columns>
        <asp:BoundField DataField="BankName" HeaderText="BankCode" />
        <asp:TemplateField>
            <HeaderTemplate>
            BankName
            </HeaderTemplate>
            <ItemTemplate>
          <%#  getBankName(Convert.ToString(DataBinder.Eval(Container, "DataItem.BankName")))%>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:BoundField DataField="Total" HeaderText="Total Count" />
    </Columns>
       <EmptyDataTemplate>No results for this search!</EmptyDataTemplate>
    <HeaderStyle HorizontalAlign="Left" />
    <RowStyle HorizontalAlign="Center" />
    </asp:GridView>
    
    
<div id="bxtrnxDetails" style="margin-top:30px;"></div>
</td>
</tr>
</table>
</asp:Content>

