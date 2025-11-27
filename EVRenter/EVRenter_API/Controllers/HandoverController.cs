using EVRenter_Service.RequestModel;
using EVRenter_Service.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EVRenter_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HandoverController : ControllerBase
    {
        private readonly IHandoverService _handoverService;
        private readonly ILogger<HandoverController> _logger;

        public HandoverController(IHandoverService handoverService, ILogger<HandoverController> logger)
        {
            _handoverService = handoverService;
            _logger = logger;
        }

        
        [HttpPost("create")]
        [Authorize]
        public async Task<IActionResult> CreateHandover([FromBody] HandoverCreateRequest request)
        {
            try
            {
                var result = await _handoverService.CreateHandoverAsync(request);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex.Message);
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while creating handover record.");
                return StatusCode(500, new { message = "Internal server error", detail = ex.Message });
            }
        }

        
        [HttpPut("{id}/confirm-handover")]
        [Authorize]
        public async Task<IActionResult> ConfirmHandover(int id)
        {
            try
            {
                await _handoverService.ConfirmHandoverAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex.Message);
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while updating handover status.");
                return StatusCode(500, new { message = "Internal server error", detail = ex.Message });
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAllHandovers()
        {
            var result = await _handoverService.GetAllHandoversAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetHandoverById(int id)
        {
            var result = await _handoverService.GetHandoverByIdAsync(id);
            if (result == null)
                return NotFound(new { message = $"Handover with ID {id} not found." });

            return Ok(result);
        }
    }
}
