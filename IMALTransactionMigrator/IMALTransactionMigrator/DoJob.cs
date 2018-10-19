using Sterling.BaseLIB.Utility;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMALTransactionMigrator
{
    class DoJob
    {
        private static string SqlConnStr = ConfigurationSettings.AppSettings["mssqlconn"].ToString();
        public static void doJobs()
        {
            //doUpdateOrac(2887);
            //Console.WriteLine("Done.");


            //                     var sql = @"select B.CODE,B.SUB_ID,b.PHONE_NO as Phone,'STERLING' as sender,b.MSG_TEXT AS text, SYSDATE as inserted_at,'' as processed_at,
            // '' as sent_at,'' as dlr_timestamp,'' as dlr_status,'' as dlr_description ,A.ADDITIONAL_REFERENCE as nuban,
            // A.GL_CODE AS ledgercode,C.BRIEF_DESC_ENG AS currency, '' as aans_status
            //   FROM IMAL.AMF A
            //   inner join (select dof.comp_code,dof.currency_code, dof.branch_code, dof.cif_sub_no, dof.gl_code,dof.sl_no, msg.code from imal.sms_messages msg
            //    inner join imal.alrt_sub sub on msg.sub_id=sub.id 
            //    inner join imal.dof dof on sub.cif_no=dof.cif_sub_no and msg.trx_op_no=dof.op_no ) h 
            //on A.COMP_CODE=h.COMP_CODE 
            // AND A.BRANCH_CODE=h.BRANCH_CODE
            // AND A.CURRENCY_CODE=h.CURRENCY_CODE
            // AND A.GL_CODE=h.GL_CODE
            // AND A.CIF_SUB_NO=h.CIF_SUB_NO
            // AND A.SL_NO=h.SL_NO
            // inner join IMAL.SMS_MESSAGES b on h.code=b.code
            // inner join imal.currencies c on h.currency_code=c.currency_code
            // where b.PROCESSED=0
            //--and c.COMP_CODE=1 ";

            var sql = @"SELECT MSG.CODE,
                   MSG.SUB_ID,
                   MSG.PHONE_NO AS PHONE,
                   'STERLING' AS SENDER,
                   MSG.MSG_TEXT AS TEXT,
                   SYSDATE AS INSERTED_AT,
                   '' AS PROCESSED_AT,
                   '' AS SENT_AT,
                   '' AS DLR_TIMESTAMP,
                   '' AS DLR_STATUS,
                   '' AS DLR_DESCRIPTION,
                   AMF.ADDITIONAL_REFERENCE AS NUBAN,
                   AMF.GL_CODE AS LEDGERCODE,
                   CUR.BRIEF_DESC_ENG AS CURRENCY,
                   '' AS AANS_STATUS,
                   DOF.DESCRIPTION2_ARAB,
                   MSG.MSG_CODE
              FROM IMAL.SMS_MESSAGES MSG
             INNER JOIN IMAL.ALRT_SUB SUB ON MSG.SUB_ID = SUB.ID
             INNER JOIN IMAL.DOF DOF ON (SUB.CIF_NO = DOF.CIF_SUB_NO OR SUB.CIF_SUB_NO = DOF.CIF_SUB_NO)
                                       AND MSG.TRX_OP_NO = DOF.OP_NO
             INNER JOIN IMAL.AMF AMF ON AMF.COMP_CODE = DOF.COMP_CODE
                                       AND AMF.BRANCH_CODE = DOF.BRANCH_CODE
                                       AND AMF.CURRENCY_CODE = DOF.CURRENCY_CODE
                                       AND AMF.GL_CODE = DOF.GL_CODE
                                       AND AMF.CIF_SUB_NO = DOF.CIF_SUB_NO
                                       AND AMF.SL_NO = DOF.SL_NO
             INNER JOIN IMAL.CURRENCIES CUR ON DOF.CURRENCY_CODE = CUR.CURRENCY_CODE
             WHERE MSG.PROCESSED = 0
               AND CUR.COMP_CODE = 1";

            Sterling.Oracle.Connect orac = new Sterling.Oracle.Connect();
            orac.SetSQL(sql);
            DataSet ds = new DataSet("proxytable");
            ds = orac.Select();
            if (!string.IsNullOrEmpty(orac.errmsg))
            {
                Console.WriteLine(orac.errmsg);
                Console.ReadLine();
            }

            if (ds != null)
            {
                if (ds.Tables[0].Rows.Count != 0)
                {
                    if (ds.Tables[0].Rows.Count != 0)
                    {
                        int nrow = ds.Tables[0].Rows.Count;
                        var MessagePartition = Partitioner.Create(ds.Tables[0].AsEnumerable());
                        Parallel.ForEach(MessagePartition, row =>
                        {
                            ProxyTables st = new ProxyTables();
                            st.Phone = row["Phone"].ToString().Trim();
                            st.Sender = row["sender"].ToString().Trim();
                           // st.Text = row["text"].ToString().Trim();
                            st.Text = new Modify().ModifyTextColumn(row["text"].ToString().Trim(), row["DESCRIPTION2_ARAB"].ToString().Trim(), row["MSG_CODE"].ToString().Trim());
                            st.InsertedAt = DateTime.Now;
                            st.Nuban = row["nuban"].ToString().Trim();
                            st.Ledgercode = row["ledgercode"].ToString().Trim();
                            st.Currency = row["currency"].ToString().Trim();
                            int retInsert = st.Insert();
                            Console.WriteLine("");
                            Console.WriteLine("Inserted record " + st.Nuban);
                        });
                    }
                }
            }
			int val = 0;
			foreach (DataRow r in ds.Tables[0].Rows)
			{
			    val = Convert.ToInt32(r["CODE"].ToString());
			    doUpdateOrac(val);
			}
		}

		private static int doUpdateOrac(int recID)
        {
            int ret = 0;
            try
            {
                var sql = "update imal.SMS_MESSAGES set PROCESSED=1,processed_date = sysdate where CODE='"+ recID + "'";
                Sterling.Oracle.Connect orac = new Sterling.Oracle.Connect();
                orac.SetSQL(sql);
                
                ret = orac.Update();
                if (!string.IsNullOrEmpty(orac.errmsg))
                {
                   new ErrorLog(orac.errmsg);
                }
			//	Console.WriteLine(recID + " Updated");
            }
            catch (Exception ex)
            {
                new ErrorLog(ex);
            }
            return ret;
        }
    }
}
