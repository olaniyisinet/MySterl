<%@ Page Language="VB" AutoEventWireup="false" CodeFile="settings.aspx.vb" Inherits="swift" %>

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
		width:968px;
            top: 0px;
            left: 0px;
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
        .style3
        {
            font-size: medium;
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
        }
	
        .style22
        {
            width: 101px;
        }
	
        .style23
        {
            width: 507px;
        }
	
        .style24
        {
            width: 100%;
        }
        .style26
        {
            width: 148px;
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
           <strong class="style3">
           <asp:Label ID="Label2" runat="server"></asp:Label>
&nbsp;</strong><br />
           <br />
           Welcome,
           <asp:Label ID="Label1" runat="server"></asp:Label>
           &nbsp;&nbsp; |&nbsp; 
           <a href="admin.aspx">Home</a>&nbsp;&nbsp; |&nbsp;&nbsp;&nbsp; 
           <a href="logout.aspx">Sign Out</a><br />
           <span class="style6">
           <br />
           </span>
           <br />
           <asp:Panel ID="Panel1" runat="server">
           <fieldset><legend>Bank Details</legend>
           <span class="style20"><span class="style6">
               <table>
                   <tr>
                       <td >
                           <%--<span class="style20">
           <span class="style6">--%>
                           <asp:Label ID="Label3" runat="server" Text="Bank name:"></asp:Label>
                           <%--      
           </span>
           </span>--%>
                       </td>
                       <td class="style23">
                           <asp:TextBox ID="txtBankName" runat="server" Width="284px"></asp:TextBox>
                       </td>
                   </tr>
                   <tr>
                       <td>
                           <asp:Label ID="Label4" runat="server" Text="Bank Code:"></asp:Label>
                       </td>
                       <td class="style23">
                           <asp:TextBox ID="txtBankCode" runat="server" Width="282px"></asp:TextBox>
                       </td>
                   </tr>
                   <tr>
                       <td >
                           <asp:Label ID="Label5" runat="server" Text="BIC Code:"></asp:Label>
                       </td>
                       <td class="style23">
                           <asp:TextBox ID="txtBic" runat="server" Width="279px"></asp:TextBox>
                       </td>
                   </tr>
                   <tr>
                       <td >
                           <asp:Label ID="Label6" runat="server" Text="NUBAN:"></asp:Label>
                       </td>
                       <td >
                           <asp:TextBox ID="txtNuban" runat="server" Width="283px"></asp:TextBox>
                       </td>
                   </tr>
                   <tr>
                       <td class="style22">
                           &nbsp;</td>
                       <td class="style23">
                           <asp:Button ID="btnSaveBank" runat="server" Text="Save Banks" OnClientClick="javascript:return confirm('Are you sure?');" />
                           <asp:Label ID="lblSuccess" runat="server" Font-Bold="True" ForeColor="Green"></asp:Label>
                           <asp:Label ID="lblFail" runat="server" Font-Bold="True" ForeColor="Red"></asp:Label>
                       </td>
                   </tr>
               </table>


               <asp:GridView ID="gvBank" runat="server" AllowPaging="True" 
               AutoGenerateColumns="False" CellPadding="4" DataSourceID="odsBankDetail" 
               ForeColor="#333333" GridLines="None" BorderStyle="Solid" DataKeyNames="bank_code" >
                   <AlternatingRowStyle BackColor="White" />
                   <Columns>
                       <asp:TemplateField ShowHeader="False">
                           <ItemTemplate>
                               <asp:LinkButton ID="LinkButton1" runat="server" CausesValidation="False" 
                                   CommandName="Delete" Text="Delete" OnClientClick="return confirm('Are you sure you want to delete this bank?\nThis is permanent deletion!');"></asp:LinkButton>
                           </ItemTemplate>
                       </asp:TemplateField>
                       <asp:BoundField DataField="Bank_code" HeaderText="Bank_code" 
                           SortExpression="Bank_code" />
                       <asp:BoundField DataField="Bank_Name" HeaderText="Bank_Name" 
                           SortExpression="Bank_Name" />
                       <asp:BoundField DataField="Status_Flag" HeaderText="Status_Flag" 
                           SortExpression="Status_Flag" />
                       <asp:BoundField DataField="Bic" HeaderText="Bic" SortExpression="Bic" />
                       <asp:BoundField DataField="Nuban" HeaderText="Nuban" SortExpression="Nuban" />
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
               </span>
               <asp:ObjectDataSource ID="odsBankDetail" runat="server" 
                   DeleteMethod="DeleteByBankCode" SelectMethod="GetAllBankDetails" 
                   TypeName="BankDetail">
                   <DeleteParameters>
                       <asp:Parameter Name="bc" Type="String" />
                   </DeleteParameters>
               </asp:ObjectDataSource>
               </span>
           </fieldset>
               
           </asp:Panel>
           <asp:Panel ID="pnlAdminSettings" runat="server">
           <fieldset><legend>Admin Settings</legend>
               <br />
               <table class="style24">
                   <tr>
                       <td class="style26">
                           <asp:Label ID="Label8" runat="server" Text="RTGS Account"></asp:Label>
                       </td>
                       <td>
                           <asp:TextBox ID="txtRtgsAccount" runat="server"></asp:TextBox>
                       </td>
                   </tr>
                   <tr>
                       <td class="style26">
                           <asp:Label ID="Label9" runat="server" Text="PL Account:"></asp:Label>
                       </td>
                       <td>
                           <asp:TextBox ID="txtPLAccount" runat="server"></asp:TextBox>
                       </td>
                   </tr>
                   <tr>
                       <td class="style26">
                           <asp:Label ID="Label10" runat="server" Text="Customer Charge:"></asp:Label>
                       </td>
                       <td>
                           <asp:TextBox ID="txtCustomerAmount" runat="server"></asp:TextBox>
                       </td>
                   </tr>
                   <tr>
                       <td class="style26">
                           <asp:Label ID="Label11" runat="server" Text="Staff Charge"></asp:Label>
                       </td>
                       <td>
                           <asp:TextBox ID="txtChargeStaffAmount" runat="server" Height="22px"></asp:TextBox>
                       </td>
                   </tr>
                   <tr>
                       <td class="style26">
                           <asp:Label ID="Label12" runat="server" Text="RTGS Email:"></asp:Label>
                       </td>
                       <td>
                           <asp:TextBox ID="txtRtgsEmail" runat="server"></asp:TextBox>
                       </td>
                   </tr>
                   <tr>
                       <td class="style26">
                           <asp:Label ID="Label13" runat="server" Text="Treasury Email:"></asp:Label>
                       </td>
                       <td>
                           <asp:TextBox ID="txtTreasuryEmail" runat="server"></asp:TextBox>
                       </td>
                   </tr>
                   <tr>
                       <td class="style26">
                           <asp:Label ID="Label14" runat="server" Text="RTGS Suspense Account:"></asp:Label>
                       </td>
                       <td>
                           <asp:TextBox ID="txtRtgSuspenseAccount" runat="server"></asp:TextBox>
                       </td>
                   </tr>
                   <tr>
                       <td class="style26">
                           <asp:Label ID="Label15" runat="server" Text="VAT Account"></asp:Label>
                       </td>
                       <td>
                           <asp:TextBox ID="txtVATAccount" runat="server"></asp:TextBox>
                       </td>
                   </tr>
                   <tr>
                       <td class="style26">
                           <asp:Label ID="Label16" runat="server" Text="Is Active?"></asp:Label>
                       </td>
                       <td>
                           <asp:RadioButtonList ID="rblActiveSet" runat="server" 
                               RepeatDirection="Horizontal">
                               <asp:ListItem Selected="True" Value="0">No</asp:ListItem>
                               <asp:ListItem Value="1">Yes</asp:ListItem>
                           </asp:RadioButtonList>
                       </td>
                   </tr>
                   <tr>
                       <td class="style26">
                           &nbsp;</td>
                       <td>
                           <asp:Button ID="btnSaveAdmin" runat="server" Text="Save/Update" OnClientClick="return confirm('Are you sure?');" />
                           <asp:Label ID="lblAdminSuccess" runat="server" ForeColor="Green"></asp:Label>
                           <asp:Label ID="lblAdminFail" runat="server" ForeColor="Red"></asp:Label>
                       </td>
                   </tr>
                   <tr>
                       <td class="style26">
                           &nbsp;</td>
                       <td>
                           &nbsp;</td>
                   </tr>
               </table>
            <asp:GridView ID="gvAdminSettings" runat="server" BorderStyle="Solid" CellPadding="4" 
                   ForeColor="#333333" GridLines="None" AllowPaging="True" 
                   AutoGenerateColumns="False" DataSourceID="odsAdmin" DataKeyNames="id">
                   <AlternatingRowStyle BackColor="White" />
                   <Columns>
                       <asp:TemplateField ShowHeader="False">
                           <ItemTemplate>
                               <asp:LinkButton ID="LinkButton1" runat="server" CausesValidation="False" 
                                   CommandName="Delete" Text="Delete" OnClientClick="return confirm('Are you sure you want to delete this setting?\nThis is a permanent deletion.');"></asp:LinkButton>
                           </ItemTemplate>
                       </asp:TemplateField>
                       <asp:BoundField DataField="ID" HeaderText="ID" SortExpression="ID" />
                       <asp:BoundField DataField="RTGSAccount" HeaderText="RTGSAccount" 
                           SortExpression="RTGSAccount" />
                       <asp:BoundField DataField="PLAccount" HeaderText="PLAccount" 
                           SortExpression="PLAccount" />
                       <asp:BoundField DataField="ChargesCustomerAmount" 
                           HeaderText="ChargesCustomerAmount" SortExpression="ChargesCustomerAmount" />
                       <asp:BoundField DataField="ChargesStaffAmount" HeaderText="ChargesStaffAmount" 
                           SortExpression="ChargesStaffAmount" />
                       <asp:BoundField DataField="RTGSEmail" HeaderText="RTGSEmail" 
                           SortExpression="RTGSEmail" />
                       <asp:BoundField DataField="TreasuryEmail" HeaderText="TreasuryEmail" 
                           SortExpression="TreasuryEmail" />
                       <asp:BoundField DataField="RTGSSuspense" HeaderText="RTGSSuspense" 
                           SortExpression="RTGSSuspense" />
                       <asp:BoundField DataField="VATAccount" HeaderText="VATAccount" 
                           SortExpression="VATAccount" />
                       <asp:BoundField DataField="VATAmount" HeaderText="VATAmount" 
                           SortExpression="VATAmount" />
                       <asp:CheckBoxField DataField="IsActive" HeaderText="IsActive" 
                           SortExpression="IsActive" />
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
               <asp:ObjectDataSource ID="odsAdmin" runat="server" DeleteMethod="DeleteByID" 
                   SelectMethod="GetAllAdminSetting" TypeName="AdminSetting">
                   <DeleteParameters>
                       <asp:Parameter Name="adminID" Type="Int32" />
                   </DeleteParameters>
               </asp:ObjectDataSource>
           </fieldset>
              
           </asp:Panel>
           <span class="style20">
           <span class="style6">
           <br />
           <asp:Panel ID="pnlFailedSwiftSettings" runat="server">
               <asp:Label ID="Label7" runat="server" Text="Send Failed Notification To Email:"></asp:Label>
               <asp:TextBox ID="txtEmailAddress" runat="server"></asp:TextBox>
               <asp:Button ID="btnSendFailedMail" runat="server" Text="Save Email Address" OnClientClick="return confirm('Are you sure?');" />
               <br />
               <asp:GridView ID="gvFailedSwifts" runat="server" AllowPaging="True" 
                   CellPadding="4" DataSourceID="odsFailedSwiftFiles" ForeColor="#333333" 
                   GridLines="None" AutoGenerateColumns="False">
                   <AlternatingRowStyle BackColor="White" />
                   <Columns>
                       <asp:TemplateField HeaderText="SendersReference" 
                           SortExpression="SendersReference">
                           <EditItemTemplate>
                               <asp:Label ID="Label1" runat="server" Text='<%# Eval("SendersReference") %>'></asp:Label>
                           </EditItemTemplate>
                           <ItemTemplate>
                               <asp:LinkButton ID="LinkButton2" runat="server" Text='<%# Bind("SendersReference") %>' OnClientClick="return confirm('Do you want to download the MT message for manual processing?');" OnClick="downloadFile"/>  
                           </ItemTemplate>
                       </asp:TemplateField>
                       <asp:BoundField DataField="OrigtBranch" HeaderText="OrigtBranch" 
                           SortExpression="OrigtBranch" />
                       <asp:BoundField DataField="SendersAccount" HeaderText="SendersAccount" 
                           SortExpression="SendersAccount" />
                       <asp:BoundField DataField="SwiftMessage" HeaderText="SwiftMessage" 
                           SortExpression="SwiftMessage" />
                       <asp:BoundField DataField="SwiftStatus" HeaderText="SwiftStatus" 
                           ReadOnly="True" SortExpression="SwiftStatus" />
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
               <asp:ObjectDataSource ID="odsFailedSwiftFiles" runat="server" 
                   SelectMethod="GetFailedSendersReferences" TypeName="FailedSwift">
               </asp:ObjectDataSource>
               <br />
           </asp:Panel>
           <br />
&nbsp;<br />
           <br />
           <br />
           <br />
               
               
           </span>
           </span>
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
    
             Sterling Bank Plcc</div>
    <br />
    <br />
    <br />
    </form>
</body>
</html>
