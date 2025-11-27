using EVRenter_Service.RequestModel;
using EVRenter_Service.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EVRenter_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExtraFeeController : ControllerBase
    {
        private readonly IExtraFeeService _extraFeeService;
        private readonly ILogger<ExtraFeeController> _logger;

        public ExtraFeeController(IExtraFeeService extraFeeService, ILogger<ExtraFeeController> logger)
        {
            _extraFeeService = extraFeeService;
            _logger = logger;
        }

        
        [HttpPost("create")]
        [Authorize]
        public async Task<IActionResult> CreateExtraFee([FromBody] ExtraFeeCreateRequest request)
        {
            try
            {
                var result = await _extraFeeService.CreateExtraFeeAsync(request);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Booking not found");
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating ExtraFee");
                return StatusCode(500, new { message = "Internal server error", detail = ex.Message });
            }
        }

        
        [HttpGet("all")]
        [Authorize]
        public async Task<IActionResult> GetAllExtraFees()
        {
            try
            {
                var fees = await _extraFeeService.GetAllExtraFeesAsync();
                if (fees == null || !fees.Any())
                    return NotFound(new { message = "No extra fees found." });

                return Ok(fees);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching all extra fees");
                return StatusCode(500, new { message = "Internal server error", detail = ex.Message });
            }
        }

        
        [HttpGet("booking/{bookingId}")]
        [Authorize]
        public async Task<IActionResult> GetExtraFeesByBooking(int bookingId)
        {
            try
            {
                var fees = await _extraFeeService.GetExtraFeesByBookingAsync(bookingId);
                if (fees == null || !fees.Any())
                    return NotFound(new { message = "No extra fees found for this booking." });

                return Ok(fees);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching extra fees by booking ID");
                return StatusCode(500, new { message = "Internal server error", detail = ex.Message });
            }
        }
    }
}
