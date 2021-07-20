#if !NET20 && !NET30 && !NET35 && !NET40
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NeoSmart.PayPalNvp
{
    public partial class PayPal
    {
        private Task<string> PostAsync(string url, string postData, CancellationToken cancel)
        {
            return PostAsync(url, postData, cancel: default);
        }

        private async Task<string> PostAsync(string url, string postData, string contentType = "application/x-www-form-urlencoded", CancellationToken cancel = default)
        {
            using (var client = new System.Net.Http.HttpClient())
            using (var httpContent = new System.Net.Http.StringContent(postData, Encoding.UTF8, contentType))
            using (var response = await client.PostAsync(url, httpContent, cancel))
            {
                return await response.Content.ReadAsStringAsync();
            }
        }

        public async Task<Dictionary<string, string>> GenericNvpAsync(string method, Dictionary<string, string> fields, CancellationToken cancel = default)
        {
            // Add some default PayPal-specific values
            fields["USER"] = _user;
            fields["PWD"] = _password;
            fields["SIGNATURE"] = _signature;
            fields["VERSION"] = NvpVersion.ToString();
            fields["METHOD"] = method;

            string nvpstr = EncodeNvpString(fields);

            // Send the POST request to PayPal
            var response = await PostAsync(NvpEndPoint, nvpstr, cancel);

            return DecodeNvpString(response);
        }

        // Start of PayPal convenience methods
        public async Task<Dictionary<string, string>> SetExpressCheckoutAsync(Dictionary<string, string> fields, CancellationToken cancel = default)
        {
            const string method = "SetExpressCheckout";
            return await GenericNvpAsync(method, fields, cancel);
        }

        public async Task<Dictionary<string, string>> GetExpressCheckoutDetailsAsync(Dictionary<string, string> fields, CancellationToken cancel = default)
        {
            const string method = "GetExpressCheckoutDetails";
            return await GenericNvpAsync(method, fields, cancel);
        }

        public async Task<Dictionary<string, string>> DoExpressCheckoutPaymentAsync(Dictionary<string, string> fields, CancellationToken cancel = default)
        {
            const string method = "DoExpressCheckoutPayment";
            return await GenericNvpAsync(method, fields, cancel);
        }

        public async Task<Dictionary<string, string>> RefundTransactionAsync(Dictionary<string, string> fields, CancellationToken cancel = default)
        {
            const string method = "RefundTransaction";
            return await GenericNvpAsync(method, fields, cancel);
        }

        public async Task<Dictionary<string, string>> DoDirectPaymentAsync(Dictionary<string, string> fields, CancellationToken cancel = default)
        {
            const string method = "DoDirectPayment";
            return await GenericNvpAsync(method, fields, cancel);
        }

        public async Task<Dictionary<string, string>> DoCaptureAsync(Dictionary<string, string> fields, CancellationToken cancel = default)
        {
            const string method = "DoCapture";
            return await GenericNvpAsync(method, fields, cancel);
        }

        public async Task<Dictionary<string, string>> DoVoidAsync(Dictionary<string, string> fields, CancellationToken cancel = default)
        {
            const string method = "DoVoid";
            return await GenericNvpAsync(method, fields, cancel);
        }

        public async Task<Dictionary<string, string>> GetTransactionDetailsAsync(Dictionary<string, string> fields, CancellationToken cancel = default)
        {
            const string method = "GetTransactionDetails";
            return await GenericNvpAsync(method, fields, cancel);
        }
    }
}
#endif
