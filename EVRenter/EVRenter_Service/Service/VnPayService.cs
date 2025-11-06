using Microsoft.Extensions.Options;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace EVRenter_Service.Service
{
    public class VnPayService
    {
        private readonly VnPayOptions _options;

        public VnPayService(IOptions<VnPayOptions> options)
        {
            _options = options.Value;
        }

        public string CreatePaymentUrl(string txnRef, decimal amount, string orderInfo, string ipAddr,
                               string orderType = "other", string bankCode = "NCB")
        {
            var now = DateTime.UtcNow.AddHours(7);

            string createDate = now.ToString("yyyyMMddHHmmss");
            string expireDate = now.AddMinutes(30).ToString("yyyyMMddHHmmss");

            long vnpAmount = (long)(amount * 100);

            var vnpParams = new SortedDictionary<string, string>(StringComparer.Ordinal)
            {
                ["vnp_Version"] = "2.1.0",
                ["vnp_Command"] = "pay",
                ["vnp_TmnCode"] = _options.TmnCode,
                ["vnp_Amount"] = vnpAmount.ToString(),
                ["vnp_CurrCode"] = "VND",
                ["vnp_TxnRef"] = txnRef,
                ["vnp_OrderInfo"] = orderInfo,
                ["vnp_OrderType"] = orderType,
                ["vnp_Locale"] = "vn",
                ["vnp_ReturnUrl"] = _options.ReturnUrl,
                ["vnp_IpAddr"] = ipAddr,
                ["vnp_CreateDate"] = createDate,
                ["vnp_ExpireDate"] = expireDate,
                ["vnp_BankCode"] = bankCode
            };

            // Ký trên chuỗi ĐÃ URL-ENCODE
            string rawData = BuildQuery(vnpParams, true);
            string secureHash = HmacSHA512(_options.HashSecret, rawData);

            string query = BuildQuery(vnpParams, true);

            Console.WriteLine("=== RAW DATA TO SIGN ===");
            Console.WriteLine(rawData);
            Console.WriteLine("=== SECURE HASH ===");
            Console.WriteLine(secureHash);
            Console.WriteLine("=== FINAL URL ===");
            Console.WriteLine($"{_options.PayUrl}?{query}&vnp_SecureHash={secureHash}");

            return $"{_options.PayUrl}?{query}&vnp_SecureHash={secureHash}";
        }


        public bool ValidateSignature(Dictionary<string, string> queryParams)
        {
            if (!queryParams.TryGetValue("vnp_SecureHash", out var receivedHash))
                return false;

            var filtered = queryParams
                .Where(x => x.Key.StartsWith("vnp_")
                         && x.Key != "vnp_SecureHash"
                         && x.Key != "vnp_SecureHashType")
                .OrderBy(x => x.Key, StringComparer.Ordinal)
                .ToDictionary(x => x.Key, x => x.Value);

            // ký trên chuỗi đã UrlEncode
            string rawData = BuildQuery(filtered, true);
            string computedHash = HmacSHA512(_options.HashSecret, rawData);

            return string.Equals(receivedHash, computedHash, StringComparison.OrdinalIgnoreCase);
        }


        private static string BuildQuery(IDictionary<string, string> values, bool urlEncode)
        {
            var sb = new StringBuilder();
            foreach (var kv in values)
            {
                if (sb.Length > 0) sb.Append('&');
                var key = kv.Key;
                var val = kv.Value;
                if (urlEncode)
                {
                    key = WebUtility.UrlEncode(key);
                    val = WebUtility.UrlEncode(val);
                }
                sb.Append(key).Append('=').Append(val);
            }
            return sb.ToString();
        }

        private static string HmacSHA512(string key, string data)
        {
            var keyBytes = Encoding.UTF8.GetBytes(key);
            var dataBytes = Encoding.UTF8.GetBytes(data);
            using var hmac = new HMACSHA512(keyBytes);
            var hash = hmac.ComputeHash(dataBytes);
            return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
        }
    }
}
