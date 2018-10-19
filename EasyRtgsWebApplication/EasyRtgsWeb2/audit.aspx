<%@ Page Language="VB" AutoEventWireup="false" CodeFile="audit.aspx.vb" Inherits="audit" %>

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
	
        .style4
        {
            width: 875px;
            border-style: solid;
            border-width: 1px;
            background-color: #FFCC99;
            height: 169px;
        }
        	
        .style11
        {
            width: 152px;
            text-align: right;
            height: 30px;
            font-size: small;
        }
        .style12
        {
            height: 30px;
            width: 437px;
        }
	
        #Button3
        {
            width: 97px;
        }
	
        .style15
        {
            width: 152px;
            text-align: right;
            height: 25px;
            font-size: small;
        }
        .style16
        {
            height: 25px;
            width: 437px;
        }
        	
        .style13
        {
            width: 152px;
            text-align: right;
            height: 36px;
        }
        .style14
        {
            height: 36px;
            width: 437px;
        }
	
        .style21
        {
            font-size: small;
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
           Welcome,
           <asp:Label ID="Label1" runat="server"></asp:Label>
           &nbsp;&nbsp; |&nbsp; 
           <a href="admin.aspx">Home</a>&nbsp;&nbsp; |&nbsp;&nbsp;&nbsp; 
           <a href="logout.aspx">Sign Out</a><br />
           <span class="style6">
           <br />
           </span>
           <br />
           <span class="style20">
           Initial Setup<br />
           <br />
           <table class="style4" style="border: thin dotted #FF9933">
               <tr>
                   <td class="style11" valign="middle">
                       &nbsp;</td>
                   <td valign="middle" class="style12">
                       <asp:Label ID="Label4" runat="server" Font-Bold="True" Font-Size="14pt" 
                           ForeColor="#339933" CssClass="style21"></asp:Label>
                   	<br />
					   <br />
					   <asp:CheckBox ID="CheckBox1" runat="server" Font-Bold="True" Font-Size="Medium" ForeColor="Red" Text="Check form IMAL Details" />
                   </td>
               </tr>
               <tr>
                   <td class="style11" valign="middle">
                       RTGS/CBN 
                       Account </td>
                   <td valign="middle" class="style12">
                    <asp:TextBox  AutoComplete="off" ID="txtAcctCbn" runat="server" 
                           BorderStyle="Solid" BorderWidth="1px" 
                        Height="26px" Width="302px"  Font-Size="12pt" Font-Names="Segoe UI Light" 
                           CssClass="style21"></asp:TextBox>
                       <span class="style21">&nbsp;
                       </span>&nbsp;<asp:RequiredFieldValidator 
                           ID="RequiredFieldValidator1" runat="server" 
                           ControlToValidate="txtAcctCbn" Display="Dynamic" 
                           ErrorMessage="must have value" EnableClientScript="False" 
                           CssClass="style21"></asp:RequiredFieldValidator>
                   </td>
               </tr>
               <tr>
                   <td class="style11" valign="middle">
                       RTGS Charges Account</td>
                   <td valign="middle" class="style12">
                    <asp:TextBox  AutoComplete="off" ID="txtAcctCharge" runat="server" 
                           BorderStyle="Solid" BorderWidth="1px" 
                        Height="26px" Width="302px"  Font-Size="12pt" Font-Names="Segoe UI Light" 
                           CssClass="style21"></asp:TextBox>
                       <span class="style21">&nbsp;&nbsp;
                       </span>
                       <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" 
                           ControlToValidate="txtAcctCharge" ErrorMessage="must have value" 
                           EnableClientScript="False" CssClass="style21"></asp:RequiredFieldValidator>
                   </td>
               </tr>
               <tr>
                   <td class="style11" valign="middle">
                       RTGS Suspense Account</td>
                   <td valign="middle" class="style12">
                       <asp:TextBox  AutoComplete="off" ID="txtAcctSuspense" runat="server" 
                           BorderStyle="Solid" BorderWidth="1px" 
                           Font-Names="Segoe UI Light" Font-Size="12pt" Height="26px" Width="303px"></asp:TextBox>
&nbsp;
                       <asp:RequiredFieldValidator ID="RequiredFieldValidator7" runat="server" 
                           ControlToValidate="txtAcctSuspense" ErrorMessage="Must enter value" 
                           Font-Size="10pt"></asp:RequiredFieldValidator>
                   </td>
               </tr>
               <tr>
                   <td class="style11" valign="middle">
                       RTGS Suspense Account-202</td>
                   <td valign="middle" class="style12">
                       <asp:TextBox  AutoComplete="off" ID="txtAcctSuspense_202" runat="server" 
                           BorderStyle="Solid" BorderWidth="1px" 
                           Font-Names="Segoe UI Light" Font-Size="12pt" Height="26px" Width="303px"></asp:TextBox>
&nbsp;
                       <asp:RequiredFieldValidator ID="RequiredFieldValidator8" runat="server" 
                           ControlToValidate="txtAcctSuspense_202" ErrorMessage="Must enter value" Font-Size="10pt"></asp:RequiredFieldValidator>
                   </td>
               </tr>
               <tr>
                   <td class="style11" valign="middle">
                       Customer Charges</td>
                   <td class="style12" valign="middle">
                    <asp:TextBox  AutoComplete="off" ID="txtCustomerCharge" runat="server" 
                           BorderStyle="Solid" BorderWidth="1px" 
                        Height="26px" Width="302px"  Font-Size="12pt" Font-Names="Segoe UI Light" 
                           CssClass="style21"></asp:TextBox>
                       <span class="style21">&nbsp;&nbsp;
                       </span>
                       <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" 
                           ControlToValidate="txtCustomerCharge" Display="Dynamic" 
                           ErrorMessage="must have value" EnableClientScript="False" 
                           CssClass="style21"></asp:RequiredFieldValidator>
                       <span class="style21">&nbsp;
                       </span>
                   </td>
               </tr>
               <tr>
                   <td class="style15" valign="middle">
                       Staff Charges</td>
                   <td class="style16" valign="middle">
                    <asp:TextBox  AutoComplete="off" ID="txtStaffCharge" runat="server" 
                           BorderStyle="Solid" BorderWidth="1px" 
                        Height="26px" Width="302px"  Font-Size="12pt" Font-Names="Segoe UI Light" 
                           CssClass="style21"></asp:TextBox>
                       <span class="style21">&nbsp;&nbsp;
                       </span>
                       <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" 
                           ControlToValidate="txtStaffCharge" ErrorMessage="must have value" 
                           EnableClientScript="False" CssClass="style21"></asp:RequiredFieldValidator>
                   </td>
               </tr>
               <tr>
           <span class="style20">
                   <td class="style11" valign="middle">
                       RTGS Email Address</td>
                   <td class="style12" valign="middle">
                    <asp:TextBox  AutoComplete="off" ID="txtRtgsEmailAddress" runat="server" 
                           BorderStyle="Solid" BorderWidth="1px" 
                        Height="26px" Width="302px"  Font-Size="12pt" Font-Names="Segoe UI Light" 
                           CssClass="style21"></asp:TextBox>
                       <span class="style21">&nbsp;&nbsp;
                       </span>
                       <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" 
                           ControlToValidate="txtRtgsEmailAddress" Display="Dynamic" 
                           ErrorMessage="must have value" EnableClientScript="False" 
                           CssClass="style21"></asp:RequiredFieldValidator>
                       <span class="style21">&nbsp;
                       </span>
                   </td>
           </span>
               </tr>
               <tr>
           <span class="style20">
                   <td class="style15" valign="middle">
                       Treasury Email Address</td>
                   <td class="style16" valign="middle">
                    <asp:TextBox  AutoComplete="off" ID="txtTreasuryEmail" runat="server" 
                           BorderStyle="Solid" BorderWidth="1px" 
                        Height="26px" Width="302px"  Font-Size="12pt" Font-Names="Segoe UI Light" 
                           CssClass="style21"></asp:TextBox>
                       <span class="style21">&nbsp;&nbsp;
                       </span>
                       <asp:RequiredFieldValidator ID="RequiredFieldValidator6" runat="server" 
                           ControlToValidate="txtTreasuryEmail" ErrorMessage="must have value" 
                           EnableClientScript="False" CssClass="style21"></asp:RequiredFieldValidator>
                   </td>
           </span>
               </tr>
               <tr>
                   <td class="style15" valign="middle">
                       &nbsp;</td>
                   <td class="style16" valign="middle">
                       &nbsp;</td>
               </tr>
               <tr>
                   <td class="style15" valign="middle">
                       List of Banks </td>
                   <td class="style16" valign="middle">
                       <asp:TextBox ID="TextBox8" runat="server" Height="389px" TextMode="MultiLine" 
                           Width="529px" Visible="False"></asp:TextBox>
                   </td>
               </tr>
               <tr>
                   <td class="style13">
                       </td>
                   <td class="style14">
                       &nbsp;<asp:Button 
                           ID="Button5" runat="server" Height="35px" 
                           onclientclick="this.value=&quot;Saving ...&quot;;" 
                           Text="Save Settings" UseSubmitBehavior="False" Width="161px" 
                           CssClass="style21" />
                   </td>
               </tr>
           </table>
           <span class="style6">
           <br />
           <br />
           </span>
           </span>
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