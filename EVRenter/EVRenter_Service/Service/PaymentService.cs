using AutoMapper;
using EVRenter_Data.Entities;
using EVRenter_Repository.UnitOfWork;
using EVRenter_Service.RequestModel;
using EVRenter_Service.ResponseModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using EVRenter_Service.IService;

namespace EVRenter_Service.Service
{
    
    

    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly VnPayService _vnPayService;

        private readonly ILogger<PaymentService> _logger;

        public PaymentService(IUnitOfWork unitOfWork, IMapper mapper, VnPayService vnPayService, ILogger<PaymentService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _vnPayService = vnPayService;

            _logger = logger;
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
            payment.Amount = booking.BaseCost;

            if (payment.PaymentType == PaymentType.Cash)
            {
                payment.Status = PaymentStatus.Success;
                payment.PaymentTime = DateTime.UtcNow;
                payment.Note = "Thanh toán tiền mặt tại quầy";

                booking.Status = 1;

                await _unitOfWork.Repository<Booking>().UpdateAsync(booking);
                await _unitOfWork.Repository<Payment>().InsertAsync(payment);
                await _unitOfWork.SaveChangesAsync();
                
                return _mapper.Map<PaymentResponseModel>(payment);
            }

            if (payment.PaymentType == PaymentType.VnPay)
            {
                booking.Status = 1;
                payment.Status = PaymentStatus.Success;
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


            var refCode = callback.vnp_TxnRef.Trim();

            _logger.LogInformation($"[IPN RECEIVED] TxnRef={refCode}");

            var payment = await _unitOfWork.Repository<Payment>()
                .AsQueryable()
                .FirstOrDefaultAsync(p => p.ReferenceCode == refCode);

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
                    booking.Status = 1;
                    await _unitOfWork.Repository<Booking>().UpdateAsync(booking);
                }
            }
            else
            {
                payment.Status = PaymentStatus.Failed;
            }

            await _unitOfWork.Repository<Payment>().UpdateAsync(payment);
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

        public async Task<IEnumerable<PaymentResponseModel>> GetAllPaymentsAsync()
        {
            var payments = await _unitOfWork.Repository<Payment>()
                .AsQueryable()
                .Include(p => p.User)
                .Include(p => p.Booking)
                .Where(p => !p.IsDelete)
                .OrderByDescending(p => p.PaymentTime)        // Sắp theo thời gian thanh toán
                .ThenByDescending(p => p.Id)                  // Nếu null, sắp theo Id giảm dần
                .Select(p => new PaymentResponseModel
                {
                    Id = p.Id,
                    BookingId = p.BookingID,
                    UserEmail = p.User != null ? p.User.Email : "(Unknown)",
                    PaymentMethod = p.PaymentType == PaymentType.VnPay ? "VNPAY" :
                                    p.PaymentType == PaymentType.Cash ? "Cash" : "Other",
                    Amount = p.Amount,
                    Status = p.Status == PaymentStatus.Pending ? "Pending" :
                             p.Status == PaymentStatus.Success ? "Success" :
                             p.Status == PaymentStatus.Failed ? "Failed" : "Unknown",
                    PaymentTime = p.PaymentTime,
                    PaymentUrl = p.PaymentUrl,
                    TransactionId = p.TransactionId,
                    ReferenceCode = p.ReferenceCode,
                    Note = p.Note
                })
                .ToListAsync();

            return payments;
        }



    }
}
