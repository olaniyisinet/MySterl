using Sterling.BaseLIB.Utility;
using Sterling.MSSQL;
using System;
using System.Data;

public class ProxyTables
{
    public int Id { get; set; }
    public string Phone { get; set; }
    public string Sender { get; set; }
    public string Text { get; set; }
    public DateTime InsertedAt { get; set; }
    public DateTime ProcessedAt { get; set; }
    public DateTime SentAt { get; set; }
    public DateTime DlrTimestamp { get; set; }
    public int DlrStatus { get; set; }
    public string DlrDescription { get; set; }
    public string Nuban { get; set; }
    public string Ledgercode { get; set; }
    public string Currency { get; set; }
    public string AansStatus { get; set; }


    public void Load()
    {
        string sql = @"SELECT id, phone, sender, text, inserted_at, processed_at, sent_at, dlr_timestamp, 
            dlr_status, dlr_description, nuban, ledgercode, currency, aans_status FROM proxytable WHERE id = @id";
        Connect cn = new Connect();
        cn.SetSQL(sql);
        cn.AddParam("@id", Id);

        DataSet ds = cn.Select();
        Id = 0;
        if (cn.num_rows > 0)
        {
            Set(ds.Tables[0].Rows[0]);
        }
    }

    public void Set(DataRow dr)
    {
        Id = Convert.ToInt32(dr["id"]);
        Phone = Convert.ToString(dr["phone"]);
        Sender = Convert.ToString(dr["sender"]);
        Text = Convert.ToString(dr["text"]);
        InsertedAt = Convert.ToDateTime(dr["inserted_at"]);
        ProcessedAt = Convert.ToDateTime(dr["processed_at"]);
        SentAt = Convert.ToDateTime(dr["sent_at"]);
        DlrTimestamp = Convert.ToDateTime(dr["dlr_timestamp"]);
        DlrStatus = Convert.ToInt32(dr["dlr_status"]);
        DlrDescription = Convert.ToString(dr["dlr_description"]);
        Nuban = Convert.ToString(dr["nuban"]);
        Ledgercode = Convert.ToString(dr["ledgercode"]);
        Currency = Convert.ToString(dr["currency"]);
        AansStatus = Convert.ToString(dr["aans_status"]);
    }
    public int Insert()
    {
        string sql = @"INSERT INTO proxytable (phone, sender, text, inserted_at,
                     nuban, ledgercode, currency)  
                     VALUES (@phone, @sender, @text, @inserted_at, 
                     @nuban, @ledgercode, @currency) ";
        Connect cn = new Connect();
        cn.SetSQL(sql);
        cn.AddParam("@phone", Phone);
        cn.AddParam("@sender", Sender);
        cn.AddParam("@text", Text);
        cn.AddParam("@inserted_at", InsertedAt);
        cn.AddParam("@nuban", Nuban);
        cn.AddParam("@ledgercode", Ledgercode);
        cn.AddParam("@currency", Currency);
        Id = Convert.ToInt32(cn.Insert());
   
		return Id;
	}

    public int Update()
    {
        string sql = "UPDATE proxytable SET phone = @phone, sender = @sender, text = @text, inserted_at = @inserted_at, processed_at = @processed_at, sent_at = @sent_at, dlr_timestamp = @dlr_timestamp, dlr_status = @dlr_status, dlr_description = @dlr_description, nuban = @nuban, ledgercode = @ledgercode, currency = @currency, aans_status = @aans_status WHERE id = @id";
        Connect cn = new Connect();
        cn.SetSQL(sql);
        cn.AddParam("@phone", Phone);
        cn.AddParam("@sender", Sender);
        cn.AddParam("@text", Text);
        cn.AddParam("@inserted_at", InsertedAt);
        cn.AddParam("@processed_at", ProcessedAt);
        cn.AddParam("@sent_at", SentAt);
        cn.AddParam("@dlr_timestamp", DlrTimestamp);
        cn.AddParam("@dlr_status", DlrStatus);
        cn.AddParam("@dlr_description", DlrDescription);
        cn.AddParam("@nuban", Nuban);
        cn.AddParam("@ledgercode", Ledgercode);
        cn.AddParam("@currency", Currency);
        cn.AddParam("@aans_status", AansStatus);
        cn.AddParam("@id", Id);
        return cn.Update();
    }

    public int Delete()
    {
        string sql = "DELETE FROM proxytable WHERE id = @id";
        Connect cn = new Connect();
        cn.SetSQL(sql);
        cn.AddParam("@id", Id);
        return cn.Delete();
    }

}