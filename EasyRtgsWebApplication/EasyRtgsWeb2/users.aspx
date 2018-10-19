<%@ Page Language="VB" AutoEventWireup="false" CodeFile="users.aspx.vb" Inherits="users" MaintainScrollPositionOnPostback="true" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>easyRTGS</title>
    <style type="text/css"> 
/*CSS RESET*/
html, body, div, span, applet, object, iframe,
h1, h2, h3, h4, h5, h6, p, blockquote, pre,
a, abbr, acronym, address, big, cite, code,
del, dfn, em, font, img, ins, kbd, q, s, samp,
small, strike, strong, sub, sup, tt, var,
dl, dt, dd, ol, ul, li,
fieldset, form, label, legend,
 caption, tbody, tfoot, thead{
	margin: 0;
	padding: 0;
	border: 0;
	outline: 0;
	font-weight: inherit;
	font-style: inherit;
	font-size: 100%;
	vertical-align: baseline;
}
/* remember to define focus styles! */
:focus {
	outline: 0;
}
body {
	line-height: 1;
	color: black;
	background: white;
}
ol, ul {
	list-style: none;
}

blockquote:before, blockquote:after,
q:before, q:after {
	content: "";
}
blockquote, q {
	quotes: "" "";
}
 
strong {
	font-weight:bold;color:#0289ce;
}
 
em {
	font-style:oblique;
}
 
p {
	margin:15px 0;
}
 
.aligncenter, div.aligncenter {
	display: block;
	margin-left: auto;
	margin-right: auto;
}
.alignleft {
	float: left;
}
.alignright {
	float: right;
}
 
h1 {font-size:180%;}
h2 {font-size:150%;}
h3 {font-size:125%;}
h4 {font-size:100%;}
h5 {font-size:90%;}
h6 {font-size:80%;}
 
a:link {color:#0289ce;}
a:hover {color:#f64274;}
 
/*End RESET - Begin Full Width CSS*/
	body {
		background:#FFFFFF;
		color:#2D1F16;
		font:13px Segoe UI Light
	}
 
	.wrap {
		position:relative;
		margin:0 auto;
		width:900px;
	}
	
	#header, #footer {
		width:100%;
		float:left;
		padding:15px 0;
	}
	
	#header {
		background:#800000;
            height: 72px;
            color: White;
        }
	
	#header .logo {
		float:left;
		width:400px;
	}
	
	#header p {
		float:right;
		width:400px;
		margin:0;
	}
	
	#content {
		padding:15px 0;
		clear:both;
		background:#FFFFFF
	}
	
	#footer {
		background:#EC3515;
		text-align:center;
	}
	
	#footer a {
		color:#fff;
	}
	
        .style1
        {
            font-size: xx-large;
        }
	
        .style2
        {
            font-size: large;
        }
        .style3
        {
            font-size: medium;
        }
	
        .style6
        {
            font-size: large;
            padding-top: 15px;
            padding-bottom: 15px;
            background-color: #FFFFFF;
        }
	
        .style20
        {
            font-size: x-large;
            padding-top: 15px;
            padding-bottom: 15px;
            background-color: #FFFFFF;
        }
	
        .style21
        {
            height: 53px;
        }
	
        .style22
        {
            height: 111px;
        }
        .style23
        {
            height: 40px;
        }
	
        .style24
        {
            width: 160px;
        }
        .style25
        {
            font-size: medium;
            width: 160px;
            height: 61px;
        }
	
        .style26
        {
            font-size: medium;
            height: 61px;
        }
        .style27
        {
            font-size: medium;
            height: 46px;
        }
	
    </style> 
</head>
<body>
    <form id="form1" runat="server">
    <div id="header" class="style1">
    
        Sterling easyRTGS<br />
        <span class="style2">Third Party Transfer Application</span></div>


       <div id="content">
       <div class="wrap">
           <strong class="style3">
           <asp:Label ID="Label2" runat="server"></asp:Label>
&nbsp;</strong><br />
           <br />
          
           <asp:Label ID="Label1" runat="server"></asp:Label>
          
           </span>
           <br />
           <br />
           <%--  <span class="style20">--%>User Management<br />
           <br />
           <div id="divAdmin"  style="height: 600px; overflow: scroll; width: 50%; float: left">
           <table class="style2">
               <tr>
                   <td>
                       <strong>Add Administrator</strong></td>
                   <td>
                       &nbsp;</td>
               </tr>
               <tr>
                   <td>
                       User Name &nbsp;
                   </td>
                   <td>
                       &nbsp;</td>
               </tr>
               <tr>
                   <td class="style21">
                       <asp:TextBox  AutoComplete="off" ID="TextBox1" runat="server" BorderStyle="Solid" BorderWidth="1px" 
                           Height="25px" Width="235px"></asp:TextBox>
                   </td>
                   <td class="style21">
                   </td>
               </tr>
               <tr>
                   <td class="style3">
                       <%--   <span class="style20">--%>
                       <asp:Button ID="btnAddAdmin" runat="server" BackColor="#993300" BorderStyle="Solid" 
                           BorderWidth="1px" ForeColor="White" Height="31px" 
                           Text="Add New Administrator" Width="223px" />
                       <%-- &nbsp;
                       <br />
                       <br />
           </span>--%>
                   </td>
                   <td class="style3">
           <span class="style20">
                       <br />
           </span>
                   </td>
               </tr>
               <tr>
                   <td class="style3" colspan="2">
           <asp:Label ID="lblAdminUser" runat="server" Font-Bold="True" ForeColor="#006600"></asp:Label>
                   </td>
               </tr>
           </table>
           </div>
           <div id="divOtherUsers"   style="height: 600px; overflow: scroll; width: 50%; float: right">
           <table class="style2">
               <tr>
                   <td>
                       <strong>Add User</strong></td>
                   <td class="style24">
                       &nbsp;</td>
               </tr>
               <tr>
                   <td>
                       User Name &nbsp;
                   </td>
                   <td class="style24">
                       &nbsp;</td>
               </tr>
               <tr>
                   <td class="style21" colspan="2">
                       <asp:TextBox  AutoComplete="off" ID="txtUsername" runat="server" 
                           BorderStyle="Solid" BorderWidth="1px" 
                           Height="25px" Width="235px"></asp:TextBox>
                   </td>
               </tr>
               <tr>
                   <td class="style23" colspan="2">
                       Role</td>
               </tr>
               <tr>
                   <td class="style22" colspan="2">
                       <asp:RadioButtonList ID="rblRole" runat="server" Height="44px">
                           <asp:ListItem Value="Inputter">CEMP/FTO</asp:ListItem>
                           <asp:ListItem Value="Authorizer">SERVICE MANAGER</asp:ListItem>
                       </asp:RadioButtonList>
                   </td>
               </tr>
               <tr>
                   <td class="style22" colspan="2">
                       Branch<br />
                       <asp:DropDownList ID="ddlBranch" runat="server" DataSourceID="odsBranch" 
                           DataTextField="BRANAME" DataValueField="BRACODE" Height="26px" Width="301px" >
                       </asp:DropDownList>
                       <asp:ObjectDataSource ID="odsBranch" runat="server" SelectMethod="GetBranch" 
                               TypeName="Gadget"></asp:ObjectDataSource>
                   </td>
               </tr>
               <tr>
                   <td class="style27" colspan="2">
                       Status</td>
               </tr>
               <tr>
                   <td class="style26" colspan="2">
                       <asp:RadioButtonList ID="rblStatus" runat="server">
                           <asp:ListItem>Active</asp:ListItem>
                           <asp:ListItem>Inactive</asp:ListItem>
                       </asp:RadioButtonList>
                   </td>
               </tr>
               <tr>
                   <td class="style26">
                       <span class="style20">
                       <asp:Button ID="btnAddNew" runat="server" BackColor="#993300" BorderStyle="Solid" 
                           BorderWidth="1px" ForeColor="White" Height="31px" 
                           Text="Add/Modify New User" Width="148px" />
                   &nbsp;
                       <br />
                       <br />
                       <br />
           </span>
                   </td>
                   <td class="style25">
                   </td>
               </tr>
               <tr>
                   <td class="style3" colspan="2">
                       <span class="style20">
                       <asp:Label ID="Label5" runat="server" Font-Bold="True" ForeColor="#006600"></asp:Label>
           </span>
                   </td>
               </tr>
           </table>
           </div>
           
           &nbsp;<asp:Button 
               ID="Button1" runat="server" 
               BackColor="#993300" BorderStyle="Solid" 
                           BorderWidth="1px" ForeColor="White" Height="31px" 
               Text="Load Admin Users" Width="148px" Visible="False" />
           <asp:Label ID="Label4" runat="server" Font-Bold="True" ForeColor="#006600"></asp:Label>
           <br />
           <br />
           <asp:DataGrid ID="DataGrid1" runat="server" AllowPaging="True" 
               BackColor="White" BorderColor="#DEDFDE" 
               BorderStyle="None" BorderWidth="1px" CellPadding="4"  
               Font-Size="11pt" ForeColor="Black" Height="22px" 
               Width="895px" AutoGenerateColumns="False" Visible="False">
               <AlternatingItemStyle BackColor="White" />
               <Columns>
                   <asp:BoundColumn DataField="username" HeaderText="Admins"></asp:BoundColumn>
                   <asp:TemplateColumn><ItemTemplate><a href="users.aspx?action=del&username=<%# container.dataitem("username")  %>">Remove</a></ItemTemplate></asp:TemplateColumn>
               </Columns>
               <FooterStyle BackColor="#CCCC99" />
               <HeaderStyle BackColor="#6B696B" Font-Bold="True" ForeColor="White" />
               <ItemStyle BackColor="#F7F7DE" />
               <PagerStyle BackColor="#F7F7DE" ForeColor="Black" HorizontalAlign="Right" 
                   Mode="NumericPages" />
               <SelectedItemStyle BackColor="#CE5D5A" Font-Bold="True" ForeColor="White" />
           </asp:DataGrid>
           <br />
           <br />
           <br />
           <%--<asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" 
               CellPadding="4" DataKeyNames="id" DataSourceID="sqlDsUsers" 
               ForeColor="#333333">
               <AlternatingRowStyle BackColor="White" />
               <Columns>
                   <asp:CommandField ShowSelectButton="True" ShowDeleteButton="True" />
                   <asp:BoundField DataField="id" HeaderText="id" InsertVisible="False" 
                       ReadOnly="True" SortExpression="id" />
                   <asp:BoundField DataField="username" HeaderText="username" 
                       SortExpression="username" />
                   <asp:BoundField DataField="name" HeaderText="name" SortExpression="name" />
                   <asp:BoundField DataField="type" HeaderText="type" SortExpression="type" />
                   <asp:BoundField DataField="Branch" HeaderText="Branch" 
                       SortExpression="Branch" />
                   <asp:BoundField DataField="Status" HeaderText="Status" 
                       SortExpression="Status" />
                   <asp:BoundField DataField="Date_created" HeaderText="Date_created" 
                       SortExpression="Date_created" />
               </Columns>
               <FooterStyle BackColor="#990000" Font-Bold="True" ForeColor="White" />
               <HeaderStyle BackColor="#990000" Font-Bold="True" ForeColor="White" />
               <PagerStyle BackColor="#FFCC66" ForeColor="#333333" HorizontalAlign="Center" />
               <RowStyle BackColor="#FFFBD6" ForeColor="#333333" />
               <SelectedRowStyle BackColor="#FFCC66" Font-Bold="True" ForeColor="Navy" />
               <SortedAscendingCellStyle BackColor="#FDF5AC" />
               <SortedAscendingHeaderStyle BackColor="#4D0000" />
               <SortedDescendingCellStyle BackColor="#FCF6C0" />
               <SortedDescendingHeaderStyle BackColor="#820000" />
           </asp:GridView>--%>
           <asp:Label ID="lblGridStatus" runat="server" Font-Bold="True" 
               ForeColor="#006600"></asp:Label>
           <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" 
               CellPadding="4" DataKeyNames="id" DataSourceID="sqlDsUsers" 
               ForeColor="#333333">
               <AlternatingRowStyle BackColor="White" />
               <Columns>
                   <asp:TemplateField ShowHeader="False">
                       <ItemTemplate>
                           <asp:LinkButton ID="LinkButton1" runat="server" CausesValidation="False" 
                               CommandName="disable" Text="Disable User" CommandArgument='<%# Eval("id") %>' OnClientClick="return confirm('Are you sure?')"></asp:LinkButton>
                       </ItemTemplate>
                   </asp:TemplateField>
                   <asp:BoundField DataField="id" HeaderText="id" InsertVisible="False" 
                       ReadOnly="True" SortExpression="id" />
                   <asp:BoundField DataField="username" HeaderText="username" 
                       SortExpression="username" />
                   <asp:BoundField DataField="name" HeaderText="name" SortExpression="name" />
                   <asp:BoundField DataField="type" HeaderText="type" SortExpression="type" />
                   <asp:BoundField DataField="Branch" HeaderText="Branch" 
                       SortExpression="Branch" />
                   <asp:BoundField DataField="Status" HeaderText="Status" 
                       SortExpression="Status" />
                   <asp:BoundField DataField="Date_created" HeaderText="Date_created" 
                       SortExpression="Date_created" />
               </Columns>
               <FooterStyle BackColor="#990000" Font-Bold="True" ForeColor="White" />
               <HeaderStyle BackColor="#990000" Font-Bold="True" ForeColor="White" />
               <PagerStyle BackColor="#FFCC66" ForeColor="#333333" HorizontalAlign="Center" />
               <RowStyle BackColor="#FFFBD6" ForeColor="#333333" />
               <SelectedRowStyle BackColor="#FFCC66" Font-Bold="True" ForeColor="Navy" />
               <SortedAscendingCellStyle BackColor="#FDF5AC" />
               <SortedAscendingHeaderStyle BackColor="#4D0000" />
               <SortedDescendingCellStyle BackColor="#FCF6C0" />
               <SortedDescendingHeaderStyle BackColor="#820000" />
           </asp:GridView>
           <%--<asp:SqlDataSource ID="sqlDsUsers" runat="server" 
               ConnectionString="<%$ ConnectionStrings:easyRtgsConn %>" 
               
               SelectCommand="SELECT [id], [username], [name], [type], [Branch], [Status], [Date_created] FROM [tblusers] WHERE TYPE='regionalchannelcoordinator' and status='Active'" 
               DeleteCommand="update tblusers set Status='Inactive' where id=@id">
               <DeleteParameters>
                   <asp:Parameter Name="id" />
               </DeleteParameters>
           </asp:SqlDataSource>--%>
           <asp:SqlDataSource ID="sqlDsUsers" runat="server" 
               ConnectionString="<%$ ConnectionStrings:easyRtgsConn %>" 
               
               SelectCommand="SELECT [id], [username], [name], [type], [Branch], [Status], [Date_created] FROM [tblusers] WHERE TYPE=@role and status='Active'" 
               DeleteCommand="UPDATE tblusers SET Status='Inactive' WHERE id=@id AND username<>@username AND Status='Active'">
               <DeleteParameters>
                   <asp:Parameter Name="id" />
                   <asp:SessionParameter DefaultValue="NULL" Name="username" 
                       SessionField="uname" />
               </DeleteParameters>
               <SelectParameters>
                   <asp:SessionParameter DefaultValue="RegionalChannelCoordinator" Name="role" 
                       SessionField="role" />
               </SelectParameters>
           </asp:SqlDataSource>
           <span class="style6">
           <br />
           <br />
           </span>
           <%--</span>--%>
           <br />
           <br />
           <br />
           <br />
           <br />
           <br />
           <br />
           <br /></div>
       </div>
         <div id="footer">
    
             Sterling Bank Plcc</div>
    <br />
    <br />
    <br />
    </form>
</body>
</html>
