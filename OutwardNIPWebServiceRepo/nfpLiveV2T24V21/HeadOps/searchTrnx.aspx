<%@ Page Language="C#" MasterPageFile="~/HeadOps/trans.master" AutoEventWireup="true" CodeFile="searchTrnx.aspx.cs" Inherits="HeadOps_searchTrnx" Title="HOP Approve" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">

<table width="100%" class="mySearch">
<tr>
<td>
<table style="background-color:#999; width:100%; border:1px solid #333;">
<tr>
    <td>
<strong>TRANSACTION SEARCH</strong>
    </td>
    <td>
DATE INITIATED START:</td>
<td>
DATE INITIATED END:</td>
    <td>
        CATEGORY</td>
    <td>
        KEYWORD</td>
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
        <asp:DropDownList ID="ddlCategory" runat="server">
        <asp:ListItem Text="All Category" Value=""></asp:ListItem>
        <asp:ListItem Text="Transaction Code" Value="1"></asp:ListItem>
        <asp:ListItem Text="Account Number eg(223-12233-1-1-0)" Value="2"></asp:ListItem>
        <asp:ListItem Text="Initiator" Value="3"></asp:ListItem>
        <asp:ListItem Text="Approver" Value="4"></asp:ListItem>
        <asp:ListItem Text="Type (0:Single;1:Bulk)" Value="5"></asp:ListItem>
        <asp:ListItem Text="Status (0:Pending;1:Approved;2:Rejected)" Value="6"></asp:ListItem>
        </asp:DropDownList></td>
    <td>
        <asp:TextBox ID="txtVal" runat="server"></asp:TextBox></td>
    <td>
            <asp:Button ID="btnSearch" runat="server" Text="Search" OnClick="btnSearch_Click" /></td>
</tr>
</table>

</td>
</tr>
<tr>
<td valign="top">
<asp:GridView ID="gvTransactions" runat="server" CellPadding="2"
     AutoGenerateColumns="False" Width="100%">
    <Columns>
        <asp:BoundField DataField="transactioncode" HeaderText="Transaction Code" />
        <asp:BoundField DataField="accnum" HeaderText="Customer Account" />
        <asp:BoundField DataField="accname" HeaderText="Customer Name" />
        <asp:BoundField DataField="inputedby" HeaderText="Initiator" />
        <asp:BoundField DataField="inputdate" HeaderText="Initiation Date" />
        <asp:BoundField DataField="approvedby" HeaderText="Approver" />        
         <asp:TemplateField HeaderText="Approved Date">
        <ItemTemplate>
        <%# sayDate(DataBinder.Eval(Container, "DataItem.approveddate"))%>
        </ItemTemplate>
        </asp:TemplateField>    
        <asp:TemplateField HeaderText="HOP Status">
        <ItemTemplate>
        <%# gm.getApproval(Convert.ToString(DataBinder.Eval(Container, "DataItem.approvevalue")))%>
        </ItemTemplate>
        </asp:TemplateField>
        <asp:BoundField DataField="cn" HeaderText="Records" />
        <asp:TemplateField HeaderText="Mandate">
        <ItemTemplate>
        <%# fileDownload(DataBinder.Eval(Container, "DataItem.docfilename"))%>
        </ItemTemplate>
        <ItemStyle HorizontalAlign="Center" />
        </asp:TemplateField>
        <asp:TemplateField>
        <ItemTemplate>
        <a href="#" onclick="loadDetails('<%# DataBinder.Eval(Container, "DataItem.transactioncode") %>');">details</a>
        </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Status">
        <ItemTemplate>
        <%# gm.getStatus(Convert.ToString(DataBinder.Eval(Container, "DataItem.statusflag")))%>
        </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Action">
        <ItemTemplate>
        <%# getAction(DataBinder.Eval(Container, "DataItem.statusflag"), DataBinder.Eval(Container, "DataItem.transactioncode"))%>
        </ItemTemplate>
        </asp:TemplateField>   
        
        <asp:TemplateField HeaderText="Decline">
        <ItemTemplate>
        <%# getDecline(DataBinder.Eval(Container, "DataItem.statusflag"), DataBinder.Eval(Container, "DataItem.transactioncode"))%>
        </ItemTemplate>
        </asp:TemplateField> 
        
        <asp:TemplateField HeaderText="Status Query">
        <ItemTemplate>
        <%# getStatusQuery(DataBinder.Eval(Container, "DataItem.statusflag"), DataBinder.Eval(Container, "DataItem.transactioncode"))%>
        </ItemTemplate>
        </asp:TemplateField>         
    </Columns>
       <EmptyDataTemplate>No results for this search!</EmptyDataTemplate>
    </asp:GridView>
    
    
<div id="bxtrnxDetails" style="margin-top:30px;"></div>
</td>
</tr>
</table>

<script type="text/javascript">

</script>
</asp:Content>

