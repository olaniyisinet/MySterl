using Doubble.DAL.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sterling;
using Sterling.MSSQL;

namespace Doubble.DAL
{
    public class DoubbleDalService
    {
        private readonly DoubbleDBEntities db = new DoubbleDBEntities();
        public void saveDoubble(string FirstName, string LastName, string BVN, string PhoneNo, string Email, DateTime DateOfBirth, string VerStatus, string Valcode)
        {
            var doubb = new DoubbleRequest { FirstName = FirstName, BVN = BVN, DateOfBirth = DateOfBirth, Email = Email, DateOfEntry = DateTime.Now, LastName = LastName, MobileNumber = PhoneNo, SterlingVerified = VerStatus, ValCode = Valcode};
            db.DoubbleRequests.Add(doubb);
            db.SaveChanges();
        }

        public int getCategoryTerm(string category)
        {
            string sql = "select Term from Categories where CategoryName = @cat";
            Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect("msconn");
            c.SetSQL(sql);
            c.AddParam("@cat", category);
            DataTable dt = c.Select("rec").Tables[0];
            return Convert.ToInt16(dt.Rows[0]["Term"]);
        }

        public void SaveForLater(string FirstName, string LastName, string MobileNumber, string BVN, string Category, string PayInAccount, string BeneficiaryName, string BeneficiaryAccount, DateTime DateOfEntry, string PayInAmount, string DAOCode, string BeneficiaryType, string Email, string Cusnum, DateTime ed, string cust, int term, string valCode, string fullName, DateTime DateOfBirth)
        {
            var lat = new For_Later { BeneficiaryAccount = BeneficiaryAccount, BeneficiaryName = BeneficiaryName, BeneficiaryType = BeneficiaryType, BVN = BVN, Category = Category, Cusnum = Cusnum, CusType = cust, DAOCode = DAOCode, DateOfBirth = DateOfBirth, DateOfEntry = DateTime.Now, EffectiveDate = ed, Email = Email, FirstName = FirstName, FullName = fullName, LastName = LastName, MobileNumber = MobileNumber, PayInAccount = PayInAccount, PayInAmount = PayInAmount, Term = term, ValCode = valCode };
            db.For_Later.Add(lat);
            db.SaveChanges();
        }

        public void insertForSterling(string FirstName, string LastName, string MobileNumber, string BVN, string Category, string PayInAccount, string BeneficiaryName, string BeneficiaryAccount, DateTime DateOfEntry, string PayInAmount, string ReferenceID, string DAOCode, string BeneficiaryType, string Email, string Cusnum, string FullName, string pwd, string arrangementId, int term, DateTime effDate, DateTime matDate, DateTime DateOfBirth)
        {
            var forSter = new ForSterling { ARRANGEMENT_ID = arrangementId, BeneficiaryAccount = BeneficiaryAccount, BeneficiaryName = BeneficiaryName, BeneficiaryType = BeneficiaryType, BVN = BVN, Category = Category, Cusnum = Cusnum, DAOCode = DAOCode, DateOfBirth = DateOfBirth, DateOfEntry = DateTime.Now, EffectiveDate = effDate, Email = Email, FirstName = FirstName, FullName = FullName, LastName = LastName, MaturityDate = matDate, MobileNumber = MobileNumber, Password = pwd, PayInAccount = PayInAccount, PayInAmount = PayInAmount, ReferenceID = ReferenceID, Term = term };
            db.ForSterlings.Add(forSter);
            db.SaveChanges();
        }
        public void insertForSterlingLat(string FirstName, string LastName, string MobileNumber, string BVN, string Category, string PayInAccount, string BeneficiaryName, string BeneficiaryAccount, DateTime DateOfEntry, string PayInAmount, string ReferenceID, string DAOCode, string BeneficiaryType, string Email, string Cusnum, string FullName, string pwd, string arrangementId, int term, DateTime effDate, DateTime matDate, DateTime DateOfBirth, string verCode)
        {
            var forSter = new ForSterling { ARRANGEMENT_ID = arrangementId, BeneficiaryAccount = BeneficiaryAccount, BeneficiaryName = BeneficiaryName, BeneficiaryType = BeneficiaryType, BVN = BVN, Category = Category, Cusnum = Cusnum, DAOCode = DAOCode, DateOfBirth = DateOfBirth, DateOfEntry = DateTime.Now, EffectiveDate = effDate, Email = Email, FirstName = FirstName, FullName = FullName, LastName = LastName, MaturityDate = matDate, MobileNumber = MobileNumber, Password = pwd, PayInAccount = PayInAccount, PayInAmount = PayInAmount, ReferenceID = ReferenceID, Term = term, ValCode =verCode };
            db.ForSterlings.Add(forSter);
            db.SaveChanges();
        }

        public void StoreToForNonSterlingTbl(string fName, string lName, string mail, string mob_num, string bvn, DateTime dob, DateTime doe, string title, string gender, string state, string homeAddress, string cusnum, string payInAcct, string valC)
        {
            var nonSter = new ForNonSterling { PayInAccount = payInAcct, BVN = bvn, CusNum = cusnum, DateOfBirth = dob, DateOfEntry = DateTime.Today, Email = mail, FirstName = fName, Gender = gender, HomeAddress = homeAddress, LastName = lName, MobileNumber = mob_num, State = state, FullName = lName + " " + fName, Title = title, ValCode = valC };
            db.ForNonSterlings.Add(nonSter);
            db.SaveChanges();
        }

        public void StoreToForNonSterlingDisapora(string fName, string lName, string mail, string mob_num, string bvn, DateTime dob, DateTime doe, string title, string gender, string state, string homeAddress, string cusnum, string payInAcct, string valC, string contactName, string contactAddress, string contactMobNo)
        {
            var nonSter = new ForNonSterling { PayInAccount = payInAcct, BVN = bvn, CusNum = cusnum, DateOfBirth = dob, DateOfEntry = DateTime.Today, Email = mail, FirstName = fName, Gender = gender, HomeAddress = homeAddress, LastName = lName, MobileNumber = mob_num, State = state, FullName = lName + " " + fName, Title = title, ValCode = valC, ContactPerson = contactName, ContactAddress = contactAddress, ContactMobileNumber = contactMobNo };
            db.ForNonSterlings.Add(nonSter);
            db.SaveChanges();
        }

        public void updateForNonSterling(string categ, string amt, string bName, string bAcc, string bAccType, string refId, string daoCode, string verCode, string arrangementId, int term, DateTime effDate, DateTime matDate, string pwd)
        {
            string sql = "update ForNonSterlings set Category=@categ,PayInAmount=@amt,BeneficiaryName=@bName,BeneficiaryAccount=@bAcc,BeneficiaryType=@bAccType,ReferenceID=@refId,DAOCode=@daoCode,ARRANGEMENT_ID=@arrangementId,Password=@pwd,Term=@term,EffectiveDate=@effDate,MaturityDate=@matDate where ValCode=@verCode";
            Connect c = new Connect("msconn");
            c.SetSQL(sql);
            c.AddParam("@categ", categ);
            c.AddParam("@amt", amt);
            c.AddParam("@bName", bName);
            c.AddParam("@bAcc", bAcc);
            c.AddParam("@bAccType", bAccType);
            c.AddParam("@refId", refId);
            c.AddParam("@daoCode", daoCode);
            c.AddParam("@verCode", verCode);
            c.AddParam("@arrangementId", arrangementId);
            c.AddParam("@term", term);
            c.AddParam("@effDate", effDate);
            c.AddParam("@matDate", matDate);
            c.AddParam("@pwd", pwd);
            c.Select("rec");
            //Mylogger.Info("Record for BVN, " + refId + " was saved to ForNonSterlings table completely.");
        }

        public void updateBeneficiary(string arrangementId, string beneficiaryAcct, string beneficiaryType, string beneficiaryName)
        {
            try
            {
                string sql = "update ForNonSterlings set BeneficiaryName=@bName, BeneficiaryAccount=@bAcc, BeneficiaryType=@bAccType where ARRANGEMENT_ID=@arrangementId";
                Connect c = new Connect("msconn");
                c.SetSQL(sql);
                c.AddParam("@bAcc", beneficiaryAcct);
                c.AddParam("@bAccType", beneficiaryType);
                c.AddParam("@bName", beneficiaryName);
                c.AddParam("@arrangementId", arrangementId);
                c.Select("rec");
            }
            catch (Exception e)
            {

                
            }
            try
            {
                string sql = "update ForSterlings set BeneficiaryName=@bName, BeneficiaryAccount=@bAcc, BeneficiaryType=@bAccType where ARRANGEMENT_ID=@arrangementId";
                Connect c = new Connect("msconn");
                c.SetSQL(sql);
                c.AddParam("@bAcc", beneficiaryAcct);
                c.AddParam("@bAccType", beneficiaryType);
                c.AddParam("@bName", beneficiaryName);
                c.AddParam("@arrangementId", arrangementId);
                c.Select("rec");
            }
            catch (Exception e)
            {


            }
        }

        public void Terminate(string arrangementId)
        {
            try
            {
                string sql = "update ForNonSterlings set Terminate='Terminate' where ARRANGEMENT_ID=@arrangementId";
                Connect c = new Connect("msconn");
                c.SetSQL(sql);
                c.AddParam("@arrangementId", arrangementId);
                c.Select("rec");
            }
            catch (Exception e)
            {


            }
            try
            {
                string sql = "update ForSterlings set Terminate='Terminate' where ARRANGEMENT_ID=@arrangementId";
                Connect c = new Connect("msconn");
                c.SetSQL(sql);
                c.AddParam("@arrangementId", arrangementId);
                c.Select("rec");
            }
            catch (Exception e)
            {


            }
        }

        

    }
}
