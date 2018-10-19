<%@ Page Language="VB" AutoEventWireup="false" CodeFile="print.aspx.vb" Inherits="print" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
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
	font-weight:bold;color:#990000;
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
            text-align: right;
            height: 30px;
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
	
        .style5
        {
            width: 175px;
            text-align: right;
        }
        	
        .style13
        {
            width: 175px;
            text-align: right;
            height: 36px;
        }
        .style21
        {
            width: 175px;
            text-align: right;
            height: 32px;
        }
        .style22
        {
            height: 32px;
            width: 437px;
        }
        .style23
        {
            width: 175px;
            text-align: right;
            height: 27px;
        }
        .style24
        {
            width: 437px;
        }
        .style25
        {
            width: 175px;
            text-align: right;
            height: 26px;
        }
        .style26
        {
            height: 26px;
            width: 437px;
        }
        .style27
        {
            width: 175px;
            text-align: right;
            height: 29px;
        }
        .style28
        {
            height: 29px;
            width: 437px;
        }
        	
    </style>
    <style type="text/css">

	
        span{
	margin: 0;
	padding: 0;
	border: 0;
	outline: 0;
	font-weight: inherit;
	font-style: inherit;
	font-size: 100%;
	vertical-align: baseline;
}
	
        .style4
        {
            width: 875px;
            border-style: solid;
            border-width: 1px;
            background-color: #FFCC99;
            height: 169px;
        }
        tbody{
	margin: 0;
	padding: 0;
	border: 0;
	outline: 0;
	font-weight: inherit;
	font-style: inherit;
	font-size: 100%;
	vertical-align: baseline;
}
	
        .style11
        {
            text-align: right;
            height: 30px;
        }
        .style12
        {
            height: 30px;
            width: 437px;
        }
	
        .style21
        {
            width: 175px;
            text-align: right;
            height: 32px;
        }
        .style22
        {
            height: 32px;
            width: 437px;
        }
        .style23
        {
            width: 175px;
            text-align: right;
            height: 27px;
        }
        .style24
        {
            width: 437px;
        }
        .style25
        {
            width: 175px;
            text-align: right;
            height: 26px;
        }
        .style26
        {
            height: 26px;
            width: 437px;
        }
        .style27
        {
            width: 175px;
            text-align: right;
            height: 29px;
        }
        .style28
        {
            height: 29px;
            width: 437px;
        }
        .style5
        {
            width: 175px;
            text-align: right;
        }
        	
        .style13
        {
            width: 175px;
            text-align: right;
            height: 36px;
        }
        .style29
        {
            color: #990000;
        }
        .style30
        {
            font-size: large;
        }
        .style31
        {
            font-size: medium;
            padding: 1px;
        }
        .style32
        {
            text-align: right;
        }
        </style>
</head>
<body onload="print();">
    <form id="form1" runat="server">
    <div>
    
           <table class="style4" style="border: thin dashed #FF9933">
               <tr>
                   <td class="style32" valign="middle" colspan="2">
                       <br />
                       <span class="style29"><strong class="style30">easyRTGS Portal</strong></span>-
                       <span class="style31">Sterling Bank Plc</span></td>
               </tr>
               <tr>
                   <td class="style11" valign="middle">
                       Transaction Reference:</td>
                   <td valign="middle" class="style12">
                       <asp:Label ID="Label17" runat="server" Font-Bold="True" ForeColor="#006600"></asp:Label>
                   </td>
               </tr>
               <tr>
                   <td class="style11" valign="middle">
                       Customer Account Number:</td>
                   <td valign="middle" class="style12">
                       <asp:Label ID="Label4" runat="server" Font-Bold="True" ForeColor="#006600"></asp:Label>
                   </td>
               </tr>
               <tr>
                   <td class="style11" valign="middle">
                       Customer Name:</td>
                   <td valign="middle" class="style12">
                       <asp:Label ID="Label5" runat="server" Font-Bold="True" ForeColor="#006600"></asp:Label>
                   </td>
               </tr>
               <tr>
                   <td class="style11" valign="middle">
                       Amount to Transfer:</td>
                   <td class="style12" valign="middle">
                       <asp:Label ID="Label7" runat="server" Font-Bold="True" ForeColor="#006600"></asp:Label>
                   </td>
               </tr>
               <tr>
                   <td class="style11" valign="middle">
                       Charges:</td>
                   <td class="style12" valign="middle">
                       <asp:Label ID="Label16" runat="server" Font-Bold="True" ForeColor="#006600"></asp:Label>
                   </td>
               </tr>
               <tr>
                   <td class="style21" valign="middle">
                       Remarks:</td>
                   <td class="style22" valign="middle">
                       <asp:Label ID="Label8" runat="server" Font-Bold="True" ForeColor="#006600"></asp:Label>
                   </td>
               </tr>
               <tr>
                   <td class="style23" valign="middle">
                       Uploaded By:</td>
                   <td class="style24" valign="middle">
                       <asp:Label ID="Label9" runat="server" Font-Bold="True" ForeColor="#006600"></asp:Label>
                   </td>
               </tr>
               <tr>
                   <td class="style11" valign="middle">
                       Upload Date:</td>
                   <td class="style12" valign="middle">
                       <asp:Label ID="Label10" runat="server" Font-Bold="True" ForeColor="#006600"></asp:Label>
                   </td>
               </tr>
               <tr>
                   <td class="style25" valign="middle">
                       Authorized By:</td>
                   <td class="style26" valign="middle">
                       <asp:Label ID="Label11" runat="server" Font-Bold="True" ForeColor="#006600"></asp:Label>
                   </td>
               </tr>
               <tr>
                   <td class="style25" valign="middle">
                       Authorized Date:</td>
                   <td class="style26" valign="middle">
                       <asp:Label ID="Label15" runat="server" Font-Bold="True" ForeColor="#006600"></asp:Label>
                   </td>
               </tr>
               <tr>
                   <td class="style23" valign="middle">
                       Treasury Approval:</td>
                   <td class="style24" valign="middle">
                       <asp:Label ID="Label12" runat="server" Font-Bold="True" ForeColor="#006600"></asp:Label>
                   </td>
               </tr>
               <tr>
                   <td class="style27" valign="middle">
                       Approved By:</td>
                   <td class="style28" valign="middle">
                       <asp:Label ID="Label13" runat="server" Font-Bold="True" ForeColor="#006600"></asp:Label>
                   </td>
               </tr>
               <tr>
                   <td class="style23" valign="middle" >
                       Customer Care Confirmation Date:</td>
                   <td class="style28" valign="middle" >
                       <asp:Label ID="lblCustCareConfirmationDate" runat="server" Font-Bold="True" ForeColor="#006600"
                           ></asp:Label>
                   </td>
               </tr>
               <tr>
                   <td class="style23" valign="middle" >
                       Customer Care Officer:</td>
                   <td class="style28" valign="middle"  >
                       <asp:Label ID="lblCustCareOfficer" runat="server" Font-Bold="True" ForeColor="#006600"
                           ></asp:Label>
                   </td>
               </tr>
               <tr>
                   <td class="style23" valign="middle"  >
                       Customer Care Comment:</td>
                   <td class="style28" valign="middle"  >
                       <asp:Label ID="lblCustCareConfirmAvailability" runat="server" Font-Bold="True" ForeColor="#006600"
                           ></asp:Label>
                       <br />
                       <asp:Label ID="lblCustCareComment" runat="server" Font-Bold="True" ForeColor="#006600"
                           ></asp:Label>
                       <br />
                   </td>
               </tr>
               <tr>
                   <td class="style27" valign="middle">
                       &nbsp;</td>
                   <td class="style28" valign="middle">
                       &nbsp;</td>
               </tr>
               <tr>
                   <td class="style23" valign="middle">
                       Customer Instruction:</td>
                   <td class="style24" valign="middle" rowspan="3">
                       <asp:Label ID="Label14" runat="server" Font-Bold="True" ForeColor="#006600"></asp:Label>
                   </td>
               </tr>
               <tr>
                   <td class="style5">
                       &nbsp;</td>
               </tr>
               <tr>
                   <td class="style13">
                       </td>
               </tr>
           </table>
    
    </div>
    </form>
</body>
</html>
