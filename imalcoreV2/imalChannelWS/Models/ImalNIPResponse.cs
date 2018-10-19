using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace imalChannelWS.Models
{
    public static class ImalNIPResponse
    {
        static string json = string.Empty;
        static Gadget g = new Gadget();
        public static HttpResponseMessage AccountNotActive(this HttpRequestMessage request, long logval)
        {
            NipFundsTransferResp rsp = new NipFundsTransferResp
            {
                responseCode = "06",
                errorCode = "06",
                skipProcessing = "false",
                originalResponseCode = "06",
                skipLog = "false"
            };
            json = JsonConvert.SerializeObject(rsp);
            g.updateResponseCode("06", logval, json);
            var response = new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            response.Content = content;
            return response;

        }
        public static HttpResponseMessage CheckSuffiBalance(this HttpRequestMessage request, long logval)
        {
            NipFundsTransferResp rsp = new NipFundsTransferResp
            {
                responseCode = "51",
                errorCode = "51",
                skipProcessing = "false",
                originalResponseCode = "51",
                skipLog = "false"
            };
            json = JsonConvert.SerializeObject(rsp);
            g.updateResponseCode("51", logval, json);
            var response = new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            response.Content = content;
            return response;
        }
        public static HttpResponseMessage CheckIndividualPerTrans(this HttpRequestMessage request, long logval)
        {
            NipFundsTransferResp rsp = new NipFundsTransferResp
            {
                responseCode = "61",
                errorCode = "61",
                skipProcessing = "false",
                originalResponseCode = "61",
                skipLog = "false"
            };
            json = JsonConvert.SerializeObject(rsp);
            g.updateResponseCode("61", logval, json);
            var response = new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            response.Content = content;
            return response;
        }
        public static HttpResponseMessage CheckCurrency(this HttpRequestMessage request, long logval)
        {
            NipFundsTransferResp rsp = new NipFundsTransferResp
            {
                responseCode = "57",
                errorCode = "57",
                skipProcessing = "false",
                originalResponseCode = "57",
                skipLog = "false"
            };
            json = JsonConvert.SerializeObject(rsp);
            g.updateResponseCode("57", logval, json);
            var response = new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            response.Content = content;
            return response;
        }
        public static HttpResponseMessage ZeroAmtorLess(this HttpRequestMessage request, long logval)
        {
            NipFundsTransferResp rsp = new NipFundsTransferResp
            {
                responseCode = "13",
                errorCode = "13",
                skipProcessing = "false",
                originalResponseCode = "13",
                skipLog = "false"
            };
            json = JsonConvert.SerializeObject(rsp);
            g.updateResponseCode("13", logval, json);
            var response = new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            response.Content = content;
            return response;
        }
        public static HttpResponseMessage CheckmaxPerTrans(this HttpRequestMessage request, long logval)
        {
            NipFundsTransferResp rsp = new NipFundsTransferResp
            {
                responseCode = "09x",
                errorCode = "09x",
                skipProcessing = "false",
                originalResponseCode = "09x",
                skipLog = "false"
            };
            json = JsonConvert.SerializeObject(rsp);
            g.updateResponseCode("09x", logval, json);
            var response = new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            response.Content = content;
            return response;
        }
        public static HttpResponseMessage CheckmaxPerDay(this HttpRequestMessage request, long logval)
        {
            NipFundsTransferResp rsp = new NipFundsTransferResp
            {
                responseCode = "09x",
                errorCode = "09x",
                skipProcessing = "false",
                originalResponseCode = "09x",
                skipLog = "false"
            };
            json = JsonConvert.SerializeObject(rsp);
            g.updateResponseCode("09x", logval, json);
            var response = new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            response.Content = content;
            return response;
        }
        public static HttpResponseMessage TransNotPermited(this HttpRequestMessage request, long logval)
        {
            NipFundsTransferResp rsp = new NipFundsTransferResp
            {
                responseCode = "57",
                errorCode = "57",
                skipProcessing = "false",
                originalResponseCode = "57",
                skipLog = "false"
            };
            json = JsonConvert.SerializeObject(rsp);
            g.updateResponseCode("57", logval, json);
            var response = new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            response.Content = content;
            return response;
        }
        public static HttpResponseMessage UnableToComputeVatFee(this HttpRequestMessage request, long logval)
        {
            NipFundsTransferResp rsp = new NipFundsTransferResp
            {
                responseCode = "10x",
                errorCode = "10x",
                skipProcessing = "false",
                originalResponseCode = "10x",//unable to compute fee and vat
                skipLog = "false"
            };
            json = JsonConvert.SerializeObject(rsp);
            g.updateResponseCode("10x", logval, json);
            var response = new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            response.Content = content;
            return response;
        }
        public static HttpResponseMessage SuccessfulNIP(this HttpRequestMessage request, long logval, string ResponseCode)
        {
            NipFundsTransferResp rsp = new NipFundsTransferResp
            {
                responseCode = "00",
                errorCode = "00",
                skipProcessing = "false",
                originalResponseCode = "00",
                skipLog = "false"
            };
            json = JsonConvert.SerializeObject(rsp);
            g.updateResponseCode("00", logval, json);
            g.updateNIPCode(ResponseCode, logval);
            var response = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            response.Content = content;
            return response;
        }
        public static HttpResponseMessage UNSuccessfulNIP(this HttpRequestMessage request, long logval, string ResponseCode)
        {
            NipFundsTransferResp rsp = new NipFundsTransferResp
            {
                responseCode = ResponseCode,
                errorCode = ResponseCode,
                skipProcessing = "false",
                originalResponseCode = ResponseCode,
                skipLog = "false"
            };
            json = JsonConvert.SerializeObject(rsp);
            g.updateNIPCode(ResponseCode, logval);         
            var response = new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            response.Content = content;
            return response;
        }
        public static HttpResponseMessage UnabletoDebitVat(this HttpRequestMessage request, long logval)
        {
            NipFundsTransferResp rsp = new NipFundsTransferResp
            {
                responseCode = "12x",
                errorCode = "12x",
                skipProcessing = "false",
                originalResponseCode = "12x",//unable to debit customer for vat
                skipLog = "false"
            };
            json = JsonConvert.SerializeObject(rsp);
            g.updateResponseCode("12x", logval, json);
            var response = new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            response.Content = content;
            return response;
        }
        public static HttpResponseMessage UnabletoDebitFee(this HttpRequestMessage request, long logval)
        {
            NipFundsTransferResp rsp = new NipFundsTransferResp
            {
                responseCode = "13x",
                errorCode = "13x",
                skipProcessing = "false",
                originalResponseCode = "13x",//unable to debit customer for fee
                skipLog = "false"
            };
            json = JsonConvert.SerializeObject(rsp);
            g.updateResponseCode("13x", logval, json);
            var response = new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            response.Content = content;
            return response;
        }
        public static HttpResponseMessage UnabletoDebitPrincipal(this HttpRequestMessage request, long logval)
        {
            NipFundsTransferResp rsp = new NipFundsTransferResp
            {
                responseCode = "14x",
                errorCode = "14x",
                skipProcessing = "false",
                originalResponseCode = "14x",//unable to debit customer for principal
                skipLog = "false"
            };
            json = JsonConvert.SerializeObject(rsp);
            g.updateResponseCode("14x", logval, json);
            var response = new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            response.Content = content;
            return response;
        }
        public static HttpResponseMessage ExceedAllowedCBNlimit(this HttpRequestMessage request, long logval)
        {
            NipFundsTransferResp rsp = new NipFundsTransferResp
            {
                responseCode = "09x",
                errorCode = "09x",
                skipProcessing = "false",
                originalResponseCode = "09x",
                skipLog = "false"
            };
            json = JsonConvert.SerializeObject(rsp);
            var response = new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            response.Content = content;
            return response;
        }
    }
}