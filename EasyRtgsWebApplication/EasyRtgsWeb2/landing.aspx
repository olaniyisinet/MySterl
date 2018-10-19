<%@ Page Language="VB" AutoEventWireup="false" CodeFile="landing.aspx.vb" Inherits="landing"     %>
 
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>
 
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
 
a:link {color:#009900;
            font-family: "Segoe UI Semibold";
        }
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
	
        .style4
        {
            width: 875px;
            border-style: solid;
            border-width: 1px;
            background-color: #FFCC99;
            height: 169px;
        }
        .style6
        {
            font-size: large;
            padding-top: 15px;
            padding-bottom: 15px;
            background-color: #FFFFFF;
        }
	
        .style11
        {
            text-align: center;
            height: 30px;
        }
        .style20
        {
            font-size: x-large;
            padding-top: 15px;
            padding-bottom: 15px;
            background-color: #FFFFFF;
        }
	
        #Button3
        {
            width: 121px;
        }
	
        .style21
        {
            width: 57px;
            height: 57px;
        }
	
        #bank
        {
            height: 26px;
        }
	
        #Button6
        {
        }
	
        #Button7
        {
        }
	
        #Button8
        {
        }
	
        #Button9
        {
        }
	
        </style> 
        <script src="scripts/jquery-1.11.0.min.js"></script>
        <script src="scripts/site.js"></script>
      <script language="javascript">
          function hide() {

              document.getElementById("output").style.visibility = "hidden";
          }
  </script>
</head>
<body>
    <form id="form1" runat="server">
    <div id="header" class="style1">
    
        <img alt="" class="style21" src="images/icon.png" />Sterling easyRTGS        <span class="style2">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Third Party Transfer Application</span></div>


       <div id="content">
       <div class="wrap">
           <strong class="style3">TROPS Landing Interface</strong><br />
           <br />
           Welcome,
           <asp:Label ID="lblFullnameAndUsername" runat="server"></asp:Label>
           &nbsp;&nbsp; |&nbsp;&nbsp;&nbsp; 
           <a href="logout.aspx">Sign Out</a><br />
           <br />
           <span class="style20">Choose Option</span><span class="style6"><br />
           <br />
           </span><strong>&nbsp;To change your options, just click on the option you want. 
           No need to press your browser&#39;s &quot;Back&quot; button.</strong><asp:Panel ID="pnlTROPS" runat="server" Visible="False">
               <table class="style4" style="border: thin dotted #FF9933">
                   <tr>
                       <td class="style11" valign="middle">
                           <asp:RadioButtonList ID="rblTropsSelection" runat="server" AutoPostBack="True" 
                               Font-Bold="True" Font-Names="Century Gothic" Font-Size="XX-Large">
                               <asp:ListItem Value="CBN">CBN Transfer</asp:ListItem>
                               <asp:ListItem Value="Interbank">Inter Bank Transfer</asp:ListItem>
                           </asp:RadioButtonList>
                       </td>
                   </tr>
                   <tr>
                       <td class="style11" valign="middle">
                           <br />
                           <asp:Label ID="lblTropsMessage" runat="server" Font-Bold="True" 
                               Font-Names="Century Gothic" Font-Size="XX-Large" 
                               Text="Please select message type for " Visible="False"></asp:Label>
                           <asp:Label ID="lblTropSelection" runat="server" Font-Bold="True" 
                               Font-Names="Century Gothic" Font-Size="XX-Large" Visible="False"></asp:Label>
                           <asp:RadioButtonList ID="rblTropsMessageType" runat="server" 
                               AutoPostBack="True" Font-Bold="True" Font-Names="Century Gothic" 
                               Font-Size="XX-Large" Visible="False">
                               <asp:ListItem Value="103">MT103</asp:ListItem>
                               <asp:ListItem Value="202">MT202</asp:ListItem>
                           </asp:RadioButtonList>
                       </td>
                   </tr>
               </table>
           </asp:Panel>
           <asp:Panel ID="pnlFTO" runat="server" Visible="False">
               <table class="style4" style="border: thin dotted #FF9933">
                   <tr>
                       <td class="style11" valign="middle" align="center">
                           <asp:Button ID="btnCbnTransfers" runat="server" Height="87px" 
                           Text="CBN Transfers" UseSubmitBehavior="False" Width="356px" Font-Bold="True" 
                               Font-Names="Century Gothic" Font-Size="XX-Large" 
                               ToolTip="For transfers on accounts that are not NUBAN compliant" />
                       </td>
                   </tr>
                   <tr>
                       <td class="style11">
                           <asp:Button ID="btnCustomerTransfers" runat="server" Height="87px" 
                           Text="Interbank Transfers" UseSubmitBehavior="False" Width="356px" 
                               Font-Bold="True" Font-Names="Century Gothic" Font-Size="XX-Large" 
                               ToolTip="For transfers to NUBAN accounts" />
                       </td>
                   </tr>
               </table>
           </asp:Panel>
           <asp:Panel ID="pnlMT202Types" runat="server" Visible="False">
               <table class="style4" style="border: thin dotted #FF9933">
                   <tr>
                       <td class="style11" valign="middle">
                           <br />
                           <asp:Label ID="lblTropsMessage0" runat="server" Font-Bold="True" 
                               Font-Names="Century Gothic" Font-Size="XX-Large" 
                               Text="Message Variants For MT202"></asp:Label>
                           <asp:RadioButtonList ID="rblMT202type" runat="server" AutoPostBack="True" 
                               Font-Bold="True" Font-Names="Century Gothic" Font-Size="XX-Large">
                               <asp:ListItem Value="RESERVATION">MT202 Reservation</asp:ListItem>
                               <asp:ListItem Value="SDF">MT202 SDF</asp:ListItem>
                               <asp:ListItem Value="REGULAR">MT202 Regular</asp:ListItem>
                           </asp:RadioButtonList>
                           <br />
                       </td>
                   </tr>
               </table>
           </asp:Panel>
           <asp:Panel ID="pnlMt202Interbank" runat="server" Visible="False">
               <table class="style4" style="border: thin dotted #FF9933">
                   <tr>
                       <td class="style11" valign="middle">
                           <br />
                           <asp:Label ID="Label1" runat="server" Font-Bold="True" 
                               Font-Names="Century Gothic" Font-Size="XX-Large" 
                               Text="Message Variants For MT202"></asp:Label>
                           <br />
                           <asp:RadioButtonList ID="rblMt202InterbankRegular" runat="server" 
                               AutoPostBack="True" Font-Bold="True" Font-Names="Century Gothic" 
                               Font-Size="XX-Large">
                               <asp:ListItem Value="REGULAR">MT202 Regular</asp:ListItem>
                           </asp:RadioButtonList>
                       </td>
                   </tr>
               </table>
           </asp:Panel>
&nbsp;<br />
           <br />
           <br />
           <br />
           <br />
           <br />
           <br />
           <br /></div>
       </div>
         <div id="footer">
    
             Sterling Bank Plc<script language="javascript">





          $("#TextBox3").blur(function () {
              var x = $(this).val();
              if (isNaN(x)) {
                  alert("Value " + x + " is not a number");
                  $(this).val("");
                  return;
              }
              var num = new Number(x);
              var formattedMoney = num.formatMoney(2, ',', '.');
              //alert(formattedMoney);
              $(this).val(formattedMoney);
          });
  </script>
  </form>
</body>
</html>
