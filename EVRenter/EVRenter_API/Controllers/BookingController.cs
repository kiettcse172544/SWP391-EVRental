using EVRenter_Service.RequestModel;
using EVRenter_Service.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EVRenter_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public BookingController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllBookings()
        {
            var response = await _bookingService.GetAllBooking();
            return Ok(response);
        }

        [HttpGet("Unapproval Booking")]
        public async Task<IActionResult> GetUnapprovalBookings()
        {
            var response = await _bookingService.GetUnapprovalBooking();
            return Ok(response);
        }

        [HttpGet("GetBookingByStaff")]
        public async Task<IActionResult> GetBookingsByStaff()  
        {
            var response = await _bookingService.GetBookingsbyStaff();
            return Ok(response);
        }

        [HttpGet("GetByUserID/{userID}")]
        public async Task<IActionResult> GetBookingsByUserID(int userID)
        {
            var response = await _bookingService.GetBookingbyUserId(userID);
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBookingById(int id)
        {


            var vehicle = await _bookingService.GetBookingByIdAsync(id);
            if (vehicle == null)
            {
                return NotFound("Booking not found.");
            }

            return Ok(vehicle);
        }
        [HttpGet("GetByCar/{vehicleID}")]
        public async Task<IActionResult> GetBookingByCar(int vehicleID)
        {


            var vehicle = await _bookingService.GetBookingByVehicleAsync(vehicleID);
            if (vehicle == null)
            {
                return NotFound("Booking not found.");
            }

            return Ok(vehicle);
        }

        [HttpPost]
        //[Authorize(Roles = "Manager")]
        public async Task<IActionResult> CreateVehicle([FromForm] BookingRequestModel request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var booking = await _bookingService.CreateBookingAsync(request);
            return CreatedAtAction(nameof(GetBookingById), new { id = booking.Id }, booking);
        }

        [HttpPut("{id}")]
        //[Authorize]
        public async Task<IActionResult> UpdateBookingStatus(int id, [FromForm] BookingUpdateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var updatedBooking = await _bookingService.UpdateBookingStatsusAsync(id, request);
            if (updatedBooking == null)
            {
                return NotFound("User not found.");
            }

            return Ok(updatedBooking);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUnpaidBooking(int id)
        {
            var result = await _bookingService.DeleteUnpaidBookingAsync(id);
            if (!result)
            {
                return NotFound("Booking not found.");
            }

            return NoContent();
        }
    }
}