<%@ Page Language="VB" AutoEventWireup="false" CodeFile="mt202_interbank.aspx.vb" Inherits="mt202_interbank" MaintainScrollPositionOnPostback="true"     %>
 
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
    
        <img alt="" class="style21" src="../../images/icon.png" />Sterling easyRTGS        <span class="style2">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Third Party Transfer Application</span></div>


       <div id="content">
       <div class="wrap">
           <strong class="style3">TROPS..::..Landing Page</strong><br />
           <br />
           Welcome,
           <asp:Label ID="lblUserFullName" runat="server"></asp:Label>
           <%-- &nbsp;&nbsp; |&nbsp;&nbsp; <a href="mytransactions.aspx">My Transactions</a>&nbsp;&nbsp; |&nbsp;&nbsp;&nbsp; 
           <a href="logout.aspx">Sign Out</a><br />
           <br />
           <span class="style20">Post Transaction</span><span class="style6"><br />--%>&nbsp; <a href="../trops_landing.aspx">Landing Page</a> || <a href="../../logout.aspx">Log Out</a><br />
           <asp:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server">
           </asp:ToolkitScriptManager>
           </span>&nbsp;<input id="hiddenUseAmtLimit" type="hidden" runat="server" />
           <br />
           <br />
           <br />
           &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
           <br />
           <asp:Panel ID="pnl202" runat="server">
           <fieldset>
           <legend><b>MT 202 Inter-Bank Transfers </b><asp:Label ID="lblMessageType" runat="server"></asp:Label>
               &nbsp;<asp:Label ID="lblMessageVariant" runat="server"></asp:Label>
               </legend>
<table class="style4" style="border: thin dotted #FF9933">
                   <tr>
                       <td class="style11" valign="middle">
                           &nbsp;</td>
                       <td valign="middle" class="style12">
                           <asp:Label ID="Label7" runat="server" Font-Bold="True" Font-Size="14pt" 
                           ForeColor="#339933"></asp:Label>
                       </td>
                   </tr>
                   <tr>
                       <td class="style11" valign="middle">
                           <asp:Label ID="lblOwnerBranch1" runat="server" Text="Source of Account:" 
                           ToolTip="Type of origin of account"></asp:Label>
                           <asp:Label ID="Label13" runat="server" Font-Bold="True" ForeColor="Red" Text="*"></asp:Label>
                       </td>
                       <td valign="middle" class="style12">
                           <asp:RadioButtonList ID="rblAccountSource" runat="server" AutoPostBack="True" ToolTip="Customer will disable the branch list. It is same as Bank Department. Branch will enable the branch list." Height="74px">
                               <asp:ListItem Selected="True">Customer</asp:ListItem>
                               <asp:ListItem Value="Department">Bank Department</asp:ListItem>
                               <asp:ListItem>Branch</asp:ListItem>
                           </asp:RadioButtonList>
                           <asp:RequiredFieldValidator ID="reqFieldEntryType0" runat="server" ControlToValidate="rblAccountSource" Display="Dynamic" EnableClientScript="False" ErrorMessage="must have value" ValidationGroup="valGroup202"></asp:RequiredFieldValidator>
                       </td>
                   </tr>
                   <tr>
                       <td class="style11" valign="middle">
                           <asp:Label ID="lblOwnerBranch2" runat="server" Text="Type of Entries:" ToolTip="Type of origin of account"></asp:Label>
                           <asp:Label ID="Label12" runat="server" Font-Bold="True" ForeColor="Red" Text="*"></asp:Label>
                       </td>
                       <td class="style12" valign="middle">
                           <asp:RadioButtonList ID="rblEntrySelection" runat="server">
                               <asp:ListItem Value="swift_only">Return of Funds(SWIFT Only)</asp:ListItem>
                               <asp:ListItem Value="swift_and_financial">Cash Swap(SWIFT and T24 entries)</asp:ListItem>
                           </asp:RadioButtonList>
                           <asp:RequiredFieldValidator ID="reqFieldEntryType" runat="server" ControlToValidate="rblEntrySelection" Display="Dynamic" EnableClientScript="False" ErrorMessage="must have value" ValidationGroup="valGroup202"></asp:RequiredFieldValidator>
                       </td>
                   </tr>
                   <tr>
                       <td class="style11" valign="middle">
                           <asp:Label ID="lblOwnerBranch0" runat="server" Text="Pick Owner Branch:" ToolTip="Branch on whose behalf you are doing the transaction"></asp:Label>
                       </td>
                       <td class="style12" valign="middle">
                           <asp:DropDownList ID="ddlOwnerBranch202" runat="server" DataSourceID="odsBranch" DataTextField="BRANAME" DataValueField="BRACODE" Enabled="False" Font-Names="Segoe UI Light" Height="26px" Width="301px">
                           </asp:DropDownList>
                           <asp:ObjectDataSource ID="odsBranch" runat="server" SelectMethod="GetBranch" TypeName="Gadget"></asp:ObjectDataSource>
                       </td>
                   </tr>
                   <tr>
                       <td class="style11" valign="middle">
                           <asp:Label ID="lblCustomerAcctLabel" runat="server" 
                               Text="Customer Account Number"></asp:Label>
                       </td>
                       <td valign="middle" class="style12">
                           <asp:TextBox  AutoComplete="off"  ID="txtCustAcct" runat="server" 
                               BorderStyle="Solid" BorderWidth="1px" 
                        Height="26px" Width="301px"  Font-Size="12pt" Font-Names="Segoe UI Light"></asp:TextBox>
                           &nbsp;<input id="btnGetCustData" 
                           style="color: #FFFFFF; font-family: 'segoe ui Light'; background-color: #008000" 
                           type="button" value="Get Customer Data" runat="server" 
                           onclick="this.value='Getting data...'; this.disabled=true;" 
                           onserverclick="show2" CausesValidation="False" visible="True" /><asp:RegularExpressionValidator 
                           ID="RegularExpressionValidator5" runat="server" 
                           ControlToValidate="txtCustAcct" Display="Dynamic" 
                           ErrorMessage="Enter a valid Account Number" 
                           ValidationExpression="^?[0-9]+(,[0-9]{3})*(\.[0-9]{2})?$" 
                           EnableClientScript="False" ValidationGroup="valGroup202" Enabled="False"></asp:RegularExpressionValidator>
                           <asp:RequiredFieldValidator 
                           ID="RequiredFieldValidator10" runat="server" 
                           ControlToValidate="txtCustAcct" Display="Dynamic" 
                           ErrorMessage="must have value" EnableClientScript="False" 
                               ValidationGroup="valGroup202" Enabled="False"></asp:RequiredFieldValidator>
                           <asp:RegularExpressionValidator ID="valRegExpNuban0" runat="server" 
                           ControlToValidate="txtCustAcct" ErrorMessage="Must be a NUBAN account number." 
                           ValidationExpression="^\d{10}$" ValidationGroup="valGroup202" Enabled="False"></asp:RegularExpressionValidator>
                       </td>
                   </tr>
                   <tr>
                       <td class="style11" valign="middle">
                           Customer Name</td>
                       <td valign="middle" class="style12">
                           <asp:TextBox  AutoComplete="off" ID="txtCustName202" runat="server" 
                               BorderStyle="Solid" BorderWidth="1px" 
                        Height="26px" Width="302px"  Font-Size="12pt" Font-Names="Segoe UI Light" 
                           ReadOnly="True"></asp:TextBox>
                           &nbsp;&nbsp;
                           <asp:RequiredFieldValidator ID="RequiredFieldValidator11" runat="server" 
                           ControlToValidate="txtCustName202" ErrorMessage="must have value" 
                           EnableClientScript="False" ValidationGroup="valGroup202" Enabled="False"></asp:RequiredFieldValidator>
                           &nbsp;</td>
                   </tr>
                   <tr>
                       <td class="style11" valign="middle">
                           Customer Account Balance</td>
                       <td valign="middle" class="style12">
                           <asp:Label ID="lblBalance202" runat="server" Font-Bold="True" Font-Size="14pt" 
                           ForeColor="#339933"></asp:Label>
                       </td>
                   </tr>
                   <tr>
                       <td class="style11" valign="middle">
                           Amount to Transfer</td>
                       <td class="style12" valign="middle">
                           <asp:TextBox  AutoComplete="off" ID="txtAmount202" runat="server" 
                               BorderStyle="Solid" BorderWidth="1px" 
                        Height="26px" Width="302px"  Font-Size="12pt" Font-Names="Segoe UI Light"></asp:TextBox>
                           &nbsp;&nbsp;
                           <asp:RequiredFieldValidator ID="RequiredFieldValidator12" runat="server" 
                           ControlToValidate="txtAmount202" Display="Dynamic" 
                           ErrorMessage="must have value" EnableClientScript="False" 
                               ValidationGroup="valGroup202"></asp:RequiredFieldValidator>
                           &nbsp;
                           <asp:RegularExpressionValidator ID="RegularExpressionValidator6" runat="server" 
                           ControlToValidate="txtAmount202" Display="Dynamic" 
                           ErrorMessage="Enter a valid amount" 
                           ValidationExpression="^?[0-9]+(,[0-9]{3})*(\.[0-9]{2})?$" 
                           EnableClientScript="False" ValidationGroup="valGroup202"></asp:RegularExpressionValidator>
                           &nbsp;<asp:CompareValidator ID="CompareValidator2" runat="server" 
                           ControlToValidate="txtAmount202" 
                           ErrorMessage="must be greater than or equal to 10million" 
                           Operator="GreaterThanEqual" Type="Double" ValueToCompare="10000000.00" 
                           Enabled="False" ValidationGroup="valGroup202"></asp:CompareValidator>
                       </td>
                   </tr>
                   <tr>
                       <td class="style11" valign="middle">
                           <%--Charges--%></td>
                       <td class="style12" valign="middle">
                           <%--<strong>Customers: </strong>
                           <asp:Label ID="Label9" runat="server"></asp:Label>
                           &nbsp;&nbsp; <strong>Staff:</strong>&nbsp;
                           <asp:Label ID="Label10" runat="server"></asp:Label>
                           &nbsp;--%>
                           <asp:CheckBox ID="CheckBox2" runat="server" AutoPostBack="True" 
                           style="font-weight: 700" Text="Concession" Enabled="False" Visible="False" />
                           &nbsp;
                           <asp:TextBox  AutoComplete="off" ID="txtConcession202" runat="server" 
                               BorderStyle="Solid" BorderWidth="1px" 
                        Height="26px" Width="63px"  Font-Size="12pt" Font-Names="Segoe UI Light" 
                               Enabled="False" ReadOnly="True" Visible="False">0.00</asp:TextBox>
                       </td>
                   </tr>
                   <tr>
                       <td class="style15" valign="middle">
                           Remarks</td>
                       <td class="style16" valign="middle">
                           <%--<asp:UpdatePanel ID="updatePanel" runat="server">
                               <ContentTemplate>--%>
                                   <br />                                   
                                    <asp:TextBox ID="txtRemarks202" runat="server" AutoComplete="off" 
                                       BorderStyle="Solid" BorderWidth="1px" Font-Names="Segoe UI Light" 
                                       Font-Size="12pt" Height="60px" TextMode="MultiLine" Width="302px"></asp:TextBox>
                                   &nbsp;&nbsp;
                                   <asp:RequiredFieldValidator ID="RequiredFieldValidator13" runat="server" 
                                       ControlToValidate="txtRemarks202" Display="Dynamic" EnableClientScript="False" 
                                       ErrorMessage="must have value" ValidationGroup="valGroup202" 
                                       Enabled="False"></asp:RequiredFieldValidator>
                                   &nbsp;
                              <%-- </ContentTemplate>                               
                               <Triggers>
                                   <asp:AsyncPostBackTrigger ControlID="rblRem" EventName="SelectedIndexChanged" />
                               </Triggers>
                           </asp:UpdatePanel>--%>
                           <br />
                       </td>
                   </tr>
                   <tr>
                       <td class="style15" valign="middle">
                           Beneficiary Bank
                       </td>
                       <td class="style16" valign="middle">
                           <asp:DropDownList ID="ddlBen202" runat="server" Height="26px" 
                               DataSourceID="odsBeneficiaryBank" DataTextField="bank_name" 
                               DataValueField="bank_code" Font-Names="Segoe UI Light" AutoPostBack="True" Width="302px">
                           </asp:DropDownList>
                           <asp:RequiredFieldValidator ID="RequiredFieldValidator14" runat="server" 
                           ControlToValidate="ddlBen202" ErrorMessage="must have value" 
                           EnableClientScript="False" Display="Dynamic" ValidationGroup="valGroup202" 
                               Enabled="False"></asp:RequiredFieldValidator>
                           <asp:ObjectDataSource ID="odsBeneficiaryBank" runat="server" 
                               SelectMethod="getBanks" TypeName="easyrtgs"></asp:ObjectDataSource>
                       </td>
                   </tr>
                   <tr>
                       <td class="style15" valign="middle">
                           <%--Beneficiary Account--%></td>
                       <td class="style16" valign="middle">
                           <asp:Label ID="lblBeneficiaryAcct202" runat="server" BorderStyle="Solid" 
                               BorderWidth="1px" Font-Names="Segoe UI Light" Height="26px" Width="302px" Font-Size="Larger"></asp:Label>
                           <asp:Button ID="LinkButton2" runat="server" CausesValidation="False" Text="Do Name Enquiry." style="color: #FFFFFF; font-family: 'segoe ui Light'; background-color: #008000" Visible="False"></asp:Button>
                           <br />
                           <input id="Button8" 
                           style="color: #FFFFFF; font-family: 'segoe ui Light'; visibility:hidden; background-color: #008000" 
                           type="button" value="Get Beneficiary Name" runat="server" 
                           onclick="this.value='Getting data...'; this.disabled=true;" 
                           onserverclick="benefName" CausesValidation="False" />
                           <br />
                           <asp:Label ID="Label11" runat="server" Font-Bold="True" Font-Size="14pt" 
                           ForeColor="#339933"></asp:Label>
                       </td>
                   </tr>
                   <tr>
                       <td class="style15" valign="middle">
                           <%--Beneficiary 
                       Name--%></td>
                       <td class="style16" valign="middle">
                           <asp:Label  
                           AutoComplete="off" ID="lblBeneName202" runat="server" BorderStyle="Solid" BorderWidth="1px" 
                        Height="26px" Width="302px"  Font-Size="Larger" Font-Names="Segoe UI Light" 
                           MaxLength="200" ReadOnly="True" Enabled="False"></asp:Label>
                       </td>
                   </tr>
                   <tr>
                       <td class="style17" valign="middle">
                           <%--Customer Instruction--%></td>
                       <td class="style18" valign="middle">
                           <asp:FileUpload ID="fupdCustInstruction" runat="server" 
                               Font-Names="Segoe UI Light" />
                           &nbsp;
                           <asp:RequiredFieldValidator ID="RequiredFieldValidator17" runat="server" 
                           ControlToValidate="fupdCustInstruction" ErrorMessage="Please select Instruction" 
                           EnableClientScript="False" ValidationGroup="valGroup202" Enabled="False"></asp:RequiredFieldValidator>
                       </td>
                   </tr>
                  <%-- <tr>
                       <td class="style5">
                           <asp:Label ID="Label22" runat="server" Text="Please Enter Your Teller ID:"></asp:Label>
                       </td>
                       <td class="style19">
                      
                           <asp:TextBox ID="txtUsername202" runat="server" BorderStyle="Solid" 
                               BorderWidth="1px" Height="26px" Width="302px"></asp:TextBox>
                       </td>
                   </tr>
                   <tr>
                       <td class="style5">
                           <asp:Label ID="Label23" runat="server" Text="Please Enter Your Token Password:"></asp:Label>
                       </td>
                       <td class="style19">
                           <asp:TextBox ID="txtToken202" runat="server" BorderStyle="Solid" 
                               BorderWidth="1px" Height="26px" Width="302px"></asp:TextBox>
                       </td>
                   </tr>--%>
                   <tr>
                   <td class="style5">
                       <asp:Label ID="Label3" runat="server" 
                           Text="Enter Your &lt;b&gt;T24 sign on name&lt;/b&gt;:"></asp:Label>
                   </td>
                   <td class="style19">
                       <asp:TextBox ID="txtUsername202" runat="server" BorderStyle="Solid" 
                           BorderWidth="1px" Height="26px" Width="302px"></asp:TextBox>
                   </td>
               </tr>
               <tr>
                   <td class="style5">
                       <asp:Label ID="Label8" runat="server" Text="Please Enter Your Token Password:"></asp:Label>
                   </td>
                   <td class="style19">
                       <asp:TextBox ID="txtToken202" runat="server" BorderStyle="Solid" 
                           BorderWidth="1px" Height="26px" Width="302px"></asp:TextBox>
                   </td>
               </tr>
                   <tr>
                       <td class="style13">
                       </td>
                       <td class="style14">
                           <asp:Button ID="btnSubmit202" runat="server" Height="35px" 
                           onclientclick="this.value=&quot;Posting...&quot;; this.disabled=true;" 
                           Text="Post Transaction" UseSubmitBehavior="False" Width="161px" 
                               ValidationGroup="valGroup202" />
                           <asp:Label ID="lblSubmissionText202" runat="server" Font-Bold="True" 
                               ForeColor="Red"></asp:Label>
                       </td>
                   </tr>
               </table>
           </fieldset>
               
           </asp:Panel>
           <br />
           <br />
           <br />
           <br />
           <br />
           <br /></div>
       </div>
         <div id="footer">
    
             Sterling Bank Plciv>
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
