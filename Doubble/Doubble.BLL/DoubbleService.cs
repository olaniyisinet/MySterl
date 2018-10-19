using Doubble.BLL.DTOs;
using Doubble.BLL.LoanService;
using Doubble.BLL.Utilities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doubble.BLL
{
    public class DoubbleService
    {
        Gadget g = new Gadget();
        string appMode = ConfigurationManager.AppSettings["appMode"];
        private readonly LoanServicesSoapClient annuity = new LoanServicesSoapClient();
        public DataTable checkUserExists(string Bvn)
        {
            DataTable record = g.bioData(Bvn);
            if (record.Rows.Count < 1)
            {
                record = g.bioData2(Bvn);
                if (record.Rows.Count < 1)
                {
                    record = g.bioData3(Bvn);
                }
            }
            
            return record;
        }

        public DataTable checkUserExists1(string Bvn)
        {
            DataTable record = g.bioData4(Bvn);
            if (record.Rows.Count < 1)
            {
                record = g.bioData5(Bvn);
                if (record.Rows.Count < 1)
                {
                    record = g.bioData3(Bvn);
                }
            }

            return record;
        }

        public DataTable ArrangementDetails(string ArrangementId)
        {
            DataTable record = g.bioData6(ArrangementId);
            if (record.Rows.Count < 1)
            {
                record = g.bioData7(ArrangementId);                
            }
            return record;
        }

        public DataTable checkPassword(string Bvn)
        {
            DataTable record = g.bioDataPa(Bvn);
            if (record.Rows.Count < 1)
            {
                record = g.bioDataPa(Bvn);

            }

            return record;
        }

        public List<BankAccount> eligibleDoubbleAccounts(string Bvn)
        {
            EWS1.banksSoapClient scc = new EWS1.banksSoapClient();
            DataTable dt = scc.getCustomerAccountsByBVN(Bvn).Tables[0];
            List<BankAccount> acctNos = new List<BankAccount>();
            BankAccount k = new BankAccount();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                try
                {
                    if (g.isAccountEligible(dt.Rows[i]["T24_LED_CODE"].ToString()))
                    {
                        k.Account = dt.Rows[i]["MAP_ACC_NO"].ToString();
                        k.ACCT_TYPE = dt.Rows[i]["ACCT_TYPE"].ToString();
                        acctNos.Add(new BankAccount() { Account = k.Account + " - " + k.ACCT_TYPE });
                    }
                }
                catch (Exception)
                {

                }

            }
            return acctNos;
        }

        public string validAmount(string amount, string variant)
        {
            string resp = "";

            if(variant == "Doubble Lumpsum" && Convert.ToDouble(amount) < 100000)
            {
                resp = "Minimum Opening Balance For Doubble Lumpsum should be 100000.";
            }
            else if(variant != "Doubble Lumpsum" && Convert.ToDouble(amount) < 5000)
            {
                resp = "Minimum Opening Balance For " + variant + " should be 5000.";
            }
            return resp;
        }

        public ClassResponse createDoubble(string cusnum,string amount, int term, string payInAcct, string benAcct, string daoCode, string category)
        {
            //Old
            ClassResponse resop = new ClassResponse();
            DateTime thisDate1 = new DateTime(2018, 09, 28);
            if (appMode == "online")
            {
                thisDate1 = DateTime.Now;
            }
            try
            {
                if (category != "Doubble Lumpsum")
                {
                    resop = annuity.CreateAnnuityDeposit(cusnum, "NGN", thisDate1, Convert.ToDecimal(amount), term, payInAcct, benAcct, benAcct, benAcct, daoCode, "");

                }
                else
                {
                    resop = annuity.CreateAnnuityDepositLumpSum(cusnum, "NGN", thisDate1, Convert.ToDecimal(amount), term, payInAcct, benAcct, benAcct, benAcct, daoCode, "");
                }
            }
            catch (Exception)
            {
                
            }
            return resop;
        }

        public string ArrangementID(string ReferenceId)
        {
            try
            {
               return annuity.GetArrangementIdFromActivityId(ReferenceId);
            }
            catch(Exception)
            {
                return "No ArrangementID.";
            }
        }

    }
}
