using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sterling.BaseLIB;
using Sterling.MSSQL;
using System.Data;
using PlentyMoney.sGTConnect;

namespace PlentyMoney
{

    public class AppConnection
    {
        public static string GetmssqlConnection()
        {
            return "mssqlCardDB";
        }        
    }

    public class PlentyRequestService
    {
        public static DataSet GetSteps(string SessionId, string Misdn)
        {
            string sql = " select * from tbl_USSD_reqstate where SessionID = @SessionId AND Msisdn = @Msisdn order by refID desc select * from tbl_USSD_Accounts ";
            Sterling.MSSQL.Connect cn = new Sterling.MSSQL.Connect();
            cn.SetSQL(sql);
            cn.AddParam("@SessionId", SessionId);
            cn.AddParam("@Msisdn", Misdn);
            DataSet ds = cn.Select();
            return ds;//.Tables[0];
        }

        public static int InsertMFINO(mFinoInfo mfino)
        {
          string sql = " BEGIN TRANSACTION   BEGIN TRY  DECLARE @transferID varchar(50) DECLARE @parentTxnID varchar(50)   DECLARE @mCount int "
                      +" SELECT @transferID = [transferID], @parentTxnID = [parentTxnID]  FROM [dbo].[tbl_USSD_reqstate] "
                      + " WHERE [sessionid] = @SessionId and [msisdn] = @Misdn and [AppID] = @AppID "
                      +" INSERT INTO [dbo].[mFinoRequestLog] ( [SessionID],[Misdn] ,[SourceMdn],[DestMdn],[Amount],[transferID],[ParentTxn],[Remark],[TxnDate],[MsgCode])  VALUES  "
                      + " (@SessionID,@Misdn ,@SourceMdn,@DestMdn,@Amount,@transferID,@parentTxnID,@Remark,GETDATE(),@MsgCode)  set @mCount = @@ROWCOUNT "
                     // +" IF(@transferID IS NOT NULL AND @parentTxnID IS NOT NULL AND @mCount = 1) "
                     + " IF(@mCount = 1) "
                      +" begin  COMMIT select 100 end ELSE begin ROLLBACK select 88 end "
                      +" END TRY BEGIN CATCH ROLLBACK select 99 END CATCH ";

            Sterling.MSSQL.Connect cn = new Sterling.MSSQL.Connect();
            cn.SetSQL(sql);
            cn.AddParam("@SessionId", mfino.SessionID);
            cn.AddParam("@Misdn", mfino.Misdn);
            cn.AddParam("@SourceMdn", mfino.SourceMdn);
            cn.AddParam("@DestMdn", mfino.DestMdn);
            cn.AddParam("@Amount", mfino.Amount);
            cn.AddParam("@Remark", mfino.Remark);
            cn.AddParam("@MsgCode", mfino.MsgCode);
            cn.AddParam("@AppID", 2);
            DataSet ds = cn.Select();
            return Convert.ToInt32( ds.Tables[0].Rows[0][0]);
        }

        public static int lottoSaveSelf(string SessionId, string Misdn,string NoOfBet, string[] numbers,string pFlag,string amount)
        {
            string numb = string.Format("{0},{1},{2},{3},{4},{5}", numbers[0],numbers[1],numbers[2],numbers[3],numbers[4],numbers[5]);
    
            string sql =  "  Insert Into [dbo].[LottoRequestLog]  "
                        + " ([SessionID],[Misdn],[BetType],[PaymentType],[NoOfPanel],[PosID],[SourceID],[pFlag],[Amount],[PanelData],[TxnDate],IsProcessed) Values  "
                        + " (@SessionID,@Misdn,@BetType,@PaymentType,@NoOfPanel,@PosID,@SourceID,@pFlag,@Amount,@PanelData,GetDate(),@IsProcessed) Select @@ROWCOUNT  ";

            Sterling.MSSQL.Connect cn = new Sterling.MSSQL.Connect();
            cn.SetSQL(sql);
            cn.AddParam("@SessionId", SessionId);
            cn.AddParam("@Misdn", Misdn);
            cn.AddParam("@BetType", Constants.betType_Self);
            cn.AddParam("@PaymentType", Constants.p_paymentType);
            cn.AddParam("@NoOfPanel", NoOfBet);
            cn.AddParam("@pFlag", pFlag);
            cn.AddParam("@PanelData", numb);
            cn.AddParam("@Amount", amount);
            cn.AddParam("@PosID", Misdn);
            cn.AddParam("@IsProcessed", 0);
            cn.AddParam("@SourceID", Constants.p_sourceId);

            DataSet ds = cn.Select();
            return Convert.ToInt32(ds.Tables[0].Rows[0][0]);
        }

        public static int lottoSaveQuick(string SessionId, string Misdn, string NoOfBet,string pFlag, string amount)
        {

            string sql  = "  Insert Into [dbo].[LottoRequestLog]  "
                        + " ([SessionID],[Misdn],[BetType],[PaymentType],[NoOfBet],[PosID],[SourceID],[pFlag],[Amount],[TxnDate],IsProcessed) Values "
                        + " (@SessionID,@Misdn,@BetType,@PaymentType,@NoOfBet,@PosID,@SourceID,@pFlag,@Amount,GetDate(),@IsProcessed) Select @@ROWCOUNT  ";

            Sterling.MSSQL.Connect cn = new Sterling.MSSQL.Connect();
            cn.SetSQL(sql);
            cn.AddParam("@SessionId", SessionId);
            cn.AddParam("@Misdn", Misdn);
            cn.AddParam("@BetType", Constants.betType_Self);
            cn.AddParam("@PanelData", Constants.p_gameID);
            cn.AddParam("@PaymentType", Constants.p_paymentType);
            cn.AddParam("@NoOfBet", NoOfBet);
            cn.AddParam("@pFlag", pFlag);
            cn.AddParam("@Amount", amount);
            cn.AddParam("@PosID", Misdn);
            cn.AddParam("@IsProcessed", 0);
            cn.AddParam("@SourceID", Constants.p_sourceId);
            DataSet ds = cn.Select();
            return Convert.ToInt32(ds.Tables[0].Rows[0][0]);
        }

        public static int UpdateLottoSelf(string sessionId, string Misdn, string panelData, string remark, WSLottoGameRequest req, WSLottoGameResponse res)
        {
            string sql = "  Update [dbo].[LottoRequestLog] "
                          + "  Set [Remark] =  @Remark,[PanelData]= @PanelData, [TicketNo] =  @TicketNo, [pFlag] = @pFlag,"
                          + " [startDrawDate] = @startDrawDate, startDrawNo = @startDrawNo, "
                          + "  [EndDrawDate] = @EndDrawDate, [EndDrawNo] = @EndDrawNo "
                          + "  Where [SessionID] =  @SessionID AND [Misdn] =  @Misdn  Select @@rowcount";

            Sterling.MSSQL.Connect cn = new Sterling.MSSQL.Connect();
            cn.SetSQL(sql);
            cn.AddParam("@SessionId", sessionId);
            cn.AddParam("@Misdn", Misdn);
            cn.AddParam("@PanelData", panelData);
            cn.AddParam("@TicketNo", res.ticketSerialNumber);
            cn.AddParam("@pFlag", Constants.success);
            cn.AddParam("@Amount", res.ticketCost);
            cn.AddParam("@startDrawDate", res.startDrawDate);
            cn.AddParam("@startDrawNo", res.startDrawNumber);
            cn.AddParam("@EndDrawDate", res.endDrawDate);
            cn.AddParam("@EndDrawNo", res.endDrawNumber);
            cn.AddParam("@Remark", remark);
            DataSet ds = cn.Select();
            return Convert.ToInt32(ds.Tables[0].Rows[0][0]);
        }

        public static int UpdateLottoQuick(string sessionId, string Misdn, string remark, string panelData, WSLottoGameRequest req, WSLottoGameResponse res)
        {
            string sql = "  Update [dbo].[LottoRequestLog] "
                          + "  Set [Remark] =  @Remark,[PanelData]= @PanelData, [TicketNo] =  @TicketNo, [AdvanceDraw] =@AdvanceDraw,  [pFlag] = @pFlag,"
                          + "  [PosID] = @PosID,[startDrawDate] = @startDrawDate, "
                          + "  [EndDrawDate] = @EndDrawDate, [EndDrawNo] = @EndDrawNo "
                          + "  Where [SessionID] =  @SessionID AND [Misdn] =  @Misdn  Select @@rowcount";

            Sterling.MSSQL.Connect cn = new Sterling.MSSQL.Connect();
            cn.SetSQL(sql);
            cn.AddParam("@SessionID", sessionId);
            cn.AddParam("@Misdn", Misdn);
            cn.AddParam("@PanelData", panelData);
            cn.AddParam("@TicketNo", res.ticketSerialNumber);
            cn.AddParam("@PaymentType", Constants.p_paymentType);
            cn.AddParam("@PosID", Misdn);
            cn.AddParam("@pFlag", Constants.success);
            cn.AddParam("@Amount", res.ticketCost);
            cn.AddParam("@startDrawDate", res.startDrawDate);
            cn.AddParam("@startDrawNo", res.startDrawNumber);
            cn.AddParam("@EndDrawDate", res.endDrawDate );
            cn.AddParam("@EndDrawNo", res.endDrawNumber);
            cn.AddParam("@AdvanceDraw", Constants.p_advanceDraws);
            cn.AddParam("@Remark", remark);
            DataSet ds = cn.Select();
            return Convert.ToInt32(ds.Tables[0].Rows[0][0]);
        }

        public static int UpdateLottoOnError(string sessionId, string Misdn, string remark,string flag)
        {

            string sql  =   "  Update [dbo].[LottoRequestLog] "
                          + "  Set [Remark] =  @Remark,  [pFlag] = @pFlag "
                          + "  Where [SessionID] =  @SessionID AND [Misdn] =  @Misdn  Select @@rowcount";
            Sterling.MSSQL.Connect cn = new Sterling.MSSQL.Connect();
            cn.SetSQL(sql);
            cn.AddParam("@SessionId", sessionId);
            cn.AddParam("@Misdn", Misdn);
            cn.AddParam("@pFlag",flag);
            cn.AddParam("@Remark", remark);
            DataSet ds = cn.Select();
            return Convert.ToInt32(ds.Tables[0].Rows[0][0]);

        }

        public static DataTable GetAccounts(string Code)
        {
            string sql = " select * from tbl_USSD_Accounts where Code = @Code";
            Sterling.MSSQL.Connect cn = new Sterling.MSSQL.Connect();
            cn.SetSQL(sql);
            cn.AddParam("@Code", Code);
            DataSet ds = cn.Select();
            return ds.Tables[0];
        }

    }
}