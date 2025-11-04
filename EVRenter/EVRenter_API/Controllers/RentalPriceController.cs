using EVRenter_Service.RequestModel;
using EVRenter_Service.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EVRenter_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RentalPriceController : ControllerBase
    {
        private readonly IRentalPriceService _priceService;

        public RentalPriceController(IRentalPriceService priceService)
        {
            _priceService = priceService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPrices()
        {
            var response = await _priceService.GetAllRentalPrice();
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPriceById(int id)
        {


            var price = await _priceService.GetPriceByIdAsync(id);
            if (price == null)
            {
                return NotFound("Price not found.");
            }

            return Ok(price);
        }

        [HttpGet("{modelId}")]
        public async Task<IActionResult> GetPriceByModelId(int modelId)
        {


            var price = await _priceService.GetPriceByModelAsync(modelId);
            if (price == null)
            {
                return NotFound("Price not found.");
            }

            return Ok(price);
        }

        [HttpPost]
        //[Authorize(Roles = "Manager")]
        public async Task<IActionResult> CreatePrice([FromForm] PriceRequestModel request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var price = await _priceService.CreatePriceAsync(request);
            return CreatedAtAction(nameof(GetPriceById), new { id = price.Id }, price);
        }

        [HttpPut]
        //[Authorize]
        public async Task<IActionResult> UpdatePrice([FromForm] PriceUpdateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var updatedPrice = await _priceService.UpdatePriceByModelAsync(request);
            if (updatedPrice == null)
            {
                return NotFound("Price not found.");
            }

            return Ok(updatedPrice);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteModel(int id)
        {
            var result = await _priceService.DeletePriceAsync(id);
            if (!result)
            {
                return NotFound("Price not found.");
            }

            return NoContent();
        }
    }
}
