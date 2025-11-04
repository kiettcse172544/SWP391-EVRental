namespace EVRenter_Service.RequestModel
{
    public class PaymentCallbackRequest
    {
        public string vnp_TxnRef { get; set; } = string.Empty;
        public string vnp_ResponseCode { get; set; } = string.Empty;
        public string vnp_TransactionNo { get; set; } = string.Empty;
        public string vnp_Amount { get; set; } = string.Empty;
        public string vnp_SecureHash { get; set; } = string.Empty;
        public Dictionary<string, string> AllParams { get; set; } = new();
    }
}
