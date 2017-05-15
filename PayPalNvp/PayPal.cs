#if !NETSTANDARD1_3
using System;
using System.Collections.Generic;
using System.Net;
using System.IO;
using System.Text;

namespace NeoSmart.PayPalNvp
{
    public partial class PayPal
    {
        private string Post(string url, string postData, string contentType = "application/x-www-form-urlencoded")
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.CachePolicy = new System.Net.Cache.RequestCachePolicy(System.Net.Cache.RequestCacheLevel.NoCacheNoStore);
            request.Method = "POST";
            request.ContentType = contentType;
            request.ContentLength = postData.Length;
            using (var requestStream = request.GetRequestStream())
            using (var writer = new StreamWriter(requestStream))
            {
                writer.Write(postData);
            }

            //Get the result
            using (var response = request.GetResponse())
            using (var responseStream = response.GetResponseStream())
            using (var reader = new StreamReader(responseStream))
            {
                return reader.ReadToEnd();
            }
        }

        public Dictionary<string, string> GenericNvp(string method, Dictionary<string, string> fields)
        {
            //Add some default PayPal-specific values
            fields["USER"] = _user;
            fields["PWD"] = _password;
            fields["SIGNATURE"] = _signature;
            fields["VERSION"] = NvpVersion.ToString();
            fields["METHOD"] = method;

            string nvpstr = EncodeNvpString(fields);

            //Send the POST request to PayPal
            var response = Post(NvpEndPoint, nvpstr);

            return DecodeNvpString(response);
        }

        //Start of PayPal convenience methods
        public Dictionary<string, string> SetExpressCheckout(Dictionary<string, string> fields)
        {
            string method = "SetExpressCheckout";
            return GenericNvp(method, fields);
        }

        public Dictionary<string, string> GetExpressCheckoutDetails(Dictionary<string, string> fields)
        {
            string method = "GetExpressCheckoutDetails";
            return GenericNvp(method, fields);
        }

        public Dictionary<string, string> DoExpressCheckoutPayment(Dictionary<string, string> fields)
        {
            string method = "DoExpressCheckoutPayment";
            return GenericNvp(method, fields);
        }
        
        public Dictionary<string, string> RefundTransaction(Dictionary<string, string> fields)
        {
            string method = "RefundTransaction";
            return GenericNvp(method, fields);
        }

        public Dictionary<string, string> DoDirectPayment(Dictionary<string, string> fields)
        {
            string method = "DoDirectPayment";
            return GenericNvp(method, fields);
        }

        public Dictionary<string, string> DoCapture(Dictionary<string, string> fields)
        {
            string method = "DoCapture";
            return GenericNvp(method, fields);
        }

        public Dictionary<string, string> DoVoid(Dictionary<string, string> fields)
        {
            string method = "DoVoid";
            return GenericNvp(method, fields);
        }

        public Dictionary<string, string> GetTransactionDetails(Dictionary<string, string> fields)
        {
            string method = "GetTransactionDetails";
            return GenericNvp(method, fields);
        }
    }
}
#endif