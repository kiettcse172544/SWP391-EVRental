using AutoMapper;
using EVRenter_Data.Entities;
using EVRenter_Repository.UnitOfWork;
using EVRenter_Service.RequestModel;
using EVRenter_Service.ResponseModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using EVRenter_Service.IService;
using EVRenter_Repository.Repositories.Payment;

namespace EVRenter_Service.Service
{
    
    

    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly VnPayService _vnPayService;
        private readonly IPaymentRepository _paymentRepository;

        private readonly ILogger<PaymentService> _logger;

        public PaymentService(IUnitOfWork unitOfWork, IMapper mapper, VnPayService vnPayService, ILogger<PaymentService> logger, IPaymentRepository paymentRepository)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _vnPayService = vnPayService;
            _paymentRepository = paymentRepository;
            _logger = logger;
        }

        
        // TẠO THANH TOÁN
        
        public async Task<PaymentResponseModel> CreatePaymentAsync(PaymentCreateRequest request, string ipAddr)
        {
            var booking = await _paymentRepository.GetBookingByIdAsync(request.BookingId);

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

                await _paymentRepository.UpdateBookingAsync(booking);
                await _paymentRepository.AddPaymentAsync(payment);
                
                
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

                await _paymentRepository.AddPaymentAsync(payment);
                

                

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

            var payment = await _paymentRepository.GetPaymentByRefCode(refCode);

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

                var booking = await _paymentRepository.GetBookingByIdAsync(payment.BookingID);
                if (booking != null)
                {
                    booking.Status = 1;
                    await _paymentRepository.UpdateBookingAsync(booking);
                }
            }
            else
            {
                payment.Status = PaymentStatus.Failed;
            }

            await _paymentRepository.UpdatePaymentAsync(payment);
            

            return true;
        }

        
        // LẤY DANH SÁCH THANH TOÁN
       
        public async Task<IEnumerable<PaymentResponseModel>> GetPaymentsByUserAsync(int userId)
        {
            var payments = await _paymentRepository.GetPaymentByUserAsync(userId);

            return _mapper.Map<IEnumerable<PaymentResponseModel>>(payments);
        }

        public async Task<IEnumerable<Payment>> GetAllPaymentsAsync()
        {
            var payments = await _paymentRepository.GetPayments();



            return payments;
        }


        public async Task Update(int bookingId, int paymentId)
        {
            var payment = await _paymentRepository.GetPaymentById(paymentId);
            var booking = await _paymentRepository.GetBookingByIdAsync(bookingId);

            if (booking != null)
            {
                booking.Status = 1;
                await _paymentRepository.UpdateBookingAsync(booking);
            }

            if (payment != null)
            {
                payment.Status = PaymentStatus.Success;
                payment.PaymentTime = DateTime.UtcNow;
                await _paymentRepository.UpdatePaymentAsync(payment);
            }


        }
    }
}
