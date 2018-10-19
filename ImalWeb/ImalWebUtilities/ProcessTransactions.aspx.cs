using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Web;
using ImalWebUtilities.model;
using log4net;

namespace ImalWebUtilities
{
    public partial class ProcessTransactions : System.Web.UI.Page
    {
        private  ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public ILog Logger
        {
            get { return _logger; }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                //get query string
                string action = Request.QueryString["action"].ToString(CultureInfo.InvariantCulture);
                if (action == "")
                {
                    result.Text = "Nothing to process";
                    return;
                }
                if (action.Equals("regularaccount"))
                {
                    string nuban = Request.QueryString["nuban"].ToString(CultureInfo.InvariantCulture);
                    var account = new Account();
                    account.Nuban = nuban;
                    var accountService = new AccountService(account, (int) ConversionTypes.RegularAccount);
                    accountService.GetAccountDetails();
                    if (accountService.ErrorMessage == null)
                    {
                        result.Text = "<table class='table table-striped'><tr><td>Account Name</td><td>" +
                                      account.CustomerLongName + "</td></tr>" +
                                      "<tr><td>Nuban Account</td><td>" + account.Nuban +
                                      "</td></tr><tr><td>Regular Account</td><td>" +
                                      account.BranchCode + "/" + account.CurrencyCode + "/" + account.GeneralLedgerCode +
                                      "/" + account.CustomerInformationFileSubNumber + "/" + account.SlNo +
                                      "</td></tr><tr><td colspan='2'>Want more? Use the advance tool</td></tr><tr><td colspan='2'><p>" +
                                      "<button type='button' id='reloadregular' class='btn btn-default btn-lg btn-block' runat='server'> Reload this screen</button></p></td></tr></table>";
                    }
                    else
                    {
                        result.Text = accountService.ErrorMessage +
                                      "<p><button type='button' id='reloadregular' class='btn btn-default btn-lg btn-block' runat='server'> Reload this screen</button></p>";
                    }
                }

                if (action.Equals("statement"))
                {
                    string nuban = Request.QueryString["nuban"].ToString(CultureInfo.InvariantCulture);
                    string username = Request.QueryString["username"].ToString(CultureInfo.InvariantCulture);
                    string password = Request.QueryString["password"].ToString(CultureInfo.InvariantCulture);
                    var user = new UserValidationService();
                    bool resp = user.IsAuthenticated(username, password);
                    string err = user.ErrorMessage;
                    if (!resp)
                    {
                        Logger.Info("User :" + username +
                                    " tried accessing the statement feature but authentication failed. reason : "+err);
                        result.Text = err +
                                      "<p><button type='button' id='reloadregular3' class='btn btn-danger btn-lg btn-block' runat='server'> Reload this screen</button></p>";
                        return;
                    }
                    var account = new Account();
                    account.Nuban = nuban;
                    var accountService = new AccountService(account, (int) ConversionTypes.RegularAccount);
                    accountService.GetAccountDetails();
                    if (accountService.ErrorMessage == null)
                    {
                        //get the statement of this account
                        var statement = new StatementService(account);
                        statement.GetStatement();
                        result.Text = "<table class='table table-striped'><tr><td>Account Name</td><td>" +
                                      account.CustomerLongName + "</td></tr>" +
                                      "<tr><td>Nuban Account</td><td>" + account.Nuban +
                                      "</td></tr><tr><td>Regular Account</td><td>" +
                                      account.BranchCode + "/" + account.CurrencyCode + "/" + account.GeneralLedgerCode +
                                      "/" + account.CustomerInformationFileSubNumber + "/" + account.SlNo +
                                      "</td></tr><tr><td>Available Balance</td><td>" +
                                      account.AvailableBalance.ToString("0,0.00") + "</td></tr>" +
                                      "<td>Current Balance</td><td>" + account.LedgerBalance.ToString("0,0.00") +
                                      "</td></tr>" +
                                      "</table>" +
                                      "<p>&nbsp;</p><p>&nbsp;</p><table class='table table-striped'><tr><td>Transaction Type</td>" +
                                      "<td>Amount</td>" +
                                      "<td>Description</td>" +
                                      "<td>Date Posted</td>" +
                                      "<td>Time Posted</td>" +
                                      "<td>Value date</td>" +
                                      "</tr>";
                        foreach (var statements in statement.Statement)
                        {
                            result.Text += "<tr>" +
                                           "<td>" + statements.Type + "</td>" +
                                           "<td>" + statements.Amount.ToString("0,0.00") + "</td>" +
                                           "<td>" + statements.Description + "</td>" +
                                           "<td>" + statements.PostedDate.ToShortDateString() + "</td>" +
                                           "<td>" + statements.TimeCreated + "</td>" +
                                           "<td>" + statements.Valuedate.ToShortDateString() + "</td>" +
                                           "</tr>";
                        }
                        Logger.Info("User :" + username + " accessed statement feature successfully ");
                        result.Text +=
                            "<tr><td colspan='2'><p><button type='button' id='reloadregular3' class='btn btn-danger btn-lg btn-block' runat='server'> Reload this screen</button></p></td></tr></table>";

                    }
                    else
                    {
                        result.Text = accountService.ErrorMessage +
                                      "<p><button type='button' id='reloadregular3' class='btn btn-danger btn-lg btn-block' runat='server'> Reload this screen</button></p>";
                    }
                }
                if (action.Equals("balance"))
                {

                    string nuban = Request.QueryString["nuban"].ToString(CultureInfo.InvariantCulture);
                    string username = Request.QueryString["username"].ToString(CultureInfo.InvariantCulture);
                    string password = Request.QueryString["password"].ToString(CultureInfo.InvariantCulture);
                    var user = new UserValidationService();
                    bool resp = user.IsAuthenticated(username, password);
                    var err = user.ErrorMessage;
                    if (!resp)
                    {
                        Logger.Info("User :" + username +
                                    " tried accessing the statement feature but authentication failed. Reason :" +err);
                        result.Text = err +
                                      "<p><button type='button' id='reloadregular3' class='btn btn-danger btn-lg btn-block' runat='server'> Reload this screen</button></p>";
                        return;
                    }
                    var account = new Account();
                    account.Nuban = nuban;
                    var accountService = new AccountService(account, (int) ConversionTypes.RegularAccount);
                    accountService.GetAccountDetails();
                    if (accountService.ErrorMessage == null)
                    {
                        Logger.Info("User :" + username + " accessed balance feature successfully ");
                        result.Text = "<table class='table table-striped'><tr><td>Account Name</td><td>" +
                                      account.CustomerLongName + "</td></tr>" +
                                      "<tr><td>Nuban Account</td><td>" + account.Nuban +
                                      "</td></tr><tr><td>Regular Account</td><td>" +
                                      account.BranchCode + "/" + account.CurrencyCode + "/" + account.GeneralLedgerCode +
                                      "/" + account.CustomerInformationFileSubNumber + "/" + account.SlNo +
                                      "</td></tr><tr><td>Available Balance</td><td>" +
                                      account.AvailableBalance.ToString("0,0.00") + "</td></tr>" +
                                      "<td>Current Balance</td><td>" + account.LedgerBalance.ToString("0,0.00") +
                                      "</td></tr>" +
                                      "<tr><td colspan='2'><p>" +
                                      "<button type='button' id='reloadregular3' class='btn btn-default btn-lg btn-block' runat='server'> Reload this screen</button></p></td></tr></table>";

                    }
                    else
                    {
                        result.Text = accountService.ErrorMessage +
                                      "<p><button type='button' id='reloadregular3' class='btn btn-default btn-lg btn-block' runat='server'> Reload this screen</button></p>";
                    }
                }

                if (action.Equals("nubanaccount"))
                {
                    int bracode = Convert.ToInt32(Request.QueryString["bracode"]);
                    int curcode = Convert.ToInt32(Request.QueryString["curcode"]);
                    int glcode = Convert.ToInt32(Request.QueryString["glcode"]);
                    int cif = Convert.ToInt32(Request.QueryString["cif"]);
                    int slno = Convert.ToInt32(Request.QueryString["slno"]);

                    var account = new Account();
                    account.BranchCode = bracode;
                    account.CurrencyCode = curcode;
                    account.GeneralLedgerCode = glcode;
                    account.CustomerInformationFileSubNumber = cif;
                    account.SlNo = slno;
                    var accountService = new AccountService(account, (int) ConversionTypes.Nuban);
                    accountService.GetAccountDetails();
                    if (accountService.ErrorMessage == null)
                    {
                        result.Text = "  <table class='table table-striped'><tr><td>Account Name</td><td>" +
                                      account.CustomerLongName + "</td></tr>" +
                                      "<tr><td>Nuban Account</td><td>" + account.Nuban +
                                      "</td></tr><tr><td>Regular Account</td><td>" +
                                      account.BranchCode + "/" + account.CurrencyCode + "/" + account.GeneralLedgerCode +
                                      "/" + account.CustomerInformationFileSubNumber + "/" + account.SlNo +
                                      "</td></tr><tr><td colspan='2'>Want more? Use the advance tool</td></tr><tr><td colspan='2'><p>" +
                                      "<button type='button' id='reloadregular2' class='btn btn-default btn-lg btn-block' runat='server'> Reload this screen</button></p></td></tr></table>";
                    }
                    else
                    {
                        result.Text = accountService.ErrorMessage +
                                      "<p><button type='button' id='reloadregular2' class='btn btn-default btn-lg btn-block' runat='server'> Reload this screen</button></p>";
                    }

                }

            }catch(Exception ex)
            {
                result.Text = "";
            }
        }
    }
}