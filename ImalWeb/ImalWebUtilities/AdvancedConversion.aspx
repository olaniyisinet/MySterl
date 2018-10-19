<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AdvancedConversion.aspx.cs" Inherits="ImalWebUtilities.AdvancedConversion" %>
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
                <div class="panel panel-danger">
                    <div class="panel-heading">
                        <h3 class="panel-title">Advanced Account Conversion</h3>
                    </div>
                    <div class="panel-body">
                        <div class="row">
                            <div class="col-md-6">
                                <div class="panel panel-default">
                                    <div class="panel-heading">
                                        <h3 class="panel-title">I have  NUBAN Accounts</h3>
                                    </div>
                                    <div class="panel-body">
                                        <asp:Literal runat="server" ID="resp1"></asp:Literal>
                                        <asp:Button ID="DownloadTemplateNuban" runat="server" Text="Download template" CssClass="btn btn-default btn-lg btn-block" OnClick="DownloadTemplateNubanClick" />
                                        <p>&nbsp;</p>
                                        <div class="form-group">
                                            <label>Select file</label>
                                            <asp:FileUpload ID="FileUpload1" runat="server" />
                                            <p class="help-block">Make sure the excel contains Nuban accounts in the first column</p>
                                        </div>
                                        <p>&nbsp;</p>
                                        <asp:Button ID="GetRegularAccounts" runat="server" Text="Get Regular Accounts" CssClass="btn btn-default btn-lg btn-block" OnClick="GetRegularAccountsClick" />
                                    </div>
                                </div>
                            </div>
                            <div class="col-md-6">
                                <div class="panel panel-default">
                                    <div class="panel-heading">
                                        <h3 class="panel-title">I have  Regular Accounts</h3>
                                    </div>
                                    <div class="panel-body">
                                         <asp:Literal runat="server" ID="resp2"></asp:Literal>
                                        <asp:Button ID="Button1" runat="server" Text="Download template" CssClass="btn btn-default btn-lg btn-block" OnClick="DownloadTemplateRegularClick" />
                                        <p>&nbsp;</p>
                                        <div class="form-group">
                                            <label>Select file</label>
                                            <asp:FileUpload ID="FileUpload2" runat="server" />
                                            <p class="help-block">Make sure the excel contains Nuban accounts in the first column</p>
                                        </div>
                                        <p>&nbsp;</p>
                                        <asp:Button ID="Button2" runat="server" Text="Get Nuban Accounts" CssClass="btn btn-default btn-lg btn-block" OnClick="GetNubanAccountsClick" />
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
