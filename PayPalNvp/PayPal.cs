using System;
using System.Collections.Generic;
using System.Net;
using System.IO;
using System.Text;

namespace NeoSmart.PayPalNvp
{
    public class PayPal
    {
        public double NvpVersion = 88.0;
        public EndPoint EndPoint { get; set; }

        private string NvpEndPoint
        {
            get
            {
                return EndPoint == EndPoint.Production
                    ? "https://api-3t.paypal.com/nvp"
                    : "https://api-3t.sandbox.paypal.com/nvp";
            }
        }

        private readonly string _user;
        private readonly string _password;
        private readonly string _signature;

        public PayPal(EndPoint endPoint, string user, string password, string signature)
        {
            EndPoint = endPoint;
            _user = user;
            _password = password;
            _signature = signature;
        }

        public PayPal(string user, string password, string signature)
            : this(EndPoint.Production, user, password, signature)
        {
        }

        private string EncodeNvpString(Dictionary<string, string> fields)
        {
            var sb = new StringBuilder();

            foreach (var kv in fields)
            {
                string encodedValue = string.IsNullOrEmpty(kv.Value) ? string.Empty : Uri.EscapeUriString(kv.Value);
                sb.AppendFormat("{0}={1}&", Uri.EscapeUriString(kv.Key.ToUpper()), encodedValue);
            }

            return sb.ToString();
        }

        private Dictionary<string, string> DecodeNvpString(string nvpstr)
        {
            var nvpMap = new Dictionary<string, string>();

            string[] pairs = nvpstr.Split('&');
            foreach (var pair in pairs)
            {
                string[] halves = pair.Split('=');

                nvpMap[Uri.UnescapeDataString(halves[0])] = halves.Length == 2 ? Uri.UnescapeDataString(halves[1]) : string.Empty;
            }

            return nvpMap;
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
            var request = (HttpWebRequest)WebRequest.Create(NvpEndPoint);
            request.CachePolicy = new System.Net.Cache.RequestCachePolicy(System.Net.Cache.RequestCacheLevel.NoCacheNoStore);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = nvpstr.Length;
            using (var requestStream = request.GetRequestStream())
            using (var writer = new StreamWriter(requestStream))
            {
                writer.Write(nvpstr);
            }

            //Get the result
            using (var response = request.GetResponse())
            using (var responseStream = response.GetResponseStream())
            using (var reader = new StreamReader(responseStream))
            {
                nvpstr = reader.ReadToEnd();
            }

            return DecodeNvpString(nvpstr);
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

        public bool WasSuccessful(Dictionary<string, string> response)
        {
            string unused1, unused2;
            return WasSuccessful(response, out unused1, out unused2);
        }

        public bool WasSuccessful(Dictionary<string, string> response, out string shortError)
        {
            string unused;
            return WasSuccessful(response, out shortError, out unused);
        }

        public bool WasSuccessful(Dictionary<string, string> response, out string shortError, out string longError)
        {
            string ack;
            var result = false;
            if (response.TryGetValue("ACK", out ack))
            {
                result = string.Compare(response["ACK"], "success", StringComparison.CurrentCultureIgnoreCase) == 0 ||
                       string.Compare(response["ACK"], "successWithWarning", StringComparison.CurrentCultureIgnoreCase) == 0;
            }

            response.TryGetValue("L_SHORTMESSAGE0", out shortError);
            response.TryGetValue("L_LONGMESSAGE0", out longError);

            if (longError == null)
            {
                longError = string.Empty;
            }

            if (shortError == null)
            {
                shortError = string.Empty;
            }

            return result;
        }
    }
}
