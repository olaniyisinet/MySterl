<%@ Page Language="VB" AutoEventWireup="false" CodeFile="custcare.aspx.vb" Inherits="main" Title="EasyRTGS :: Customer Care"    %>
 
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
	
        .style22
        {
            width: 100%;
        }
	
        .style23
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
           <strong class="style3">Customer Care Page</strong><br />
           <br />
           Welcome,
           <asp:Label ID="Label1" runat="server"></asp:Label>
           &nbsp;&nbsp; |&nbsp;&nbsp; <a href="mytransactions.aspx">My Transactions</a>&nbsp;&nbsp; |&nbsp;&nbsp;&nbsp; 
           <a href="logout.aspx">Sign Out</a><br />
           <br />
           <br />
           <table class="style22">
               <tr>
                   <td align="center">
                       <asp:ValidationSummary ID="ValidationSummary1" runat="server" />
                   </td>
               </tr>
               <tr>
                   <td align="center">
                       <asp:GridView ID="gvPendingCustomerCare" runat="server" 
                           AutoGenerateColumns="False" CellPadding="4" DataKeyNames="TransactionID" 
                           DataSourceID="sqlDsPendingCustomerCare" ForeColor="#333333" 
                           EmptyDataText="&lt;b&gt;There are no pending transactions.&lt;/b&gt;">
                           <AlternatingRowStyle BackColor="White" />
                           <Columns>
                               <asp:CommandField ShowSelectButton="True" />
                               <asp:BoundField DataField="TransactionID" HeaderText="Transaction ID" 
                                   SortExpression="TransactionID" />
                               <asp:BoundField DataField="Customer_name" HeaderText="Customer Name" 
                                   SortExpression="Customer_name" />
                               <asp:BoundField DataField="Customer_account" HeaderText="Customer Account" 
                                   SortExpression="Customer_account" />
                               <asp:BoundField DataField="amount" HeaderText="Amount" 
                                   SortExpression="amount" DataFormatString="{0:d}" />
                               <asp:BoundField DataField="charges" HeaderText="Charges" 
                                   SortExpression="charges" DataFormatString="{0:d}" />
                               <asp:BoundField DataField="Remarks" HeaderText="Remarks" 
                                   SortExpression="Remarks" />
                               <asp:BoundField DataField="Uploaded_by" HeaderText="Uploaded By" 
                                   SortExpression="Uploaded_by" />
                               <asp:BoundField DataField="Status" HeaderText="Status" 
                                   SortExpression="Status" />
                           </Columns>
                           <FooterStyle BackColor="#990000" Font-Bold="True" ForeColor="White" />
                           <HeaderStyle BackColor="#990000" Font-Bold="True" ForeColor="White" />
                           <PagerStyle BackColor="#FFCC66" ForeColor="#333333" HorizontalAlign="Center" />
                           <RowStyle BackColor="#FFFBD6" ForeColor="#333333" />
                           <SelectedRowStyle BackColor="#FFCC66" Font-Bold="True" ForeColor="Navy" />
                           <SortedAscendingCellStyle BackColor="#FDF5AC" />
                           <SortedAscendingHeaderStyle BackColor="#4D0000" />
                           <SortedDescendingCellStyle BackColor="#FCF6C0" />
                           <SortedDescendingHeaderStyle BackColor="#820000" />
                       </asp:GridView>
                       <asp:SqlDataSource ID="sqlDsPendingCustomerCare" runat="server" 
                           ConnectionString="<%$ ConnectionStrings:easyRtgsConn %>" 
                           SelectCommand="SELECT [TransactionID], [Customer_name], [Customer_account], [amount], [charges], [Remarks], [Uploaded_by], [Status] FROM [transactions] WHERE ([Status] = @Status)">
                           <SelectParameters>
                               <asp:SessionParameter DefaultValue="CustomerCare" Name="Status" 
                                   SessionField="role" Type="String" />
                           </SelectParameters>
                       </asp:SqlDataSource>
                   </td>
               </tr>
               <tr>
                   <td>
                       &nbsp;</td>
               </tr>
           </table>
           <asp:Panel ID="pnlRequestDetails" runat="server" Visible="False">
               <asp:DetailsView ID="dtVwTransaction" runat="server" 
    CellPadding="4" ForeColor="#333333" Height="60px" 
    Width="891px" AutoGenerateRows="False" DataSourceID="sqlDsDetails">
                   <AlternatingRowStyle BackColor="White" />
                   <CommandRowStyle BackColor="#FFFFC0" Font-Bold="True" />
                   <FieldHeaderStyle BackColor="#FFFF99" Font-Bold="True" />
                   <Fields>
                       <asp:BoundField DataField="TransactionID" HeaderText="Transaction ID" 
                           SortExpression="TransactionID" />
                       <asp:BoundField DataField="Customer_name" HeaderText="Customer name" 
                           SortExpression="Customer_name" />
                       <asp:BoundField DataField="Customer_account" HeaderText="Customer account" 
                           SortExpression="Customer_account" />
                       <asp:BoundField DataField="amount" HeaderText="Amount" 
                           SortExpression="amount" DataFormatString="{0:d}" />
                       <asp:BoundField DataField="charges" HeaderText="Charges" 
                           SortExpression="charges" DataFormatString="{0:d}" />
                       <asp:BoundField DataField="Remarks" HeaderText="Remarks" 
                           SortExpression="Remarks" />
                       <asp:BoundField DataField="Uploaded_by" HeaderText="Uploaded By" 
                           SortExpression="Uploaded_by" />
                       <asp:BoundField DataField="Customer_email" HeaderText="Customer Email" 
                           SortExpression="Customer_email" />
                   </Fields>
                   <FooterStyle BackColor="#990000" Font-Bold="True" ForeColor="White" />
                   <HeaderStyle BackColor="#990000" Font-Bold="True" ForeColor="White" />
                   <PagerStyle BackColor="#FFCC66" ForeColor="#333333" HorizontalAlign="Center" />
                   <RowStyle BackColor="#FFFBD6" ForeColor="#333333" />
               </asp:DetailsView>
               <table class="style22">
                   <tr>
                       <td align="right" class="style23" colspan="2">
                           &nbsp;</td>
                   </tr>
                   <tr>
                       <td align="right" class="style23" colspan="2">
                           &nbsp;</td>
                   </tr>
                   <tr>
                       <td align="right" class="style23">
                           <asp:Label ID="Label3" runat="server" Font-Bold="True" Text="Responder Name:"></asp:Label>
                       </td>
                       <td>
                           <asp:TextBox ID="txtResponderName" runat="server" Font-Names="Century Gothic" 
                               Width="489px"></asp:TextBox>
                           <asp:RequiredFieldValidator ID="valReqResponderName" runat="server" 
                               ErrorMessage="Please enter the Responder Name ." 
                               ControlToValidate="txtResponderName">*</asp:RequiredFieldValidator>
                       </td>
                   </tr>
                   <tr>
                       <td align="right" class="style23">
                           <asp:Label ID="Label4" runat="server" Font-Bold="True" 
                               Text="Customer Response:"></asp:Label>
                       </td>
                       <td>
                           <asp:TextBox ID="txtCustomerResponse" runat="server" Font-Names="Century Gothic" 
                               Height="66px" TextMode="MultiLine" Width="489px"></asp:TextBox>
                           <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" 
                               ErrorMessage="Please enter the Customer Response" 
                               ControlToValidate="txtCustomerResponse">*</asp:RequiredFieldValidator>
                       </td>
                   </tr>
                   <tr>
                       <td align="right" class="style23">
                           <asp:Label ID="Label2" runat="server" Font-Bold="True" Text="Comment:"></asp:Label>
                       </td>
                       <td>
                           <asp:TextBox ID="txtCustomerCareComment" runat="server" 
                               Font-Names="Century Gothic" Height="66px" TextMode="MultiLine" Width="489px"></asp:TextBox>
                       </td>
                   </tr>
                   <tr>
                       <td align="right" class="style23">
                           &nbsp;</td>
                       <td>
                           <asp:Button ID="btnApprove" runat="server" Text="Approve" />
                           &nbsp;
                           <asp:Button ID="btnReject" runat="server" Text="Reject" />
                           &nbsp;<asp:Label ID="lblMsg" runat="server"></asp:Label>
                       </td>
                   </tr>
               </table>
               <asp:SqlDataSource ID="sqlDsDetails" runat="server" 
                   ConnectionString="<%$ ConnectionStrings:easyRtgsConn %>" 
                   SelectCommand="SELECT [TransactionID], [Customer_name], [Customer_account], [amount], [charges], [Remarks], [Uploaded_by], [Customer_email] FROM [transactions] WHERE (([Status] = 'CustomerCare') AND ([TransactionID] = @TransactionID))">
                  <SelectParameters>
                        <asp:ControlParameter 
                          Name="TransactionID" 
                          ControlID="gvPendingCustomerCare" 
                          PropertyName="SelectedDataKey[0]" />
                      </SelectParameters>
               </asp:SqlDataSource>
           </asp:Panel>
           <br />
           <br />
           <br />
           <br />
           <br />
           <br /></div>
       </div>
         <div id="footer">
    
             Sterling Bank Plc Bank Plc
             <%-- var formattedMoney = num.formatMoney(2, ',', '.');
              //alert(formattedMoney);
              $(this).val(formattedMoney);
          });--%>
          </div>
  </script>
  </form>
</body>
</html>
