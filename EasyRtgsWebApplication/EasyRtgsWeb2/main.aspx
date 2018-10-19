<%@ Page Language="VB" AutoEventWireup="false" CodeFile="main.aspx.vb" Inherits="main"     %>
 
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
	
        #Select1 {
			width: 302px;
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
           <strong class="style3">Branch Initiator</strong><br />
           <br />
           Welcome,
           <asp:Label ID="Label1" runat="server"></asp:Label>
           &nbsp;&nbsp; |&nbsp;&nbsp; <a href="mytransactions.aspx">My Transactions</a>&nbsp;&nbsp; |&nbsp;&nbsp;&nbsp; 
           <a href="logout.aspx">Sign Out</a><br />
           <br />
           <span class="style20">Post Transaction</span><span class="style6"><br />
           <br />
           <asp:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server">
           </asp:ToolkitScriptManager>
           </span>&nbsp;<input id="hiddenUseAmtLimit" type="hidden" runat="server" />
           <table class="style4" style="border: thin dotted #FF9933">
               <tr>
                   <td class="style11" valign="middle">
                       &nbsp;</td>
                   <td valign="middle" class="style12">
                       <asp:Label ID="Label2" runat="server" Font-Bold="True" Font-Size="14pt" 
                           ForeColor="#339933"></asp:Label>
                   </td>
               </tr>
               <tr>
                   <td class="style11" valign="middle">
                       <asp:Label ID="lblOwnerBranch" runat="server" Text="Owner Branch:" 
                           ToolTip="Branch on whose behalf you are doing the transaction"></asp:Label>
                   </td>
                   <td valign="middle" class="style12">
                       <%--<asp:DropDownList ID="ddlOwnerBranch" runat="server" Width="301px" 
                           DataSourceID="odsOwnerBranch" DataTextField="BranchName" 
                           DataValueField="BranchCode">
                           <asp:ListItem Value="0">-Please Choose-</asp:ListItem>
                           <asp:ListItem Value="223">223-Head Office</asp:ListItem>
                       </asp:DropDownList>--%>
                       <asp:DropDownList ID="ddlOwnerBranch" runat="server" Width="301px" 
                           DataSourceID="odsOwnerBranchFromDb" DataTextField="Name" 
                           DataValueField="BranchCode">
                           <asp:ListItem Value="0">-Please Choose-</asp:ListItem>
                           <asp:ListItem Value="223">223-Head Office</asp:ListItem>
                       </asp:DropDownList>
                       <asp:ObjectDataSource ID="odsOwnerBranchFromDb" runat="server" 
                           SelectMethod="GetBrancheDetails" TypeName="BranchDetail">
                       </asp:ObjectDataSource>
                   </td>
               </tr>
               <tr>
                   <td class="style11" valign="middle">
                       Customer Account Number</td>
                   <td valign="middle" class="style12">
                    <asp:TextBox  AutoComplete="off"  ID="TextBox1" runat="server" BorderStyle="Solid" BorderWidth="1px" 
                        Height="26px" Width="301px"  Font-Size="12pt" Font-Names="Segoe UI Light"></asp:TextBox>
                   &nbsp;
					   
					   <input id="Button3" 
                           style="color: #FFFFFF; font-family: 'segoe ui Light'; background-color: #008000" 
                           type="button" value="Get Customer Data" runat="server" 
                           onclick="this.value='Getting data...'; this.disabled=true;" 
                           onserverclick="show2" CausesValidation="False" />
					   
					   <asp:RegularExpressionValidator ID="RegularExpressionValidator2" runat="server" 
                           ControlToValidate="TextBox1" Display="Dynamic" 
                           ErrorMessage="Enter a valid Account Number" 
                           ValidationExpression="^?[0-9]+(,[0-9]{3})*(\.[0-9]{2})?$" 
                           EnableClientScript="False"></asp:RegularExpressionValidator>
                       <asp:RequiredFieldValidator 
                           ID="RequiredFieldValidator1" runat="server" 
                           ControlToValidate="TextBox1" Display="Dynamic" 
                           ErrorMessage="must have value" EnableClientScript="False"></asp:RequiredFieldValidator>
                       <asp:RegularExpressionValidator ID="valRegExpNuban" runat="server" 
                           ControlToValidate="TextBox1" ErrorMessage="Must be a NUBAN account number." 
                           ValidationExpression="^\d{10}$"></asp:RegularExpressionValidator>
                   </td>
               </tr>
               <tr>
                   <td class="style11" valign="middle">
                       Customer Name</td>
                   <td valign="middle" class="style12">
                    <asp:TextBox  AutoComplete="off" ID="TextBox2" runat="server" BorderStyle="Solid" BorderWidth="1px" 
                        Height="26px" Width="302px"  Font-Size="12pt" Font-Names="Segoe UI Light" 
                           ReadOnly="True"></asp:TextBox>
                   &nbsp;&nbsp;
                       <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" 
                           ControlToValidate="TextBox2" ErrorMessage="must have value" 
                           EnableClientScript="False"></asp:RequiredFieldValidator>
                   &nbsp;</td>
               </tr>
               <tr>
                   <td class="style11" valign="middle">
                       Customer Account Balance</td>
                   <td valign="middle" class="style12">
                       <asp:Label ID="Label3" runat="server" Font-Bold="True" Font-Size="14pt" 
                           ForeColor="#339933"></asp:Label>
                   </td>
               </tr>
               <tr>
                   <td class="style11" valign="middle">
                       Amount to Transfer</td>
                   <td class="style12" valign="middle">
                    <asp:TextBox  AutoComplete="off" ID="TextBox3" runat="server" BorderStyle="Solid" BorderWidth="1px" 
                        Height="26px" Width="302px"  Font-Size="12pt" Font-Names="Segoe UI Light"></asp:TextBox>
                   &nbsp;&nbsp;
                       <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" 
                           ControlToValidate="TextBox3" Display="Dynamic" 
                           ErrorMessage="must have value" EnableClientScript="False"></asp:RequiredFieldValidator>
                       &nbsp;
                       <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" 
                           ControlToValidate="TextBox3" Display="Dynamic" 
                           ErrorMessage="Enter a valid amount" 
                           ValidationExpression="^?[0-9]+(,[0-9]{3})*(\.[0-9]{2})?$" 
                           EnableClientScript="False"></asp:RegularExpressionValidator>
                   &nbsp;<asp:CompareValidator ID="CompareValidator1" runat="server" 
                           ControlToValidate="TextBox3" 
                           ErrorMessage="must be greater than or equal to 10million" 
                           Operator="GreaterThanEqual" Type="Double" ValueToCompare="10000000.00" 
                           Enabled="False"></asp:CompareValidator>
                   </td>
               </tr>
               <tr>
                   <td class="style11" valign="middle">
                       Charges</td>
                   <td class="style12" valign="middle">
                       <strong>Customers: </strong>
                       <asp:Label ID="Label4" runat="server"></asp:Label>
                       &nbsp;&nbsp; <strong>Staff:</strong>&nbsp;
                       <asp:Label ID="Label5" runat="server"></asp:Label>
                   &nbsp;
                       <asp:CheckBox ID="CheckBox1" runat="server" AutoPostBack="True" 
                           style="font-weight: 700" Text="Concession" />
&nbsp;
                    <asp:TextBox  AutoComplete="off" ID="TextBox6" runat="server" BorderStyle="Solid" BorderWidth="1px" 
                        Height="26px" Width="63px"  Font-Size="12pt" Font-Names="Segoe UI Light">0.00</asp:TextBox>
                   </td>
               </tr>
               <tr>
                   <td class="style15" valign="middle">
                       Remarks</td>
                   <td class="style16" valign="middle">
                    <asp:TextBox  AutoComplete="off" ID="TextBox4" runat="server" BorderStyle="Solid" BorderWidth="1px" 
                        Height="60px" Width="302px"  Font-Size="12pt" Font-Names="Segoe UI Light" 
                           TextMode="MultiLine"></asp:TextBox>
                   &nbsp;&nbsp;
                       <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" 
                           ControlToValidate="TextBox4" ErrorMessage="must have value" 
                           EnableClientScript="False" Display="Dynamic"></asp:RequiredFieldValidator>
                   &nbsp;</td>
               </tr>
               <tr>
                   <td class="style15" valign="middle">
                       Beneficiary Bank                  </td>
                   <td class="style16" valign="middle">
                     
                       <asp:DropDownList ID="DropDownList1" runat="server" Height="26px">
             
                         
                       </asp:DropDownList>
                       <asp:RequiredFieldValidator ID="RequiredFieldValidator7" runat="server" 
                           ControlToValidate="DropDownList1" ErrorMessage="must have value" 
                           EnableClientScript="False" Display="Dynamic"></asp:RequiredFieldValidator>
</td>
               </tr>
               <tr>
                   <td class="style15" valign="middle">
                       Beneficiary Account</td>
                   <td class="style16" valign="middle">
                     
                    <asp:TextBox  AutoComplete="off" ID="TextBox7" runat="server" BorderStyle="Solid" BorderWidth="1px" 
                        Height="26px" Width="302px"  Font-Size="12pt" Font-Names="Segoe UI Light" 
                           MaxLength="10" AutoPostBack="True"></asp:TextBox>
&nbsp;<asp:RequiredFieldValidator ID="RequiredFieldValidator9" runat="server" 
                           ControlToValidate="TextBox7" ErrorMessage="must have value" 
                           EnableClientScript="False" Display="Dynamic"></asp:RequiredFieldValidator>
                       <asp:LinkButton ID="LinkButton1" runat="server" CausesValidation="False" Visible="False">Try again?</asp:LinkButton>
                       <br />
                       <input id="Button6" 
                           style="color: #FFFFFF; font-family: 'segoe ui Light'; visibility:hidden; background-color: #008000" 
                           type="button" value="Get Beneficiary Name" runat="server" 
                           onclick="this.value='Getting data...'; this.disabled=true;" 
                           onserverclick="benefName" CausesValidation="False" /><asp:RegularExpressionValidator 
                           ID="RegularExpressionValidator4" runat="server" ControlToValidate="TextBox7" 
                           ErrorMessage="Must be a NUBAN account number." ValidationExpression="^\d{10}$"></asp:RegularExpressionValidator>
                       <br />
                       <asp:Label ID="Label6" runat="server" Font-Bold="True" Font-Size="14pt" 
                           ForeColor="#339933"></asp:Label>
                   </td>
               </tr>
               <tr>
                   <td class="style15" valign="middle">
                       Beneficiary 
                       Name</td>
                   <td class="style16" valign="middle">
                       <asp:TextBox  
                           AutoComplete="off" ID="TextBox5" runat="server" BorderStyle="Solid" BorderWidth="1px" 
                        Height="26px" Width="302px"  Font-Size="12pt" Font-Names="Segoe UI Light" 
                            ReadOnly="True"></asp:TextBox>
                       <asp:RequiredFieldValidator ID="RequiredFieldValidator8" runat="server" 
                           ControlToValidate="TextBox5" ErrorMessage="must have value" 
                           EnableClientScript="False" Display="Dynamic"></asp:RequiredFieldValidator>
                       </td>
               </tr>
               <tr>
                   <td class="style17" valign="middle">
                       Customer Instruction</td>
                   <td class="style18" valign="middle">
                       <asp:FileUpload ID="FileUpload1" runat="server" />
                   &nbsp;
                       <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" 
                           ControlToValidate="FileUpload1" ErrorMessage="Please select Instruction" 
                           EnableClientScript="False"></asp:RequiredFieldValidator>
                   </td>
               </tr>
               <tr>
                   <td class="style5">
                       &nbsp;</td>
                   <td class="style19">
                       Max size 
                       1MB,&nbsp; must be a scanned .JPG,.PNG file or PDF</td>
               </tr>
               <tr>
                   <td class="style5">
                       <asp:Label ID="Label24" runat="server" 
                           Text="Enter Your &lt;b&gt;T24 sign on name&lt;/b&gt;:"></asp:Label>
                   </td>
                   <td class="style19">
                       <asp:TextBox ID="txtUsername103" runat="server" BorderStyle="Solid" 
                           BorderWidth="1px" Height="26px" Width="302px"></asp:TextBox>
                   </td>
               </tr>
               <tr>
                   <td class="style5">
                       <asp:Label ID="Label25" runat="server" Text="Please Enter Your Token Password:"></asp:Label>
                   </td>
                   <td class="style19">
                       <asp:TextBox ID="txtToken103" runat="server" BorderStyle="Solid" 
                           BorderWidth="1px" Height="26px" Width="302px"></asp:TextBox>
                   </td>
               </tr>
               <tr>
                   <td class="style13">
                       </td>
                   <td class="style14">
                       <asp:Button ID="Button5" runat="server" Height="35px" 
                           onclientclick="this.value=&quot;Posting...&quot;; this.disabled=true;" 
                           Text="Post Transaction" UseSubmitBehavior="False" Width="161px" />
                       <asp:Label ID="lblSubmissionText" runat="server" Font-Bold="True" 
                           ForeColor="Red"></asp:Label>
                   </td>
               </tr>
           </table>
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
