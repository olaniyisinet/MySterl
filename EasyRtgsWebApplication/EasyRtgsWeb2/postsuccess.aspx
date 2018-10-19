<%@ Page Language="VB" AutoEventWireup="false" CodeFile="postsuccess.aspx.vb" Inherits="postsuccess" %>

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
	
    </style> 
</head>
<body>
    <form id="form1" runat="server">
    <div id="header" class="style1">
    
        Sterling easyRTGS<br />
        <span class="style2">Third Party Transfer Application</span></div>


       <div id="content">
       <div class="wrap">
           Welcome,
           <asp:Label ID="Label1" runat="server"></asp:Label>
           &nbsp;&nbsp; |&nbsp; 
           <asp:HyperLink ID="hypHome" runat="server" >Home</asp:HyperLink>
           <%--<a href="main.aspx">Home</a>--%>&nbsp;&nbsp; |&nbsp;&nbsp; 
           <asp:HyperLink ID="hypMyTransactions" runat="server" NavigateUrl="~/mytransactions.aspx" >My Transactions</asp:HyperLink>&nbsp;&nbsp; |&nbsp;&nbsp;&nbsp;<asp:HyperLink ID="hypLogout" runat="server" NavigateUrl="~/logout.aspx" >Log Out</asp:HyperLink>
           &nbsp;<%--<a href="logout.aspx">Sign Out</a>--%><br />
           <br />
           <br />
           <br />
           <br />
           <span class="style20">Posting was Successful.</span> <br />
            <br />
           <asp:Panel ID="Panel1" runat="server" Height="147px">
           <span class="style6"> <strong><asp:HyperLink ID="hypLinkBranch" runat="server" 
               NavigateUrl="~/main.aspx">Post New Transaction</asp:HyperLink></strong></span>&nbsp;           
           <span class="style6"> <strong>
               <br />
               <asp:HyperLink ID="hypLinkTrops" runat="server" 
               NavigateUrl="~/tropsmain.aspx">Post New Transaction</asp:HyperLink></strong></span>
           </asp:Panel>
          
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
</body>
</html>
