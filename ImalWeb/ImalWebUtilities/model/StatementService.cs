using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;
using log4net;

namespace ImalWebUtilities.model
{
    public class StatementService
    {
        private readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public Account Account { get; set; }
        public int ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
        public List<Statement> Statement { get; set; }

        public ILog Logger
        {
            get { return _logger; }
        }

        public StatementService(Account account)
        {
            Account = account;
            Statement = new List<Statement>();
        }

        public void GetStatement()
        {
            try
            {
                var dp = new DatabaseParamaters();
                dp.AddParameter(":a", Account.Nuban);
                const string query = "select * from (select case when a.cv_amount < 0 then 'CR' else 'DR' end as type," +
                                     "case when a.cv_amount < 0 then a.cv_amount * -1 else cv_amount end as  amount,a.description," +
                                     "a.post_date,a.time_created,a.value_date from imal.dof_hst a,imal.amf b where a.cif_sub_no = b.cif_sub_no " +
                                     "and a.BRANCH_CODE = b.BRANCH_CODE and a.CURRENCY_CODE = b.CURRENCY_CODE and a.GL_CODE = b.GL_CODE and a.SL_NO = b.SL_NO" +
                                     " and b.additional_reference = :a order by a.post_date desc, a.time_created desc) statement where rownum <=10";
                var ds = new DatabaseService((int)DatabaseOperations.OracleDb, query, (int)DatabaseOperations.Select, dp);
                ds.Process();
                if (ds.Ds.Tables[0].Rows.Count == 0)
                {
                    ErrorCode = -1; //No transaction found
                    ErrorMessage = "NO transaction was found for this user";
                    return;
                }
                ErrorCode = 1;
                foreach(DataRow datarow in ds.Ds.Tables[0].Rows)
                {
                    var statement = new Statement
                                        {
                                            Type = Convert.ToString(datarow["type"]),
                                            Amount = Convert.ToDecimal(datarow["amount"]),
                                            Description = Convert.ToString(datarow["description"]),
                                            PostedDate = Convert.ToDateTime(datarow["post_date"]),
                                            TimeCreated = Convert.ToString(datarow["time_created"]),
                                            Valuedate = Convert.ToDateTime(datarow["value_date"])
                                        };
                    Statement.Add(statement);
                }
            }
            catch (Exception ex)
            {
                ErrorCode = -2; //A fatal error occured
                Logger.Fatal(ex);
                ErrorMessage = "Something happened with the connection! Try again";
            }
        }
    }
}