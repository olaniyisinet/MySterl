<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="ImalWebUtilities.Default" %>

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
    <script src="js/jquery-1.11.0.min.js"></script>
    <!-- Latest compiled and minified JavaScript -->
    <script src="js/bootstrap.min.js"></script>
    
    <link href="css/glyphicons-extended.min.css" rel="stylesheet" type="text/css">
    <meta name="viewport" content="width=device-width" />

    <title>IMAL Core Banking Utilities</title>
   <script src="js/modernizr-2.6.2.js"></script>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container-fluid">
            <%Response.WriteFile("UpperLinks.html"); %>

            <div class="row">
                <div class="col-md-4">
                    <div class="panel panel-success">
                        <div class="panel-heading">
                            <h3 class="panel-title">Account Conversion</h3>
                        </div>
                        <div class="panel-body">
                            <table class="table table-striped">
                                <tr>
                                    <td>I have just one account </td>
                                    <td><span class="glyphicon glyphicon-ok-sign"></span></td>
                                </tr>
                                <tr>
                                    <td>I need the Nuban/Regular equivalence </td>
                                    <td><span class="glyphicon glyphicon-ok-sign"></span></td>
                                </tr>

                                <tr>
                                    <td colspan="2">
                                        <p>
                                            <button type="button" class="btn btn-success btn-lg btn-block" onclick="window.location='SimpleConversion.aspx'">
                                                Continue
                                            </button>
                                            

                                        </p>
                                    </td>

                                </tr>
                            </table>

                        </div>
                    </div>
                </div>
                <div class="col-md-4">
                    <div class="panel panel-danger">
                        <div class="panel-heading">
                            <h3 class="panel-title">Account Conversion Advanced</h3>
                        </div>
                        <div class="panel-body">
                            <table class="table table-striped">
                                <tr>
                                    <td>I have multiple accounts</td>
                                    <td><span class="glyphicon glyphicon-ok-sign"></span></td>
                                </tr>
                                <tr>
                                    <td>I need their Nuban/Regular equivalence</td>
                                    <td><span class="glyphicon glyphicon-ok-sign"></span></td>
                                </tr>
                                <tr>
                                    <td colspan="2">
                                        <p>
                                              <button type="button" class="btn btn-danger btn-lg btn-block" onclick="window.location='AdvancedConversion.aspx'">
                                                  Continue
                                              </button>
                                        

                                        </p>
                                    </td>

                                </tr>
                            </table>
                        </div>
                    </div>
                </div>
                <div class="col-md-4">
                    <div class="panel panel-default">
                        <div class="panel-heading">
                            <h3 class="panel-title">More Advanced Stuffs </h3>
                        </div>
                        <div class="panel-body">
                            <table class="table table-striped">
                                <tr>
                                    <td>I need to do more on IMAL </td>
                                    <td><span class="glyphicon glyphicon-ok-sign"></span></td>
                                </tr>
                                <tr>
                                    <td>I have my network access credentials</td>
                                    <td><span class="glyphicon glyphicon-ok-sign"></span></td>
                                </tr>
                                <tr>
                                    <td colspan="2">
                                        <p>
                                            <button type="button" class="btn btn-default btn-lg btn-block" onclick="window.location='AdvancedStuff.aspx'">Continue</button>

                                        </p>
                                    </td>

                                </tr>
                            </table>
                        </div>
                    </div>
                </div>
            </div>
        </div>

    </form>
    
      <!--[if lt IE 9]>
  <script src="js/html5shiv.js"></script>
  <script src="js/respond.min.js"></script>
<![endif]-->
</body>
</html>
