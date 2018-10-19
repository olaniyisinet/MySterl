<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AdvancedStuff.aspx.cs" Inherits="ImalWebUtilities.AdvancedStuff" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <!-- Latest compiled and minified CSS -->
    <link rel="stylesheet" href="css/bootstrap.min.css" />

    <!-- Optional theme -->
    <!--[if lt IE 9]>
      <script src="js/html5shiv.min.js"></script>
      <script src="js/respond.min.js"></script>
    <![endif]-->

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
                        <h3 class="panel-title">DO More on IMAL Core Banking</h3>
                    </div>
                    <div class="panel-body">
                        <div class="row">
                            <div class="col-md-12">
                                <div class="panel panel-default">
                                    <div class="panel-heading">
                                        <h3 class="panel-title">I have a NUBAN Account</h3>
                                    </div>
                                    <div class="panel-body">
                                        <ajaxToolkit:ToolkitScriptManager EnablePartialRendering="true" runat="Server" ID="ScriptManager1" />
                                        <div id="displayregular3">
                                            <div class="form-group">
                                                <label for="nuban">NUBAN Account Number</label>
                                                <ajaxToolkit:FilteredTextBoxExtender ID="FilteredTextBoxExtender1" runat="server" FilterType="Numbers"
                                                    TargetControlID="nuban" />
                                                <asp:TextBox ID="nuban" runat="server" placeholder="Enter  Nuban" CssClass="form-control" MaxLength="30"></asp:TextBox>

                                            </div>

                                            <div class="form-group">
                                                <label>My AD username</label>
                                                
                                                <asp:TextBox ID="username" runat="server" placeholder="Enter username" CssClass="form-control"></asp:TextBox>

                                            </div>

                                            <div class="form-group">
                                                <label>My AD Password</label>

                                                <asp:TextBox ID="password" runat="server" placeholder="Enter password" CssClass="form-control" TextMode="Password"></asp:TextBox>

                                            </div>

                                            <div class="form-group">
                                                <label>I want to view</label>

                                                <select class="form-control" runat="server" id="item">
                                                     <option value ="0">--Select an option --</option>
                                                    <option value="balance">Customer Balance</option>
                                                    <option value="statement">Last 10 transactions</option>
                                                   
                                                    
                                                </select>

                                            </div>

                                            <p>
                                                <button type="button" id="dotrnx" class="btn btn-default btn-lg btn-block">Submit</button>

                                            </p>
                                        </div>
                                        <div id="processregular3"></div>
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

