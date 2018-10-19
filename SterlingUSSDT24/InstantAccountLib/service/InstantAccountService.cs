using com.sbp.instantacct.entity;
using Sterling.MSSQL;
using System;
using System.Data;

namespace com.sbp.instantacct.service
{
    public class InstantAccountService
    {
        public static InstantAccount GetById(int Id)
        {
            string sql = @"SELECT Id, SessionId, Mobile,BVN, IsCustomer, ExistingAccountNumber,  
Statusflag, DateAdded, DateUpdated, CustomerId, AccountNumber, DateOpened, ApprovedBy, DateApproved 
FROM tbl_acct_requests WHERE Id = @Id";
            Connect cn = new Connect("instantaccountdb");
            cn.SetSQL(sql);
            cn.AddParam("@Id", Id);
            var item = new InstantAccount();
            item.Id = 0;
            DataSet ds = cn.Select();
            if (cn.num_rows > 0)
            {
                item.Set(ds.Tables[0].Rows[0]);
            }
            return item;
        }

        public static int Insert(InstantAccount item)
        {
            string sql = @"INSERT INTO tbl_acct_requests 
(SessionId, Mobile,BVN, IsCustomer, ExistingAccountNumber, Statusflag, DateAdded, DateUpdated, 
CustomerId, AccountNumber, DateOpened, ApprovedBy, DateApproved)  
VALUES ( @SessionId, @Mobile,@BVN, @IsCustomer, @ExistingAccountNumber, @Statusflag, @DateAdded, 
@DateUpdated, @CustomerId, @AccountNumber, @DateOpened, @ApprovedBy, @DateApproved) ";
            Connect cn = new Connect("instantaccountdb");
            cn.SetSQL(sql);
            cn.AddParam("@SessionId", item.SessionId);
            cn.AddParam("@Mobile", item.Mobile);
            cn.AddParam("@BVN", item.BVN);
            cn.AddParam("@IsCustomer", item.IsCustomer);
            cn.AddParam("@ExistingAccountNumber", item.ExistingAccountNumber);
            cn.AddParam("@Statusflag", item.Statusflag);
            cn.AddParam("@DateAdded", item.DateAdded);
            cn.AddParam("@DateUpdated", item.DateUpdated);
            cn.AddParam("@CustomerId", item.CustomerId);
            cn.AddParam("@AccountNumber", item.AccountNumber);
            cn.AddParam("@DateOpened", item.DateOpened);
            cn.AddParam("@ApprovedBy", item.ApprovedBy);
            cn.AddParam("@DateApproved", item.DateApproved);
            item.Id = Convert.ToInt32(cn.Insert());
            return item.Id;
        }

        public static int Update(InstantAccount item)
        {
            string sql = @"UPDATE tbl_acct_requests SET SessionId = @SessionId, Mobile = @Mobile, BVN = @BVN, IsCustomer = @IsCustomer, 
ExistingAccountNumber = @ExistingAccountNumber, Statusflag = @Statusflag, DateAdded = @DateAdded, DateUpdated = @DateUpdated, 
CustomerId = @CustomerId, AccountNumber = @AccountNumber, DateOpened = @DateOpened, ApprovedBy = @ApprovedBy, 
DateApproved = @DateApproved WHERE Id = @Id";

            Connect cn = new Connect("instantaccountdb");
            cn.SetSQL(sql);
            cn.AddParam("@SessionId", item.SessionId);
            cn.AddParam("@Mobile", item.Mobile);
            cn.AddParam("@BVN", item.BVN);
            cn.AddParam("@IsCustomer", item.IsCustomer);
            cn.AddParam("@ExistingAccountNumber", item.ExistingAccountNumber);
            cn.AddParam("@Statusflag", item.Statusflag);
            cn.AddParam("@DateAdded", item.DateAdded);
            cn.AddParam("@DateUpdated", item.DateUpdated);
            cn.AddParam("@CustomerId", item.CustomerId);
            cn.AddParam("@AccountNumber", item.AccountNumber);
            cn.AddParam("@DateOpened", item.DateOpened);
            cn.AddParam("@ApprovedBy", item.ApprovedBy);
            cn.AddParam("@DateApproved", item.DateApproved);
            cn.AddParam("@Id", item.Id);
            return cn.Update();
        }

        public static int Delete(InstantAccount item)
        {
            string sql = "DELETE FROM tbl_acct_requests WHERE Id = @Id";
            Connect cn = new Connect();
            cn.SetSQL(sql);
            cn.AddParam("@Id", item.Id);
            return cn.Delete();
        }
    }
}
