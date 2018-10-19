using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using PlentyMoney.sGTConnect;//.GTConnect;
using System.Net;
using MMWS;
using System.Diagnostics;
using System.Web;
using System.Security.Cryptography.X509Certificates;
using System.IO;
using System.Net.Security;


namespace PlentyMoney
{
    public class PlentyPostService
    {
        public static int msgType = 3;

        public static string PostPlentyMoney(string SessionId, string Misdn)
        {
            string resp = string.Empty;
            string destWallet = string.Empty;
            try
            {
                DataSet ds = PlentyRequestService.GetSteps(SessionId, Misdn);
                DataTable dtVal = ds.Tables[0];
                DataTable dtConfig = ds.Tables[1];

                var Qsuspense = (from p in dtConfig.AsEnumerable()
                                 where p.Field<int>("code") == 100
                                 select new
                                 {
                                     var1 = p.Field<string>("var1"),
                                     var2 = p.Field<string>("var2"),
                                     var3 = p.Field<string>("var3")
                                 }).FirstOrDefault();

                var configQuery = (from p in dtConfig.AsEnumerable()
                                 where p.Field<int>("code") == 300
                                 select new
                                 {
                                     var1 = p.Field<string>("var1"),//QlotteryAmt
                                     var2 = p.Field<string>("var2"),//QauthKey
                                     var3 = p.Field<string>("var3")//QcertPath
                                 }).FirstOrDefault();

                var configMfino = (from p in dtConfig.AsEnumerable()
                                   where p.Field<int>("code") == 400
                                   select new
                                   {
                                       var1 = p.Field<string>("var1"),//ChannelID
                                       var2 = p.Field<string>("var2"),//InstitutionID
                                   }).FirstOrDefault();

                if (dtVal.Rows.Count > 0)
                {
                    string detval = dtVal.Rows[0]["params"].ToString();
                    string[] detarray = detval.Split('|');
                    string playType = detarray[0];
                    string sequence = detarray[1].Trim().TrimStart().TrimEnd();
                    string pin = detarray[2];
                    int repeatcounter = 0;
                    int numericCounter = 0;
                    int rangecounter = 0;

                    if (playType == "1")
                    {
                        int n;
                        bool isNumeric = int.TryParse(sequence, out n);
                        if (isNumeric)
                        {
                            int seq = Convert.ToInt32(sequence);
                            if (seq > 0 && seq <= 2)
                            {
                                mFinoInfo mf = new mFinoInfo();
                                mf.Amount = (Convert.ToInt32(seq) * Convert.ToInt32(configQuery.var1)).ToString();
                                mf.DestMdn = Qsuspense.var1;
                                mf.mFlag = "0";
                                mf.Misdn = Misdn;
                                mf.PIN = pin;
                                mf.SessionID = SessionId;
                                mf.SourceMdn = Misdn;
                                mf.AuthKey = configQuery.var2;
                                mf.plentyCertPath = configQuery.var3;
                                mf.InstitutionID = configMfino.var2;
                                mf.ChannelID = configMfino.var1;

                                resp = PostPayRequest(mf,Convert.ToInt32(playType), null, seq, Misdn);
                            }
                            else
                            {
                                resp = "Max. allowable bet is 2!";
                            }
                        }
                        else
                        {
                            // Not numeric
                            resp = "Lottery numbers must be numeric!";
                        }
                    }

                    else if (playType == "2")//self pick
                    {
                        //validate sequence count (most be 6),
                        string[] splitSeq = sequence.Split(',');
                        if (splitSeq.Length == 6)
                        {
                            //check for repetition
                            foreach (string x in splitSeq)
                            {
                                if (x != ",")
                                {
                                    int count = splitSeq.AsEnumerable().Count(c => c == x);
                                    if (count > 1)
                                    {
                                        repeatcounter = repeatcounter + 1;
                                    }
                                }
                            }

                            //check for valid numeric character
                            foreach (string x in splitSeq)
                            {
                                if (x != ",")
                                {
                                    int n;
                                    bool isNumeric = int.TryParse(x.ToString(), out n);
                                    if (!isNumeric)
                                    {
                                        numericCounter = numericCounter + 1;
                                    }
                                }
                            }

                            //valid 1 - 40 ?
                            foreach (string x in splitSeq)
                            {
                                if (x != ",")
                                {
                                    int xi = Convert.ToInt16(x);
                                    if (xi < 1 || xi > 40)
                                    {
                                        rangecounter = rangecounter + 1;
                                    }
                                }
                            }

                            if (repeatcounter == 0)// good to go
                            {
                                //valid numerics
                                if (numericCounter == 0)
                                {
                                    if (rangecounter == 0)
                                    {
                                        mFinoInfo mf = new mFinoInfo();
                                        mf.Amount = configQuery.var1;
                                        mf.DestMdn = Qsuspense.var1;
                                        mf.AuthKey = configQuery.var2;
                                        mf.mFlag = "0";
                                        mf.Misdn = Misdn;
                                        mf.PIN = pin;
                                        mf.SessionID = SessionId;
                                        mf.SourceMdn = Misdn;
                                        mf.plentyCertPath = configQuery.var3;
                                        resp = PostPayRequest(mf, Convert.ToInt32(playType), splitSeq, 0, Misdn);
                                    }
                                    else
                                    {
                                        //Not in Range
                                        resp = "Lottery numbers must be between 1 and 40 insclusive!";
                                    }
                                }
                                else
                                {
                                    // Not numeric
                                    resp = "Lottery numbers must be numeric!";
                                }
                            }
                            else  //Repetition detected
                            {
                                resp = "Lottery numbers must not be repeated!";
                            }
                        }
                        else
                        {
                            //Unknow number of sequence
                            resp = "Lottery numbers must be six (6)";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                resp = ex.Message +" Request cannot be completed at this time!";
            }
            return resp;
        }

        public static string DoSelf(string[] splitSeq, string Misdn, string sessionId, mFinoInfo mFino)
        {
            string resp = "";
            try
            {
                Array.Sort(splitSeq);
                int val = PlentyRequestService.lottoSaveSelf(sessionId, Misdn, "1", splitSeq, Constants.pending,mFino.Amount);
                if (val == 1)
                {
                    //WebRequest.DefaultWebProxy = new WebProxy("10.0.0.120", 80)
                    //{
                    //    Credentials = new NetworkCredential("ibsservice", "Sterling123")
                    //};

                    WSHeaderMessage hmsg = new WSHeaderMessage();
                    hmsg.channelId = Constants.p_channelId;
                    hmsg.currentTime = DateTime.Now;
                    hmsg.messageId = Constants.p_messageId;
                    hmsg.posId = Misdn;
                    hmsg.sourceId = Constants.p_sourceId;
                    hmsg.extRefId = sessionId;
                    hmsg.applicationVersion = 1.0;

                    //Self Pick Panel
                    LottoRequestPanelData data = new LottoRequestPanelData();
                    data.betType = Convert.ToSByte(Constants.p_betType);
                    data.luckyPickFlag = Convert.ToSByte(Constants.p_luckyPickFlag_self);
                    data.betMultiplier = Convert.ToSByte(Constants.p_multiplier);
                    byte x1 = Convert.ToByte(splitSeq[0]);
                    byte x2 = Convert.ToByte(splitSeq[1]);
                    byte x3 = Convert.ToByte(splitSeq[2]);
                    byte x4 = Convert.ToByte(splitSeq[3]);
                    byte x5 = Convert.ToByte(splitSeq[4]);
                    byte x6 = Convert.ToByte(splitSeq[5]);
                    data.matrix1 = new byte[] { x1, x2, x3, x4, x5, x6 };
                    LottoRequestPanelData[] darray = new LottoRequestPanelData[] { data };

                    WSLottoGameRequest req = new WSLottoGameRequest();
                    req.gameId = Convert.ToSByte(Constants.p_gameID);
                    req.paymentType = Convert.ToSByte(Constants.p_paymentType);
                    req.numberOfPanels = Convert.ToSByte("1");
                    req.numberOfDraws = Convert.ToSByte(Constants.p_noOfDraws);
                    req.advancedDraw = Convert.ToSByte(Constants.p_advanceDraws);
                    req.drawOffset = Convert.ToSByte(Constants.p_drawsOffset);
                    req.lottoRequestPanelData = darray;
                    X509Certificate2 cert = new X509Certificate2(mFino.plentyCertPath);
                    GTConnectService cl = new GTConnectService();
                    cl.headerMsg = hmsg;
                    cl.ClientCertificates.Add(cert);
                    System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };
                    cl.Url = Constants.urlLive;
                    WSLottoGameResponse res = cl.routeLottoGameRequest(req);
                    string numbers = string.Empty;
                    string resTring = string.Format("TicketNo:{0}", res.ticketSerialNumber) + "%0A Bet(s):(";
                    foreach (LottoResponsePanelData dat in res.lottoResponsePanelData)
                    {
                        byte[] matbyte = dat.matrix1;
                        for (int i = 0; i < 6; i++)
                        {
                            resTring = resTring + Convert.ToString(matbyte[i]) + ",";
                            numbers = numbers + Convert.ToString(matbyte[i]) + ",";
                        }

                        resTring = resTring.Remove(resTring.LastIndexOf(","), 1);
                        resTring = resTring + ")%0A";
                    }

                   //resTring = resTring.Substring(0, resTring.Length - 1);
                    resTring = resTring + string.Format("Draw No: {0}%0A Date: {1}%0A", res.startDrawNumber, res.startDrawDate);
                    resp = string.Format("Ticket No : {0}%0A", res.ticketSerialNumber);
                    PlentyRequestService.UpdateLottoSelf(sessionId, Misdn, numbers, resTring, req, res);                   
                }
                else
                { 
                    //when failed ....inhouse error
                    resp = "Error has occurred! please try again later.";
                    PlentyRequestService.UpdateLottoOnError(sessionId, Misdn, resp, Constants.failed);
                }               
            }
            catch (Exception ex)
            {
                resp = ex.Message;
                ErrorLog log = new ErrorLog(ex);
                PlentyRequestService.UpdateLottoOnError(sessionId, Misdn, ex.Message,Constants.failed);
            }
            return resp;
        }

        public static string DoQuick(int noOfBets, string Misdn,string sessionId,mFinoInfo mFino)
        {
            int drawNo = noOfBets;
            string resp = "";
            try
            {
                int quickVal = PlentyRequestService.lottoSaveQuick(sessionId, Misdn, drawNo.ToString(), Constants.pending, mFino.Amount);
                if (quickVal == 1)
                {
                    //WebRequest.DefaultWebProxy = new WebProxy("10.0.0.120", 80)
                    //{
                    //    Credentials = new NetworkCredential("ibsservice", "Sterling123")
                    //};

                    WSHeaderMessage hmsg = new WSHeaderMessage();
                    hmsg.channelId = Constants.p_channelId;
                    hmsg.currentTime = DateTime.Now;
                    hmsg.messageId = Constants.p_messageId;
                    hmsg.posId = Misdn;
                    hmsg.sourceId = Constants.p_sourceId;
                    hmsg.extRefId = sessionId;
                    hmsg.applicationVersion = 1.0;

                    //Quick Pick panel ...
                    LottoRequestPanelData[] darray = new LottoRequestPanelData[drawNo];
                    for (int i = 0; i < darray.Length; i++)
                    {
                        darray[i] = new LottoRequestPanelData();
                        darray[i].betType = Convert.ToSByte(Constants.p_betType);
                        darray[i].luckyPickFlag = Convert.ToSByte(Constants.p_luckyPickFlag_quick);
                        darray[i].betMultiplier = Convert.ToSByte(Constants.p_multiplier);
                    }

                    WSLottoGameRequest req = new WSLottoGameRequest();
                    req.gameId = Convert.ToSByte(Constants.p_gameID);
                    req.paymentType = Convert.ToSByte(Constants.p_paymentType);
                    req.numberOfPanels = Convert.ToSByte(drawNo);//no of tickets
                    req.numberOfDraws = Convert.ToSByte(Constants.p_noOfDraws);
                    req.advancedDraw = Convert.ToSByte(Constants.p_advanceDraws);
                    req.drawOffset = Convert.ToSByte(Constants.p_drawsOffset);
                    req.lottoRequestPanelData = darray;

                    X509Certificate2 cert = new X509Certificate2(mFino.plentyCertPath);//Constants.certPath
                    GTConnectService cl = new GTConnectService();
                    cl.headerMsg = hmsg;
                    cl.ClientCertificates.Add(cert);
                    cl.Url = Constants.urlLive;
                    System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };
                    WSLottoGameResponse res = cl.routeLottoGameRequest(req);
                    string numbers = string.Empty;

                    string resTring = string.Format("TicketNo:{0}", res.ticketSerialNumber) + "%0ABet(s):(";
                    foreach (LottoResponsePanelData dat in res.lottoResponsePanelData)
                    {
                        byte[] matbyte = dat.matrix1;
                        for (int i = 0; i < 6; i++)
                        {
                            numbers = numbers + Convert.ToString(matbyte[i]) + ",";
                            resTring = resTring + Convert.ToString(matbyte[i]) + ",";
                        }
                        resTring = resTring.Remove(resTring.LastIndexOf(","), 1);
                        resTring = resTring + ")%0A";
                        numbers = numbers + "|";
                    }
                    //resTring = resTring.Substring(0, resTring.Length - 1);// +"%0A";
                    resTring = resTring + string.Format("Draw No: {0}%0ADate: {1}", res.startDrawNumber,res.startDrawDate);
                    PlentyRequestService.UpdateLottoQuick(sessionId, Misdn, resTring, numbers, req, res);
                    resp = string.Format("Ticket No : {0}", res.ticketSerialNumber);
                }
                else
                {
                    resp = "Error has occurred! please try again later.";
                    PlentyRequestService.UpdateLottoOnError(sessionId, Misdn, resp, Constants.failed);
                }
            }
            catch (Exception ex)
            {
                //log failed to DB for reversal
                resp = ex.Message;
                ErrorLog log = new ErrorLog(ex);
                PlentyRequestService.UpdateLottoOnError(sessionId, Misdn, ex.Message,Constants.failed);
            }
            return resp;
        }

        public static string PostPayRequest(mFinoInfo mfino,int betType,string[] spreq,int noOfBet,string misdn)
        {
            string resp = "-1";
            try
            {
                Request r = new Request(mfino.SourceMdn, mfino.PIN, mfino.SessionID);
                r.AuthKey = mfino.AuthKey;
                r.Mobile = mfino.SourceMdn;
                r.InstID = mfino.InstitutionID;
                r.ChannelID = Convert.ToInt32(mfino.ChannelID);
                r.Service = Constants.mfino_Service;
                r.Name = Constants.mfino_transferInquiry;
                r.SourcePocketCode = "1";
                r.DestPocketCode = "1";
                r.Amount = mfino.Amount;
                r.DestMobile = mfino.DestMdn;
                r.PIN = mfino.PIN;
                r.SessionID = mfino.SessionID;

                string trResp =  DoTransfer(r);
                string messageCode =  XMLTool.GetNodeAttribute(trResp, "message", "code");
                string msg =  XMLTool.GetNodeData(trResp, "message");
                mfino.Remark = msg;
                mfino.MsgCode = messageCode;
                int mRetval = PlentyRequestService.InsertMFINO(mfino);
                if (mRetval == 100)
                {
                   if (messageCode == "293")
                   {
                        if (betType == 1)//quick
                        {
                            resp = DoQuick(noOfBet, misdn, mfino.SessionID,mfino);
                        }
                        else if (betType == 2)//self
                        {
                            resp = DoSelf(spreq, misdn, mfino.SessionID,mfino);
                        }
                   }
                   else // when payment failed
                   {
                    resp = mfino.Remark.Replace("ERROR:", "").TrimStart().TrimEnd().Replace(". ", ".");
                   }
                }
                else
                {
                    resp = "Error has occurred! please try again later.";
                }               
            }
            catch (Exception ex)
            {
                ErrorLog lg = new ErrorLog(ex);
                resp = "Error has occurred, Please try again later!";
            }
            return resp;
        }

        public static string DoTransfer(Request r)
        {
          string trResp = "0";
          string keys = Authentication.DoLogin(r, r.PIN);
          r.AuthKey = keys;
          string inqResp = Transfer.DoInquiry(r);
          string retVal = XMLTool.GetNodeAttribute(inqResp, "message", "code");
          if (retVal == "72")
          {
              r.Confirmed = "1";
              trResp = Transfer.DoConfirm(r);
          }
          else
          {
              trResp = inqResp;
          }
          return trResp;
        }
    }    
}