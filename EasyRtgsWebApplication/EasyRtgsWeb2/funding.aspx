<%@ Page Language="VB" AutoEventWireup="false" CodeFile="funding.aspx.vb" Inherits="funding" %>

<%@ Register assembly="RJS.Web.WebControl.PopCalendar" namespace="RJS.Web.WebControl" tagprefix="rjs" %>

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
		width:960px;
            top: 0px;
            left: 0px;
        }
	
	#header, #footer {
		width:100%;
		float:left;
		padding:15px 0;
	}
	
	#header {
		background:#800000;
            height: 85px;
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
	
    </style> 
</head>
<body>
    <form id="form1" runat="server">
    <div id="header" class="style1">
    
        <img alt="" class="style21" src="images/icon.png" />Sterling easyRTGS<br />
        <span class="style2">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Third Party Transfer Application</span></div>


       <div id="content">
       <div class="wrap">
           <strong class="style3">
           <asp:Label ID="Label2" runat="server"></asp:Label>
&nbsp;</strong><br />
           <br />
           Welcome,
           <asp:Label ID="Label1" runat="server"></asp:Label>
           &nbsp;&nbsp; |&nbsp; 
           <a href="main.aspx">Home</a>&nbsp;&nbsp; |&nbsp;&nbsp; <a href="main.aspx">My Transactions</a>&nbsp;&nbsp; | &nbsp;<a href="funding.aspx">Funding 
           Report</a>&nbsp; | &nbsp;<a href="logout.aspx">Sign Out</a><br />
           <br />
           <span class="style20">
           <asp:Label ID="Label4" runat="server" Font-Size="15pt" Font-Bold="True">Funding Reports</asp:Label>
           </span>
           <span class="style6">
           <br />
           <strong>
           <br />
           [From] </strong><asp:TextBox  AutoComplete="off" ID="TextBox1" runat="server" Height="32px" 
               Width="220px" ReadOnly="True"></asp:TextBox>
&nbsp;<rjs:PopCalendar ID="PopCalendar1" runat="server" Control="TextBox1" 
               Format="yyyy mm dd" />
&nbsp; <strong>
           [To]</strong> <asp:TextBox  AutoComplete="off" ID="TextBox2" runat="server" Height="32px" 
               Width="220px" ReadOnly="True"></asp:TextBox>
&nbsp;<rjs:PopCalendar ID="PopCalendar2" runat="server" Control="TextBox2" 
               Format="yyyy mm dd" />
           &nbsp;&nbsp;<%--<asp:DropDownList ID="DropDownList1" runat="server" Height="32px" Width="108px">
               <asp:ListItem Value="a">All</asp:ListItem>
               <asp:ListItem>Paid</asp:ListItem>
               <asp:ListItem>Failed</asp:ListItem>
               <asp:ListItem>Discarded</asp:ListItem>
           </asp:DropDownList>--%><asp:DropDownList ID="DropDownList1" runat="server" Height="32px" Width="112px">
               <asp:ListItem Value="a">All</asp:ListItem>
               <asp:ListItem>Authorized</asp:ListItem>
               <asp:ListItem>Discarded</asp:ListItem>
               <asp:ListItem Value="Failed">Failed</asp:ListItem>
               <asp:ListItem>Paid</asp:ListItem>
<asp:ListItem>Uploaded</asp:ListItem>
               <asp:ListItem>SwiftInProcess</asp:ListItem>
           </asp:DropDownList>
           <br />
           <br />
           &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; <asp:Button ID="Button1" runat="server" Font-Names="Segoe UI Light" 
               Font-Size="11pt" Width="169px" Text="Generate Report" />
           </span>&nbsp;
           <span class="style6">
           <asp:Button ID="Button2" runat="server" Font-Names="Segoe UI Light" 
               Font-Size="11pt" Width="169px" Text="Export to Excel" 
               UseSubmitBehavior="False" />
           </span>
           <br />
           <br />
           <span class="style20">
           <asp:Label ID="Label5" runat="server" Font-Size="15pt" Font-Bold="True"></asp:Label>
           <br />
           </span>
           <br />
           <asp:DataGrid ID="DataGrid1" runat="server" AutoGenerateColumns="False" 
               BackColor="White" BorderColor="#DEDFDE" BorderStyle="None" BorderWidth="1px" 
               CellPadding="4" ForeColor="Black" Height="40px" 
               Width="1007px">
               <AlternatingItemStyle BackColor="White" />
               <Columns>
                   <asp:BoundColumn DataField="TransactionID" HeaderText="REF NO"></asp:BoundColumn>
                   <asp:BoundColumn DataField="Beneficiary" HeaderText="RECEIVER"></asp:BoundColumn>                
                   <asp:BoundColumn DataField="Beneficiary_Bank" HeaderText="RECEIVER BANK"></asp:BoundColumn>
                   <asp:BoundColumn DataField="amount" HeaderText="AMOUNT"></asp:BoundColumn>
                   <asp:BoundColumn DataField="remarks" HeaderText="REMARKS"></asp:BoundColumn>
                   <asp:BoundColumn DataField="customer_name" HeaderText="SENDER"></asp:BoundColumn>
                   <asp:BoundColumn DataField="customer_account" HeaderText="SENDER ACCOUNT"></asp:BoundColumn>
                   <asp:BoundColumn DataField="charges" HeaderText="CHARGES"></asp:BoundColumn>
                   <asp:BoundColumn DataField="status" HeaderText="STATUS"></asp:BoundColumn> 
                   <asp:TemplateColumn><HeaderTemplate>DATE</HeaderTemplate><ItemTemplate><%# Convert.ToDateTime(Container.DataItem("date")).ToString("dd-MMM-yyyy")%></ItemTemplate></asp:TemplateColumn>
                 
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
           <br />
           <br />
           <br /></div>
       </div>
         <div id="footer">
    
             Sterling Bank Plc    <br />
    <br />
    <br />
    </form>
</body>
</html>
