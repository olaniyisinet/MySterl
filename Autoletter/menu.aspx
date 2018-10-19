﻿<%@ Page Language="VB" AutoEventWireup="false" CodeFile="menu.aspx.vb" Inherits="menu" %>

 
<!DOCTYPE html /> 
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
   <title>Auto Letter Generation:::Sterling Bank Plc</title>
   <style type="text/css">
        .feed {
	-moz-box-shadow:inset 0px 1px 0px 0px #f5978e;
	-webkit-box-shadow:inset 0px 1px 0px 0px #f5978e;
	box-shadow:inset 0px 1px 0px 0px #f5978e;
	background:-webkit-gradient( linear, left top, left bottom, color-stop(0.05, #f24537), color-stop(1, #c62d1f) );
	background:-moz-linear-gradient( center top, #f24537 5%, #c62d1f 100% );
	filter:progid:DXImageTransform.Microsoft.gradient(startColorstr='#f24537', endColorstr='#c62d1f');
	background-color:#f24537;
	-moz-border-radius:6px;
	-webkit-border-radius:6px;
	border-radius:6px;
	border:1px solid #d02718;
	display:inline-block;
	color:#ffffff;
	font-family:arial;
	font-size:15px;
	font-weight:bold;
	padding:6px 24px;
	text-decoration:none;
	text-shadow:1px 1px 0px #810e05;
}.feed:hover {
	background:-webkit-gradient( linear, left top, left bottom, color-stop(0.05, #c62d1f), color-stop(1, #f24537) );
	background:-moz-linear-gradient( center top, #c62d1f 5%, #f24537 100% );
	filter:progid:DXImageTransform.Microsoft.gradient(startColorstr='#c62d1f', endColorstr='#f24537');
	background-color:#c62d1f;
}.feed:active {
	position:relative;
	top:1px;
}
/* This imageless css button was generated by CSSButtonGenerator.com */
</style>
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
	    border-style: none;
            border-color: inherit;
            border-width: 0;
            margin: 0;
            padding: 0;
            outline: 0;
            font-weight: inherit;
	        font-style: inherit;
	font-size: small;
	        vertical-align: baseline;
}

@font-face {
	font-family: segoe ui light;
	src: url('segoeuil.ttf');
}

p{
	font-family: segoe ui light; /* no .ttf */
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
            color: #C0C0C0;
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
		background:#333333;
            height: 141px;
            color: White;
               background-image: url('Texture3.jpg'); 
             background-repeat: repeat;
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
		background:#333333;
		text-align:center;
            height: 91px;
               background-image: url('Texture3.jpg'); 
             background-repeat: repeat;
        }
	
	#footer a {
		color:#fff;
	}
	
       
	
        .style2
        {
            width: 1064px;
            border-collapse: collapse;
            height: 104px;
        }
        .style3
        {
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
    
     
	
        .style6
        {
        }
        .style10
        {
            font-size: x-large;
            padding: 1px;
        }
            
     
	
        .style12
        {
            width: 500px;
            height: 100px;
        }
    
     
	
        .style13
        {
            height: 27px;
        }
        .style14
        {
            color: #333333;
        }
    
     
	
        .style11
        {
            font-size: x-small;
        }
			 
		     
	
        .auto-style1
        {
            height: 53px;
        }
			 
		     
	
    </style>


      <script src="jquery-1.8.3.js" type="text/javascript"></script>
    <link href="jquery-ui.css" rel="stylesheet" type="text/css" />
    <script src="jquery-ui.js" type="text/javascript"></script>

<script language="javascript">

    function show() {
        document.getElementById("someDiv").style.visibility = "visible";

    }

    function hide() {
        document.getElementById("someDiv").style.visibility = "hidden";
    }
        </script>
</head>
<body onload="hide();"  >
    <form id="form1" runat="server">
    <div id="header">
    
        <table class="style2">
            <tr>
                <td class="style3" valign="middle" align="right">
                    <span class="style16"><strong>Welcome</strong></span><span class="style17">,&nbsp;
                    </span>
                    <asp:Label ID="Label2" runat="server" CssClass="style17" ForeColor="#FF3300"></asp:Label>
                    <span class="style17">&nbsp;&nbsp;&nbsp;&nbsp; |&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; </span>
                    <a class="style17" 
                        href="default2.aspx">Sign Out</a><span 
                        class="style16">&nbsp;&nbsp;&nbsp; </span>
                </td>
            </tr>
            <tr>
                <td class="style3" valign="middle">
                    <br />
                    <img alt="" class="style12" src="images/header.png" /><br />
                </td>
            </tr>
        </table>
    
    </div>

      <div id="content">
    
          <br />
          <table  align="center" style="width: 699px; height: 251px;" bgcolor="White" 
              class="boxshadow1">
              <tr>
                  <td class="style6" rowspan="3" valign="middle">
                      &nbsp;</td>
                  <td valign="middle" class="style13">
                      <span class="style10">menu option</span></td>
              </tr>
               
              <tr>
                  <td valign="middle" align="center">
                      <asp:Button ID="Button1" runat="server" BackColor="#990000" CssClass="feed" 
                          Font-Names="Century Gothic" ForeColor="White" Height="59px" Text="Fixed Deposit" 
                          Width="319px"  UseSubmitBehavior="False" />
                      <br />
                      <br />
                      <br />
                      <asp:Button ID="Button2" runat="server" BackColor="#990000" CssClass="feed" 
                          Font-Names="Century Gothic" ForeColor="White" Height="59px" Text="Bankers Acceptance" 
                          Width="320px"  UseSubmitBehavior="False" />
                  </td>
              </tr>
              </table>
          <br />
          <br />
          <br />
          <br />
    
    </div>

      <div id="footer">
    
          &nbsp;<strong>Powered by Technology Group</strong></div>
    </form>
</body>
</html>
