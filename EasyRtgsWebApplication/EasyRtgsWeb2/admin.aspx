<%@ Page Language="VB" AutoEventWireup="false" CodeFile="admin.aspx.vb" Inherits="admin" %>

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
            color: #006600;
        }
	
        .style21
        {
            width: 100%;
            border-collapse: collapse;
        }
        .style22
        {
            width: 163px;
            text-align: center;
        }
	
        .style23
        {
            color: #FFFFFF;
        }
	
        .style25
        {
            width: 56px;
            height: 54px;
        }
	
        .style26
        {
            width: 54px;
            height: 54px;
        }
        .style27
        {
            width: 48px;
            height: 48px;
        }
	
    </style> 
    <script language="javascript">
        function users() {
            window.location = "users.aspx";
                   }


                   function audit() {
                       window.location = "audit.aspx";
                   }
                   function txns() {
                       window.location = "txns.aspx";
                   }


                   function swift() {
                       window.location = "swift.aspx";
                   }

                   function settings() {
                       window.location = "settings.aspx";
                   }


        function chg() {
            document.getElementById("users").style.backgroundColor = "#800000";

        }


        function chg2() {
            document.getElementById("users").style.backgroundColor = "gray";
        }

        function chgt() {
            document.getElementById("txns").style.backgroundColor = "#800000";

        }


        function chgt2() {
            document.getElementById("txns").style.backgroundColor = "gray";
        }

        function chga() {
            document.getElementById("audit").style.backgroundColor = "#800000";

        }


        function chga2() {
            document.getElementById("audit").style.backgroundColor = "gray";
        }

        function chgs() {
            document.getElementById("swift").style.backgroundColor = "#800000";

        }

        
        function chgs2() {
            document.getElementById("swift").style.backgroundColor = "gray";
        }

        function chgsettings() {
            document.getElementById("settings").style.backgroundColor = "#800000";

        }


        function chgsettings2() {
            document.getElementById("settings").style.backgroundColor = "gray";
        }

    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div id="header" class="style1">
    
        Sterling easyRTGS<br />
        <span class="style2">Third Party Transfer Application</span></div>


       <div id="content">
       <div class="wrap">
           <span class="style20">Administrator</span><span class="style6"><br />
           <br />
           </span>
           <br />
           Welcome,
           <asp:Label ID="Label1" runat="server"></asp:Label>
           &nbsp;&nbsp; |&nbsp;&nbsp; <a href="admin.aspx">Home</a>&nbsp;&nbsp; |&nbsp;&nbsp;&nbsp; 
           <a href="logout.aspx">Sign Out</a><br />
           <br />
           <br />
           <br />
           <br />
           <br />
           <br />
           <table class="style21">
               <tr valign="middle" align="center">
                   <td class="style22" valign="middle" id="users" onclick="users();" 
                       onmouseover="chg();" onmouseout="chg2();" bgcolor="#666666">
                       <strong class="style23">
                       <br />
                       <img alt="" class="style25" src="images/stock_people.png" /><br />
                       User Management</strong><br />
                       <br />
                       <br />
                   </td>
                    <td class="style22" valign="middle" id="txns" onclick="txns();" 
                       onmouseover="chgt();" onmouseout="chgt2();" bgcolor="#666666">
                       <strong class="style23">
                       <br />
                        <img alt="" class="style26" src="images/money.png" /><br />
                        Transactions Manager</strong><br />
                       <br />
                       <br />
                   </td>
                    <td class="style22" valign="middle" id="audit" onclick="audit();" 
                       onmouseover="chga();" onmouseout="chga2();" bgcolor="#666666">
                       <strong class="style23">
                       <br />
                        <img alt="" class="style27" src="images/laptop1.png" /><br />
                        Setup</strong><br />
                       <br />
                       <br />
                   </td>
                    
                       <td class="style22" valign="middle" id="swift" onclick="swift();" 
                       onmouseover="chgs();" onmouseout="chgs2();" bgcolor="#666666">
                       <strong class="style23">
                       <br />
                        <img alt="" class="style27" src="images/money.png" /><br />
                        Swift Manager</strong><br />
                       <br />
                       <br />
                   </td>
                   <%--A new column for settings comes here--%>
                   <td class="style22" valign="middle" id="settings" onclick="settings();" 
                       onmouseover="chgsettings();" onmouseout="chgsettings2();" bgcolor="#666666">
                       <strong class="style23">
                       <br />
                        <img alt="" class="style27" src="images/settings.jpg" /><br />
                        Settings</strong><br />
                       <br />
                       <br />
                   </td>
               </tr>
           </table>
           <br />
           <br />
           &nbsp;<br />
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
</body>
</html>
