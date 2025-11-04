namespace EVRenter_Service.ResponseModel
{
    public class LoginResponseModel
    {
        public int UserId { get; set; }
        public string Email { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string Role { get; set; } = null!;
        public int? StationId { get; set; } = null!;
        public string Verified { get; set; } = null!;
        public string Phone {  get; set; } = null!;
        public string Token { get; set; } = null!;

        
    }
}
