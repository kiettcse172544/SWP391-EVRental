namespace EVRenter_Service.RequestModel
{
    public class PaymentCreateRequest
    {
        /// <summary>
        /// ID của booking cần thanh toán
        /// </summary>
        public int BookingId { get; set; }

        /// <summary>
        /// ID người thanh toán (user)
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Số tiền thanh toán
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Phương thức thanh toán: "Cash" hoặc "VnPay"
        /// </summary>
        public string PaymentMethod { get; set; } = string.Empty;

        /// <summary>
        /// Ghi chú (nếu có)
        /// </summary>
        public string? Note { get; set; }
    }
}
