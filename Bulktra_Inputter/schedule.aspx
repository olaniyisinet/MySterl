<%@ Page Language="VB" AutoEventWireup="false" CodeFile="schedule.aspx.vb" Inherits="schedule" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">


<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Bulktra Version 2:::Sterling Bank Plc</title>
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
	margin: 0;
	padding: 0;
	border: 0;
	outline: 0;
	font-weight: inherit;
	font-style: inherit;
	font-size: 100%;
	vertical-align: baseline;
}

@font-face {
	font-family: Segoe UI;
	src: url('segoeuil.ttf');
}

p{
	font-family: Segoe UI; /* no .ttf */
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
		font:13px Segoe UI
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
            height: 89px;
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
	
       
	
        .style2
        {
            width: 671px;
            border-collapse: collapse;
            height: 86px;
            color: #FFFFFF;
            font-size: large;
        }
        .style3
        {
            width: 57px;
        }
        .style4
        {
            font-size: xx-large;
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
    
     
	
        .style12
        {
            font-size: large;
            padding: 1px;
        }
    
     
	
        .style13
        {
            width: 49px;
            height: 49px;
        }
    
     
	
        .style14
        {
            font-size: xx-large;
            font-weight: bold;
            color: #008000;
            padding-top: 15px;
            padding-bottom: 15px;
            background-color: #FFFFFF;
        }
        .style16
        {
            font-size: medium;
        }
        .style18
        {
            font-size: x-large;
            padding-top: 15px;
            padding-bottom: 15px;
            background-color: #FFFFFF;
            font-weight: bold;
        }
    
     
	
        #Button4
        {
            height: 32px;
            width: 152px;
        }
            
     
	
        .style19
        {
            width: 1000px;
            border-collapse: collapse;
        }
            
     
	
    </style>
    <script language="javascript" type="text/javascript">
 
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div id="header">
    
        <table class="style2">
            <tr>
                <td class="style3" valign="middle">
                    <br />
                    <img alt="" class="style13" src="images/icon3.png" /><br />
                    <br />
                </td>
                <td class="style4" valign="middle">
                    <strong>Bulktra version 2.0</strong><br />
                    <span class="style12">Bulk payment system</span></td>
            </tr>
        </table>
    
    </div>

      <div id="content">
    
          <asp:Label ID="label2" runat="server"></asp:Label>
    
          <br />
          <br />
          <table align="center" class="style19">
              <tr>
                  <td>
          <span class="style18">Schedule</span><span class="style14">&nbsp; </span>
                      <span class="style16"><p>
                  &nbsp;<asp:Label ID="Label3" runat="server" Font-Bold="True" Font-Size="13pt" 
                      ForeColor="#006600"></asp:Label>
              </p>
              <p>
                          &nbsp;<asp:Button ID="Button1" runat="server" BackColor="#339933" 
                              Font-Names="Verdana" Font-Size="9pt" ForeColor="White" Height="31px" 
                              Text="Export to Excel" Width="128px" />
          </p>
&nbsp;<asp:DataGrid ID="DataGrid1" runat="server" AutoGenerateColumns="False" 
                      CellPadding="4" ForeColor="#333333" Height="22px" Width="1000px" 
              Font-Size="9pt">
                      <AlternatingItemStyle BackColor="White" />
                      <Columns>
                       <asp:TemplateColumn><HeaderTemplate>Name</HeaderTemplate><ItemTemplate><%# getname(Container.DataItem("sub_acct_code").ToString, Container.DataItem("bra_code").ToString)%></ItemTemplate></asp:TemplateColumn>
                         <asp:BoundColumn DataField="bra_code" HeaderText="Branch Code">
                          </asp:BoundColumn>
                          <asp:BoundColumn DataField="cus_num" HeaderText="Customer Number">
                          </asp:BoundColumn>
                          <asp:BoundColumn DataField="cur_code" HeaderText="Currency Code">
                          </asp:BoundColumn>
                          <asp:BoundColumn DataField="led_code" HeaderText="Ledger Code">
                          </asp:BoundColumn>
                          <asp:BoundColumn DataField="sub_acct_code" HeaderText="Nuban Account">
                          </asp:BoundColumn>
                          <asp:BoundColumn DataField="deb_cre_ind" HeaderText="Debit/Credit ">
                          </asp:BoundColumn>
                          <asp:BoundColumn DataField="tra_amt" HeaderText="Amount"></asp:BoundColumn>
                          <asp:BoundColumn DataField="remarks" HeaderText="Remarks"></asp:BoundColumn>
                          <asp:BoundColumn DataField="status" HeaderText="Status"></asp:BoundColumn>
                      </Columns>
                      <FooterStyle BackColor="#990000" Font-Bold="True" ForeColor="White" />
                      <HeaderStyle BackColor="#990000" Font-Bold="True" ForeColor="White" />
                      <ItemStyle BackColor="#FFFBD6" ForeColor="#333333" />
                      <PagerStyle BackColor="#FFCC66" ForeColor="#333333" HorizontalAlign="Center" />
                      <SelectedItemStyle BackColor="#FFCC66" Font-Bold="True" ForeColor="Navy" />
                  </asp:DataGrid>
          </span>
    
                  </td>
              </tr>
          </table>
          <br />
          <span class="style18">&nbsp;</span></div>

      <div id="footer">
    
          Powered By Technology Group</div>
    </form>
</body>
</html>