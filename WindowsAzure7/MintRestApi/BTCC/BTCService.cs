using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace MintRestApi.BTCC
{
    public class BTCService
    {
        private const string accessKey = "6351c6e2-f6d0-4403-a2ab-f1fee2fc782e";
        private const string secretKey = "bb7cf19c-29be-4a52-bebe-8ed1e3caad77";
        private const string url = "https://api.btcchina.com/api.php/payment";
        private const string callback_url = "http://mintrestapi2.cloudapp.net/MintRESTfulAPI.svc/BTCResponse";
        private const string method = "createPurchaseOrder";

        public static Response GetPurchaseOrder(string id)
        {
            try
            {
                ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                string method = "getPurchaseOrder";
                string param = string.Format("{0}", id);
                TimeSpan timeSpan = DateTime.UtcNow - new DateTime(1970, 1, 1);
                long milliSeconds = Convert.ToInt64(timeSpan.TotalMilliseconds * 1000);
                string tonce = Convert.ToString(milliSeconds);
                NameValueCollection parameters = new NameValueCollection() { 
                    { "tonce", tonce },
                    { "accesskey", accessKey },
                    { "requestmethod", "post" },
                    { "id", "1" },
                    { "method", method },
                    { "params", param } 
                };
                string paramsHash = GetHMACSHA1Hash(secretKey, BuildQueryString(parameters));
                string base64String = Convert.ToBase64String(
                Encoding.ASCII.GetBytes(accessKey + ':' + paramsHash));
                string url = "https://api.btcchina.com/api.php/payment";
                string postData = "{\"method\": \"" + method + "\", \"params\": [" + id + "], \"id\": 1}";
                var res = SendPostByWebRequest(url, base64String, tonce, postData);
                return res;
            }
            catch (Exception e)
            {
                Trace.TraceError(e.ToString());
                throw e;
            }
        }

        public static Response CreateBTCOrder(string amount, string id)
        {
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            string param = amount + ",USD," + callback_url + "," + callback_url + "," + id + ",Funding CSV";
            TimeSpan timeSpan = DateTime.UtcNow - new DateTime(1970, 1, 1);
            long milliSeconds = Convert.ToInt64(timeSpan.TotalMilliseconds * 1000);
            string tonce = Convert.ToString(milliSeconds);
            NameValueCollection parameters = new NameValueCollection() { 
                    { "tonce", tonce },
                    { "accesskey", accessKey },
                    { "requestmethod", "post" },
                    { "id", "1" },
                    { "method", method },
                    { "params", param } 
                };
            string paramsHash = BTCService.GetHMACSHA1Hash(secretKey, BTCService.BuildQueryString(parameters));
            string base64String = Convert.ToBase64String(
            Encoding.ASCII.GetBytes(accessKey + ':' + paramsHash));
            string postData = "{\"method\": \"" + method + "\", \"params\": [" + amount + ",\"USD\",\"" + callback_url + "\",\"" + callback_url + "\",\"" + id + "\",\"Funding CSV\"], \"id\": 1}";
            Response res = BTCService.SendPostByWebRequest(url, base64String, tonce, postData);
            return res;
        }

        public static Response SendPostByWebRequest(string url, string base64, string tonce, string postData)
        {
            WebRequest webRequest = WebRequest.Create(url);
            //WebRequest webRequest = HttpWebRequest.Create(url);
            if (webRequest == null)
            {
                Console.WriteLine("Failed to create web request for url: " + url);
                return null;
            }

            byte[] bytes = Encoding.ASCII.GetBytes(postData);

            webRequest.Method = "POST";
            webRequest.ContentType = "application/json-rpc";
            webRequest.ContentLength = bytes.Length;
            webRequest.Headers["Authorization"] = "Basic " + base64;
            webRequest.Headers["Json-Rpc-Tonce"] = tonce;
            try
            {
                // Send the json authentication post request
                using (Stream dataStream = webRequest.GetRequestStream())
                {
                    dataStream.Write(bytes, 0, bytes.Length);
                    dataStream.Close();
                }
                // Get authentication response
                using (WebResponse response = webRequest.GetResponse())
                {
                    using (var stream = response.GetResponseStream())
                    {
                        using (var reader = new StreamReader(stream))
                        {
                            var ss = reader.ReadToEnd();
                            Response m = JsonConvert.DeserializeObject<Response>(ss);
                            return m;
                        }
                    }
                }
            }
            catch (WebException ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public static string BuildQueryString(NameValueCollection parameters)
        {
            List<string> keyValues = new List<string>();
            foreach (string key in parameters)
            {
                keyValues.Add(key + "=" + parameters[key]);
            }
            var ss = String.Join("&", keyValues.ToArray());
            return String.Join("&", keyValues.ToArray());
        }

        public static string GetHMACSHA1Hash(string secret_key, string input)
        {
            HMACSHA1 hmacsha1 = new HMACSHA1(Encoding.ASCII.GetBytes(secret_key));
            MemoryStream stream = new MemoryStream(Encoding.ASCII.GetBytes(input));
            byte[] hashData = hmacsha1.ComputeHash(stream);

            // Format as hexadecimal string.
            StringBuilder hashBuilder = new StringBuilder();
            foreach (byte data in hashData)
            {
                hashBuilder.Append(data.ToString("x2"));
            }
            return hashBuilder.ToString();
        }
    }
}