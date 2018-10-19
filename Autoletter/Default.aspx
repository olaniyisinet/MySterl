<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Default.aspx.vb" Inherits="_Default" %>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Auto Letter Generation:::Sterling Bank Plc</title>
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
		background:#333333 url('Texture3.jpg');
            height: 3px;
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
            width: 671px;
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
            width: 536px;
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
    
     
	
        .style14
        {
            width: 563px;
            height: 285px;
        }
    
     
	
        .style15
        {
            width: 19px;
        }
    
     
	
        .style16
        {
            width: 19px;
            height: 55px;
        }
        .style17
        {
            height: 55px;
        }
    
     
	
    </style>


      <script src="jquery-1.8.3.js" type="text/javascript"></script>
    <link href="jquery-ui.css" rel="stylesheet" type="text/css" />
    <script src="jquery-ui.js" type="text/javascript"></script>

</head>
<body>
    <form id="form1" runat="server">

      <div id="content">
    
          <br />
          <br />
          <br />
          <br />
    
          <br />
          <table  align="center" style="width: 900px; height: 344px;" bgcolor="White">
              <tr>
                  <td class="style6" rowspan="6" valign="middle">
                      &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                      <img alt=""   src="images/banner1.png" /></td>
                  <td valign="middle" class="style15">
                      &nbsp;</td>
                  <td valign="middle">
                      <span class="style10">Sign In</span></td>
              </tr>
              <tr>
                  <td valign="middle" class="style15">
                      &nbsp;</td>
                  <td valign="middle">
                      <asp:Label ID="Label1" runat="server" Font-Bold="True" ForeColor="Red"></asp:Label>
                  </td>
              </tr>
              <tr>
                  <td valign="middle" class="style15">
                      &nbsp;</td>
                  <td valign="middle">
                      Username<br />
                      <asp:TextBox ID="TextBox1" runat="server" CssClass="txt" Height="34px" 
                          Width="262px" placeholder="Username"></asp:TextBox>
                  </td>
              </tr>
              <tr>
                  <td valign="middle" class="style16">
                      </td>
                  <td valign="middle" class="style17">
                      Password<br />
                      <asp:TextBox ID="TextBox2" runat="server" CssClass="txt" Height="34px" 
                          Width="262px" TextMode="Password" placeholder="Password"></asp:TextBox>
                  </td>
              </tr>
              <tr>
                  <td valign="middle" class="style15">
                      &nbsp;</td>
                  <td valign="middle">
                      <asp:Button ID="Button1" runat="server" BackColor="#990000" CssClass="txt" 
                          Font-Names="Century Gothic" ForeColor="White" Height="36px" Text="Sign In" 
                          Width="138px" />
                  </td>
              </tr>
              <tr>
                  <td valign="middle" class="style15">
                      &nbsp;</td>
                  <td valign="middle">
                      &nbsp;</td>
              </tr>
          </table>
          <br />
          <br />
          <br />
          <br />
    
    </div>

    </form>
</body>
</html>