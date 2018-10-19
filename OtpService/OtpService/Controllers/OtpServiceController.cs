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
namespace OtpService.Controllers
{
    public class OtpServiceController : ApiController
    {

        [HttpGet]
        [ActionName("TestGet")]
        [ResponseType(typeof(OtpSendStatus))]
        public HttpResponseMessage TestGet()
        {
            return Request.CreateResponse(HttpStatusCode.OK, "goofy");
        }

        [HttpPost]
        [ActionName("SendOtpToWhatsAppTwilio")]
        [ResponseType(typeof(OtpSendStatus))]
        public HttpResponseMessage SendOtpToWhatsAppTwilio([FromBody] TwiilioReq req)
        {
            if (!ModelState.IsValid)
            {
                GenericApiResponse<OtpSendStatus> rsp = new GenericApiResponse<OtpSendStatus>();
                rsp.data = new OtpSendStatus { OtpSent = false };
                rsp.message = "All parameters are required";
                rsp.response = "00";
                return Request.CreateResponse(HttpStatusCode.BadRequest, rsp);
            }
            IbsTokenToolBox T = new IbsTokenToolBox();

            var result = 1;

            if (result == 1)
            {
                // const string accountSid = "ACe405d45e52f7f7056ed132b23b5859d5";//live
                //   const string authToken = "3fd0f6cbd89a9d386837dc381d0180a7";//live
                const string accountSid = "ACe405d45e52f7f7056ed132b23b5859d5";
                const string authToken = "3fd0f6cbd89a9d386837dc381d0180a7";//tes

                // live:
                //   Auth token:
                //3fd0f6cbd89a9d386837dc381d0180a7
                //Accountsid: ACe405d45e52f7f7056ed132b23b5859d5
                //Test:
                //accountsid
                //AC766994cf88c84022e63d39e7e7269bc1
                //Auth token:
                //ade5db099190dab4eb33f8d16e97d750


                TwilioClient.Init(accountSid, authToken);
                Twilio.TwilioClient.ValidateSslCertificate();
                TwilioClient.SetAccountSid(accountSid);
                var client = TwilioClient.GetRestClient();
                string number = "whatsapp:+" + req.WhatsAppNumber; //you must pass a "234..."number
                try
                {
                    var message = MessageResource.Create(
                    body: "Hi Customer, your OTP from Sterling Bank is: " + req.OTP,
                    from: new Twilio.Types.PhoneNumber("whatsapp:+14155238886"), //sterling whatsapp number here
                    to: new Twilio.Types.PhoneNumber(number));//customer number

                    GenericApiResponse<dynamic> rsp = new GenericApiResponse<dynamic>();

                    var d = message.Status;
                    if (d.ToString() == "queued")
                    {
                        rsp.message = message.Status.ToString();
                        return Request.CreateResponse(HttpStatusCode.OK, rsp);
                    }
                    rsp.message = message.Status.ToString();
                    return Request.CreateResponse(HttpStatusCode.BadGateway, rsp);
                }
                catch (Exception ex)
                {

                    GenericApiResponse<dynamic> rsp = new GenericApiResponse<dynamic>();
                    TwilioException d = new TwilioException(ex.ToString(), ex);

                    rsp.data = d.InnerException.ToString();
                    rsp.message = d.Data.ToString(); //ex.Message;
                    return Request.CreateResponse(HttpStatusCode.Conflict, rsp);
                }



            }
            else
            {
                GenericApiResponse<OtpSendStatus> rsp = new GenericApiResponse<OtpSendStatus>();
                rsp.data = new OtpSendStatus { OtpSent = false };
                rsp.message = "Unable to send Otp";
                rsp.response = "99";
                return Request.CreateResponse(HttpStatusCode.BadRequest, rsp);
            }

        }

        /// <summary>
        /// send otp to a bank user using nuban, this method takes in a nuban and AppId
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("SendOtpToNuban")]
        [ResponseType(typeof(OtpSendStatus))]
        public HttpResponseMessage SendOtpToNuban([FromBody] GenerateOtpViaNuban req)
        {
            if (!ModelState.IsValid)
            {
                GenericApiResponse<OtpSendStatus> rsp = new GenericApiResponse<OtpSendStatus>();
                rsp.data = new OtpSendStatus { OtpSent = false };
                rsp.message = "All parameters are required";
                rsp.response = "00";

                return Request.CreateResponse(HttpStatusCode.BadRequest, rsp);
            }

            Toolbox T = new Toolbox();
            int result = T.doGenerateOtp(req.nuban, req.Appid);
            if (result == 1)
            {
                GenericApiResponse<OtpSendStatus> rsp = new GenericApiResponse<OtpSendStatus>();
                rsp.data = new OtpSendStatus { OtpSent = true };
                rsp.message = "OTP SENT";
                rsp.response = "00";

                return Request.CreateResponse(HttpStatusCode.OK, rsp);

            }
            else
            {
                GenericApiResponse<OtpSendStatus> rsp = new GenericApiResponse<OtpSendStatus>();
                rsp.data = new OtpSendStatus { OtpSent = false };
                rsp.message = "An error Occurred while sending otp";

                return Request.CreateResponse(HttpStatusCode.BadRequest, rsp);
            }

        }
        /// <summary>
        /// send sms to mobile number
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("sendSms")]
        [ResponseType(typeof(OtpSendStatus))]
        public HttpResponseMessage SendSms([FromBody] SendSmsReq req)
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
            int result = T.SendSms(req.msg, req.mobnum, req.Appid);

            if (result == 1)
            {
                GenericApiResponse<dynamic> rsp = new GenericApiResponse<dynamic>();
                rsp.data = new { MessageSent = true };
                rsp.message = "Message sent";
                rsp.response = "00";

                return Request.CreateResponse(HttpStatusCode.OK, rsp);
            }
            else
            {
                GenericApiResponse<dynamic> rsp = new GenericApiResponse<dynamic>();

                rsp.data = new { MessageSent = false };
                rsp.message = "Unable to send message";
                rsp.response = "99";

                return Request.CreateResponse(HttpStatusCode.BadRequest, rsp);
            }
        }

        /// <summary>
        /// send otp for mobile and mail, it takes in the nuban, destination/source mail and also a mail subject
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("GenerateOtpForPhoneAndMail")]
        [ResponseType(typeof(OtpSendStatus))]
        public HttpResponseMessage GenerateOtpForPhoneAndMail([FromBody] doGenerateOtpAndMailReq req)
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
            string result = T.doGenerateOtpAndMail(req.nuban, req.Appid, req.destinationEmail, req.sourceEmail, req.subject);
            if (result.StartsWith("1"))
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


        //doGenerateOtpAndMailByPhoneNumber
        /// <summary>
        /// Generate Otp for mail and phoneNumber
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("GenerateOtpByMailAndPhoneNumber")]
        [ResponseType(typeof(OtpSendStatus))]
        public HttpResponseMessage GenerateOtpByMailAndPhoneNumber([FromBody] doGenerateOtpAndMailReq req)
        {
            //public string doGenerateOtpAndMailByPhoneNumber(string phoneNumber, string nuban, Int32 Appid, string destinationEmail, string sourceEmail, string subject)

            if (!ModelState.IsValid)
            {
                GenericApiResponse<dynamic> rsp = new GenericApiResponse<dynamic>();
                rsp.data = new { MessageSent = false };
                rsp.message = "All parameters are required";
                rsp.response = "99";
                return Request.CreateResponse(HttpStatusCode.BadRequest, rsp);
            }
            Toolbox T = new Toolbox();

            string result = T.doGenerateOtpAndMailByPhoneNumber(req.mobnum, req.nuban, req.Appid, req.destinationEmail, req.sourceEmail, req.subject);
            if (result.StartsWith("1*"))
            {
                GenericApiResponse<dynamic> rsp = new GenericApiResponse<dynamic>();
                rsp.data = new { MessageSent = false };
                rsp.message = "Sent";
                rsp.response = "00";
                return Request.CreateResponse(HttpStatusCode.OK, rsp);
            }
            else
            {
                GenericApiResponse<dynamic> rsp = new GenericApiResponse<dynamic>();

                rsp.data = new { MessageSent = false };
                rsp.response = "99";
                rsp.message = "unable to send message";

                return Request.CreateResponse(HttpStatusCode.BadRequest, rsp);
            }

        }

        /// <summary>
        /// Verify Otp, this takes in the nuban, Otp and AppID
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>

        [HttpPost]
        [ActionName("VerifyOtp")]
        [ResponseType(typeof(OtpSendStatus))]
        public HttpResponseMessage VerifyOtp([FromBody] GenerateOtpViaNuban req)
        {
            //(string nuban, string otp, Int32 Appid)
            if (!ModelState.IsValid)
            {
                GenericApiResponse<dynamic> rsp = new GenericApiResponse<dynamic>();
                rsp.data = new { MessageSent = false };
                rsp.message = "All parameters are required";
                rsp.response = "99";
                return Request.CreateResponse(HttpStatusCode.BadRequest, rsp);
            }
            Toolbox T = new Toolbox();

            int result = T.verifyOtp(req.nuban, req.otp, req.Appid);

            if (result == 1)
            {
                GenericApiResponse<dynamic> rsp = new GenericApiResponse<dynamic>();
                rsp.data = new { Otpverify = true };
                rsp.message = "Sent";
                rsp.response = "00";
                return Request.CreateResponse(HttpStatusCode.OK, rsp);
            }
            else
            {
                GenericApiResponse<dynamic> rsp = new GenericApiResponse<dynamic>();
                rsp.data = new { Otpverify = false };
                rsp.message = "All parameters are required";
                rsp.response = "99";
                return Request.CreateResponse(HttpStatusCode.BadRequest, rsp);
            }

        }

        /// <summary>
        /// verifyOtpHigherDelay
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("verifyOtpHigherDelay")]
        [ResponseType(typeof(OtpSendStatus))]
        public HttpResponseMessage verifyOtpHigherDelay([FromBody] verifyOtpHigherDelayReq req)
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

            int result = T.verifyOtpHigherDelay(req.nuban, req.otp, req.Appid, req.minutes);

            if (result == 1)
            {
                GenericApiResponse<dynamic> rsp = new GenericApiResponse<dynamic>();
                rsp.data = new { Otpverify = true };
                rsp.message = "Sent";
                rsp.response = "00";
                return Request.CreateResponse(HttpStatusCode.OK, rsp);
            }
            else
            {
                GenericApiResponse<dynamic> rsp = new GenericApiResponse<dynamic>();
                rsp.data = new { Otpverify = false };
                rsp.message = "All parameters are required";
                rsp.response = "99";
                return Request.CreateResponse(HttpStatusCode.BadRequest, rsp);
            }
        }
        /// <summary>
        /// use mobile and send otp to user
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("doGenerateOtpByMobile")]
        [ResponseType(typeof(OtpSendStatus))]
        public HttpResponseMessage doGenerateOtpByMobile([FromBody] doGenerateOtpByMobileReq req)
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
            var result = T.doGenerateOtpByMobile(req.mobnum, req.Appid, req.EMAIL);

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
                rsp.message = "Could not Verify";
                rsp.response = "99";
                return Request.CreateResponse(HttpStatusCode.BadRequest, rsp);
            }
        }

        /// <summary>
        /// verify otp sent to mobile 
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("verifyOtpByMobile")]
        [ResponseType(typeof(OtpSendStatus))]
        public HttpResponseMessage verifyOtpByMobile([FromBody] verifyOtpByMobileReq req)
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
            var result = T.verifyOtpByMobile(req.mobile, req.otp, req.Appid);
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
                rsp.message = "Could not verify";
                rsp.response = "99";
                return Request.CreateResponse(HttpStatusCode.BadRequest, rsp);
            }
        }

        /// <summary>
        /// Generate OTP: leave the "mail" field blank, the nuban field can be a unique identifier for te non-Sterling apps
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("GenerateOtpAndMailByPhoneNumberWithMailBody")]
        [ResponseType(typeof(OtpSendStatus))]
        public HttpResponseMessage GenerateOtpAndMailByPhoneNumberWithMailBody([FromBody] MailByPhoneWithMailBody req)
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
            var result = T.doGenerateOtpAndMailByPhoneNumberWithMailBody(req.phoneNumber, req.nuban, req.Appid, req.destinationEmail, req.sourceEmail, req.subject, req.mailBody);

            if (result.StartsWith("1*"))
            {
                GenericApiResponse<dynamic> rsp = new GenericApiResponse<dynamic>();
                rsp.data = new { MessageSent = true };
                rsp.message = "successful";
                rsp.response = "00";
                return Request.CreateResponse(HttpStatusCode.OK, rsp);
            }
            else
            {
                GenericApiResponse<dynamic> rsp = new GenericApiResponse<dynamic>();
                rsp.data = new { MessageSent = false };
                rsp.message = "An error occured";
                rsp.response = "99";
                return Request.CreateResponse(HttpStatusCode.BadRequest, rsp);
            }
        }

    }
}
