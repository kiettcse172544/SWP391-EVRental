namespace EVRenter_Service.Service
{
    /// <summary>
    /// Class cấu hình dùng để bind section "VnPay" trong appsettings.json.
    /// </summary>
    public class VnPayOptions
    {
        public string TmnCode { get; set; } = string.Empty;
        public string HashSecret { get; set; } = string.Empty;
        public string PayUrl { get; set; } = string.Empty;
        public string ReturnUrl { get; set; } = string.Empty;
        public string IpnUrl { get; set; } = string.Empty;
    }
}
