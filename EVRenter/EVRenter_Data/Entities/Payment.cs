using Microsoft.EntityFrameworkCore;
using System;

namespace EVRenter_Data.Entities
{
    public enum PaymentType
    {
        Cash = 1,   // Thanh toán tiền mặt tại quầy
        VnPay = 2   // Thanh toán online qua VNPAY
    }

    public enum PaymentStatus
    {
        Pending = 0, // Chờ thanh toán
        Success = 1, // Thành công
        Failed = 2   // Thất bại
    }

    public class Payment : BaseEntity
    {
        public int BookingID { get; set; }
        public int UserID { get; set; }

        /// <summary>
        /// Loại thanh toán (1 = Tiền mặt, 2 = VNPAY)
        /// </summary>
        public PaymentType PaymentType { get; set; }

        /// <summary>
        /// Mã giao dịch từ VNPAY (null nếu thanh toán tiền mặt)
        /// </summary>
        public string? TransactionId { get; set; }

        /// <summary>
        /// Mã tham chiếu (vnp_TxnRef) để đối soát callback VNPAY
        /// </summary>
        public string? ReferenceCode { get; set; }

        /// <summary>
        /// URL thanh toán (nếu có, dành cho VNPAY)
        /// </summary>
        public string? PaymentUrl { get; set; }

        /// <summary>
        /// Thời điểm thanh toán thành công (null nếu chưa thanh toán)
        /// </summary>
        public DateTime? PaymentTime { get; set; }

        [Precision(18, 2)]
        public decimal Amount { get; set; }

        /// <summary>
        /// Trạng thái thanh toán (0 = Pending, 1 = Success, 2 = Failed)
        /// </summary>
        public PaymentStatus Status { get; set; }

        /// <summary>
        /// Mã phản hồi từ VNPAY (VD: "00" = Thành công)
        /// </summary>
        public string? ResponseCode { get; set; }

        /// <summary>
        /// Ghi chú thêm (VD: "Thanh toán tiền mặt tại quầy")
        /// </summary>
        public string? Note { get; set; }

        // Navigation properties
        public virtual Booking Booking { get; set; }
        public virtual User User { get; set; }
    }
}
