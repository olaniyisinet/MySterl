using BVNVerification.DTOs;
using BVNVerification.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Http;
using System.Web.Script.Serialization;


namespace BVNVerification.Controllers
{
    public enum httpVerb
    {
        GET,
        POST,
        PUT,
        DELETE
    }

    public class BVNVerificationController : ApiController
    {
        private readonly string baseUrl = WebConfigurationManager.AppSettings["BaseUrl"];
        private readonly string password = WebConfigurationManager.AppSettings["Password"];
        private readonly string username = WebConfigurationManager.AppSettings["Username"];
        private readonly string aesKey = WebConfigurationManager.AppSettings["AES_KEY"];
        private readonly string iv = WebConfigurationManager.AppSettings["IVKey"];
        private readonly BVNVerificationDBEntities db = new BVNVerificationDBEntities();



        // GET: BVNVerification
        [Route("VerifySingleBVN")]
        [HttpPost]
        public Object VerifySingleBVN([FromBody] BVNClass bvn)
        {
            var verSingle = new VerifySingleBVN();
            verSingle.AuthorizationBase = Base64Encode(username + ":" + password);
            verSingle.BVN = bvn.BVN;
            verSingle.Signature = GenerateSHA256String(username + DateTime.Today.ToString("yyyyMMdd") + password);
            verSingle.TimeIn = DateTime.Now;

            string responseMessage = string.Empty;
            var url = baseUrl + "VerifySingleBVN";
            var apiRequest = (HttpWebRequest)WebRequest.Create(url);

            apiRequest.Method = "POST";
            apiRequest.ContentType = "application/json";
            apiRequest.Accept = "application/json";
            apiRequest.Headers.Add("Authorization", Base64Encode(username + ":" + password));
            apiRequest.Headers.Add("SIGNATURE", GenerateSHA256String(username + DateTime.Today.ToString("yyyyMMdd") + password));
            apiRequest.Headers.Add("SIGNATURE_METH", "SHA256");

            string jsonOrder = JsonConvert.SerializeObject(bvn);
            verSingle.JSonReq = jsonOrder;
            verSingle.EncryptedJSonReq = Encrypt(jsonOrder);

            var data = Encoding.UTF8.GetBytes(Encrypt(jsonOrder));
            apiRequest.ContentLength = data.Length;

            string message = apiRequest.Method + url + apiRequest.ContentType;

            using (var stream = apiRequest.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }

            HttpWebResponse response = null;
            try
            {
                response = (HttpWebResponse)apiRequest.GetResponse();
                using (Stream responseStream = response.GetResponseStream())
                {
                    if (responseStream != null)
                    {
                        using (StreamReader reader = new StreamReader(responseStream))
                        {
                            responseMessage = reader.ReadToEnd();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                responseMessage = "{\"errorMessages\":[\"" + e.Message.ToString() + "\"],\"errors\":{}}";

                verSingle.EncryptedJSonResp = "";
                verSingle.JSonResp = responseMessage;
                db.VerifySingleBVNs.Add(verSingle);
                db.SaveChanges();
                
                return responseMessage;
            }
            finally
            {
                if (response != null)
                {
                    ((IDisposable)response).Dispose();
                }
            }
            
            string test = Decrypt(StringToByteArray(responseMessage));
            
            JavaScriptSerializer js = new JavaScriptSerializer();

            VerifySingleBVNResp items = JsonConvert.DeserializeObject<VerifySingleBVNResp>(test);
            verSingle.EncryptedJSonResp = responseMessage;
            verSingle.JSonResp = test;
            verSingle.ResponseCode = items.ResponseCode;
            db.VerifySingleBVNs.Add(verSingle);
            db.SaveChanges();
            if (items.ResponseCode == "00" || items.ResponseCode != "00")
            {
                return items;
            }


            return "Error Occured";

        }


        [Route("VerifyMultipleBVN")]
        [HttpPost]
        public Object VerifyMultipleBVN([FromBody] MultipleBVN bvn)
            {
            var verMulti = new VerifyMultipleBVN();
            verMulti.AuthorizationBase = Base64Encode(username + ":" + password);
            verMulti.BVNs = bvn.BVNS;
            verMulti.Signature = GenerateSHA256String(username + DateTime.Today.ToString("yyyyMMdd") + password);
            verMulti.TimeIn = DateTime.Now;

            string responseMessage = string.Empty;
            var url = baseUrl + "VerifyMultipleBVN";
            var apiRequest = (HttpWebRequest)WebRequest.Create(url);

            string dt = DateTime.Today.ToString("yyyyMMdd");


            apiRequest.Method = "POST";
            apiRequest.ContentType = "application/json";
            apiRequest.Accept = "application/json";
            apiRequest.Headers.Add("Authorization", Base64Encode(username + ":" + password));
            apiRequest.Headers.Add("SIGNATURE", GenerateSHA256String(username + DateTime.Today.ToString("yyyyMMdd") + password));
            apiRequest.Headers.Add("SIGNATURE_METH", "SHA256");

            string jsonOrder = JsonConvert.SerializeObject(bvn);
            verMulti.JSonReq = jsonOrder;
            verMulti.EncryptedJSonReq = Encrypt(jsonOrder);


            var data = Encoding.UTF8.GetBytes(Encrypt(jsonOrder));
            apiRequest.ContentLength = data.Length;

            string message = apiRequest.Method + url + apiRequest.ContentType;

            using (var stream = apiRequest.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }

            HttpWebResponse response = null;
            try
            {
                response = (HttpWebResponse)apiRequest.GetResponse();
                using (Stream responseStream = response.GetResponseStream())
                {
                    if (responseStream != null)
                    {
                        using (StreamReader reader = new StreamReader(responseStream))
                        {
                            responseMessage = reader.ReadToEnd();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                verMulti.EncryptedJSonResp = "";
                verMulti.JSonResp = responseMessage;
                db.VerifyMultipleBVNs.Add(verMulti);
                db.SaveChanges();

                responseMessage = "{\"errorMessages\":[\"" + e.Message.ToString() + "\"],\"errors\":{}}";
            }
            finally
            {
                if (response != null)
                {
                    ((IDisposable)response).Dispose();
                }
            }

            string test = Decrypt(StringToByteArray(responseMessage));
            JavaScriptSerializer js = new JavaScriptSerializer();

            //List<VerifySingleBVNResp> items = JsonConvert.DeserializeObject<List<VerifySingleBVNResp>>(test);
            RootObject items = JsonConvert.DeserializeObject<RootObject>(test);
            verMulti.EncryptedJSonResp = responseMessage;
            verMulti.JSonResp = test;
            verMulti.ResponseCode = items.ResponseCode;
            db.VerifyMultipleBVNs.Add(verMulti);
            db.SaveChanges();

            if (items.ResponseCode == "00" || items.ResponseCode != "00")
            {
                return items;
            }


            return "Error Occured";

        }

        [Route("GetSingleBVN")]
        [HttpPost]
        public Object GetSingleBVN([FromBody] BVNClass bvn)
        {
            var getSingle = new GetSingleBVN();
            getSingle.AuthorizationBase = Base64Encode(username + ":" + password);
            getSingle.BVN = bvn.BVN;
            getSingle.Signature = GenerateSHA256String(username + DateTime.Today.ToString("yyyyMMdd") + password);
            getSingle.TimeIn = DateTime.Now;

            string responseMessage = string.Empty;
            var url = baseUrl + "GetSingleBVN";
            var apiRequest = (HttpWebRequest)WebRequest.Create(url);
            
            apiRequest.Method = "POST";
            apiRequest.ContentType = "application/json";
            apiRequest.Accept = "application/json";
            apiRequest.Headers.Add("Authorization", Base64Encode(username + ":" + password));
            apiRequest.Headers.Add("SIGNATURE", GenerateSHA256String(username + DateTime.Today.ToString("yyyyMMdd") + password));
            apiRequest.Headers.Add("SIGNATURE_METH", "SHA256");
            string jsonOrder = JsonConvert.SerializeObject(bvn);
            getSingle.JSonReq = jsonOrder;
            getSingle.EncryptedJSonReq = Encrypt(jsonOrder);

            var data = Encoding.UTF8.GetBytes(Encrypt(jsonOrder));
            apiRequest.ContentLength = data.Length;

            string message = apiRequest.Method + url + apiRequest.ContentType;

            using (var stream = apiRequest.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }

            HttpWebResponse response = null;
            try
            {
                response = (HttpWebResponse)apiRequest.GetResponse();
                using (Stream responseStream = response.GetResponseStream())
                {
                    if (responseStream != null)
                    {
                        using (StreamReader reader = new StreamReader(responseStream))
                        {
                            responseMessage = reader.ReadToEnd();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                getSingle.EncryptedJSonResp = "";
                getSingle.JSonResp = responseMessage;
                db.GetSingleBVNs.Add(getSingle);
                db.SaveChanges();

                responseMessage = "{\"errorMessages\":[\"" + e.Message.ToString() + "\"],\"errors\":{}}";
            }
            finally
            {
                if (response != null)
                {
                    ((IDisposable)response).Dispose();
                }
            }

            string test = Decrypt(StringToByteArray(responseMessage));
            //JavaScriptSerializer js = new JavaScriptSerializer();

            BVNS items = JsonConvert.DeserializeObject<BVNS>(test);

            getSingle.EncryptedJSonResp = responseMessage;
            getSingle.JSonResp = test;
            getSingle.ResponseCode = items.ResponseCode;
            db.GetSingleBVNs.Add(getSingle);
            db.SaveChanges();
            if (items.ResponseCode == "00" || items.ResponseCode != "00")
            {
                return items;
            }


            return "Error Occured";

        }


        [Route("GetMultipleBVN")]
        [HttpPost]
        public Object GetMultipleBVN([FromBody] MultipleBVN bvn)
        {
            var getMulti = new GetMultipleBVN();
            getMulti.AuthorizationBase = Base64Encode(username + ":" + password);
            getMulti.BVNs = bvn.BVNS;
            getMulti.Signature = GenerateSHA256String(username + DateTime.Today.ToString("yyyyMMdd") + password);
            getMulti.TimeIn = DateTime.Now;

            string responseMessage = string.Empty;
            var url = baseUrl + "GetMultipleBVN";
            var apiRequest = (HttpWebRequest)WebRequest.Create(url);

            string dt = DateTime.Today.ToString("yyyyMMdd");


            apiRequest.Method = "POST";
            apiRequest.ContentType = "application/json";
            apiRequest.Accept = "application/json";
            apiRequest.Headers.Add("Authorization", Base64Encode(username + ":" + password));
            apiRequest.Headers.Add("SIGNATURE", GenerateSHA256String(username + DateTime.Today.ToString("yyyyMMdd") + password));
            apiRequest.Headers.Add("SIGNATURE_METH", "SHA256");
            string jsonOrder = JsonConvert.SerializeObject(bvn);
            getMulti.JSonReq = jsonOrder;
            getMulti.EncryptedJSonReq = Encrypt(jsonOrder);

            var data = Encoding.UTF8.GetBytes(Encrypt(jsonOrder));
            apiRequest.ContentLength = data.Length;

            string message = apiRequest.Method + url + apiRequest.ContentType;

            using (var stream = apiRequest.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }

            HttpWebResponse response = null;
            try
            {
                response = (HttpWebResponse)apiRequest.GetResponse();
                using (Stream responseStream = response.GetResponseStream())
                {
                    if (responseStream != null)
                    {
                        using (StreamReader reader = new StreamReader(responseStream))
                        {
                            responseMessage = reader.ReadToEnd();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                getMulti.EncryptedJSonResp = "";
                getMulti.JSonResp = responseMessage;
                db.GetMultipleBVNs.Add(getMulti);
                db.SaveChanges();
                responseMessage = "{\"errorMessages\":[\"" + e.Message.ToString() + "\"],\"errors\":{}}";
            }
            finally
            {
                if (response != null)
                {
                    ((IDisposable)response).Dispose();
                }
            }

            string test = Decrypt(StringToByteArray(responseMessage));
            JavaScriptSerializer js = new JavaScriptSerializer();

            //List<VerifySingleBVNResp> items = JsonConvert.DeserializeObject<List<VerifySingleBVNResp>>(test);
            RootObjectA items = JsonConvert.DeserializeObject<RootObjectA>(test);
            getMulti.EncryptedJSonResp = responseMessage;
            getMulti.JSonResp = test;
            getMulti.ResponseCode = items.ResponseCode;
            db.GetMultipleBVNs.Add(getMulti);
            db.SaveChanges();

            if (items.ResponseCode == "00" || items.ResponseCode != "00")
            {
                return items;
            }


            return "Error Occured";

        }

        [Route("IsBVNWatchlisted")]
        [HttpPost]
        public Object IsBVNWatchlisted([FromBody] BVNClass bvn)
        {
            var isBvnWatchlisted = new Models.IsBVNWatchlisted();
            isBvnWatchlisted.AuthorizationBase = Base64Encode(username + ":" + password);
            isBvnWatchlisted.BVN = bvn.BVN;
            isBvnWatchlisted.Signature = GenerateSHA256String(username + DateTime.Today.ToString("yyyyMMdd") + password);
            isBvnWatchlisted.TimeIn = DateTime.Now;

            string responseMessage = string.Empty;
            var url = baseUrl + "IsBVNWatchlisted";
            var apiRequest = (HttpWebRequest)WebRequest.Create(url);

            string dt = DateTime.Today.ToString("yyyyMMdd");


            apiRequest.Method = "POST";
            apiRequest.ContentType = "application/json";
            apiRequest.Accept = "application/json";
            apiRequest.Headers.Add("Authorization", Base64Encode(username + ":" + password));
            apiRequest.Headers.Add("SIGNATURE", GenerateSHA256String(username + DateTime.Today.ToString("yyyyMMdd") + password));
            apiRequest.Headers.Add("SIGNATURE_METH", "SHA256");
            string jsonOrder = JsonConvert.SerializeObject(bvn);
            string k = Encrypt(jsonOrder);
            isBvnWatchlisted.JSonReq = jsonOrder;
            isBvnWatchlisted.EncryptedJSonReq = Encrypt(jsonOrder);
            var data = Encoding.UTF8.GetBytes(Encrypt(jsonOrder));
            apiRequest.ContentLength = data.Length;

            string message = apiRequest.Method + url + apiRequest.ContentType;

            using (var stream = apiRequest.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }

            HttpWebResponse response = null;
            try
            {
                response = (HttpWebResponse)apiRequest.GetResponse();
                using (Stream responseStream = response.GetResponseStream())
                {
                    if (responseStream != null)
                    {
                        using (StreamReader reader = new StreamReader(responseStream))
                        {
                            responseMessage = reader.ReadToEnd();
                        }
                    }
                }
            }
            catch (Exception e)
            {

                responseMessage = "{\"errorMessages\":[\"" + e.Message.ToString() + "\"],\"errors\":{}}";
                isBvnWatchlisted.EncryptedJSonResp = "";
                isBvnWatchlisted.JSonResp = responseMessage;
                db.IsBVNWatchlisteds.Add(isBvnWatchlisted);
                db.SaveChanges();
            }
            finally
            {
                if (response != null)
                {
                    ((IDisposable)response).Dispose();
                }
            }

            string test = Decrypt(StringToByteArray(responseMessage));
            JavaScriptSerializer js = new JavaScriptSerializer();

            DTOs.IsBVNWatchlisted items = JsonConvert.DeserializeObject<DTOs.IsBVNWatchlisted>(test);

            isBvnWatchlisted.EncryptedJSonResp = responseMessage;
            isBvnWatchlisted.JSonResp = test;
            isBvnWatchlisted.ResponseCode = items.ResponseCode;
            db.IsBVNWatchlisteds.Add(isBvnWatchlisted);
            db.SaveChanges();
            if (items.ResponseCode == "00" || items.ResponseCode != "00")
            {
                return items;
            }


            return "Error Occured";

        }

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public static string Base64Decode(string enc)
        {
            byte[] data = Convert.FromBase64String(enc);
            string decodedString = Encoding.UTF8.GetString(data);
            return decodedString;
        }

        public static string GenerateSHA256String(string inputString)
        {
            SHA256 sha256 = SHA256Managed.Create();
            byte[] bytes = Encoding.UTF8.GetBytes(inputString);
            byte[] hash = sha256.ComputeHash(bytes);
            return GetStringFromHash(hash);
        }

        private static string GetStringFromHash(byte[] hash)
        {
            StringBuilder result = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                result.Append(hash[i].ToString("X2"));
            }
            return result.ToString();
        }


        public static string Encrypt(string word)
        {
            //Log.Instance.Info(word);
            string prestr;
            byte[] result = null;
            // string word = "1";
            byte[] wordBytes = Encoding.UTF8.GetBytes(word);
            using (MemoryStream ms = new MemoryStream())

            {
                using (RijndaelManaged AES = new RijndaelManaged())
                {
                    AES.KeySize = 256;
                    AES.BlockSize = 128;
                    //AES.Key = Encoding.UTF8.GetBytes("0KPYzlVLeHuQgzcS");
                    //AES.IV = Encoding.UTF8.GetBytes("DgexcNbmMx39vkQV");
                    AES.Key = Encoding.UTF8.GetBytes("z5EQhC+eEYDn9aM2");
                    AES.IV = Encoding.UTF8.GetBytes("fM973OaLAaABLPno");
                    AES.Mode = CipherMode.CBC;

                    using (var cs = new CryptoStream(ms, AES.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(wordBytes, 0, wordBytes.Length);
                        cs.Close();
                    }
                    byte[] encryptedBytes = ms.ToArray();
                    //result = encryptedBytes;
                    prestr = ByteArrayToString(encryptedBytes).ToUpper();
                    // result = ByteArrayToString(encryptedBytes);
                }
            }
            return prestr;
        }



        static string Decrypt(byte[] cipher)
        {
            //string word = "A7BA53AAA7D67CA8CC54913DA398E189";
            byte[] wordBytes = cipher;//StringToByteArray(word);
            byte[] byteBuffer = new byte[wordBytes.Length];
            var str = "";
            using (MemoryStream ms = new MemoryStream())
            {
                using (RijndaelManaged AES = new RijndaelManaged())
                {
                    AES.KeySize = 256;
                    AES.BlockSize = 128;
                    AES.Key = Encoding.UTF8.GetBytes("z5EQhC+eEYDn9aM2");
                    AES.IV = Encoding.UTF8.GetBytes("fM973OaLAaABLPno");

                    AES.Mode = CipherMode.CBC;

                    using (var cs = new CryptoStream(ms, AES.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(wordBytes, 0, wordBytes.Length);
                        cs.Close();
                    }
                    byte[] decryptedBytes = ms.ToArray();
                    str = System.Text.Encoding.UTF8.GetString(decryptedBytes);

                }
            }
            return str;
        }

        public static string ByteArrayToString(byte[] ba)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }
        public static byte[] StringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }


        //internal static object Encrypt()
        //{
        //    throw new NotImplementedException();
        //}



    }
}