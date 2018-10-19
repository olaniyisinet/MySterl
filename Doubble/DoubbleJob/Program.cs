//using DoubbleJob.AnnuityServ;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using Doubble;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Reflection;
using System.Globalization;
using Doubble.DAL;
using Doubble.MAIL;
using Doubble.BLL;
using Doubble.BLL.LoanService;
using System.Management;
using DoubbleJob.Eacbs;
using Sterling.MSSQL;

[assembly: log4net.Config.XmlConfigurator(Watch = true)]

namespace DoubbleJob
{
    class Program
    {

        static void Main(string[] args)
        {
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("...Doubble Job is running...");
            Mylogger.Info("Doubble Job started running: " + DateTime.Now);
            Console.WriteLine("");
            
            treatAnnuity();
            processInvestmentLetter();
            processCreaxn();

            Console.WriteLine("Doubble Job Stopped running at: " + DateTime.Now);
            Mylogger.Info("Doubble Job Stopped running at: " + DateTime.Now);
            //Console.WriteLine("Press Enter to Exit");
            //Console.ReadLine();
            

        }

        static void updateTbls()
        {
            DoubbleService d = new DoubbleService();
            Gadget g = new Gadget();
            DataTable dt = selectTreated().Tables[0];
           
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (checkRecs(dt.Rows[i]["ValCode"].ToString(), dt.Rows[i]["BVN"].ToString(), dt.Rows[i]["CusType"].ToString()) == "false")
                {
                    DataTable record = d.checkPassword(dt.Rows[i]["BVN"].ToString());
                    g.insertForSterlingLat(dt.Rows[i]["FirstName"].ToString(), dt.Rows[i]["LastName"].ToString(), dt.Rows[i]["MobileNumber"].ToString(), dt.Rows[i]["BVN"].ToString(), dt.Rows[i]["Category"].ToString(), dt.Rows[i]["PayInAccount"].ToString(), dt.Rows[i]["BeneficiaryName"].ToString(), dt.Rows[i]["BeneficiaryAccount"].ToString(), DateTime.Today, dt.Rows[i]["PayInAmount"].ToString(), dt.Rows[i]["ReferenceID"].ToString(), dt.Rows[i]["DAOCode"].ToString(), dt.Rows[i]["BeneficiaryType"].ToString(), dt.Rows[i]["Email"].ToString(), dt.Rows[i]["Cusnum"].ToString(), dt.Rows[i]["FullName"].ToString(), record.Rows[0]["Password"].ToString(), dt.Rows[i]["ARRANGEMENT_ID"].ToString(), Convert.ToInt16(dt.Rows[i]["Term"]), DateTime.Today, DateTime.Today.AddYears(Convert.ToInt16(dt.Rows[i]["Term"])), Convert.ToDateTime(dt.Rows[i]["DateOfBirth"]), dt.Rows[i]["ValCode"].ToString());

                    
                    if (record.Rows.Count > 0)
                    {
                        if (dt.Rows[i]["Category"].ToString() != "Doubble Lumpsum")
                        {
                            g.sendExDoubble(dt.Rows[i]["FirstName"].ToString(), g.thAppend(DateTime.Today.Day.ToString()), dt.Rows[i]["Email"].ToString());
                        }
                        else
                        {
                            g.sendExDoubbleLump(dt.Rows[i]["FirstName"].ToString(), dt.Rows[i]["Email"].ToString());
                        }

                    }

                }
            }


        }

        static DataSet selectTreated()
        {

            DateTime datt = new DateTime(2018, 8, 28);
            string sql = "select * from For_Later where ReferenceID != 'NULL' OR RTRIM(LTRIM(ReferenceID)) != '' OR ReferenceID != 'false' AND DateOfEntry < @dayte";
            Connect c = new Connect("msconn");
            c.SetSQL(sql);
            c.AddParam("@dayte", DateTime.Today.AddDays(1));
            DataSet ds = c.Select("rec");
            return ds;
        }

        static DataSet selectDoubb()
        {
            string sql = "select * from T24FDD2 where PRODUCT = 'ANNUITY.TERM' AND ARR_STATUS = 'CURRENT'";
            Connect c = new Connect("conn");
            c.SetSQL(sql);
            DataSet ds = c.Select("rec");
            return ds;
        }

        static DataSet selectLumpsum()
        {
            string sql = "select * from T24FDD2 where PRODUCT = 'LUMPSUM.TERM' AND ARR_STATUS = 'CURRENT'";
            Connect c = new Connect("conn");
            c.SetSQL(sql);
            DataSet ds = c.Select("rec");
            return ds;
        }

        static void treatAnnuity()
        {

            Gadget g = new Gadget();
            string[] strArr = null;
            char[] splitchar = { ' ' };
            DataTable dt = selectDoubb().Tables[0];

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                try
                {
                    DateTime newdt = DateTime.ParseExact(dt.Rows[i]["START_DATE"].ToString(), "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture);
                    DateTime promdt = new DateTime();
                    try
                    {
                        promdt = DateTime.ParseExact(dt.Rows[i]["LAST_PROMPT"].ToString(), "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture);
                    }
                    catch (Exception e)
                    {
                        Mylogger.Error(e);
                        promdt = new DateTime(2018, 01, 01);
                    }
                    strArr = dt.Rows[i]["SHORT_NAME"].ToString().Split(splitchar);
                    if (newdt.Day - DateTime.Today.Day == 1 && promdt != DateTime.Today)
                    {

                        g.sendReminderMail(strArr[0], dt.Rows[i]["AMOUNT"].ToString(), DateTime.Today.AddDays(1).ToString(), dt.Rows[i]["EMAIL_1"].ToString());
                        Mylogger.Info("Reminder was sent for " + dt.Rows[i]["ARRANGEMENT_ID"].ToString().Trim());
                        updateLastPrompt(dt.Rows[i]["ARRANGEMENT_ID"].ToString().Trim());

                    }
                    else if (newdt.Day - DateTime.Today.Day == 3 && promdt != DateTime.Today)
                    {
                        g.sendReminderMail(strArr[0], dt.Rows[i]["AMOUNT"].ToString(), DateTime.Today.AddDays(3).ToString(), dt.Rows[i]["EMAIL_1"].ToString());
                        Mylogger.Info("Reminder was sent for " + dt.Rows[i]["ARRANGEMENT_ID"].ToString().Trim());
                        updateLastPrompt(dt.Rows[i]["ARRANGEMENT_ID"].ToString().Trim());
                    }
                    else if (newdt.Day - DateTime.Today.Day == 7 && promdt != DateTime.Today)
                    {
                        g.sendReminderMail(strArr[0], dt.Rows[i]["AMOUNT"].ToString(), DateTime.Today.AddDays(7).ToString(), dt.Rows[i]["EMAIL_1"].ToString());
                        Mylogger.Info("Reminder was sent for " + dt.Rows[i]["ARRANGEMENT_ID"].ToString().Trim());
                        updateLastPrompt(dt.Rows[i]["ARRANGEMENT_ID"].ToString().Trim());
                    }
                }
                catch (Exception e)
                {

                    Mylogger.Error("Error " + e + " was encountered while sending reminder.");
                }
            }
        }


        static void processCreaxn()
        {
            DataTable dt = selectNoRef().Tables[0];
            LoanServicesSoapClient annuity = new LoanServicesSoapClient();
            string arrId = "";
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (checkRecs(dt.Rows[i]["ValCode"].ToString(), dt.Rows[i]["BVN"].ToString(), dt.Rows[i]["CusType"].ToString()) == "false" && Convert.ToDateTime(dt.Rows[i]["EffectiveDate"]) <= DateTime.Today)
                {
                    try
                    {
                        createDoubble(dt.Rows[i]["Category"].ToString(), dt.Rows[i]["Cusnum"].ToString(), Convert.ToDecimal(dt.Rows[i]["PayInAmount"]), Convert.ToInt16(dt.Rows[i]["Term"]), dt.Rows[i]["PayInAccount"].ToString(), dt.Rows[i]["BeneficiaryAccount"].ToString(), dt.Rows[i]["DAOCode"].ToString(), dt.Rows[i]["BeneficiaryName"].ToString(), dt.Rows[i]["BeneficiaryType"].ToString(), dt.Rows[i]["ValCode"].ToString(), dt.Rows[i]["FirstName"].ToString(), dt.Rows[i]["Email"].ToString(), dt.Rows[i]["CusType"].ToString(), dt.Rows[i]["LastName"].ToString(), dt.Rows[i]["MobileNumber"].ToString(), dt.Rows[i]["BVN"].ToString(), dt.Rows[i]["BVN"].ToString(), Convert.ToInt16(dt.Rows[i]["ID"]), Convert.ToDateTime(dt.Rows[i]["DateOfBirth"]));
                        Mylogger.Info("Dobble was initiated for User with ID " + dt.Rows[i]["ID"].ToString().Trim());
                    }
                    catch (Exception e)
                    {
                        Mylogger.Error("Error " + e + " was encountered while creating doubble for " + dt.Rows[i]["ID"].ToString().Trim() + ".");
                    }
                }
                else
                {
                    arrId = annuity.GetArrangementIdFromActivityId(checkRecs(dt.Rows[i]["ValCode"].ToString(), dt.Rows[i]["BVN"].ToString(), dt.Rows[i]["CusType"].ToString()));
                    updateRefId(Convert.ToInt16(dt.Rows[i]["ID"]), checkRecs(dt.Rows[i]["ValCode"].ToString(), dt.Rows[i]["BVN"].ToString(), dt.Rows[i]["CusType"].ToString()), arrId);
                    //update Reference ID
                }
            }


        }

        static DataSet selectSterlingInvestment()
        {
            string sql = "select * from ForSterlings where InvestmentLetter = '0' AND DateOfEntry < @dayte AND ReferenceID != 'NULL' AND ARRANGEMENT_ID != 'NULL'";
            Connect c = new Connect("msconn");
            c.SetSQL(sql);
            c.AddParam("@dayte", DateTime.Today.AddDays(1));
            DataSet ds = c.Select("rec");
            return ds;
        }

        static void updateForSterlInv(string arrangementId)
        {
            string sql = "update ForSterlings set InvestmentLetter='1' where ARRANGEMENT_ID=@arrangementId";
            Connect c = new Connect("msconn");
            c.SetSQL(sql);
            c.AddParam("@arrangementId", arrangementId);
            c.Select("rec");
        }
        static void updateForNonSterlInv(string arrangementId)
        {
            string sql = "update ForNonSterlings set InvestmentLetter='1' where ARRANGEMENT_ID=@arrangementId";
            Connect c = new Connect("msconn");
            c.SetSQL(sql);
            c.AddParam("@arrangementId", arrangementId);
            c.Select("rec");
        }


        static DataSet selectNoSterlingInvestment()
        {
            string sql = "select * from ForNonSterlings where InvestmentLetter = '0' AND DateOfEntry <= @dayte AND ReferenceID != 'NULL' AND ARRANGEMENT_ID != 'NULL'";
            Connect c = new Connect("msconn");
            c.SetSQL(sql);
            c.AddParam("@dayte", DateTime.Today.AddDays(1));
            DataSet ds = c.Select("rec");
            return ds;
        }

        static DataSet selectAutoLetter(string arrangementId)
        {
            string sql = "select * from T24FDD2 where ARRANGEMENT_ID = @arrangementId";
            Connect c = new Connect("conn");
            c.SetSQL(sql);
            c.AddParam("@arrangementId", arrangementId);
            DataSet ds = c.Select("rec");
            return ds;
        }

        static void processInvestmentLetter()
        {

            DataTable dt = selectSterlingInvestment().Tables[0];
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                try
                {
                    genPDF(dt.Rows[i]["ARRANGEMENT_ID"].ToString(), dt.Rows[i]["Email"].ToString());
                    Mylogger.Info("Investment Letter was generated for " + dt.Rows[i]["ARRANGEMENT_ID"].ToString());
                    updateForSterlInv(dt.Rows[i]["ARRANGEMENT_ID"].ToString());
                }
                catch (Exception e)
                {
                    Mylogger.Error("Error " + e + " was encountered while processing investment letter for " + dt.Rows[i]["ARRANGEMENT_ID"].ToString() + ".");

                }
            }

            DataTable dt2 = selectNoSterlingInvestment().Tables[0];
            for (int i = 0; i < dt2.Rows.Count; i++)
            {
                try
                {
                genPDF(dt.Rows[i]["ARRANGEMENT_ID"].ToString(), dt.Rows[i]["Email"].ToString());
                Mylogger.Info("Investment Letter was generated for " + dt.Rows[i]["ARRANGEMENT_ID"].ToString());
                updateForNonSterlInv(dt.Rows[i]["ARRANGEMENT_ID"].ToString());
                }
                catch (Exception e)
                {
                    Mylogger.Error("Error " + e + " was encountered while processing investment letter for " + dt.Rows[i]["ARRANGEMENT_ID"].ToString() + ".");

                }
            }



        }

        static void genPDF(string arrangementId, string mail)
        {
            DataTable dt = selectAutoLetter(arrangementId).Tables[0];
            System.Random rd = new System.Random();
            string bno = DateTime.Now.Year + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Second.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Hour.ToString() + rd.Next(100, 9999);
            string msgbody = "";
            string fname = "";
            string oname = "";
            int rndseed;
            string htmlstr = "";
            string referenceId = "";
            referenceId = (dt.Rows[0]["CUSTOMER"].ToString() + dt.Rows[0]["CO_CODE"].ToString() + dt.Rows[0]["AMOUNT"].ToString().Replace(",", "").Replace(".", "")).PadRight(15, '0');

            string cur = dt.Rows[0]["CURRENCY"].ToString().Trim();

            string basis = dt.Rows[0]["DAY_BASIS"].ToString().Trim();
            string phoneNo = dt.Rows[0]["PHONE_1"].ToString().Trim();
            //htmlstr = System.IO.File.ReadAllText(@"E:\Tolu\ToBeDeployed\Doubble\DoubbleJob\templates\Investment_Letter.html");
            string path = Directory.GetCurrentDirectory() + "\\Investment_Letter.html";
            htmlstr = System.IO.File.ReadAllText(path);
            DateTime newdt = DateTime.ParseExact(dt.Rows[0]["MATURITY_DATE"].ToString(), "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture);

            string t4 = (newdt.DayOfWeek).ToString();
            string getday = "";
            string prod = "";
            var paddingadd = "";
            int valadd = 0;

            prod = dt.Rows[0]["PRODUCT"].ToString();
            string account = dt.Rows[0]["LINKED_APPL_ID"].ToString();


            string term = dt.Rows[0]["TERM"].ToString();

            if (term.Contains("D"))
            {
                getday = term.ToString().Replace("D", "");
                valadd = 1;
                paddingadd = " DAYS";
            }
            else if (term.Contains("M"))
            {
                getday = term.ToString().Replace("M", "");
                valadd = 30;
                paddingadd = " MONTHS";
            }
            else if (term.Contains("Y"))
            {
                getday = term.ToString().Replace("Y", "");
                valadd = 365;
                if (Convert.ToInt16(getday) > 1)
                {
                    paddingadd = " YEARS";
                }
                else
                {
                    paddingadd = " YEAR";
                }


            }
            int getday2;

            if (t4 == "0")
            {
                getday2 = Convert.ToInt16(getday);
            }
            else
            {
                getday2 = Convert.ToInt16(getday);
            }

            int getdayval = getday2 * valadd;
            double valueamt = Convert.ToDouble(dt.Rows[0]["AMOUNT"]);
            double getrate = Convert.ToDouble(dt.Rows[0]["EFFECTIVE_RATE"]) / 100.0;
            double Intt;
            if (basis == "A")
            {
                Intt = (getdayval / 360) * getrate * valueamt;
            }
            else
            {
                Intt = (getdayval / 365) * getrate * valueamt;
            }

            DateTime dtt = DateTime.ParseExact(dt.Rows[0]["START_DATE"].ToString(), "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture);
            DateTime matdtt = DateTime.ParseExact(dt.Rows[0]["MATURITY_DATE"].ToString(), "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture);
            DateTime valdtt = DateTime.ParseExact(dt.Rows[0]["START_DATE"].ToString(), "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture);


            string payOutDt = dtt.AddMonths(61).ToString("dd-MMM-yyyy");
            double payOutAmt = 0;
            string prinContr = "Monthly Contribution";
            string prinContrComm = "for 60 months";
            string tenorComm = "";
            string mpoComm = "for 60 months effective";
            string firstComm = "You are required to meet your monthly contribution to fully obtain the payout for the stated tenor.";
            string secComm = "However, premature termination of deposit will attract a penal rate of 50% on the interest earned.";
            string thirComm = "This is an auto generated letter and requires no signature.";

            if (getday == "10" || getday == "3654" || getday == "120")
            {
                if (prod == "ANNUITY.TERM")
                {
                    getday = "10";
                    paddingadd = " YEARS";
                    payOutAmt = Math.Round(Convert.ToDouble(dt.Rows[0]["AMOUNT"]) * 1.5, 2);
                    tenorComm = "5 years pay-in and 5 years pay-out period";
                }
                else
                {
                    getday = "10";
                    paddingadd = " YEARS";
                    payOutAmt = Math.Round(Convert.ToDouble(dt.Rows[0]["AMOUNT"]) * 200 / 6000, 2);
                    prinContr = "Principal";
                    prinContrComm = "";
                    tenorComm = "Lumpsum pay-in and Monthly pay-out from year 6";
                    firstComm = "Premature termination of deposit will attract a penal rate of 50% on the interest earned.";
                    secComm = "";
                    thirComm = "This is an auto generated letter and requires no signature.";
                }
            }
            else if (getday == "15" || getday == "180" || getday == "5480")
            {
                getday = "15";
                paddingadd = " YEARS";
                payOutAmt = Convert.ToDouble(dt.Rows[0]["AMOUNT"]);
                tenorComm = "5 years pay-in and 10 years pay-out period";
                mpoComm = "for 120 months effective";
            }
            else if (getday == "6" || getday == "72" || getday == "2194")
            {
                getday = "6";
                paddingadd = " YEARS";
                payOutAmt = Math.Round(Convert.ToDouble(dt.Rows[0]["AMOUNT"]) * 125 / 100, 2);
                payOutDt = dtt.AddMonths(37).ToString("dd-MMM-yyyy");
                prinContrComm = "for 36 months";
                tenorComm = "3 years pay-in and 3 years pay-out period";
                mpoComm = "for 36 months effective";
            }

            string anndtt;

            if (dtt.Day.ToString().EndsWith("1"))
            {
                anndtt = dtt.Day.ToString() + "st Of Every Month";
            }
            else if (dtt.Day.ToString().EndsWith("2"))
            {
                anndtt = dtt.Day.ToString() + "nd Of Every Month";
            }
            else if (dtt.Day.ToString().EndsWith("3"))
            {
                anndtt = dtt.Day.ToString() + "rd Of Every Month";
            }
            else
            {
                anndtt = dtt.Day.ToString() + "th Of Every Month";
            }
            string test;
            string test1;
            string[] strArray;
            // test1 = myTI.ToTitleCase(dr("STREET").ToString)
            test = dtt.ToString("dd-MMMM-yyyy");
            strArray = test.Split('-');
            char[] myChar = new[] { '0' };

            if (strArray[0].EndsWith("1"))
            {
                test1 = strArray[0].ToString().TrimStart(myChar) + "st Of " + strArray[1].ToString() + ", " + strArray[2].ToString();
            }
            else if (strArray[0].EndsWith("2"))
            {
                test1 = strArray[0].ToString().TrimStart(myChar) + "nd Of " + strArray[1].ToString() + ", " + strArray[2].ToString();
            }
            else if (strArray[0].EndsWith("3"))
            {
                test1 = strArray[0].ToString().TrimStart(myChar) + "rd Of " + strArray[1].ToString() + ", " + strArray[2].ToString();
            }
            else
            {
                test1 = strArray[0].ToString().TrimStart(myChar) + "th Of " + strArray[1].ToString() + ", " + strArray[2].ToString();
            }

            TextInfo myTI = CultureInfo.CurrentCulture.TextInfo;
            banksSoapClient bs = new banksSoapClient();
            DataTable dj = bs.getAccountFullInfo(dt.Rows[0]["LINKED_APPL_ID"].ToString()).Tables[0];
            string addr1 = "";
            //string addr2 = "";
            if (dj.Rows.Count > 0)
            {
                addr1 = dj.Rows[0]["ADD_LINE1"].ToString();
            }
            else
            {
                string sql = "select * from ForNonSterlings where ARRANGEMENT_ID = @arrId";
                Connect c = new Connect("msconn");
                c.SetSQL(sql);
                c.AddParam("@arrId", arrangementId);
                DataTable dd = c.Select("rec").Tables[0];
                addr1 = dd.Rows[0]["HomeAddress"].ToString();
            }


            string htmlstr2 = @"";
            htmlstr2 = htmlstr.Replace("%date%", dtt.ToString("dd-MMM-yyyy"))
            .Replace("%name%", dt.Rows[0]["SHORT_NAME"].ToString() + " (" + dt.Rows[0]["CUSTOMER"].ToString() + ") ")
            .Replace("%fname%", dt.Rows[0]["SHORT_NAME"].ToString())
            .Replace("%refno%", referenceId)
            .Replace("%address1%", myTI.ToTitleCase(addr1.ToLower()))
            //.Replace("%address2%", myTI.ToTitleCase(dt.Rows[0]["TOWN_COUNTRY"].ToString().ToLower()))
            .Replace("%amount%", (Math.Round(Convert.ToDouble(dt.Rows[0]["AMOUNT"]), 2)).ToString())
            .Replace("%currency%", cur)
            .Replace("%interest%", dt.Rows[0]["EFFECTIVE_RATE"].ToString() + " %")
            .Replace("%days%", getday.ToString() + paddingadd)
            .Replace("%effectiveDate%", dtt.ToString("dd-MMMM-yyyy"))
            .Replace("%MaturityDate%", matdtt.ToString("dd-MMMM-yyyy"))
            .Replace("%AnnDate%", anndtt.ToString())
            .Replace("%PayOutAmt%", payOutAmt.ToString())
            .Replace("%PayOutDt%", payOutDt.ToString())
            .Replace("%prinContr%", prinContr.ToString())
            .Replace("%prinContrComm%", prinContrComm.ToString())
            .Replace("%tenorComm%", tenorComm.ToString())
            .Replace("%mpoComm%", mpoComm.ToString())
            .Replace("%firstComm%", firstComm.ToString())
            .Replace("%secComm%", secComm.ToString())
            .Replace("%thirComm%", thirComm.ToString());


            rndseed = rd.Next(1, 99999);
            string dir = ConfigurationManager.AppSettings["out"] + bno;
            string dir2 = ConfigurationManager.AppSettings["out2"] + bno;
            fname = ConfigurationManager.AppSettings["out"] + bno + "\\" + dt.Rows[0]["SHORT_NAME"].ToString().Replace("'", "`").Replace("&", "-").Replace("\\", "-").Replace("/", "-").Replace("\"\"", "") + rndseed + ".pdf";
            oname = ConfigurationManager.AppSettings["out2"] + bno + "\\" + dt.Rows[0]["SHORT_NAME"].ToString().Replace("'", "`").Replace("&", "-").Replace("\\", "-").Replace("/", "-").Replace("\"\"", "") + rndseed + ".pdf";

            // create pdf for each record
            createPDF(htmlstr2, fname, dir);

            // add watermark to pdf
            AddWaterMarktoPdf(fname, oname, referenceId, dir2);

            msgbody = "Dear " + dt.Rows[0]["SHORT_NAME"].ToString() + "<br/><br/>" + "Find attached your Doubble By Sterling Confirmation letter.<br/>Please note your reference number for future verification.<br/><br/><b>Letter Reference No:</b> " + referenceId + "<br/><br/>Sterling Bank Plc";
            Gadget g = new Gadget();
            g.sendmail(mail, "ebusiness@sterlingbankng.com", "TERM DEPOSIT LETTER FROM STERLING BANK", msgbody, oname, dt.Rows[0]["SHORT_NAME"].ToString().Replace("'", "`").Replace("&", "-").Replace("\\", "-").Replace("/", "-").Replace("\"\"", "") + rndseed + ".pdf");


            Mylogger.Info("Investment Letter was generated for " + arrangementId);

        }

        static void createPDF(string htmlstr, string pdfname, string dir)
        {

            string line = htmlstr;
            iTextSharp.text.Document doc = new iTextSharp.text.Document(PageSize.A4, 7.0F, 7.0F, 7.0F, 0F);
            StringReader fsNew = new StringReader(line);
            // Dim document As New Document(PageSize.A4, 80, 50, 30, 65)
            iTextSharp.text.Document document = new iTextSharp.text.Document(PageSize.A4, 7.0F, 7.0F, 7.0F, 0F);

            if (!Directory.Exists(pdfname))
            {
                Directory.CreateDirectory(dir);
            }

            using (FileStream fs = new FileStream(pdfname, FileMode.Create))
            {
                PdfWriter.GetInstance(document, fs);
                using (StringReader stringReader = new StringReader(line))
                {
                    System.Collections.Generic.List<IElement> parsedList = iTextSharp.text.html.simpleparser.HTMLWorker.ParseToList(stringReader, null/* TODO Change to default(_) if this is not a reference type */);
                    document.Open();
                    // parse each html object and add it to the pdf document


                    foreach (object item in parsedList)
                        document.Add((IElement)item);

                    document.Close();
                }
            }
        }


        static void AddWaterMarktoPdf(string Inputpath, string OutputPath, string watermarkText, string dir2)
        {
            iTextSharp.text.pdf.PdfReader pdfReader = new iTextSharp.text.pdf.PdfReader(Inputpath);
            // create stream of filestream or memorystream etc. to create output file
            if (!Directory.Exists(OutputPath))
            {
                Directory.CreateDirectory(dir2);
            }

            FileStream stream = new FileStream(OutputPath, FileMode.OpenOrCreate);
            // create pdfstamper object which is used to add addtional content to source pdf file
            iTextSharp.text.pdf.PdfStamper pdfStamper = new iTextSharp.text.pdf.PdfStamper(pdfReader, stream);
            // iterate through all pages in source pdf
            for (int pageIndex = 1; pageIndex <= pdfReader.NumberOfPages; pageIndex++)
            {
                // Rectangle class in iText represent geomatric representation... in this case, rectanle object would contain page geomatry
                iTextSharp.text.Rectangle pageRectangle = pdfReader.GetPageSizeWithRotation(pageIndex);
                // pdfcontentbyte object contains graphics and text content of page returned by pdfstamper
                PdfContentByte pdfData = pdfStamper.GetUnderContent(pageIndex);
                // create fontsize for watermark
                pdfData.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, BaseFont.NOT_EMBEDDED), 40);
                // create new graphics state and assign opacity
                PdfGState graphicsState = new PdfGState();
                graphicsState.FillOpacity = 0.4F;
                // set graphics state to pdfcontentbyte
                pdfData.SetGState(graphicsState);
                // set color of watermark
                pdfData.SetColorFill(BaseColor.LIGHT_GRAY);


                // indicates start of writing of text
                pdfData.BeginText();
                // show text as per position and rotation
                pdfData.ShowTextAligned(Element.ALIGN_CENTER, watermarkText, pageRectangle.Width / (float)2, pageRectangle.Height / (float)2, 45);
                // call endText to invalid font set
                pdfData.EndText();
            }
            // close stamper and output filestream
            pdfStamper.Close();
            stream.Close();
        }





        static DataSet selectNoRef()
        {
            string sql = "select * from For_Later where ReferenceID is NULL AND EffectiveDate < @dayte  OR ReferenceID = 'false' OR RTRIM(LTRIM(ReferenceID)) = ''";
            Connect c = new Connect("msconn");
            c.SetSQL(sql);
            c.AddParam("@dayte", DateTime.Today.AddDays(1));
            DataSet ds = c.Select("rec");
            return ds;
        }

        static string checkRecs(string valCode, string bvn, string cusType)
        {
            string tret = "false";

            if (cusType == "Onboard")
            {
                string sql = "select * from ForNonSterlings where ValCode = @valCode AND BVN = @bvn AND ReferenceID != 'NULL'";
                Connect c = new Connect("msconn");
                c.SetSQL(sql);
                c.AddParam("@valCode", valCode);
                c.AddParam("@bvn", bvn);
                DataSet ds = c.Select("rec");
                DataTable dt = ds.Tables[0];
                if (dt.Rows.Count > 0)
                {
                    tret = dt.Rows[0]["ReferenceID"].ToString();
                }
            }
            else
            {
                string sql = "select * from ForSterlings where ValCode = @valCode AND BVN = @bvn AND ReferenceID != 'NULL'";
                Connect c = new Connect("msconn");
                c.SetSQL(sql);
                c.AddParam("@valCode", valCode);
                c.AddParam("@bvn", bvn);
                DataSet ds = c.Select("rec");
                DataTable dt = ds.Tables[0];
                if (dt.Rows.Count > 0)
                {
                    tret = dt.Rows[0]["ReferenceID"].ToString();
                }
            }
            return tret;
        }

        static void createDoubble(string cat, string cusNum, decimal amt, int term, string payInAcc, string benAcc, string daoCode, string benName, string benType, string valCode, string fName, string mail, string cusType, string lName, string mobNum, string bvn, string fullName, int id, DateTime dob)
        {
            Gadget g = new Gadget();
            DoubbleDalService dal = new DoubbleDalService();
            DoubbleMAILService mailServ = new DoubbleMAILService();
            DoubbleService d = new DoubbleService();
            ClassResponse resop = new ClassResponse();
            string arrId = "";
            DateTime thisDate1 = new DateTime(2018, 7, 3);

            try
            {
                resop = d.createDoubble(cusNum, amt.ToString(), term, payInAcc, benAcc, daoCode, cat);
                if (resop.ResponseCode == "1")
                {
                    Mylogger.Info("Doubble was created for " + bvn);

                    try
                    {
                        string pwd = g.RandomString(10);
                        arrId = d.ArrangementID(resop.ReferenceID);
                        updateRefId(id, resop.ReferenceID, arrId);
                        DataTable record = d.checkPassword(bvn);
                        if (record.Rows.Count > 0)
                        {                            
                                g.insertForSterlingLat(fName, lName, mobNum, bvn, cat, payInAcc, benName, benAcc, DateTime.Today, amt.ToString(), resop.ReferenceID, daoCode, benType, mail, cusNum, fullName, record.Rows[0]["Password"].ToString(), arrId, term, DateTime.Today, DateTime.Today.AddYears(term), dob, valCode);
                            
                        }
                        else
                        {
                            if (cusType == "Onboard")
                            {
                                g.updateForNonSterling(cat, amt.ToString(), benName, benAcc, benType, resop.ReferenceID, daoCode, valCode, arrId, term, DateTime.Today, DateTime.Today.AddYears(term), pwd);

                            }
                            else
                            {
                                g.insertForSterlingLat(fName, lName, mobNum, bvn, cat, payInAcc, benName, benAcc, DateTime.Today, amt.ToString(), resop.ReferenceID, daoCode, benType, mail, cusNum, fullName, pwd, arrId, term, DateTime.Today, DateTime.Today.AddYears(term), dob, valCode);
                            }


                        }
                        if (record.Rows.Count > 0)
                        {
                            if (cat != "Doubble Lumpsum")
                            {
                                g.sendExDoubble(fName, g.thAppend(DateTime.Today.Day.ToString()), mail);
                            }
                            else
                            {
                                g.sendExDoubbleLump(fName, mail);
                            }

                        }
                        else
                        {
                            if (cat != "Doubble Lumpsum")
                            {
                                g.sendDoubbleOpeningMail(fName, g.thAppend(DateTime.Today.Day.ToString()), mail, pwd);
                            }
                            else
                            {
                                g.LsendDoubbleOpeningMail(fName, mail, pwd);
                            }
                        }
                        


                    }
                    catch (Exception e)
                    {
                        Mylogger.Error("BVN, " + bvn + " encountered error: " + e + " while creating Doubble Product. Error from service is: " + resop.ResponseText);

                    }

                }
                else
                {
                    Mylogger.Info("Doubble could not be created for " + bvn + ", reason is: " + resop.ResponseText);

                }
            }
            catch (Exception e)
            {
                //Mylogger.Error("BVN, " + model.BVN + " returned error: " + e + " while creating Doubble Product. Error from service is: " + resop.ResponseText);

            }

        }

        static void updateRefId(int id, string refId, string arrId)
        {
            string sql = "update For_Later set ReferenceID=@refid,ARRANGEMENT_ID=@arrId where id=@id";
            Connect c = new Connect("msconn");
            c.SetSQL(sql);
            c.AddParam("@id", id);
            c.AddParam("@refid", refId);
            c.AddParam("@arrId", arrId);
            c.Select("rec");

        }


        static void updateLastPrompt(string arrangementId)
        {
            string sql = "update T24FDD2 set LAST_PROMPT=@lprompt where ARRANGEMENT_ID=@arrangementId";
            Connect c = new Connect("conn");
            c.SetSQL(sql);
            c.AddParam("@arrangementId", arrangementId);
            c.AddParam("@lprompt", DateTime.Today.ToString("yyyyMMdd"));
            c.Select("rec");
        }



    }
}
