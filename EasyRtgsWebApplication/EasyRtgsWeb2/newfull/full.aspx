<%@ Page Language="VB" AutoEventWireup="false" CodeFile="full.aspx.vb" Inherits="full" %>

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
            width: 175px;
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
	
        .style21
        {
            width: 52px;
            text-align: right;
            height: 51px;
        }
        .style22
        {
            height: 32px;
            width: 437px;
        }
        .style23
        {
            text-align: right;
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
            text-align: right;
            }
        .style28
        {
            height: 29px;
            width: 437px;
        }
        .style29
        {
            height: 30px;
            width: 437px;
            text-align: right;
        }
	
        .style30
        {
            font-size: x-large;
        }
	
        .style31
        {
            text-align: right;
            height: 29px;
        }
	
    </style>     
     <script language="javascript">
                      function hide() {

                          document.getElementById("output").style.visibility = "hidden";
                      }


                      function conf() {
                          var ans = confirm('This action will cause a DEBIT/CREDIT. Continue?');
                          document.getElementById("ans").value = ans;
                      }

                      function rson() {
                          var r = window.prompt("Enter reason for discarding:", "");
                          document.getElementById("reason").value = r;

                      }
  </script>

    <script language="javascript">
        function getParameterByName(name) {

            var match = RegExp('[?&]' + name + '=([^&]*)')
                    .exec(window.location.search);

            return match && decodeURIComponent(match[1].replace(/\+/g, ' '));

        }
        function open1() {
            window.open("print.aspx?tid=" + getParameterByName("tid"), "Print", "location=1,status=1,scrollbars=1, width=800,height=600");
        }

      
function Button4_onclick() {

}

    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div id="header" class="style1">
    <input type="hidden" id="reason" runat="server" />
        <img alt="" class="style21" src="images/icon.png" />Sterling easyRTGS<br />
        <span class="style2">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Third Party Transfer Application</span></div>


       <div id="content">
       <div class="wrap">
           Welcome,
           <asp:Label ID="Label1" runat="server"></asp:Label>
           &nbsp;&nbsp; |&nbsp; 
           <a href="main.aspx">Home</a>&nbsp;&nbsp; |&nbsp;&nbsp; <a href="main.aspx">My Transactions</a>&nbsp;&nbsp; | &nbsp;<a href="funding.aspx">Funding 
           Report</a>&nbsp; | &nbsp;<a href="logout.aspx">Sign Out</a><br />
           <br />
           <span class="style20">Transaction Details for ID:
           <asp:Label ID="Label3" runat="server" Font-Bold="True"></asp:Label>
           </span>
          <%-- <span class="style6">--%>
           <br />
           <br />
           <asp:Button ID="Button1" runat="server" Font-Names="Segoe UI Light" 
               Font-Size="11pt" Width="169px" 
               
               UseSubmitBehavior="False" onclientclick="conf();" />
&nbsp;&nbsp;<input id="Button2" 
               style="font-family: 'segoe ui Light'; font-size: 16px; font-weight: bold; width: 169px; height: 35px;" 
               type="button" value="Pay" runat="server" onserverclick="complete" 
               onclick="this.value='Paying...'; this.disabled=true;" />&nbsp;
          <%-- <asp:Button ID="Button3" runat="server" Font-Names="Segoe UI Light" 
               Font-Size="11pt" Width="169px" 
               onclientclick="rson();" 
               Text="Discard Transaction" UseSubmitBehavior="False" />--%>
               <asp:Button ID="Button3" runat="server" Font-Names="Segoe UI Light" 
               Font-Size="11pt" Width="169px"               
               Text="Discard Transaction" UseSubmitBehavior="False" />
           &nbsp;
          <%-- <span class="style20">--%>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
           <%--<input id="Button4" 
               style="font-family: 'segoe ui Light'; font-size: 16px; font-weight: bold; width: 169px; height: 35px;" 
               type="button" value="Reject Request" runat="server" onserverclick="reverseAllEntries" 
               onclick="this.value='Reversing All Entries...'; this.disabled=true;" onclick="return Button4_onclick()" />--%>
               <input id="Button4" 
               style="font-family: 'segoe ui Light'; font-size: 16px; font-weight: bold; width: 169px; height: 35px;" 
               type="button" value="Reject Request" runat="server" onserverclick="reverseAllEntries" 
               onclick="this.value='Reversing All Entries...'; this.disabled=true;" />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
           <br />
           <br />
           <asp:Label ID="Label21" runat="server" Text="Comment:"></asp:Label>
               <asp:TextBox ID="txtComment" runat="server" Height="63px" 
               TextMode="MultiLine" ToolTip="Comment" Width="387px"></asp:TextBox>&nbsp;
           <asp:Label ID="Label16" runat="server" Font-Bold="True" Font-Size="11pt" 
               ForeColor="#003300"></asp:Label>
          <%-- </span>--%>
           <br />
           <br />
          <%-- </span>--%>
           <input type="hidden" id="ans" runat="server" />
           <table class="style4" style="border: thin dashed #FF9933; font-size: medium;" 
               bgcolor="#FFFFCC">
               <tr>
                   <td class="style11" valign="middle">
                       <br />
                   </td>
                   <td valign="middle" class="style29">
                       <a href="javascript:void(0);" onclick="open1();"><strong>Send to Printer</strong></a></td>
               </tr>
               <tr>
                   <td class="style11" valign="middle" bgcolor="#FFFFCC">
                       Transaction REF:</td>
                   <td valign="middle" class="style12" bgcolor="#FFFFCC">
                       <asp:Label ID="Label20" runat="server" Font-Bold="True" ForeColor="#006600"></asp:Label>
                   </td>
               </tr>
               <tr>
                   <td class="style11" valign="middle" bgcolor="#FFFFCC">
                       Customer Account Number:</td>
                   <td valign="middle" class="style12" bgcolor="#FFFFCC">
                       <asp:Label ID="Label4" runat="server" Font-Bold="True" ForeColor="#006600"></asp:Label>
                   </td>
               </tr>
               <tr>
                   <td class="style11" valign="middle" bgcolor="#FFFFCC">
                       Customer Name:</td>
                   <td valign="middle" class="style12" bgcolor="#FFFFCC">
                       <asp:Label ID="Label5" runat="server" Font-Bold="True" ForeColor="#006600"></asp:Label>
                   </td>
               </tr>
               <tr>
                   <td class="style11" valign="middle" bgcolor="#FFFFCC">
                       Amount to Transfer:</td>
                   <td class="style12" valign="middle" bgcolor="#FFFFCC">
                       <asp:Label ID="Label7" runat="server" Font-Bold="True" ForeColor="#006600"></asp:Label>
                   </td>
               </tr>
               <tr>
                   <td class="style11" valign="middle" bgcolor="#FFFFCC">
                       Charges:</td>
                   <td class="style12" valign="middle" bgcolor="#FFFFCC">
                       <asp:Label ID="Label17" runat="server" Font-Bold="True" ForeColor="#006600"></asp:Label>
                   </td>
               </tr>
               <tr>
                   <td class="style21" valign="middle" bgcolor="#FFFFCC">
                       Remarks:</td>
                   <td class="style22" valign="middle" bgcolor="#FFFFCC">
                       <asp:Label ID="Label8" runat="server" Font-Bold="True" ForeColor="#006600"></asp:Label>
                   </td>
               </tr>
               <tr>
                   <td class="style23" valign="middle" bgcolor="#FFFFCC">
                       Beneficiary:</td>
                   <td class="style24" valign="middle" bgcolor="#FFFFCC">
                       <asp:Label ID="Label18" runat="server" Font-Bold="True" ForeColor="#006600"></asp:Label>
                   </td>
               </tr>
               <tr>
                   <td class="style23" valign="middle" bgcolor="#FFFFCC">
                       Beneficiary Bank:</td>
                   <td class="style24" valign="middle" bgcolor="#FFFFCC">
                       <asp:Label ID="Label19" runat="server" Font-Bold="True" ForeColor="#006600"></asp:Label>
                   </td>
               </tr>
               <tr>
                   <td class="style23" valign="middle" bgcolor="#FFFFCC">
                       Beneficiary Account Number:</td>
                   <td class="style24" valign="middle" bgcolor="#FFFFCC">
                       <asp:Label ID="lblBenAcctNum" runat="server" Font-Bold="True" 
                           ForeColor="#006600"></asp:Label>
                   </td>
               </tr>
               <tr>
                   <td class="style23" valign="middle" bgcolor="#FFFFCC">
                       Uploaded By:</td>
                   <td class="style24" valign="middle" bgcolor="#FFFFCC">
                       <asp:Label ID="Label9" runat="server" Font-Bold="True" ForeColor="#006600"></asp:Label>
                   </td>
               </tr>
               <tr>
                   <td class="style11" valign="middle" bgcolor="#FFFFCC">
                       Upload Date:</td>
                   <td class="style12" valign="middle" bgcolor="#FFFFCC">
                       <asp:Label ID="Label10" runat="server" Font-Bold="True" ForeColor="#006600"></asp:Label>
                   </td>
               </tr>
               <tr>
                   <td class="style25" valign="middle" bgcolor="#FFFFCC">
                       Authorized By:</td>
                   <td class="style26" valign="middle" bgcolor="#FFFFCC">
                       <asp:Label ID="Label11" runat="server" Font-Bold="True" ForeColor="#006600"></asp:Label>
                   </td>
               </tr>
               <tr>
                   <td class="style25" valign="middle" bgcolor="#FFFFCC">
                       Authorized Date:</td>
                   <td class="style26" valign="middle" bgcolor="#FFFFCC">
                       <asp:Label ID="Label15" runat="server" Font-Bold="True" ForeColor="#006600"></asp:Label>
                   </td>
               </tr>
               <tr>
                   <td class="style23" valign="middle" bgcolor="#FFFFCC">
                       Treasury Approval:</td>
                   <td class="style24" valign="middle" bgcolor="#FFFFCC">
                       <asp:Label ID="Label12" runat="server" Font-Bold="True" ForeColor="#006600"></asp:Label>
                   </td>
               </tr>
               <tr>
                   <td class="style31" valign="middle" bgcolor="#FFFFCC">
                       Approved By:</td>
                   <td class="style28" valign="middle" bgcolor="#FFFFCC">
                       <asp:Label ID="Label13" runat="server" Font-Bold="True" ForeColor="#006600"></asp:Label>
                   </td>
               </tr>
               <tr>
                   <td class="style27" valign="middle" bgcolor="#FFFFCC" colspan="2">
                       <strong class="style30">Scanned Customer Instruction<br />
                       </strong>
                       <asp:Label ID="Label14" runat="server" Font-Bold="True" ForeColor="#006600"></asp:Label>
                   </td>
               </tr>
               </table>
           &nbsp;
           <br />
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
    
             Sterling Bank Plc</div>
    <br />
    <br />
    <br />
    </form>
</body>
</html>
