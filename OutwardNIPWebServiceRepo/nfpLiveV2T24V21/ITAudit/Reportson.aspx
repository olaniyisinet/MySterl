<%@ Page Language="C#" MasterPageFile="~/ITAudit/all.master" AutoEventWireup="true" CodeFile="Reportson.aspx.cs" Inherits="ITAudit_Reportson" Title="Untitled Page" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<table width="100%" class="mySearch">
<tr>
<td>
<table style="background-color:#999; width:100%; border:1px solid #333;">
    <tr>
        <td colspan="4">
            <h2>Search Report From Sterling Bank to NIBSS</h2></td>
    </tr>
<tr>
    <td>
<strong>Transaction SessionID</strong></td>
    <td>
DATE INITIATED START:</td>
<td>
DATE INITIATED END:</td>
    <td>
    </td>
</tr>
<tr>
    <td>
        <asp:TextBox ID="txtsessionid" runat="server"></asp:TextBox></td>
    <td>
<wpt:calendar id="calendar1" runat="server"></wpt:calendar>
    </td>
<td>
<wpt:calendar id="calendar2" runat="server"></wpt:calendar></td>
    <td>
            <asp:Button ID="btnSearch" runat="server" Text="Search" OnClick="btnSearch_Click" />
        |<asp:Button ID="btn_export" runat="server" OnClick="btn_export_Click" Text="Export to Excel" /></td>
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
        <asp:BoundField DataField="sessionid" HeaderText="Sessionid" />
        <asp:BoundField DataField="channelcode" HeaderText="Channelcode" />
        <asp:BoundField DataField="paymentRef" HeaderText="PaymentRef" />
        <asp:BoundField DataField="mandateRefNum" HeaderText="MandateRefNum" />
        <asp:BoundField DataField="BillerID" HeaderText="BillerID" />
        <asp:BoundField DataField="BillerName" HeaderText="BillerName" />
        <asp:BoundField DataField="benebank" HeaderText="BeneficiaryBank" />
        <asp:BoundField DataField="benename" HeaderText="BeneficiaryName" />
         <asp:TemplateField>
            <HeaderTemplate>
                Amt
            </HeaderTemplate>
            <ItemTemplate>
          <%# g.printMoney (Convert.ToDecimal((DataBinder.Eval(Container, "DataItem.amt"))))%>
            </ItemTemplate>
        </asp:TemplateField>
        
        <asp:BoundField DataField="cusnum" HeaderText="Customer Account" />
        <asp:BoundField DataField="accname" HeaderText="AccountName" />
        <asp:BoundField DataField="Remark" HeaderText="Remark" />
        <asp:BoundField DataField="inputedby" HeaderText="InputBy" />
        <asp:BoundField DataField="inputdate" HeaderText="Inputdate" />
        <asp:BoundField DataField="approvedby" HeaderText="Approvedby" />
        <asp:BoundField DataField="approveddate" HeaderText="Approveddate" />
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

