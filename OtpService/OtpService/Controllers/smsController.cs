using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Twilio.AspNet.Mvc;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;
using OtpService.Models;
using OtpService.Models.Request;
using OtpService.Models.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Twilio;
using Twilio.Exceptions;
using Twilio.Rest.Api.V2010.Account;
using Twilio.TwiML;
using System.Data;
using OtpService.Controllers;

namespace OtpService.Controllers
{
    public class smsController : TwilioController // Controller
    {
        // GET: sms
        public ActionResult ReceiveSms()
        {


            //   const string authToken = "3fd0f6cbd89a9d386837dc381d0180a7";//live
            const string accountSid = "ACe405d45e52f7f7056ed132b23b5859d5";
            const string authToken = "3fd0f6cbd89a9d386837dc381d0180a7";//tes


            var resp = Request.Form["body"].ToString();
            var phonenumber = Request.Form["From"];
            phonenumber = phonenumber.Replace("whatsapp:+234", "0").Trim();
            var user = Request.UserAgent.ToString();
            //based on the response you can then send msgs
            if (resp.ToString() == "1" || resp.ToLower() == "balance" || resp.ToLower() == "bal")
            {
                //1 is for balance

                string bal = "";
                string AccountName = "";
                Whatsappbank.banksSoapClient Getbalance = new Whatsappbank.banksSoapClient();
                DataSet userAccts = Getbalance.getCustomerAccountsByMobileNo2(phonenumber);
                if (userAccts != null && userAccts.Tables.Count > 0)
                {
                    bal = userAccts.Tables[0].Rows[0]["Balance"].ToString(); //userAccts.Tables[0].Rows[1]["columnName"];
                    AccountName = userAccts.Tables[0].Rows[0]["NUBAN"].ToString(); //userAccts.Tables[0].Rows[1]["columnName"];

                }
                //parse response and move one
                string msg = "Your balance for account: " + AccountName + " is: " + bal + " Naira, have a great day";
                var responses = new MessagingResponse();
                responses.Message(msg);
                // response.Message.
                return TwiML(responses);
            }
            else if (resp == "2" || resp.ToLower() == "bvn")
            {
                string bal = "";
                string AccountName = "";
                Whatsappbank.banksSoapClient Getbalance = new Whatsappbank.banksSoapClient();
                DataSet userAccts = Getbalance.getCustomerAccountsByMobileNo2(phonenumber);
                string bvn = "";
                if (userAccts != null && userAccts.Tables.Count > 0)
                {
                    bvn = userAccts.Tables[0].Rows[0]["BVN"].ToString();
                    AccountName = userAccts.Tables[0].Rows[0]["NUBAN"].ToString();
                    string msg = "Your bvn for account: " + AccountName + " is: " + bvn + ", have a great day";
                    var responses = new MessagingResponse();
                    responses.Message(msg);
                    // response.Message.
                    return TwiML(responses);
                }

            }
            else if (resp.ToLower() == "hi" || resp.ToLower() == "hello" || resp.ToLower() == "sup" || resp.ToLower() == "hey")
            {
                string AccountName = "";
                Whatsappbank.banksSoapClient Getbalance = new Whatsappbank.banksSoapClient();
                DataSet userAccts = Getbalance.getCustomerAccountsByMobileNo2(phonenumber);
                AccountName = userAccts.Tables[0].Rows[1]["AccountName"].ToString();
                string msg = "Hi " + AccountName + " 😁,  ⚈ send  *'1'* for account balance ₦  ⚈ *'2'* for your bvn number💡 .  ⚈ To send money 💸 e.g simply type *'send 3000 to 0063891248'*";
                var responses = new MessagingResponse();
                responses.Message(msg);
                // response.Message.
                return TwiML(responses);
            }
            else if (resp.ToLower().StartsWith("send") || resp.ToLower().StartsWith("transfer"))
            {
                //send 5000 to 0063891248
                //create a unique id for this transaction and store it in a table
                //when the user returns, use their mobile number to fetch their last trxn and move treat
                string[] separatingChars = { "to", "send" };

                try
                {
                    resp = resp.ToLower().Replace("send", "");
                    resp = resp.ToLower().Replace("to", "*");
                    //split response
                    resp = resp.Trim();
                    string[] splittedRespons = resp.Split('*');
                    string amount = splittedRespons[0].ToString();
                    string reciepientNUban = splittedRespons[1].ToString().Trim();
                    //get the nuban name
                    string AccountName = "";
                    Whatsappbank.banksSoapClient Getbalance = new Whatsappbank.banksSoapClient();

                    DataSet userAccts = Getbalance.getAccountFullInfo(reciepientNUban);
                    AccountName = userAccts.Tables[0].Rows[0]["CUS_SHO_NAME"].ToString();
                    string msg = "Send " + amount + " Naira to " + AccountName + " reply with the otp sent to you to confirm this transaction";
                    string email = userAccts.Tables[0].Rows[0]["EMAIL"].ToString();
                    Toolbox T = new Toolbox();

                    //log the trxn records to a treating table before responding
                    int insertResopnse = T.InsertWhatsappTransferRequest(phonenumber, amount, reciepientNUban);
                    if (insertResopnse == 1)
                    {
                        //send otp
                        int OtpResponse = T.doGenerateOtpByMobile(phonenumber, 1, email);
                        var respons = new MessagingResponse();
                        respons.Message(msg);
                        return TwiML(respons);
                    }
                    else if (insertResopnse == 2)
                    {

                        msg = "Insufficient funds!! try to send a lower amount than your balance";
                        var respons = new MessagingResponse();
                        respons.Message(msg);
                        return TwiML(respons);
                    }
                    else
                    {
                        msg = "Could not process transaction, kindly re-initiate this trnasaction";
                        var responses = new MessagingResponse();
                        responses.Message(msg);
                        return TwiML(responses);
                    }


                }
                catch (Exception ex)
                {
                    //log the exception later
                    string msg = "An error occured while getting details, try again later";
                    var responses = new MessagingResponse();
                    responses.Message(msg);
                    return TwiML(responses);
                }

            }
            else if (resp.Length == 6 && resp.All(char.IsDigit))//true means it is an otp, 
            {

                //validate the otp against the senders phone number on the db
                Toolbox T = new Toolbox();
                int otpValid = T.verifyOtpByMobile(phonenumber, resp, 1);
                // if (otpValid == 1)
                if(true)
                {
                    //if validate, proces the trxn
                    //send back response based on processing of trxn
                    //fetch the trxn by passing in the records on 

                // var TransferStatus = T.ConsumateWhatsappTransfer(phonenumber,"0");
                
                    // if(TransferStatus == 1)
                    if(true)
                    {
                        string msg = "Your transfer was successful";
                        var responses = new MessagingResponse();
                        responses.Message(msg);
                        return TwiML(responses);
                    }
                    else
                    {
                        var respond = new MessagingResponse();
                        respond.Message("Unable to complete transaction currently, kindly restart this transaction");
                        return TwiML(respond);
                    }
                }
                else if(otpValid == 2)
                {
                    string msg = "Expired OTP, kindly re-start transaction to start get a new OTP";
                    var responses = new MessagingResponse();
                    responses.Message(msg);
                    return TwiML(responses);
                }
                else
                {
                    string msg = "Invalid otp, kindly use the otp sent to you previuosly or re-start transaction to start afresh";
                    var responses = new MessagingResponse();
                    responses.Message(msg);
                    return TwiML(responses);
                }


            }


            var response = new MessagingResponse();
            response.Message("Hi Customer, type 'hi' to get a *full menu* ");
            return TwiML(response);

        }
    }
}
