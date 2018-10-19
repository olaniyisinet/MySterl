//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net;
//using System.Net.Http;
//using System.Web.Http;

//namespace OtpService.Controllers
//{
//    public class IncomingMessageController : ApiController
//    {
//    }
//}

using System.Net.Http;
using System.Text;
using System.Web.Http;
using Twilio.TwiML;
using Twilio.TwiML.Messaging;
using OtpService.Models;

using Twilio.AspNet.Mvc;

namespace OtpService.Controllers
{
    public class TwilioMessagingRequest
    {
        public string Body { get; set; }//contents the option choses by the user
        //so we receive the otp and validate, if it correct the send the balance or initiate transfer
    }

    public class TwilioVoiceRequest
    {
        public string From { get; set; }
      //  public string userOption { get; set; }
    }

    public class IncomingController : ApiController
    {

        //[HttpPost]
        //public ActionResult Index()
        //{
        //    var requestBody = Request.Form["Body"];
        //    var response = new MessagingResponse();
        //    if (requestBody == "hello")
        //    {
        //        response.Message("Hi!");
        //    }
        //    else if (requestBody == "bye")
        //    {
        //        response.Message("Goodbye");
        //    }

        //    return TwiML(response);
        //}
        [Route("voice")]
        [AcceptVerbs("POST")]
        [ValidateTwilioRequest]
        public HttpResponseMessage PostVoice([FromBody] TwilioVoiceRequest voiceRequest)
        {

            //receive the sterling otp here and validate it then, 
            //receive the account, get the balance and return it here 
            var message =
                "Thanks for calling! " +
                $"Your inputted OTP number is {voiceRequest.From}. " +
                "I got your call because of Sterling Twilio's webhook. " +
                "Goodbye!";

            var response = new VoiceResponse();
            response.Say(message);
            response.Hangup();

            return ToResponseMessage(response.ToString());
        }

        [Route("message")]
        [AcceptVerbs("POST")]
        [ValidateTwilioRequest]
        public HttpResponseMessage PostMessage([FromBody] TwilioMessagingRequest messagingRequest)
        {

            //receive the chosen option for the user and get their balance and send it to them
            var message =
                $"Your whatsapp text to me was {messagingRequest.Body.Length} characters long. " +
                "Webhooks are neat :)";

            string nuban = "";
            //get balance and send it to the user
            message = "103993884776 Naira";
            var response = new MessagingResponse();
            response.Append(new Message(message));

            return ToResponseMessage(response.ToString());
        }

        private static HttpResponseMessage ToResponseMessage(string response)
        {
            return new HttpResponseMessage
            {
                Content = new StringContent(response, Encoding.UTF8, "application/xml")
            };
        }
    }
}