namespace EVRenter_Service.ResponseModel
{
    public class PaymentResponseModel
    {
        public int Id { get; set; }
        public int BookingId { get; set; }
        public string UserEmail { get; set; } = string.Empty;
        public string PaymentMethod { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime? PaymentTime { get; set; }
        public string? PaymentUrl { get; set; }
        public string? TransactionId { get; set; }
        public string? ReferenceCode { get; set; }
        public string? Note { get; set; }
    }
}
