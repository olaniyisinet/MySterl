<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SimpleConversion.aspx.cs" Inherits="ImalWebUtilities.SimpleConversion" %>

<%@ Register TagPrefix="ajaxToolkit" Namespace="AjaxControlToolkit" Assembly="AjaxControlToolkit" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <!-- Latest compiled and minified CSS -->
    <link rel="stylesheet" href="css/bootstrap.min.css" />
    <!--[if lt IE 9]>
      <script src="js/html5shiv.min.js"></script>
      <script src="js/respond.min.js"></script>
    <![endif]-->

    <!-- Optional theme -->
    <link rel="stylesheet" href="css/bootstrap-theme.min.css" />

    <!-- Latest compiled and minified JavaScript -->

    <script src="js/jquery-1.11.0.min.js"></script>
    <script src="js/UserControl.js"></script>

    <script src="js/bootstrap.min.js"></script>
    <link href="css/glyphicons-extended.min.css" rel="stylesheet" type="text/css" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />

    <title>IMAL Core Banking Utilities</title>
</head>
<body>
    <form id="form1" runat="server">
        <%Response.WriteFile("UpperLinks.html"); %>

        <div class="row">
            <div class="col-md-2"></div>
            <div class="col-md-8">
                <div class="panel panel-success">
                    <div class="panel-heading">
                        <h3 class="panel-title">Simple Account Conversion</h3>
                    </div>
                    <div class="panel-body">
                        <div class="row">
                            <div class="col-md-6">
                                <div class="panel panel-default">
                                    <div class="panel-heading">
                                        <h3 class="panel-title">I have a NUBAN Account</h3>
                                    </div>
                                    <div class="panel-body">
                                        <ajaxToolkit:ToolkitScriptManager EnablePartialRendering="true" runat="Server" ID="ScriptManager1" />
                                        <div id="displayregular">
                                            <div class="form-group">
                                                <label for="nuban">NUBAN Account Number</label>
                                                <ajaxToolkit:FilteredTextBoxExtender ID="FilteredTextBoxExtender1" runat="server" FilterType="Numbers"
                                                    TargetControlID="nuban" />
                                                <asp:TextBox ID="nuban" runat="server" placeholder="Enter 10 Digit Nuban" CssClass="form-control" MaxLength="10"></asp:TextBox>

                                            </div>
                                            <p>
                                                <button type="button" id="regularaccount" class="btn btn-default btn-lg btn-block">Get Regular Account</button>

                                            </p>
                                        </div>
                                        <div id="processregular"></div>
                                    </div>
                                </div>
                            </div>
                            <div class="col-md-6">
                                <div class="panel panel-default">
                                    <div class="panel-heading">
                                        <h3 class="panel-title">I have a Regular Account</h3>
                                    </div>
                                    <div class="panel-body">

                                        <div id="displaynuban">
                                        <div class="form-group">
                                            <label>Branch Code</label>
                                            <ajaxToolkit:FilteredTextBoxExtender ID="FilteredTextBoxExtender2" runat="server" FilterType="Numbers"
                                                TargetControlID="bracode" />
                                            <asp:TextBox ID="bracode" runat="server" placeholder="Enter 1 digit branch code" CssClass="form-control" MaxLength="2"></asp:TextBox>

                                        </div>

                                        <div class="form-group">
                                            <label>Currency Code (e.g 566)</label>
                                            <ajaxToolkit:FilteredTextBoxExtender ID="FilteredTextBoxExtender3" runat="server" FilterType="Numbers"
                                                TargetControlID="curcode" />
                                            <asp:TextBox ID="curcode" runat="server" placeholder="Enter 3 digit Currency Code" CssClass="form-control" MaxLength="3"></asp:TextBox>

                                        </div>

                                        <div class="form-group">
                                            <label>Gl Code (e.g 210153)</label>
                                            <ajaxToolkit:FilteredTextBoxExtender ID="FilteredTextBoxExtender4" runat="server" FilterType="Numbers"
                                                TargetControlID="glcode" />
                                            <asp:TextBox ID="glcode" runat="server" placeholder="Enter 6 digit GL code" CssClass="form-control" MaxLength="6"></asp:TextBox>

                                        </div>

                                        <div class="form-group">
                                            <label>Customer Number (CIF e.g 100149)</label>
                                            <ajaxToolkit:FilteredTextBoxExtender ID="FilteredTextBoxExtender5" runat="server" FilterType="Numbers"
                                                TargetControlID="cif" />
                                            <asp:TextBox ID="cif" runat="server" placeholder="Enter 8 digit CIF number" CssClass="form-control" MaxLength="8"></asp:TextBox>

                                        </div>

                                        <div class="form-group">
                                            <label>Sl Number </label>
                                            <ajaxToolkit:FilteredTextBoxExtender ID="FilteredTextBoxExtender6" runat="server" FilterType="Numbers"
                                                TargetControlID="slno" />
                                            <asp:TextBox ID="slno" runat="server" placeholder="Enter 1 digit SL number" CssClass="form-control" MaxLength="1"></asp:TextBox>

                                        </div>
                                        <p>
                                            <button type="button" id="nubanaccount" class="btn btn-default btn-lg btn-block">Get NUBAN</button>

                                        </p>
                                        </div>
                                        <div id="processnuban"></div>
                                    </div>
                                </div>
                            </div>
                            
                            
                        </div>

                    </div>
                </div>
            </div>
            <div class="col-md-2"></div>
        </div>

        <div class="row">
            <div class="col-md-2"></div>
            <div class="col-md-8">
                <a href="Default.aspx">&laquo; Go Back</a>
            </div>
            <div class="col-md-2"></div>
        </div>
    </form>
    
        <!--[if lt IE 9]>
  <script src="js/html5shiv.js"></script>
  <script src="js/respond.min.js"></script>
<![endif]-->
</body>
</html>
