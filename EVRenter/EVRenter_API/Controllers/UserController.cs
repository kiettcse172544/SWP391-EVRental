using EVRenter_Data.Entities;
using EVRenter_Service.RequestModel;
using EVRenter_Service.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EVRenter_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        // Lấy tất cả người dùng (hiện tại chỉ hỗ trợ lấy tất cả, không có lọc và phân trang)
        // Giữ lại để tương thích ngược với code cũ
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var response = await _userService.GetAllUsersAsync();
            return Ok(response);
        }

        [HttpGet("GetAllRenters")]
        public async Task<IActionResult> GetAllRentersForStaff()
        {
            var response = await _userService.GetAllRentersAsync();
            return Ok(response);
        }


        // Lấy người dùng theo ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {

            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            return Ok(user);
        }

        
        [HttpGet("GetRenter/{id}")]
        public async Task<IActionResult> GetRenterById(int id)
        {
            var user = await _userService.GetRentalByIdAsync(id);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            return Ok(user);
        }

        // Tạo người dùng mới
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromForm] UserCreateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userService.CreateUserAsync(request);
            return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, user);
        }

        // Tạo hồ sơ của renter mới
        [HttpPost("CreateRenterProfile")]
        public async Task<IActionResult> CreateRenterProfile([FromForm] RenterProfileRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userService.InitializeRenterProfileAsync(request);
            return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, user);
        }

        [HttpPost("CreateStaffProfile")]
        public async Task<IActionResult> CreateStaffProfile([FromForm] StaffProfileRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userService.InitializeStaffProfileAsync(request);
            return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, user);
        }

        // Cập nhật người dùng
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromForm] UserUpdateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var updatedUser = await _userService.UpdateUserAsync(id, request);
            if (updatedUser == null)
            {
                return NotFound("User not found.");
            }

            return Ok(updatedUser);
        }

        // Cập nhật người thue
        [HttpPut("UpdateRenter/{id}")]
        public async Task<IActionResult> UpdateRenter(int id, [FromForm] RenterUpdateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var updatedUser = await _userService.UpdateRenterAsync(id, request);
            if (updatedUser == null)
            {
                return NotFound("User not found.");
            }

            return Ok(updatedUser);
        }

        [HttpPut("UpdateStaff/{id}")]
        public async Task<IActionResult> UpdateStaff([FromForm] StaffUpdateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var updatedUser = await _userService.UpdateStaffAsync(request);
            if (updatedUser == null)
            {
                return NotFound("User not found.");
            }

            return Ok(updatedUser);
        }

        [HttpPut("RebootRenterType")]
        public async Task<IActionResult> RebootRenterType()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var updatedUser = await _userService.RebootRenterType();

            return Ok();
        }

        // Xóa người dùng
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var result = await _userService.DeleteUserAsync(id);
            if (!result)
            {
                return NotFound("User not found.");
            }

            return NoContent();
        }

        [HttpPut("UpdateVerifiedStatus/{id}/{status}")]
        public async Task<IActionResult> UpdateVerifiedStatus(int id, int status)
        {
            try
            {
                bool check = await _userService.UpdateVerifiedStatus(id, status);
                return Ok(new { Message = "Succsessfull"} );
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}
