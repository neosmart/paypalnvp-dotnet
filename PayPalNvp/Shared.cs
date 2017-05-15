using System;
using System.Collections.Generic;
using System.Text;

namespace NeoSmart.PayPalNvp
{
    public partial class PayPal
    {
        public double NvpVersion = 88.0;
        public EndPoint EndPoint { get; set; }

        private string NvpEndPoint => EndPoint == EndPoint.Production
            ? "https://api-3t.paypal.com/nvp"
            : "https://api-3t.sandbox.paypal.com/nvp";

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

        public static string EncodeNvpString(Dictionary<string, string> fields)
        {
            var sb = new StringBuilder();

            foreach (var kv in fields)
            {
                string encodedValue = string.IsNullOrEmpty(kv.Value) ? string.Empty : Uri.EscapeUriString(kv.Value);
                sb.AppendFormat("{0}={1}&", Uri.EscapeUriString(kv.Key.ToUpper()), encodedValue);
            }

            return sb.ToString();
        }

        public static Dictionary<string, string> DecodeNvpString(string nvpstr)
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

        public static bool WasSuccessful(Dictionary<string, string> response)
        {
            string unused1, unused2;
            return WasSuccessful(response, out unused1, out unused2);
        }

        public static bool WasSuccessful(Dictionary<string, string> response, out string shortError)
        {
            string unused;
            return WasSuccessful(response, out shortError, out unused);
        }

        public static bool WasSuccessful(Dictionary<string, string> response, out string shortError, out string longError)
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
