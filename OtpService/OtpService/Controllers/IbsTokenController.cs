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
    public class IbsTokenController : ApiController
    {

        /// <summary>
        /// Send otp to customer's whatsapp using Twilio
        /// </summary>
        /// <param name="req">the number must start with the 234 format</param>
        /// <returns>200</returns>
        /// <remarks>the message parameter is optional</remarks>
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
        [HttpPost]
        [ActionName("SendOtpToViaCustId")]
        [ResponseType(typeof(OtpSendStatus))]
        public HttpResponseMessage SendOtpToViaCustId([FromBody] GenerateOtpVIaCustIdReq req)
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

            var result = T.SendTokenCustId(req.UserIDD, req.mail, req.phonenumber);

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
                rsp.message = "Unable to send Otp";
                rsp.response = "99";
                return Request.CreateResponse(HttpStatusCode.BadRequest, rsp);
            }

        }


        [HttpPost]
        [ActionName("ValidateOtpToViaCustId")]
        [ResponseType(typeof(OtpSendStatus))]
        public HttpResponseMessage ValidateOtpToViaCustId([FromBody] ValidateOtpVIaCustIdReq req)
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

            var result = T.ValidateSentTokenCustId(req.LoginID, req.AppID, req.Token);
            if (result == 1)
            {
                GenericApiResponse<dynamic> rsp = new GenericApiResponse<dynamic>();
                rsp.data = new { OtpValidated = true };
                rsp.message = "OTP SENT";
                rsp.response = "00";
                return Request.CreateResponse(HttpStatusCode.OK, rsp);

            }
            else
            {
                GenericApiResponse<dynamic> rsp = new GenericApiResponse<dynamic>();
                rsp.data = new { OtpValidated = false };
                rsp.message = "Unable to validate Otp";
                rsp.response = "99";
                return Request.CreateResponse(HttpStatusCode.BadRequest, rsp);
            }

        }


    }
}
