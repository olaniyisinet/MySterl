using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Newtonsoft.Json;
using OtpService.Controllers;
using OtpService.Models;
using OtpService.Models.Request;
using OtpService.Models.Response;

namespace OtpService.Controllers
{
    public class PayliteController : ApiController
    {
        /// <summary>
        /// This method registers a new user, and creartes a wallet, this wallet Id is their BVN
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("RegisterUser")]
        [ResponseType(typeof(RegisterUserResp))]
        public HttpResponseMessage RegisterCustomer([FromBody] RegisterUser req)
        {
            Toolbox T = new Toolbox();
            //validate request
            if (!ModelState.IsValid)
            {
                GenericApiResponse<dynamic> rsp = new GenericApiResponse<dynamic>();
                // rsp.data = new RegisterUserResp { status = "99", userid = "" };
                rsp.message = "All parameters are required";
                var errors = ModelState.Select(x => x.Value.Errors)
                           .Where(y => y.Count > 0)
                           .ToList();
                rsp.data = errors;
                return Request.CreateResponse(HttpStatusCode.BadRequest, rsp);
            }

            if (T.ValidateDuplicateEnrollment(req))
            {
                GenericApiResponse<RegisterUserResp> rsp = new GenericApiResponse<RegisterUserResp>();
                rsp.data = new RegisterUserResp { status = "99", userid = "" };
                rsp.message = "phonenumber, email or handle already used before(Duplicate Records)";
                return Request.CreateResponse(HttpStatusCode.BadRequest, rsp);
            }

            if (T.SaveUserenrollment(req) == 1)
            {
                GenericApiResponse<RegisterUserResp> rsp = new GenericApiResponse<RegisterUserResp>();
                rsp.data = new RegisterUserResp { status = "00", userid = req.bvn };
                rsp.message = "successful";
                return Request.CreateResponse(HttpStatusCode.OK, rsp);
            }
            else
            {
                GenericApiResponse<RegisterUserResp> rsp = new GenericApiResponse<RegisterUserResp>();
                rsp.data = new RegisterUserResp { status = "99", userid = "" };
                rsp.message = "Unable To Save Records";
                return Request.CreateResponse(HttpStatusCode.BadRequest, rsp);
            }


        }
        /// <summary>
        /// To login a user
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("LoginUser")]
        [ResponseType(typeof(RegisterUserResp))]
        public HttpResponseMessage LoginUser([FromBody] LoginUser req)
        {
            Toolbox T = new Toolbox();
            //validate request
            if (!ModelState.IsValid)
            {
                GenericApiResponse<dynamic> rsp = new GenericApiResponse<dynamic>();
                rsp.data = new { UserExists = "99" };
                rsp.message = "failed";
                return Request.CreateResponse(HttpStatusCode.BadRequest, rsp);
            }
            if (T.Login(req))
            {
                GenericApiResponse<dynamic> rsp = new GenericApiResponse<dynamic>();
                rsp.data = new { status = "00" };
                rsp.message = "successful";
                return Request.CreateResponse(HttpStatusCode.OK, rsp);
            }
            else
            {
                GenericApiResponse<dynamic> rsp = new GenericApiResponse<dynamic>();
                rsp.data = new { status = "99" };
                rsp.message = "failed";
                return Request.CreateResponse(HttpStatusCode.Forbidden, rsp);
            }



        }
        /// <summary>
        /// To recover forgotten password
        /// </summary>
        /// <returns>200</returns>
        [HttpPut]
        [ActionName("ForgotPassword")]
        public HttpResponseMessage ForgotPassword([FromBody] ForgotPassword req)
        {
            Toolbox T = new Toolbox();
            //send mail to user
            if (!ModelState.IsValid)
            {
                GenericApiResponse<dynamic> rsp = new GenericApiResponse<dynamic>();
                rsp.data = new { AccountStatus = false };
                rsp.response = "99";
                rsp.message = "invalid request";
                return Request.CreateResponse(HttpStatusCode.BadRequest, rsp);
            }
            if (T.ForgotPassword(req))
            {
                GenericApiResponse<dynamic> rsp = new GenericApiResponse<dynamic>();
                rsp.data = new { AccountStatus = false };
                rsp.response = "00";
                rsp.message = "successful";
                return Request.CreateResponse(HttpStatusCode.OK, rsp);
            }
            else
            {
                GenericApiResponse<dynamic> rsp = new GenericApiResponse<dynamic>();
                rsp.data = new { AccountStatus = false };
                rsp.response = "99";
                rsp.message = "Unable to change password";
                return Request.CreateResponse(HttpStatusCode.BadRequest, rsp);
            }
        }

        /// <summary>
        /// used to update user data, use the bvn as the handle/userid
        /// </summary>
        /// <returns>200</returns>
        [HttpPut]
        [ActionName("UpdateUserDetails")]
        public HttpResponseMessage UpdateUser([FromBody] UpdateUser req)
        {
            Toolbox T = new Toolbox();
            if (!ModelState.IsValid)
            {
                GenericApiResponse<dynamic> rsp = new GenericApiResponse<dynamic>();
                rsp.data = new { UpdateCompleted = false };
                rsp.response = "99";
                rsp.message = "invalid request";
                return Request.CreateResponse(HttpStatusCode.BadRequest, rsp);
            }

            if (T.UpdateFxUser(req))
            {
                GenericApiResponse<dynamic> rsp = new GenericApiResponse<dynamic>();
                rsp.data = new { UpdateCompleted = true };
                rsp.response = "00";
                rsp.message = "Successful";
                return Request.CreateResponse(HttpStatusCode.OK, rsp);
            }
            else
            {
                GenericApiResponse<dynamic> rsp = new GenericApiResponse<dynamic>();
                rsp.data = new { AccountStatus = false };
                rsp.response = "99";
                rsp.message = "failed";
                return Request.CreateResponse(HttpStatusCode.BadRequest, rsp);
            }
        }
        /// <summary>
        /// get the details of a user.mail of user..the bvn is the handle to use
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpGet]
        [ActionName("GetUserDetails")]
        public HttpResponseMessage GetUserDetails([FromUri] GetUser req)
        {
            Toolbox T = new Toolbox();
            if (!ModelState.IsValid)
            {
                GenericApiResponse<dynamic> rsp = new GenericApiResponse<dynamic>();
                rsp.data = new { GetUserDetails = false };
                rsp.response = "99";
                rsp.message = "invalid request";
                return Request.CreateResponse(HttpStatusCode.BadRequest, rsp);
            }

            Tuple<bool, List<RegisterUser>> res;
            res = (T.GetUserDetails(req));
            if (res.Item1)
            {
                GenericApiResponse<dynamic> rsp = new GenericApiResponse<dynamic>();
                rsp.data = new { GetUserDetails = true };
                rsp.response = "00";
                rsp.message = "successful";
                return Request.CreateResponse(HttpStatusCode.OK, res.Item2);
            }
            else
            {
                GenericApiResponse<dynamic> rsp = new GenericApiResponse<dynamic>();
                rsp.data = new { GetUserDetails = false };
                rsp.response = "99";
                rsp.message = "Unable to get details";
                return Request.CreateResponse(HttpStatusCode.BadRequest, rsp);
            }

        }

        //
        /// <summary>
        /// pass the bvn (aka ID of the user to get their full wallet details)
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpGet]
        [ActionName("GetFullWalletDetails")]
        public HttpResponseMessage GetFullWalletDetails([FromUri] GetUser req)
        {
            Toolbox T = new Toolbox();
            if (!ModelState.IsValid)
            {
                GenericApiResponse<dynamic> rsp = new GenericApiResponse<dynamic>();
                rsp.data = new { GetUserDetails = false };
                rsp.response = "99";
                rsp.message = "invalid request, pass all parameters";
                return Request.CreateResponse(HttpStatusCode.BadRequest, rsp);
            }
            try
            {
                var Resp = T.GetFullWalletDetails(req.Bvn);

                if (Resp.Item1)
                {
                    GenericApiResponse<dynamic> rsp = new GenericApiResponse<dynamic>();
                    rsp.data = new { GetUserDetails = true };
                    rsp.response = "00";
                    rsp.message = "successful";
                    return Request.CreateResponse(HttpStatusCode.OK, Resp.Item2);
                }
                else
                {

                    GenericApiResponse<dynamic> rsp = new GenericApiResponse<dynamic>();
                    rsp.data = new { GetUserDetails = false };
                    rsp.response = "99";
                    rsp.message = "invalid request, pass all parameters";
                    return Request.CreateResponse(HttpStatusCode.BadRequest, rsp);
                }

            }
            catch (Exception e)
            {
                GenericApiResponse<dynamic> rsp = new GenericApiResponse<dynamic>();
                rsp.data = new { GetUserDetails = false };
                rsp.response = "99";
                rsp.message = "An error occurred";
                return Request.CreateResponse(HttpStatusCode.BadRequest, rsp);
            }
        }
        //- id
        //- handle/email/phone number
        //- recipient name
        //- amount
        //- reference/description
        //- payment date
        //- repeat type { never, daily, weekly, monthly, yearly }
        //send money using the handle..
        //*steps*
        //get the receipient handle--->>fetch their wallet-->>check for balance validity-->>fetch wallet of sender(their bvn)-->>make a wallet transfer-->> give a response
        /// <summary>
        /// send money to user wallet via a registered handle/username. pass the bvn (ID) of the sender. the payment ref(unique) and
        ///"00" resp means it went successful.
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("SendMoneyViaUserHandle")]
        public HttpResponseMessage SendMoneyViaUserHandle([FromBody] SendMoneyViaUserHandleReq req)
        {
            Toolbox T = new Toolbox();
            if (!ModelState.IsValid)
            {
                GenericApiResponse<dynamic> rsp = new GenericApiResponse<dynamic>();
                rsp.data = new { GetUserDetails = false };
                rsp.response = "99";
                rsp.message = "invalid request, pass all parameters";
                return Request.CreateResponse(HttpStatusCode.BadRequest, rsp);
            }
            //validate receipient and handle and return balance
            try
            {
                var GetWalletDetails = T.GetUserDetails(req.Bvn);
                if (GetWalletDetails.Item1)
                {
                    //process the trxn..get receipient acct number
                    var GetBeneficiaryID = T.GetbeneficiaryBvn(req.ReceipientHandle);
                    if (GetBeneficiaryID.Item1)
                    {
                        //process the transfer using the gotten bvn from the spd above
                        var ReciepientBvn = GetBeneficiaryID.Item2.Rows[0]["Bvn"].ToString();
                        //now we have the sender and receiver bvn (AKA acct number) 
                        //just call the wallet transfer class at this point
                        var debit = T.WalletTransfer(req.Amount, req.Bvn, ReciepientBvn.Trim(), req.PaymentReference, req.ReceipientName);

                        if (debit.response == "00")
                        {
                            GenericApiResponse<dynamic> rsp = new GenericApiResponse<dynamic>();
                            rsp.data = new { transferStatus = true };
                            rsp.response = "00";
                            rsp.message = "successful";
                            //log trxn
                            T.LogPayliteTrxn(req, ReciepientBvn, 1);
                            return Request.CreateResponse(HttpStatusCode.OK, rsp);
                        }
                        else
                        {
                            GenericApiResponse<dynamic> rsp = new GenericApiResponse<dynamic>();
                            rsp.data = new { transferStatus = false };
                            rsp.response = "99";
                            rsp.message = "unable to complete transaction";
                            T.LogPayliteTrxn(req, ReciepientBvn, 0);
                            return Request.CreateResponse(HttpStatusCode.PreconditionFailed, rsp);
                        }

                    }
                    else
                    {
                        GenericApiResponse<dynamic> rsp = new GenericApiResponse<dynamic>();
                        rsp.data = new { GetUserDetails = false };
                        rsp.response = "99";
                        rsp.message = "unable to get beneficiary acct";
                        T.LogPayliteTrxn(req, "unable to get", 0);
                        return Request.CreateResponse(HttpStatusCode.BadRequest, rsp);
                    }

                }
                else
                {
                    GenericApiResponse<dynamic> rsp = new GenericApiResponse<dynamic>();
                    rsp.data = new { GotuserDetails = false };
                    rsp.response = "99";
                    rsp.message = "unable to get wallet details";
                    T.LogPayliteTrxn(req, "unable to get", 0);
                    return Request.CreateResponse(HttpStatusCode.BadRequest, rsp);
                }
            }
            catch (Exception e)
            {


                GenericApiResponse<dynamic> rsp = new GenericApiResponse<dynamic>();
                rsp.data = new { GetUserDetails = false };
                rsp.response = "99";
                rsp.message = "An error occured: " + e.ToString();
                new ErrorLog("Error while sending funds: " + e.ToString());
                return Request.CreateResponse(HttpStatusCode.BadRequest, rsp);
            }

        }

        /// <summary>
        /// get the transactions between 2 dates, the fromdate being the most recent date
        /// </summary>
        /// <param name="bvn"></param>
        /// <returns></returns>

        [HttpGet]
        [ActionName("GetUserTransactions")]
        public HttpResponseMessage GetUserTransactions([FromUri] GetUSerTrxnReq bvn)
        {
            if (!ModelState.IsValid)
            {
                GenericApiResponse<dynamic> rsp = new GenericApiResponse<dynamic>();
                rsp.data = new { BidStatus = false };
                rsp.response = "99";
                rsp.message = "invalid request";
                return Request.CreateResponse(HttpStatusCode.BadRequest, rsp);
            }
            Toolbox T = new Toolbox();
            try
            {
                var GotUSerDetails = T.GetUserTrxns(bvn);
                if (GotUSerDetails.Item1)
                {
                    GenericApiResponse<dynamic> rsp = new GenericApiResponse<dynamic>();
                    rsp.data = new { GetUserDetails = true };
                    rsp.response = "00";
                    rsp.message = "Records gotten";
                    return Request.CreateResponse(HttpStatusCode.OK, GotUSerDetails.Item2);
                }
                else
                {
                    GenericApiResponse<dynamic> rsp = new GenericApiResponse<dynamic>();
                    rsp.data = new { BidStatus = false };
                    rsp.response = "99";
                    rsp.message = "unable to get history";
                    return Request.CreateResponse(HttpStatusCode.Gone, rsp);
                }
            }
            catch (Exception e)
            {
                //log
                GenericApiResponse<dynamic> rsp = new GenericApiResponse<dynamic>();
                rsp.data = new { BidStatus = false };
                rsp.response = "99";
                rsp.message = "unable to get history";
                new ErrorLog(e.ToString());
                return Request.CreateResponse(HttpStatusCode.InternalServerError, rsp);

            }



        }
        //transfers
        //fund wallet from cards--
        //cashout


        //This is to Debit any bank's card
        //{ "customerId":"sterling@gmail.com", "amount":"2500.99", "currency":"NGN", "pin":"1234", "expiry_date":"1911"|1909, "cvv":"823"|123,"pan":"5198990000000185"|5061030000000000084}
        /// <summary>
        /// this debits a users card and credits a wallet, pass the bvn of the user alongside card details
        /// </summary>
        /// <param name="Req"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("DebitAnyBankCard")]
        public HttpResponseMessage DebitAnyBankCard([FromBody] DebitAnyBankCardReq Req)
        {
            string jsoncontent = ""; string requestJSON = "";
            string BaseUrl = ConfigurationManager.AppSettings["DebitAnyCardUrl"].ToString();
            string apipath = "api/Payment/PurchasesNoOTP";
            Toolbox T = new Toolbox();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BaseUrl);
                requestJSON = JsonConvert.SerializeObject(Req);
                var cont = new StringContent(requestJSON, System.Text.Encoding.UTF8, "application/json");
                var result = client.PostAsync(apipath, cont).Result;
                try
                {
                    if (result.StatusCode == HttpStatusCode.Accepted || result.StatusCode == HttpStatusCode.OK)
                    {
                        jsoncontent = result.Content.ReadAsStringAsync().Result;
                        jsoncontent = jsoncontent.Replace(@"\", "");
                        jsoncontent = jsoncontent.Replace("\"{", "{");
                        jsoncontent = jsoncontent.Replace("}\"", "}");
                        //debit the wallet using the bvn of the customer's cards
                        string payref = DateTime.Now + ": Walletfunding";
                        //from acct would be our Paylite Pool acct

                        string PayliteAcct = ConfigurationManager.AppSettings["PayliteAcct"].ToString();
                        var resp = T.WalletTransfer(Req.amount, PayliteAcct, Req.bvn, payref, Req.customerId);
                        if (resp.response == "00")
                        {
                            GenericApiResponse<dynamic> rsp = new GenericApiResponse<dynamic>();
                            rsp.data = new { sent = true };
                            rsp.response = "00";
                            rsp.message = "successful";
                            return Request.CreateResponse(HttpStatusCode.OK, rsp);
                        }
                        else
                        {
                            GenericApiResponse<dynamic> rsp = new GenericApiResponse<dynamic>();
                            rsp.data = new { sent = true };
                            rsp.response = "00";
                            rsp.message = "successful, could not debit wallet";
                            return Request.CreateResponse(HttpStatusCode.OK, rsp);
                        }

                        //var resp1 = JsonConvert.DeserializeObject<EtagBalResSucc>(jsoncontent);

                        //if responsecode is "T0" fire the DebitAnyBankCardWithOTP method
                        //********************************************************************************
                        //if responsecode is "S0" load the cardinal form page using data in the response gotten from this method.
                        //<body onload = 'form1.submit()' > 
                        //<form id = "form1" runat = "server">           
                        //<input name = "TermUrl" id = "TermUrl" runat = "server"/>
                        //<input name = "MD" id = "MD" runat = "server"/>
                        //<input name = "PaReq" id = "PaReq" runat = "server"/>
                        //</form>
                        //</body>
                        //AFTER THIS fire the DebitVISAcardOnly method

                    }
                }
                catch
                {
                    jsoncontent = result.Content.ReadAsStringAsync().Result;
                    jsoncontent = jsoncontent.Replace(@"\", "");
                    jsoncontent = jsoncontent.Replace("\"{", "{");
                    jsoncontent = jsoncontent.Replace("}\"", "}");
                    return Request.CreateResponse(HttpStatusCode.BadGateway, jsoncontent);
                }

            }
            return Request.CreateResponse(HttpStatusCode.OK, jsoncontent);

        }
        // fund wallet via a sterling acct
        /// <summary>
        /// fund wallet from cards--fund wallet via a sterling acct. the toacct would be the bvn (paylite walletID)
        /// the response will be "00" for successful trxns. pas the payment ref and remarks
        /// </summary>
        /// <param name="req"></param>
        /// <returns>00</returns>
        [HttpPost]
        [ActionName("SterlingAccountToWallet")]
        public HttpResponseMessage SterlingAccountToWallet([FromBody] WalletToWalletReq req)
        {
            Toolbox T = new Toolbox();
            if (!ModelState.IsValid)
            {
                GenericApiResponse<dynamic> rsp = new GenericApiResponse<dynamic>();
                rsp.data = new { GetUserDetails = false };
                rsp.response = "99";
                rsp.message = "invalid request";
                return Request.CreateResponse(HttpStatusCode.BadRequest, rsp);
            }
            try
            {
                var resp = T.SBPT24txnRequest(req.amt, req.frmacct, req.toacct, req.paymentRef, req.remarks);
                if (resp.response == "00")
                {
                    GenericApiResponse<dynamic> rsp = new GenericApiResponse<dynamic>();
                    rsp.data = new { sent = true };
                    rsp.response = "00";
                    rsp.message = resp.message;
                    return Request.CreateResponse(HttpStatusCode.OK, rsp);
                }
                else
                {
                    GenericApiResponse<dynamic> rsp = new GenericApiResponse<dynamic>();
                    rsp.data = new { sent = false };
                    rsp.response = "99";
                    rsp.message = resp.message;
                    return Request.CreateResponse(HttpStatusCode.BadRequest, rsp);
                }
            }
            catch (Exception e)
            {
                GenericApiResponse<dynamic> rsp = new GenericApiResponse<dynamic>();
                rsp.data = new { sent = false };
                rsp.response = "99";
                rsp.message = "An error occured";
                return Request.CreateResponse(HttpStatusCode.BadRequest, rsp);
            }
        }
        //wallet to wallet-done
        /// <summary>
        /// This transfers money from a wallet to another, the toacct and frmacct is the bvn, i.e the walletID
        /// We agreed to use the bvn as the userID, since it is unique
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("WalletToWalletTransfers")]
        public HttpResponseMessage WalletToWalletTransfers([FromBody] WalletToWalletReq req)
        {
            Toolbox T = new Toolbox();
            if (!ModelState.IsValid)
            {
                GenericApiResponse<dynamic> rsp = new GenericApiResponse<dynamic>();
                rsp.data = new { GetUserDetails = false };
                rsp.response = "99";
                rsp.message = "invalid request";
                return Request.CreateResponse(HttpStatusCode.BadRequest, rsp);
            }
            try
            {
                var resp = T.WalletTransfer(req.amt, req.frmacct, req.toacct, req.paymentRef, req.remarks);
                if (resp.response == "00")
                {
                    GenericApiResponse<dynamic> rsp = new GenericApiResponse<dynamic>();
                    rsp.data = new { sent = true };
                    rsp.response = "00";
                    rsp.message = "successful";
                    return Request.CreateResponse(HttpStatusCode.OK, rsp);
                }
                else
                {
                    GenericApiResponse<dynamic> rsp = new GenericApiResponse<dynamic>();
                    rsp.data = new { GetUserDetails = false };
                    rsp.response = "99";
                    rsp.message = "An error occured";
                    return Request.CreateResponse(HttpStatusCode.BadRequest, rsp);
                }
            }
            catch (Exception e)
            {
                GenericApiResponse<dynamic> rsp = new GenericApiResponse<dynamic>();
                rsp.data = new { GetUserDetails = false };
                rsp.response = "99";
                rsp.message = "An error occured";
                return Request.CreateResponse(HttpStatusCode.BadRequest, rsp);
            }

        }
        /// <summary>
        /// Buy airtime for user, bvn is the user id. 
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("BuyAirtimeFromWallet")]
        public HttpResponseMessage BuyAirtimeFromWallet([FromBody] AirTimeReq req)
        {
            if (!ModelState.IsValid)
            {
                GenericApiResponse<dynamic> rsp = new GenericApiResponse<dynamic>();
                rsp.data = new { GetUserDetails = false };
                rsp.response = "99";
                rsp.message = "invalid request parameter";
                return Request.CreateResponse(HttpStatusCode.BadRequest, rsp);
            }
            Toolbox T = new Toolbox();
            //validate wallet account using bvn
            try
            {
                var IsValidUser = T.GetUserDetails(req.bvn);
                if (IsValidUser.Item1)
                {
                    //debit the wallet
                    string PayliteAcct = ConfigurationManager.AppSettings["PayliteAcct"].ToString();
                    string PayRef = "Airtime To :" + req.PhoneNumber;
                    string Remarks = "Airtime From: " + req.bvn;
                    var AirtimeResponse = T.WalletTransfer(req.amount, req.bvn, PayliteAcct, PayRef, Remarks);

                    //if successful 
                    if (AirtimeResponse.response == "00")
                    {
                        //public string ReferenceID { get; set; }
                        //public string RequestType { get; set; }
                        //public string Mobile { get; set; }
                        //public string Beneficiary { get; set; }
                        //public string Amount { get; set; }
                        //public string NUBAN { get; set; }
                        //public string NetworkID { get; set; }
                        //public string Type { get; set; } //2
                        AirtimeRechargeReq Ar = new AirtimeRechargeReq
                        {
                            Amount = req.amount,
                            Beneficiary = req.PhoneNumber,
                            Mobile = req.bvn,
                            NUBAN = "0063891248",//hard code this to my acct
                            NetworkID = req.NetworkProvider,
                            ReferenceID = req.PhoneNumber + req.bvn,
                            RequestType = "932",
                            Type = "1"
                        };


                        //call ibs service to give airtime
                        var resp = T.AirtimeRecharge(Ar);

                        if (resp.response == "00")
                        {
                            GenericApiResponse<dynamic> rsp = new GenericApiResponse<dynamic>();
                            rsp.data = new { AirtimeBought = true };
                            rsp.response = "00";
                            rsp.message = "successful";
                            return Request.CreateResponse(HttpStatusCode.OK, rsp);
                        }
                        else
                        {
                            GenericApiResponse<dynamic> rsp = new GenericApiResponse<dynamic>();
                            rsp.data = new { AirtimeBought = false };
                            rsp.response = "96";
                            rsp.message = "Debited Wallet, but unable to debit PaylitePool, had to rollback wallet debit";
                            //roll back wallet debit
                            var AirtimeRespon = T.WalletTransfer(req.amount, PayliteAcct, req.bvn, PayRef, Remarks);
                            return Request.CreateResponse(HttpStatusCode.BadRequest, rsp);
                        }
                    }
                    else
                    {
                        GenericApiResponse<dynamic> rsp = new GenericApiResponse<dynamic>();
                        rsp.data = new { AirtimeBought = false };
                        rsp.response = "99";
                        rsp.message = "Unable to debit sender wallet";
                        return Request.CreateResponse(HttpStatusCode.BadRequest, rsp);
                    }
                }
                else
                {
                    GenericApiResponse<dynamic> rsp = new GenericApiResponse<dynamic>();
                    rsp.data = new { AirtimeBought = false };
                    rsp.response = "98";
                    rsp.message = "Sending Wallet not found";
                    return Request.CreateResponse(HttpStatusCode.BadRequest, rsp);
                }
            }
            catch (Exception e)
            {
                GenericApiResponse<dynamic> rsp = new GenericApiResponse<dynamic>();
                rsp.data = new { AirtimeBought = false };
                rsp.response = "98";
                rsp.message = "Sending Wallet not found";
                return Request.CreateResponse(HttpStatusCode.BadRequest, rsp);
            }
            //call airtime service
            //get debit wallet

        }
        [HttpPost]
        [ActionName("SendOtp")]
        public HttpResponseMessage SendOtp([FromBody] SendOtpReq req)
        {
            if (!ModelState.IsValid)
            {
                GenericApiResponse<dynamic> rsp = new GenericApiResponse<dynamic>();

                rsp.data = new { MessageSent = false };
                rsp.message = "All parameters are required";
                rsp.response = "99";

                return Request.CreateResponse(HttpStatusCode.BadRequest, rsp);
            }
            Toolbox T = new Toolbox();
            int Appid = 69; //appid for Paylite
            int result = T.doGenerateOtpByMobile(req.mobile, Appid, req.email);
            if (result == 1)
            {
                GenericApiResponse<dynamic> rsp = new GenericApiResponse<dynamic>();

                rsp.data = new { MessageSent = true };
                rsp.message = "Sent";
                rsp.response = "00";
                return Request.CreateResponse(HttpStatusCode.OK, rsp);
            }
            else
            {
                GenericApiResponse<dynamic> rsp = new GenericApiResponse<dynamic>();
                rsp.data = new { MessageSent = false };
                rsp.response = "99";
                rsp.message = "Not Sent";
                return Request.CreateResponse(HttpStatusCode.BadRequest, rsp);

            }

        }

        //[HttpPost]
        //[ActionName("ValidateOtp")]
        //public HttpResponseMessage SeValidateOtp([FromBody] SeValidateOtpReq req)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        GenericApiResponse<dynamic> rsp = new GenericApiResponse<dynamic>();
        //        rsp.data = new { OtpVerified = false };
        //        rsp.message = "All parameters are required";
        //        rsp.response = "99";
        //        return Request.CreateResponse(HttpStatusCode.BadRequest, rsp);
        //    }
        //    Toolbox T = new Toolbox();
        //    int AppID = 69;
        //    int result = T.verifyOtp(req.mobile,req.Otp,AppID);
        //}
        /// <summary>
        /// Get paycode to user to cashout via ATM
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("CashOutViaPayCode")]
        public HttpResponseMessage SeValidateOtp([FromBody] CashOutViaPayCodeReq req)
        {
            if (!ModelState.IsValid)
            {
                GenericApiResponse<dynamic> rsp = new GenericApiResponse<dynamic>();
                rsp.data = new { OtpVerified = false };
                rsp.message = "All parameters are required:";
                rsp.response = "99";
                return Request.CreateResponse(HttpStatusCode.BadRequest, rsp);
            }
            Toolbox T = new Toolbox();
            string AppID = "77";
            string PaylitePoolAcctPayCode = ConfigurationManager.AppSettings["PayCodePoolAcct"].ToString();

            try
            {
                //debit the wallet
                string debitWalletResp = "00";
                if (debitWalletResp == "00")
                {
                    //debit the paylite pool acccount and generate a paycode
                    //if it fails, i will rollback the earlier wallet trxn


                    //Lumi .please do the Paycode magic below and return the response below..thanks
                    string BaseUrl = ConfigurationManager.AppSettings["BaseUrlPayCode"];
                    string jsoncontent = ""; string requestJSON = "";
                    string transRef = T.GenerateRndNumber(3) + DateTime.Now.ToString("hhss" +AppID);
                    PaycodeReq r = new PaycodeReq();
                    r.accountNo = req.AccountNumber;
                    r.amount = req.Amount;
                    r.appid = AppID;
                    r.oneTimePin = req.OneTimePin;
                    r.subscriber = req.mobile;
                    r.ttid = transRef;
                    r.transactionRef = transRef;
                    r.paymentChannel = "ATM";
                    r.codeGenerationChannel = AppID;
                    requestJSON = JsonConvert.SerializeObject(r);
                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(BaseUrl);
                        string apipath = "api/Paycode/GenerateTokenReq";
                       // var cont = new StringContent(requestJSON, System.Text.Encoding.UTF8, "application/json");
                        //var result = client.PostAsync(apipath, cont).Result;
                        var result =  client.PostAsJsonAsync(apipath, r).Result;
                        if (result.StatusCode == HttpStatusCode.Created || result.StatusCode == HttpStatusCode.OK)//successful response
                        {
                            jsoncontent =  result.Content.ReadAsStringAsync().Result;
                            var resp = JsonConvert.DeserializeObject<SuccPaycodeResp>(jsoncontent);

                            //This is the token  resp.payWithMobileToken 
                            string sourceEmail = ConfigurationManager.AppSettings["PaycodeSourceEmail"];
                            string subject = ConfigurationManager.AppSettings["PaycodeEmailSubject"];

                            ewsTest.ServiceSoapClient ews = new ewsTest.ServiceSoapClient();
                            var resp2 = ews.SendMail(req.CustomerEmail, sourceEmail, "<p>Dear Customer, your Paycode from Sterling bank is: " + resp.payWithMobileToken + "</strong></p> <br> <p>This paycode is valid for 60 minutes. </p>", subject);
                            return Request.CreateResponse(HttpStatusCode.OK, resp);
                        }
                        else
                        {
                            jsoncontent = result.Content.ReadAsStringAsync().Result;
                            var content = JsonConvert.DeserializeObject<dynamic>(jsoncontent);
                            
                            return Request.CreateResponse(HttpStatusCode.BadRequest, jsoncontent);
                        }
                    }


                    GenericApiResponse<dynamic> rsps = new GenericApiResponse<dynamic>();
                    rsps.data = new { PayCodeGenerated = true };
                    rsps.message = "success";
                    rsps.response = "00";
                    return Request.CreateResponse(HttpStatusCode.BadRequest, rsps);

                }
                else
                {
                    GenericApiResponse<dynamic> rsp = new GenericApiResponse<dynamic>();
                    rsp.data = new { PayCodeGenerated = false };
                    rsp.message = "All parameters are required:";
                    rsp.response = "99";
                    return Request.CreateResponse(HttpStatusCode.BadRequest, rsp);
                }

            }
            catch (Exception ex)
            {
                GenericApiResponse<dynamic> rsp = new GenericApiResponse<dynamic>();
                rsp.data = new { PayCodeGenerated = false };
                rsp.message = "An error occured";
                rsp.response = "99";
                new ErrorLog(ex);
                return Request.CreateResponse(HttpStatusCode.BadRequest, rsp);
            }

            // int result = T.verifyOtp(req.mobile, req.Otp, AppID);
        }

        //paycode trxns
        //airtime service
        //data top-up

    }
}
