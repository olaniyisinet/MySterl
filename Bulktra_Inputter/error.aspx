<%@ Page Language="VB" AutoEventWireup="false" CodeFile="error.aspx.vb" Inherits="error2" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Bulktra Version 2:::Sterling Bank Plc</title>
    <style type="text/css"> 
        /*CSS RESET*/
html, body, div, span, applet, object, iframe,
h1, h2, h3, h4, h5, h6,  blockquote, pre,
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

@font-face {
	font-family: Segoe UI;
	src: url('segoeuil.ttf');
}

p{
	font-family: Segoe UI; /* no .ttf */
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
	font-weight:bold;
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
		font:13px Segoe UI
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
            height: 89px;
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
		background:#FFFFFF;
            height: 315px;
        }
	
	#footer {
		background:#EC3515;
		text-align:center;
            height: 4px;
        }
	
	#footer a {
		color:#fff;
	}
	
       
	
        .style2
        {
            width: 849px;
            border-collapse: collapse;
            height: 86px;
        }
        .style3
        {
            width: 57px;
        }
        .style4
        {
            font-size: xx-large;
        }
	
       
	
        .boxshadow1 {
-moz-box-shadow: 0 0 5px #888;
-webkit-box-shadow: 0 0 5px#888;
box-shadow: 0 0 5px #888;
}
 
.boxshadow2 {
-moz-box-shadow: 0 0 5px 5px #888;
-webkit-box-shadow: 0 0 5px 5px#888;
box-shadow: 0 0 5px 5px #888;
}

	
	.roundImage

{

-moz-border-radius: 7px;

	-webkit-border-radius: 7px;

	border-radius: 7px;
    -moz-box-shadow: 0 0 5px #888;
-webkit-box-shadow: 0 0 5px#888;
box-shadow: 0 0 5px #888;
}
	
				 .txt 
    {
         -moz-border-radius: 10px;
    -webkit-border-radius: 10px;
    border-radius: 10px;
    background-color:#ffffff;
            }
    
     
	
        .style8
        {
            height: 21px;
        }
            
     
	
        .style12
        {
            font-size: large;
            padding: 1px;
        }
        .style13
        {
            width: 63px;
            height: 64px;
        }
    
     
	
        .style14
        {
            height: 27px;
        }
    
     
	
        .style15
        {
            height: 2px;
        }
    
     
	
    </style>
     <link href="css/layout.css" rel="stylesheet" type="text/css" />
        <link href="css/menu.css" rel="stylesheet" type="text/css" />

</head>
<body>
    <form id="form1" runat="server">
    <div id="header">
    
        <table class="style2">
            <tr>
                <td class="style3" valign="middle">
                    <br />
                    <img alt="" class="style13" src="images/icon3.png" /><br />
                    <br />
                </td>
                <td class="style4" valign="middle">
                    <strong>Bulktra version 2.0</strong><br />
                    <span class="style12">Bulk payment system</span></td>
            </tr>
        </table>
    
    </div>

      <div id="content">
         
          <br />
         
          <asp:Label ID="Label1" runat="server"></asp:Label>
    
          <br />
          <br />
          <table  align="center" style="width: 812px; height: 171px;" bgcolor="#FFCC99" 
              class="boxshadow1" style="margin:5px;">
              <tr>
                  <td class="style15">
                                   
          <br />
    &nbsp;</td>
              </tr>
              <tr>
                  <td class="style14">
                      <asp:Label ID="Label4" runat="server" Font-Bold="False" 
                          Font-Names="Segoe UI" Font-Size="15pt" ForeColor="#990000">An error occurred trying to process your request</asp:Label>
                      <br />
                      <br />
                      &nbsp;<br />
                  </td>
              </tr>
              <tr>
                  <td class="style8">
                      &nbsp;</td>
              </tr>
              </table>
    
          <br />
          <br />
          <br />
          <br />
          <br />
          <br />
          <br />
          <br />
    
    </div>

    </form>
</body>
</html>
