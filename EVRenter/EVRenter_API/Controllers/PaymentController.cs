using EVRenter_Service.RequestModel;
using EVRenter_Service.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EVRenter_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly ILogger<PaymentController> _logger;

        public PaymentController(IPaymentService paymentService, ILogger<PaymentController> logger)
        {
            _paymentService = paymentService;
            _logger = logger;
        }

        
        // TẠO THANH TOÁN
        
        [HttpPost("create")]
        [Authorize]
        public async Task<IActionResult> CreatePayment([FromBody] PaymentCreateRequest request)
        {
            try
            {
                var clientIp = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1";

                var result = await _paymentService.CreatePaymentAsync(request, clientIp);
                _paymentService.Update(result.BookingId, result.Id);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Booking not found.");
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Unsupported payment method.");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error when creating payment.");
                return StatusCode(500, new { message = "Internal server error", detail = ex.Message });
            }
        }

      
        // XỬ LÝ IPN CALLBACK (SERVER-TO-SERVER)
        
        [HttpGet("vnpay/ipn")]
        [Authorize]
        public async Task<IActionResult> VnPayIpn()
        {
            try
            {
                var queryParams = HttpContext.Request.Query.ToDictionary(
                    kvp => kvp.Key, kvp => kvp.Value.ToString());

                _logger.LogInformation("[IPN RAW QUERY] "
                    + string.Join("&", queryParams.Select(kvp => $"{kvp.Key}={kvp.Value}")));

                var callbackRequest = new PaymentCallbackRequest
                {
                    AllParams = queryParams,
                    vnp_TxnRef = queryParams.GetValueOrDefault("vnp_TxnRef") ?? "",
                    vnp_ResponseCode = queryParams.GetValueOrDefault("vnp_ResponseCode") ?? "",
                    vnp_TransactionNo = queryParams.GetValueOrDefault("vnp_TransactionNo") ?? "",
                    vnp_SecureHash = queryParams.GetValueOrDefault("vnp_SecureHash") ?? ""
                };

                bool success = await _paymentService.HandleVnPayCallbackAsync(callbackRequest);

                return Content(success
                    ? "{\"RspCode\":\"00\",\"Message\":\"Confirm Success\"}"
                    : "{\"RspCode\":\"99\",\"Message\":\"Error processing payment\"}",
                    "application/json");
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Invalid VNPAY signature");
                return Content("{\"RspCode\":\"97\",\"Message\":\"Invalid signature\"}", "application/json");
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Payment not found");
                return Content("{\"RspCode\":\"01\",\"Message\":\"Payment not found\"}", "application/json");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during VNPAY IPN");
                return Content("{\"RspCode\":\"99\",\"Message\":\"Unknown error\"}", "application/json");
            }
        }

        
        // LỊCH SỬ THANH TOÁN THEO USER
        
        [HttpGet("user/{userId}")]
        [Authorize]
        public async Task<IActionResult> GetPaymentsByUser(int userId)
        {
            try
            {
                var payments = await _paymentService.GetPaymentsByUserAsync(userId);
                if (payments == null || !payments.Any())
                    return NotFound(new { message = "No payments found for this user" });

                return Ok(payments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching payment history");
                return StatusCode(500, new { message = "Internal server error", detail = ex.Message });
            }
        }

        [HttpGet("all")]
        [Authorize]
        public async Task<IActionResult> GetAllPayments()
        {
            try
            {
                var payments = await _paymentService.GetAllPaymentsAsync();

                if (payments == null || !payments.Any())
                    return NotFound(new { message = "No payments found" });

                return Ok(payments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching all payments");
                return StatusCode(500, new { message = "Internal server error", detail = ex.Message });
            }
        }
    }
}
