using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web.UI.WebControls;
using ImalWebUtilities.model;
using OfficeOpenXml;
using log4net;

namespace ImalWebUtilities
{
    public partial class AdvancedConversion : System.Web.UI.Page
    {
        private readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public static readonly string[] ValidExtensions = new[] { ".xls", ".xlsx" };
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void DownloadTemplateNubanClick(object sender, EventArgs e)
        {
            const string filename = @"~/NubanToRegularAccount.xlsx";
            Response.Clear();
            Response.ContentType = "application/force-download";
            Response.AddHeader("content-disposition", "attachment; filename=nfile.xls");
            Response.WriteFile(filename);
            Response.End();
        }

        protected void DownloadTemplateRegularClick(object sender, EventArgs e)
        {

            const string filename = @"~/RegularAccountToNuban.xlsx";
            Response.Clear();
            Response.ContentType = "application/force-download";
            Response.AddHeader("content-disposition", "attachment; filename=rfile.xls");
            Response.WriteFile(filename);
            Response.End();
        }

        protected void GetRegularAccountsClick(object sender, EventArgs e)
        {
            //get the uploaded file
            FileUpload file = FileUpload1;
            string fileName = file.FileName;

            if (fileName.Equals(""))
            {

                resp1.Text = MessageFormatter.Error("Please upload a file");
                return;
            }


            //validate that it is an excel file
            string ext = Path.GetExtension(fileName);
            if (ext != null && !ValidExtensions.Contains(ext.ToLower()))
            {
                resp1.Text = MessageFormatter.Error("Only excel files are allowed");
                return;

            }
            //in this method, I expect the nuban to be on the firt column
            //const string filename = @"C:\excel\file10.xlsx";
            //var files = new FileInfo(Server.MapPath(file.PostedFile));
            using (var package = new ExcelPackage(file.PostedFile.InputStream))
            {
                ExcelWorkbook workbook = package.Workbook;
                if (workbook.Worksheets.Count > 0)
                {
                    for (int j = 1; j <= workbook.Worksheets.Count; j++)
                    {
                        ExcelWorksheet currentWorkSheet = workbook.Worksheets[j];
                        //if current workshet has no rows, then wtf???
                        if (currentWorkSheet.Dimension.End.Row == 0)
                        {
                            continue;
                        }
                        for (int i = 1; i <= currentWorkSheet.Dimension.End.Row; i++)
                        {
                            //get the nuban value
                            string nuban = currentWorkSheet.Cells[i, 1].Value.ToString().PadLeft(10, '0');
                            if (Regex.IsMatch(nuban, @"^(\d{10})\b"))
                            {
                                //continue;
                                var account = new Account();
                                account.Nuban = nuban;
                                var accountService = new AccountService(account, (int)ConversionTypes.RegularAccount);
                                accountService.GetAccountDetails();
                                if (accountService.ErrorMessage == null)
                                {
                                    currentWorkSheet.Cells[i, 2].Value = Convert.ToInt32(account.BranchCode);
                                    currentWorkSheet.Cells[i, 3].Value = Convert.ToInt32(account.CurrencyCode);
                                    currentWorkSheet.Cells[i, 4].Value = Convert.ToInt32(account.GeneralLedgerCode);
                                    currentWorkSheet.Cells[i, 5].Value = Convert.ToInt32(account.CustomerInformationFileSubNumber);
                                    currentWorkSheet.Cells[i, 6].Value = Convert.ToInt32(account.SlNo);
                                    currentWorkSheet.Cells[i, 7].Value = account.Status;
                                    currentWorkSheet.Cells[i, 8].Value = Convert.ToDecimal(account.AvailableBalance);
                                }
                                else
                                {
                                    currentWorkSheet.Cells[i, 9].Value = accountService.ErrorMessage;
                                }
                            }
                            else
                            {
                                currentWorkSheet.Cells[i, 9].Value = "The nuban number does not seem to be correct";
                            }


                        }
                    }
                    var memory = new MemoryStream();
                    package.SaveAs(memory);

                    //let the user download the file
                    Response.Clear();
                    Response.ContentType = "application/force-download";
                    Response.AddHeader("content-disposition", "attachment; filename=" + file.FileName);
                    Response.BinaryWrite(memory.ToArray());
                    Response.End();
                }
                else
                {
                    resp1.Text = MessageFormatter.Error("The excel file you have entered has no sheet " + file.PostedFile.FileName);
                    //return;
                }
            }
            //I will fill the next 5 column with the other account values
            //query the database and fill the excel with the content
            //push back the content to the user to download




        }

        protected void GetNubanAccountsClick(object sender, EventArgs e)
        {
            //get the uploaded file
            FileUpload file = FileUpload2;
            string fileName = file.FileName;

            if (fileName.Equals(""))
            {

                resp2.Text = MessageFormatter.Error("Please upload a file");
                return;
            }


            //validate that it is an excel file
            string ext = Path.GetExtension(fileName);
            if (ext != null && !ValidExtensions.Contains(ext.ToLower()))
            {
                resp2.Text = MessageFormatter.Error("Only excel files are allowed");
                return;

            }
            //in this method, I expect the nuban to be on the firt column
            //const string filename = @"C:\excel\file10.xlsx";
            //var files = new FileInfo(Server.MapPath(file.PostedFile));
            using (var package = new ExcelPackage(file.PostedFile.InputStream))
            {
                ExcelWorkbook workbook = package.Workbook;
                if (workbook.Worksheets.Count > 0)
                {
                    for (int j = 1; j <= workbook.Worksheets.Count; j++)
                    {
                        ExcelWorksheet currentWorkSheet = workbook.Worksheets[j];
                        //if current workshet has no rows, then wtf???
                        if (currentWorkSheet.Dimension.End.Row == 0)
                        {
                            continue;
                        }
                        for (int i = 1; i <= currentWorkSheet.Dimension.End.Row; i++)
                        {
                            //get the nuban value

                            int bracode, curcode, glcode, cif, slno;
                            try
                            {
                                bracode = Convert.ToInt32(currentWorkSheet.Cells[i, 1].Value.ToString().PadLeft(1, '0'));
                                curcode = Convert.ToInt32(currentWorkSheet.Cells[i, 2].Value.ToString().PadLeft(3, '0'));
                                glcode = Convert.ToInt32(currentWorkSheet.Cells[i, 3].Value.ToString().PadLeft(6, '0'));
                                cif = Convert.ToInt32(currentWorkSheet.Cells[i, 4].Value.ToString().PadLeft(8, '0'));
                                slno = Convert.ToInt32(currentWorkSheet.Cells[i, 5].Value.ToString().PadLeft(3, '0'));
                                                      }
                            catch (Exception ex)
                            {
                                currentWorkSheet.Cells[i, 7].Value = "Error occured with one of the values.";
                                continue;
                            }
                            //continue;
                            var account = new Account();
                            account.BranchCode = bracode;
                            account.CurrencyCode = curcode;
                            account.GeneralLedgerCode = glcode;
                            account.CustomerInformationFileSubNumber = cif;
                            account.SlNo = slno;
                            var accountService = new AccountService(account, (int)ConversionTypes.Nuban);
                            accountService.GetAccountDetails();
                            if (accountService.ErrorMessage == null)
                            {
                                
                                currentWorkSheet.Cells[i, 6].Value = Convert.ToString(account.Nuban).PadLeft(10,'0');
                                currentWorkSheet.Cells[i, 7].Value = account.Status;
                                currentWorkSheet.Cells[i, 8].Value = Convert.ToDecimal(account.AvailableBalance);
                            }
                            else
                            {
                                currentWorkSheet.Cells[i, 9].Value = accountService.ErrorMessage;
                            }



                        }
                    }
                    var memory = new MemoryStream();
                    package.SaveAs(memory);

                    //let the user download the file
                    Response.Clear();
                    Response.ContentType = "application/force-download";
                    Response.AddHeader("content-disposition", "attachment; filename=" + file.FileName);
                    Response.BinaryWrite(memory.ToArray());
                    Response.End();
                }
                else
                {
                    resp2.Text = MessageFormatter.Error("The excel file you have entered has no sheet " + file.PostedFile.FileName);
                    //return;
                }
            }
            //I will fill the next 5 column with the other account values
            //query the database and fill the excel with the content
            //push back the content to the user to download




        }
    }
}