using System;
using System.Data;

namespace MMWS
{
    public class Request
    {
        private decimal _amount;
        private decimal _fee;
        private decimal _confirmed;
        private int _companyID;

        public int ChannelID { get; set; }
        public string Service { get; set; }
        public string Mobile { get; set; }
        public string PIN { get; set; }
        public string Name { get; set; }
        public string InstID { get; set; }
        public string AuthKey { get; set; }
        public string SessionID { get; set; }
        public string ActivationCode { get; set; }
        public string NewPIN { get; set; }
        public string ConfirmPIN { get; set; }
        public string SourcePocketCode { get; set; }
        public string Confirmed { get; set; }
        public string Confirmedx
        {
            get { return _confirmed.ToString("00000.00"); }
            set
            {
                try
                {
                    _confirmed = Convert.ToDecimal(value);
                }
                catch
                {
                    _confirmed = 0;
                }
            }
        }

        public string DestPocketCode { get; set; }
        public object DestMobile { get; set; }
        public object DestBankAccount { get; set; }
        public object ParentTxnID { get; set; }
        public object TransferID { get; set; }
        public string Amount
        {
            get { return _amount.ToString("00000.00"); }
            set
            {
                try
                {
                    _amount = Convert.ToDecimal(value);
                }
                catch
                {
                    _amount = 0;
                }
            }
        }

        public string Fee
        {
            get { return _fee.ToString(); }
            set
            {
                try
                {
                    _fee = Convert.ToDecimal(value);
                }
                catch
                {
                    _fee = 0;
                }
            }
        }

        public Request(string mobile, string pin, string sess)
        {
            ChannelID = 6;
            Service = "";
            Mobile = mobile;
            PIN = pin;
            Name = "";
            InstID = "";
            AuthKey = "";
            SessionID = sess;
            ActivationCode = "";
            NewPIN = "";
            ConfirmPIN = "";
            Confirmed = "";
            DestPocketCode = "";
            DestMobile = "";
            DestBankAccount = "";
        }

        public string GetLogin()
        {
            string sql = "select authkey from tbl_USSD_reqstate where sessionid = @si and msisdn = @mb";
            Connect2 cn = new Connect2(sql);
            cn.addparam("@si", SessionID);
            cn.addparam("@mb", Mobile);
            return cn.selectScalar();
        }

        public int SetLogin(string ak)
        {
            string sql = "update tbl_USSD_reqstate set authkey = @ak  where sessionid = @si and msisdn = @mb";
            Connect2 cn = new Connect2(sql);
            cn.addparam("@ak", ak);
            cn.addparam("@si", SessionID);
            cn.addparam("@mb", Mobile);
            return cn.update();
        }

        public void GetParentIDs()
        {
            string sql = "select parentTxnID, transferID from tbl_USSD_reqstate where sessionid = @si and msisdn = @mb";
            Connect2 cn = new Connect2(sql);
            cn.addparam("@si", SessionID);
            cn.addparam("@mb", Mobile);
            DataSet ds = cn.@select();

            ParentTxnID = "";
            TransferID = "";

            if(cn.num_rows > 0)
            {
                DataRow dr = ds.Tables[0].Rows[0];
                ParentTxnID = Convert.ToString(dr["parentTxnID"]);
                TransferID = Convert.ToString(dr["transferID"]);
            }
        }

        public int SetParentIDs(string parentTxnID, string transferID)
        {
            string sql = "update tbl_USSD_reqstate set parentTxnID = @pi, transferID = @ti  where sessionid = @si and msisdn = @mb";
            Connect2 cn = new Connect2(sql);
            cn.addparam("@pi", parentTxnID);
            cn.addparam("@ti", transferID);
            cn.addparam("@si", SessionID);
            cn.addparam("@mb", Mobile);
            return cn.update();
        }

        public string SubFirstName { get; set; }

        public string SubLastName { get; set; }

        public object DestBankCode { get; set; }

        public string CompanyID
        {
            get { return _companyID.ToString("00"); }
            set { _companyID = Convert.ToInt32(value); }
        }

        public string AgentCode { get; set; }
    }
}
