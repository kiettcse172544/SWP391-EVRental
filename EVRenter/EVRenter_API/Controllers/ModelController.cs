using EVRenter_Service.RequestModel;
using EVRenter_Service.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EVRenter_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ModelController : ControllerBase
    {
        private readonly IModelService _modelService;

        public ModelController(IModelService modelService)
        {
            _modelService = modelService;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAllModels()
        {
            var response = await _modelService.GetAllModel();
            return Ok(response);
        }

        [HttpGet("SyncQuantity")]
        public async Task<IActionResult> SyncQuantity()
        {
            await _modelService.RebootModelQuantitiesAsync();
            return Ok();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetModelById(int id)
        {


            var model = await _modelService.GetModelByIdAsync(id);
            if (model == null)
            {
                return NotFound("Model not found.");
            }

            return Ok(model);
        }

        [HttpGet("{stationId}")]
        public async Task<IActionResult> GetModelsByStation(int stationId)
        {


            var model = await _modelService.GetModelByStationAsync(stationId);
            if (model == null)
            {
                return NotFound("Model not found.");
            }

            return Ok(model);
        }

        [HttpPost]
        //[Authorize(Roles = "Manager")]
        public async Task<IActionResult> CreateModel([FromForm] ModelRequestModel request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var model = await _modelService.CreateModelAsync(request);
            return CreatedAtAction(nameof(GetModelById), new { id = model.Id }, model);
        }

        [HttpPut("{id}")]
        //[Authorize]
        public async Task<IActionResult> UpdateModel(int id, [FromForm] ModelUpdateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var updatedModel = await _modelService.UpdateModelAsync(id, request);
            if (updatedModel == null)
            {
                return NotFound("Model not found.");
            }

            return Ok(updatedModel);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteModel(int id)
        {
            var result = await _modelService.DeleteModelAsync(id);
            if (!result)
            {
                return NotFound("Model not found.");
            }

            return NoContent();
        }
    }
}
