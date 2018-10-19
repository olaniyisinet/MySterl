<%@ Page Language="VB" AutoEventWireup="false" CodeFile="main.aspx.vb" Inherits="main" %>

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
    
     
	
        .style6
        {
        }
            
     
	
        .style8
        {
            height: 21px;
        }
        .style10
        {
            font-size: x-large;
            padding: 1px;
        }
        .style11
        {
            text-align: right;
        }
    
        #test1{

        }

        #test2{

        }
	
        .style12
        {
            font-size: large;
            padding: 1px;
        }
        .style13
        {
            width: 63px;
            height: 64px;
        }
    
     
	
        .style15
        {
            height: 79px;
        }
            
     
	
        .style17
        {
            width: 187px;
        }
    
     
	
        .style18
        {
            background-color: #FFFFCC;
        }
        .style19
        {
            height: 21px;
            background-color: #FFFFCC;
        }
    
     
	
        .style20
        {
            width: 139px;
        }
    
     
	
    </style>
     <script language="javascript">

         function show() {
             document.getElementById("someDiv").style.visibility = "visible";

         }

         function hide() {
             document.getElementById("someDiv").style.visibility = "hidden";
         }
        </script>
</head>
<body onload="hide();">
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
    
          <asp:Label ID="Label2" runat="server"></asp:Label>
    
          <br />
          <br />
          <br />
          <br />
          <table  align="center" style="width: 852px; height: 171px;" bgcolor="#FFCC99" 
              class="boxshadow1" border="0">
              <tr>
                  <td class="style20">
                      &nbsp;</td>
                  <td class="style11" valign="middle" colspan="2">
                      <span class="style10">Upload Schedule<br />
                      <br />
                      </span></td>
              </tr>
              <tr>
                  <td class="style6" colspan="3">
                      <table align="center" class="roundImage" style="width: 579px; border-color:#FFFFCC;" 
                          bgcolor="#FFFFCC">
                          <tr>
                  <td class="style19">
                      <asp:Label ID="Label1" runat="server" Font-Bold="True" ForeColor="Red"></asp:Label>
                  </td>
                          </tr>
                          <tr>
                  <td class="style18">
    
          <asp:Label ID="Label7" runat="server"></asp:Label>
    
                  </td>
                          </tr>
                          <tr>
                  <td class="style18">
                      <asp:DropDownList ID="DropDownList3" runat="server" Height="26px" Width="317px" 
                          CssClass="roundImage">
                      </asp:DropDownList>
                  </td>
                          </tr>
                          <tr>
                  <td class="style18">
                      Select Type<br />
                      <asp:DropDownList ID="DropDownList1" runat="server" Height="26px" Width="316px" 
                          CssClass="roundImage">
                          <asp:ListItem>NUBAN</asp:ListItem>
                          <%--<asp:ListItem>IMAL</asp:ListItem>--%>
                      </asp:DropDownList>
                  </td>
                          </tr>
                          <tr>
                  <td class="style18">
                      Select Explanantion Type</td>
                          </tr>
                          <tr>
                  <td class="style18">
                      <asp:DropDownList ID="DropDownList2" runat="server" Height="26px" Width="317px" 
                          AutoPostBack="True" CssClass="roundImage">
                      </asp:DropDownList>
                  &nbsp;
                      <asp:Label ID="Label5" runat="server" Font-Bold="True" Font-Size="12pt" 
                          ForeColor="#CC0000"></asp:Label>
                  </td>
                          </tr>
                          <tr>
                  <td class="style18">
                      Title of Schedule</td>
                          </tr>
                          <tr>
                  <td class="style18">
                      <asp:TextBox ID="TextBox1" runat="server" Height="26px" Width="401px"></asp:TextBox>
&nbsp;
                  </td>
                          </tr>
                          <tr>
                  <td class="style18">
                      Select Excel File<br />
                      <asp:FileUpload ID="FileUpload1" runat="server" Width="411px" Height="26px" />
                  </td>
                          </tr>
                          <tr>
                  <td class="style18">
                      <asp:Button ID="Button1" runat="server" BackColor="#990000" CssClass="txt" 
                          Font-Names="Century Gothic" ForeColor="White" Height="32px" Text="Upload" 
                          Width="226px" 
                          onclientclick="show();this.value=&quot;Validating..Please wait&quot;; this.disabled=true;" 
                          UseSubmitBehavior="False" />
                  &nbsp;</td>
                          </tr>
                          <tr>
                  <td class="style18">
                      <br />
    <div id="someDiv">
        <img alt="" class="style10" 
            src="spinner.gif" /><br />
        <span class="style11">&nbsp; Validating... Please wait</span></div>
                              </td>
                          </tr>
                          <tr>
                  <td class="style18">
                      <asp:Label ID="Label6" runat="server" Font-Bold="False" Font-Size="10pt"></asp:Label>
                              </td>
                          </tr>
                          </table>
                  </td>
              </tr>
              <tr>
                  <td class="style20">
                      &nbsp;</td>
                  <td align="left" class="style17">
                      &nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;&nbsp;
                  </td>
                  <td align="right">
                      <asp:Button ID="Button4" runat="server" BackColor="#339933" CssClass="txt" 
                          Font-Names="Century Gothic" ForeColor="White" Height="30px" Text="Submit to HOP" 
                          Width="132px" 
                          onclientclick="this.value=&quot;Submitting...&quot;; this.disabled=true;" 
                          UseSubmitBehavior="False" />
                  &nbsp;
                      <asp:Button ID="Button2" runat="server" BackColor="#990000" CssClass="txt" 
                          Font-Names="Century Gothic" ForeColor="White" Height="30px" Text="Export to Excel" 
                          Width="132px" UseSubmitBehavior="False" />
                  </td>
              </tr>
              <tr>
                  <td class="style15" align="center" colspan="3">
                      <asp:Label ID="Label3" runat="server" Font-Bold="False" Font-Size="10pt" 
                          Height="24px" Width="67px"></asp:Label>
                      <br />
                      <br />
                      <br />
                      <br />
                      </td>
              </tr>
              <tr>
                  <td class="style20">
                      &nbsp;</td>
                  <td align="left" colspan="2">
                      <asp:Label ID="Label4" runat="server" Font-Bold="False" 
                          Font-Names="Segoe UI" Font-Size="15pt" ForeColor="#990000">Contents of Uploaded File</asp:Label>
                  </td>
              </tr>
              <tr>
                  <td class="style8" colspan="3">
                  
                      <asp:DataGrid ID="DataGrid1" runat="server" AutoGenerateColumns="False" 
                      CellPadding="4" ForeColor="#333333" Height="22px" Width="1068px" 
                      PageSize="100" Font-Size="8pt" style="color: #000000; font-size: small">
                      <AlternatingItemStyle BackColor="White" />
                      <Columns>
                       <asp:TemplateColumn  ItemStyle-ForeColor="Red" ItemStyle-Font-Bold="true" ItemStyle-CssClass="g"><ItemTemplate> <%# check(Container.DataItem("bra_code").ToString, Container.DataItem("cus_num").ToString, Container.DataItem("cur_code").ToString, Container.DataItem("led_code").ToString, Container.DataItem("sub_acct_code").ToString, Container.DataItem("tra_amt").ToString)%></ItemTemplate>

                    <ItemStyle CssClass="g" Font-Bold="True" ForeColor="Red"></ItemStyle>
                          </asp:TemplateColumn>
                          <asp:TemplateColumn  ItemStyle-ForeColor="Red" ItemStyle-Font-Bold="true" ItemStyle-CssClass="g"><ItemTemplate> <%# check2(Container.DataItem("bra_code").ToString, Container.DataItem("cus_num").ToString, Container.DataItem("cur_code").ToString, Container.DataItem("led_code").ToString, Container.DataItem("sub_acct_code").ToString, Container.DataItem("tra_amt").ToString)%></ItemTemplate>


                    <ItemStyle CssClass="g" Font-Bold="True" ForeColor="Red"></ItemStyle>
                          </asp:TemplateColumn>
                           <asp:TemplateColumn  ItemStyle-ForeColor="Red" ItemStyle-Font-Bold="true" ItemStyle-CssClass="g"><ItemTemplate> <%# check3(Container.DataItem("bra_code").ToString, Container.DataItem("cus_num").ToString, Container.DataItem("cur_code").ToString, Container.DataItem("led_code").ToString, Container.DataItem("sub_acct_code").ToString, Container.DataItem("tra_amt").ToString, Container.DataItem("deb_cre_ind").ToString)%></ItemTemplate>

                    <ItemStyle CssClass="g" Font-Bold="True" ForeColor="Red"></ItemStyle>

                          </asp:TemplateColumn>

                         <asp:TemplateColumn><HeaderTemplate>Name</HeaderTemplate><ItemTemplate> <%# getname(Container.DataItem("sub_acct_code").ToString, Container.DataItem("bra_code").ToString)%></ItemTemplate></asp:TemplateColumn>
 

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
                         
                          <asp:TemplateColumn><HeaderTemplate>Amount</HeaderTemplate><ItemTemplate> <%# FormatNumber(Container.DataItem("tra_amt").ToString, 2)%></ItemTemplate></asp:TemplateColumn>
                        
                          <asp:BoundColumn DataField="remarks" HeaderText="Remarks"></asp:BoundColumn>
                           <asp:BoundColumn DataField="docnum" HeaderText="DocNum"></asp:BoundColumn>
                       
                      </Columns>
                      <FooterStyle BackColor="#990000" Font-Bold="True" ForeColor="White" />
                      <HeaderStyle BackColor="#990000" Font-Bold="True" ForeColor="White" />
                      <ItemStyle BackColor="#FFFBD6" ForeColor="#333333" />
                      <PagerStyle BackColor="#FFCC66" ForeColor="#333333" HorizontalAlign="Center" 
                          Mode="NumericPages" />
                      <SelectedItemStyle BackColor="#FFCC66" Font-Bold="True" ForeColor="Navy" />
                  </asp:DataGrid>
                      </td>
              </tr>
          </table>
          <br />
          <br />
          <br />
          <br />
    
    </div>

      <div id="footer">
    
          Powered By Technology Group</div>
    </form>
</body>
</html>