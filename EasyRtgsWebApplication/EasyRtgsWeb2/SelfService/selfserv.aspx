<%@ Page Language="VB" AutoEventWireup="false" CodeFile="selfserv.aspx.vb" Inherits="selfserv" %>

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
	
        </style> 
</head>
<body>
    <form id="form1" runat="server">
    <asp:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server">
           </asp:ToolkitScriptManager>
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
           <a href="../mytransactions.aspx">Home</a>&nbsp;&nbsp; |&nbsp;&nbsp;&nbsp; 
           <a href="../logout.aspx">Sign Out</a><br />
           <span class="style6">
           <br />
           </span>
           <br />
           <asp:Panel ID="Panel5" runat="server">
                    <table class="style7">
                        <tr>
                            <td class="style14">
                                &nbsp;</td>
                            <td>
                                <asp:Label ID="Label3" runat="server" Font-Bold="True" Font-Size="Larger" 
                                    Font-Underline="True" Text="Review Transactions That Have Not Been Posted."></asp:Label>
                                <br />
                                <asp:Label ID="Label4" runat="server" Font-Size="Large" Font-Strikeout="False" 
                                    Text="&lt;u&gt;Instructions:&lt;/u&gt;Click in the box below to select a date. Click on the &quot;Get Unposted Entry&quot; button to show the entries for that date."></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td class="style14">
                                <asp:Literal ID="Literal1" runat="server" Text="Select Date:"></asp:Literal>
                            </td>
                            <td>
                                <asp:TextBox ID="txtDateUnpostedEntries" runat="server" Width="200px"></asp:TextBox>
                                <asp:CalendarExtender ID="txtDateUnpostedEntries_CalendarExtender" 
                                    runat="server" Enabled="True" Format="dd-MMM-yyyy" 
                                    TargetControlID="txtDateUnpostedEntries">
                                </asp:CalendarExtender>
                            </td>
                        </tr>
                        <tr>
                            <td class="style14">
                                &nbsp;</td>
                            <td>
                                <asp:Button ID="btnGetEntryForDate" runat="server" Text="Get Unposted Entry" />
                            </td>
                        </tr>
                    </table>
                    <asp:Label ID="Label5" runat="server" 
                        Text="Currently Showing Unposted Entries For Date"></asp:Label>
                    <asp:Label ID="lblCurrentDate" runat="server"></asp:Label>
                    <br />
                    <br />
                    <asp:Label ID="lblResultStatus" runat="server" Font-Bold="False" 
                        Font-Names="Tahoma" Font-Size="Larger"></asp:Label>
                    <br />
                    <div style="overflow: scroll; width: 1000px;">
                        <asp:GridView ID="gvUnpostedTransactions" runat="server" 
                            AutoGenerateColumns="False" CellPadding="4" DataKeyNames="ID" 
                            DataSourceID="sqlDsUnpostedTransactions" Font-Size="X-Small" 
                            ForeColor="#333333" 
                            
                            EmptyDataText="&lt;b&gt;There are no unposted entries for the selected date.&lt;/b&gt;" 
                            GridLines="None">
                            <AlternatingRowStyle BackColor="White" />
                            <Columns>
                                <asp:TemplateField ShowHeader="False">
                                    <ItemTemplate>
                                        &nbsp; &nbsp;
                                        <asp:Button ID="btnRepost" runat="server" 
                                            CommandArgument="<%# Container.DataItemIndex %>" CommandName="Post Entry" 
                                            OnClientClick="return confirm('Have you checked on T24 that it is not already on T24?\nIf you found this entry on T24, click Cancel here and then click on the button: \'Already Posted\'.\nAre you sure you want to repost?');" 
                                            Text="Post Entry" ToolTip="Click if this entry was NOT found on T24" 
                                            Width="120px" />
                                        &nbsp; &nbsp;
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField ShowHeader="False">
                                    <ItemTemplate>
                                        &nbsp; &nbsp;
                                        <asp:Button ID="btnAlreadyPosted" runat="server" 
                                            CommandArgument="<%# Container.DataItemIndex %>" CommandName="Already Posted" 
                                            OnClientClick="return confirm('Have you checked on T24 that it is not already on T24?\nIf you DID NOT find this entry on T24, click Cancel here and then click on the button: \'Post Entry\'.');" 
                                            Text="Already Posted" ToolTip="Click if this entry was found on T24" 
                                            Width="120px" />
                                        &nbsp; &nbsp;
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:BoundField DataField="SN" HeaderText="SN" SortExpression="SN" />
                                <asp:BoundField DataField="Status" HeaderText="Status" InsertVisible="False" 
                                    ReadOnly="True" SortExpression="Status" />
                                <asp:BoundField DataField="ID" HeaderText="ID" InsertVisible="False" 
                                    ReadOnly="True" SortExpression="ID" Visible="False" />                                
                                <asp:BoundField DataField="Entry Date" HeaderText="Entry Date" 
                                    SortExpression="Entry Date" />
                                <asp:BoundField DataField="Reason" HeaderText="Reason" 
                                    SortExpression="Reason" />
                                <asp:BoundField DataField="Account Name" HeaderText="Account Name" 
                                    SortExpression="Account Name" />
                                <asp:BoundField DataField="Customer's Account" HeaderText="Customer's Account" 
                                    SortExpression="Customer's Account" />
                                <asp:BoundField DataField="Account To Credit" HeaderText="Account To Credit" 
                                    SortExpression="Account To Credit" />
                                <asp:BoundField DataField="Amount" HeaderText="Amount" 
                                    SortExpression="Amount" />
                                <asp:BoundField DataField="Explanation Code" HeaderText="Explanation Code" 
                                    SortExpression="Explanation Code" />
                                <asp:BoundField DataField="Remarks" HeaderText="Remarks" 
                                    SortExpression="Remarks" />
                                <asp:BoundField DataField="Senders Reference" HeaderText="Senders Reference" 
                                    SortExpression="Senders Reference" />
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
                    </div>
                    <br />
                    <asp:SqlDataSource ID="sqlDsUnpostedTransactions" runat="server" 
                        
                        ConnectionString="<%$ ConnectionStrings:easyRtgsConn %>" SelectCommand="select a.TransactionID SN,a.ID, a.Status, a.Status + '-' + a.ErrorText Reason,b.TransactionID [Senders Reference],a.EntryDate [Entry Date], 
a.CustomerName [Account Name],a.DrCustAcct [Customer's Account],a.CrCommAcct [Account To Credit], a.Amount, a.expl_code [Explanation Code], a.Remarks from TransactTemp2 a inner join transactions b on a.TransactionID=b.TransactionID where (a.Status like ('%fail')  or a.Status like ('%sent'))
and CONVERT(datetime,a.Entrydate)&gt;=CONVERT(datetime,@entrydate)
order by a.ID desc, EntryDate	desc  
">
                        <SelectParameters>
                            <asp:SessionParameter DefaultValue="0" Name="entrydate" 
                                SessionField="entrydate" />
                        </SelectParameters>
                    </asp:SqlDataSource>
                </asp:Panel>
           <br />
           <span class="style20">
           <span class="style6">
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
