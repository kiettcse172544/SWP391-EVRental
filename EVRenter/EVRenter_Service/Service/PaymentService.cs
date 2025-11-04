using AutoMapper;
using EVRenter_Data.Entities;
using EVRenter_Repository.UnitOfWork;
using EVRenter_Service.RequestModel;
using EVRenter_Service.ResponseModel;
using Microsoft.EntityFrameworkCore;

namespace EVRenter_Service.Service
{
    /// <summary>
    /// Interface định nghĩa các nghiệp vụ thanh toán của hệ thống EVRenter.
    /// </summary>
    public interface IPaymentService
    {
        /// <summary>
        /// Tạo giao dịch thanh toán cho một booking.
        /// - Nếu phương thức là "Cash" → lưu DB và đánh dấu thành công.
        /// - Nếu phương thức là "VnPay" → tạo giao dịch Pending và sinh URL thanh toán.
        /// </summary>
        /// <param name="request">Thông tin yêu cầu thanh toán.</param>
        /// <param name="ipAddr">Địa chỉ IP của client (dùng để gửi cho VNPAY).</param>
        /// <returns>Thông tin PaymentResponseModel bao gồm URL thanh toán (nếu VNPAY).</returns>
        Task<PaymentResponseModel> CreatePaymentAsync(PaymentCreateRequest request, string ipAddr);

        /// <summary>
        /// Xử lý callback (IPN) từ VNPAY Sandbox.
        /// - Kiểm tra chữ ký (HMAC SHA512).
        /// - Cập nhật trạng thái thanh toán (Success / Failed).
        /// - (Tuỳ chọn) Cập nhật trạng thái Booking nếu thành công.
        /// </summary>
        /// <param name="callback">Thông tin callback từ VNPAY.</param>
        /// <returns>True nếu xử lý hợp lệ.</returns>
        Task<bool> HandleVnPayCallbackAsync(PaymentCallbackRequest callback);

        /// <summary>
        /// Lấy danh sách các giao dịch thanh toán của một người dùng.
        /// </summary>
        /// <param name="userId">ID người dùng cần xem lịch sử thanh toán.</param>
        /// <returns>Danh sách các PaymentResponseModel.</returns>
        Task<IEnumerable<PaymentResponseModel>> GetPaymentsByUserAsync(int userId);
    }

    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly VnPayService _vnPayService;

        public PaymentService(IUnitOfWork unitOfWork, IMapper mapper, VnPayService vnPayService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _vnPayService = vnPayService;
        }

        
        // TẠO THANH TOÁN
        
        public async Task<PaymentResponseModel> CreatePaymentAsync(PaymentCreateRequest request, string ipAddr)
        {
            var booking = await _unitOfWork.Repository<Booking>()
                .AsQueryable()
                .Include(b => b.User)
                .FirstOrDefaultAsync(b => b.Id == request.BookingId && !b.IsDelete);

            if (booking == null)
                throw new KeyNotFoundException("Booking not found.");

            var payment = _mapper.Map<Payment>(request);
            payment.Status = PaymentStatus.Pending;
            payment.PaymentTime = null;
            payment.ReferenceCode = Guid.NewGuid().ToString("N");
            payment.UserID = request.UserId;
            payment.BookingID = request.BookingId;
            payment.Amount = request.Amount;

            if (payment.PaymentType == PaymentType.Cash)
            {
                payment.Status = PaymentStatus.Success;
                payment.PaymentTime = DateTime.UtcNow;
                payment.Note = "Thanh toán tiền mặt tại quầy";

                await _unitOfWork.Repository<Payment>().InsertAsync(payment);
                await _unitOfWork.SaveChangesAsync();
                return _mapper.Map<PaymentResponseModel>(payment);
            }

            if (payment.PaymentType == PaymentType.VnPay)
            {
                string paymentUrl = _vnPayService.CreatePaymentUrl(
                    payment.ReferenceCode,
                    payment.Amount,
                    $"Thanh toán đơn hàng {payment.BookingID}",
                    ipAddr
                );

                payment.PaymentUrl = paymentUrl;

                await _unitOfWork.Repository<Payment>().InsertAsync(payment);
                await _unitOfWork.SaveChangesAsync();

                var response = _mapper.Map<PaymentResponseModel>(payment);
                response.PaymentUrl = paymentUrl;
                return response;
            }

            throw new InvalidOperationException("Unsupported payment method.");
        }

        
        // XỬ LÝ CALLBACK IPN TỪ VNPAY
        
        public async Task<bool> HandleVnPayCallbackAsync(PaymentCallbackRequest callback)
        {
            var payment = await _unitOfWork.Repository<Payment>()
                .AsQueryable()
                .FirstOrDefaultAsync(p => p.ReferenceCode == callback.vnp_TxnRef);

            if (payment == null)
                throw new KeyNotFoundException("Payment not found for this transaction.");

            if (!_vnPayService.ValidateSignature(callback.AllParams))
                throw new UnauthorizedAccessException("Invalid VNPAY signature.");

            payment.TransactionId = callback.vnp_TransactionNo;
            payment.ResponseCode = callback.vnp_ResponseCode;

            if (callback.vnp_ResponseCode == "00")
            {
                payment.Status = PaymentStatus.Success;
                payment.PaymentTime = DateTime.UtcNow;

                var booking = await _unitOfWork.Repository<Booking>()
                    .AsQueryable()
                    .FirstOrDefaultAsync(b => b.Id == payment.BookingID);
                if (booking != null)
                {
                    booking.Status = 2;
                    await _unitOfWork.Repository<Booking>().Update(booking, booking.Id);
                }
            }
            else
            {
                payment.Status = PaymentStatus.Failed;
            }

            await _unitOfWork.Repository<Payment>().Update(payment, payment.Id);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        
        // LẤY DANH SÁCH THANH TOÁN
       
        public async Task<IEnumerable<PaymentResponseModel>> GetPaymentsByUserAsync(int userId)
        {
            var payments = await _unitOfWork.Repository<Payment>()
                .AsQueryable()
                .Include(p => p.User)
                .Where(p => p.UserID == userId && !p.IsDelete)
                .ToListAsync();

            return _mapper.Map<IEnumerable<PaymentResponseModel>>(payments);
        }
    }
}
