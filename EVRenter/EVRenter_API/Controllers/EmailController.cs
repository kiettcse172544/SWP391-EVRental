using EVRenter_Service.IService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EVRenter_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        [HttpPost("{bookingId}/send-signature-email")]
        public async Task<IActionResult> SendSignatureEmail(int bookingId, [FromServices] IBookingEmailService bookingEmailService)
        {
            try
            {
                var result = await bookingEmailService.SendSignatureEmailAsync(bookingId);
                if (!result)
                    return BadRequest(new { message = "Gửi email thất bại." });
                return Ok(new { message = "Đã gửi email xác nhận ký hợp đồng thành công." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("confirm-signature")]
        public async Task<IActionResult> ConfirmSignature([FromQuery] string token, [FromServices] IBookingEmailService signatureService)
        {
            try
            {
                var result = await signatureService.ConfirmSignatureAsync(token);
                if (result)
                    return Ok(new { message = "Hợp đồng đã được ký xác nhận thành công." });

                return BadRequest(new { message = "Xác nhận không thành công." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

    }
}
