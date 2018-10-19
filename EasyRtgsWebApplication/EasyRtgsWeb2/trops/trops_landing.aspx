<%@ Page Language="VB" AutoEventWireup="false" CodeFile="trops_landing.aspx.vb" Inherits="trops_landing"     %>
 
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
        .style5
        {
            width: 152px;
            text-align: right;
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
            width: 152px;
            text-align: right;
            height: 30px;
        }
        .style12
        {
            height: 30px;
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
	
        .style15
        {
            width: 152px;
            text-align: right;
            height: 25px;
        }
        .style16
        {
            height: 25px;
            width: 437px;
        }
        .style17
        {
            width: 152px;
            text-align: right;
            height: 24px;
        }
        .style18
        {
            height: 24px;
            width: 437px;
        }
        .style19
        {
            width: 437px;
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
            width: 139px;
        }
	
        .auto-style1 {
            width: 100%;
        }
        .auto-style2 {
            width: 176px;
        }
        .auto-style3 {
            width: 193px;
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
    
        <img alt="" class="style21" src="../images/icon.png" />Sterling easyRTGS        <span class="style2">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Third Party Transfer Application</span></div>


       <div id="content">
       <div class="wrap">
           <strong class="style3">TROPS..::..Landing Page</strong><br />
           <br />
           Welcome,
           <asp:Label ID="lblUserFullName" runat="server"></asp:Label>
          <%-- &nbsp;&nbsp; |&nbsp;&nbsp; <a href="mytransactions.aspx">My Transactions</a>&nbsp;&nbsp; |&nbsp;&nbsp;&nbsp; 
           <a href="logout.aspx">Sign Out</a><br />
           <br />
           <span class="style20">Post Transaction</span><span class="style6"><br />--%>
           &nbsp; <%--<a href="trops_landing.aspx">Landing Page</a> ||--%> <a href="../mytransactions.aspx">My Transactions</a>&nbsp;&nbsp; |&nbsp;&nbsp;<a href="../logout.aspx">Log Out</a><br />
           <asp:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server">
           </asp:ToolkitScriptManager>
           </span>&nbsp;<input id="hiddenUseAmtLimit" type="hidden" runat="server" />
           &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
           <br />
           &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
           <br />
           <table class="auto-style1">
               <tr>
                   <td class="auto-style3"><strong>
                       <asp:Label ID="Label1" runat="server" Text="INTER-BANK TRANSACTIONS:"></asp:Label>
                       </strong></td>
                   <td class="auto-style2">
           <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="~/trops/interbank/mt103_interbank.aspx">Inter-Bank Transfer MT103</asp:HyperLink>
                   </td>
                   <td>
           <asp:HyperLink ID="HyperLink2" runat="server" NavigateUrl="~/trops/interbank/mt202_interbank.aspx">Inter-Bank Transfer MT202</asp:HyperLink>
                   </td>
               </tr>
               <tr>
                   <td class="auto-style3"><strong>
                       <asp:Label ID="Label2" runat="server" Text="CBN TRANSACTIONS:"></asp:Label>
                       </strong></td>
                   <td class="auto-style2">
           <asp:HyperLink ID="HyperLink3" runat="server" NavigateUrl="~/trops/cbn/mt103_cbn.aspx">CBN Transfer MT103</asp:HyperLink>
                   </td>
                   <td>
           <asp:HyperLink ID="HyperLink4" runat="server" NavigateUrl="~/trops/cbn/mt202_cbn.aspx">CBN Transfer MT202</asp:HyperLink>
                   </td>
               </tr>
           </table>
           <br />
           <br />
           <br />
           <br />
           <br />
           <br /></div>
       </div>
         <div id="footer">
    
             Sterling Bank Plc</div>
    <br />
    <br />
    <br />
    </form>


      <script language="javascript">





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
</body>
</html>
