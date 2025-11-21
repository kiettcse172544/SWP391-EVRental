namespace EVRenter_Service.RequestModel
{
    public class ChangePasswordRequestModelV2
    {

        public int UserId { get; set; }


        public string CurrentPassword { get; set; }


        public string NewPassword { get; set; }


        public string ConfirmNewPassword { get; set; }
    }
}
