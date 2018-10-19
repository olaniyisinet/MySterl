using Sterling.MSSQL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Doubble.BLL.Utilities
{
    class Gadget
    {

        public DataTable bioData(string bvn)
        {
            string sql = "select TOP (1) * from ForSterlings where BVN = @bvn and Terminate is NULL";
            Connect c = new Connect("msconn");

            c.SetSQL(sql);
            c.AddParam("@bvn", bvn);
            return c.Select("rec").Tables[0];

        }

        public DataTable bioData2(string bvn)
        {
            string sql = "select TOP (1) * from ForNonSterlings where BVN = @bvn and Terminate is NULL";
            Connect c = new Connect("msconn");

            c.SetSQL(sql);
            c.AddParam("@bvn", bvn);
            return c.Select("rec").Tables[0];

        }

        public DataTable bioData3(string bvn)
        {
            string sql = "select TOP (1) * from For_Later where BVN = @bvn";
            Connect c = new Connect("msconn");

            c.SetSQL(sql);
            c.AddParam("@bvn", bvn);
            return c.Select("rec").Tables[0];

        }

        public DataTable bioData4(string bvn)
        {
            string sql = "select TOP (1) [FirstName] ,[LastName] ,[MobileNumber] ,[BVN] ,[Category] ,[PayInAccount] ,[BeneficiaryName] ,[BeneficiaryAccount] ,[DateOfEntry] ,[PayInAmount] ,[ReferenceID] ,[DAOCode] ,[BeneficiaryType] ,[Email] ,[Cusnum] ,[FullName] ,[InvestmentLetter] ,[ARRANGEMENT_ID] ,[Term] ,[EffectiveDate] ,[MaturityDate] ,[DateOfBirth] from ForSterlings where BVN = @bvn and Terminate is NULL";
            Connect c = new Connect("msconn");

            c.SetSQL(sql);
            c.AddParam("@bvn", bvn);
            return c.Select("rec").Tables[0];

        }

        public DataTable bioData5(string bvn)
        {
            string sql = "select TOP (1) [Title] ,[FirstName] ,[LastName] ,[Gender] ,[MobileNumber] ,[Email] ,[BVN] ,[DateOfBirth] ,[State] ,[HomeAddress] ,[Category] ,[DateOfEntry] ,[PayInAccount] ,[BeneficiaryName] ,[CusNum] ,[BeneficiaryAccount] ,[BeneficiaryType] ,[PayInAmount] ,[ReferenceID] ,[DAOCode] ,[Status_Flag] ,[InvestmentLetter] ,[ARRANGEMENT_ID] ,[FullName] ,[Term] ,[EffectiveDate] ,[MaturityDate] ,[ContactPerson] ,[ContactAddress] ,[ContactMobileNumber] from ForNonSterlings where BVN = @bvn and Terminate is NULL";
            Connect c = new Connect("msconn");

            c.SetSQL(sql);
            c.AddParam("@bvn", bvn);
            return c.Select("rec").Tables[0];

        }

        public DataTable bioData6(string ArrangementId)
        {
            string sql = "select [FirstName] ,[LastName] ,[MobileNumber] ,[BVN] ,[Category] ,[PayInAccount] ,[BeneficiaryName] ,[BeneficiaryAccount] ,[DateOfEntry] ,[PayInAmount] ,[ReferenceID] ,[DAOCode] ,[BeneficiaryType] ,[Email] ,[Cusnum] ,[FullName] ,[InvestmentLetter] ,[ARRANGEMENT_ID] ,[Term] ,[EffectiveDate] ,[MaturityDate] ,[DateOfBirth] from ForSterlings where ARRANGEMENT_ID = @ArrangementId";
            Connect c = new Connect("msconn");

            c.SetSQL(sql);
            c.AddParam("@ArrangementId", ArrangementId);
            return c.Select("rec").Tables[0];

        }

        public DataTable bioData7(string ArrangementId)
        {
            string sql = "select [Title] ,[FirstName] ,[LastName] ,[Gender] ,[MobileNumber] ,[Email] ,[BVN] ,[DateOfBirth] ,[State] ,[HomeAddress] ,[Category] ,[DateOfEntry] ,[PayInAccount] ,[BeneficiaryName] ,[CusNum] ,[BeneficiaryAccount] ,[BeneficiaryType] ,[PayInAmount] ,[ReferenceID] ,[DAOCode] ,[Status_Flag] ,[InvestmentLetter] ,[ARRANGEMENT_ID] ,[FullName] ,[Term] ,[EffectiveDate] ,[MaturityDate] ,[ContactPerson] ,[ContactAddress] ,[ContactMobileNumber] from ForNonSterlings where ARRANGEMENT_ID = @ArrangementId";
            Connect c = new Connect("msconn");

            c.SetSQL(sql);
            c.AddParam("@ArrangementId", ArrangementId);
            return c.Select("rec").Tables[0];

        }

        public DataTable bioDataPa(string bvn)
        {
            string sql = "select TOP (1) * from ForSterlings where BVN = @bvn AND Password != 'NULL'";
            Connect c = new Connect("msconn");
            c.SetSQL(sql);
            c.AddParam("@bvn", bvn);
            return c.Select("rec").Tables[0];

        }

        public DataTable bioDataPa2(string bvn)
        {
            string sql = "select TOP (1) * from ForNonSterlings where BVN = @bvn AND Password != 'NULL'";
            Connect c = new Connect("msconn");

            c.SetSQL(sql);
            c.AddParam("@bvn", bvn);
            return c.Select("rec").Tables[0];

        }

        public bool isAccountEligible(string led_code)
        {
            bool isValid = false;

            Connect c = new Connect("msconn");
            string query = "select [Description] from Allowed_Ledgers where Cat_Code = @led_code ";
            c.SetSQL(query);
            c.AddParam("@led_code", led_code);
            DataSet ds = c.Select("rec");
            DataTable dt = ds.Tables[0];

            if (dt.Rows.Count > 0)
            {
                isValid = true;
            }

            return isValid;
        }
    }
}
