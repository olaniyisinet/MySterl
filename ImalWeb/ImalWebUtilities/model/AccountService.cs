using System;
using System.Reflection;
using log4net;

namespace ImalWebUtilities.model
{
    public class AccountService
    {
        private readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public Account Account { get; set; }
        public int ConversionType { get; set; }
        public int ErrorCode { get; set; }
        public string ErrorMessage { get; set; }

        public ILog Logger
        {
            get { return _logger; }
        }

        public AccountService(Account account, int type)
        {
            Account = account;
            ConversionType = type;
        }

        public void GetAccountDetails()
        {
            try
            {
                var dp = new DatabaseParamaters();
                string query = "select b.additional_reference,address1_eng,address2_eng,tel,branch_code,currency_code,gl_code,cif_sub_no,sl_no,brief_name_eng,long_name_eng,date_opnd,status," +
                               "case when ytd_cv_bal< 0 then ytd_cv_bal * -1 else ytd_cv_bal end ytd_cv_bal," +
                               "case when cv_avail_bal < 0 then cv_avail_bal * -1 else cv_avail_bal end cv_avail_bal,last_trans_date,b.DATE_CLOSD,b.DATE_REINSTATED from imal.amf b left outer join " +
                               "IMAL.AMF_ADDRESS a on a.acc_cif = b.cif_sub_no and a.acc_br = b.BRANCH_CODE and a.ACC_CY = b.CURRENCY_CODE and a.ACC_GL = b.GL_CODE and a.ACC_SL = b.SL_NO ";
                if (ConversionType == Convert.ToInt32(ConversionTypes.Nuban))
                {
                    dp.AddParameter(":a", Account.BranchCode);
                    dp.AddParameter(":b", Account.CurrencyCode);
                    dp.AddParameter(":c", Account.GeneralLedgerCode);
                    dp.AddParameter(":d", Account.CustomerInformationFileSubNumber);
                    dp.AddParameter(":e", Account.SlNo);
                    query += " where b.branch_code = :a AND b.currency_code = :b AND b.gl_code = :c AND b.cif_sub_no = :d AND b.sl_no = :e";
                }
                else
                {
                    dp.AddParameter(":a", Account.Nuban);
                    query += " where b.additional_reference = :a";
                }
                var ds = new DatabaseService((int)DatabaseOperations.OracleDb, query, (int)DatabaseOperations.Select, dp);
                ds.Process();
                if (ds.Ds.Tables[0].Rows.Count == 0)
                {
                    ErrorCode = -1; //Account details cannot be found
                    ErrorMessage = "Account cannot be found! Are you missing something?";
                    return;
                }
                ErrorCode = 1;
                var drc = ds.Ds.Tables[0].Rows;
                var row = drc[0]; //get the single row

                //update the details into the DTO Accounts
                Account.Nuban = Convert.ToString(row["additional_reference"]);
                Account.BranchCode = Convert.ToInt32(row["branch_code"]);
                Account.CurrencyCode = Convert.ToInt32(row["currency_code"]);
                Account.GeneralLedgerCode = Convert.ToInt32(row["gl_code"]);
                Account.SlNo = Convert.ToInt32(row["sl_no"]);
                Account.CustomerInformationFileSubNumber = Convert.ToInt32(row["cif_sub_no"]);
                Account.CustomerShortName = Convert.ToString("brief_name_eng");
                Account.CustomerLongName = Convert.ToString(row["long_name_eng"]);
                Account.DateOpened = Convert.ToDateTime(row["date_opnd"]);
                Account.Status = Convert.ToString(row["status"]);
                Account.DateClosed = Convert.ToString(row["date_closd"]);
                Account.DateReinstated = Convert.ToString(row["date_reinstated"]);
                Account.LedgerBalance = row["ytd_cv_bal"]==DBNull.Value ? 0 : Convert.ToDecimal(row["ytd_cv_bal"]);
                Account.AvailableBalance = row["cv_avail_bal"] == DBNull.Value ? 0 : Convert.ToDecimal(row["cv_avail_bal"]);
               
                Account.Address1 = Convert.ToString(row["address1_eng"]);
                Account.Address2 = Convert.ToString(row["address2_eng"]);
                Account.Telephone = Convert.ToString(row["tel"]);
            }catch(Exception ex)
            {
                ErrorCode = -2; //A fatal error occured
                Logger.Fatal(ex);
                ErrorMessage = "Something happened with the connection! Try again";
            }

        }

    }
}