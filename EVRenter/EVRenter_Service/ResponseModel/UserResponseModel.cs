using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVRenter_Service.ResponseModel
{
    public class UserResponseModel
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public bool IsActive { get; set; }
        public bool IsEmailVerified { get; set; }
        public int? RoleID { get; set; }
    }

    public class RenterResponseModel
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string IdCard { get; set; }
        public string DriverLicense { get; set; }
        public string Address { get; set; }
        public bool IsActive { get; set; }
        public bool IsEmailVerified { get; set; }
        public int BookingCount { get; set; }
        public int CusType {  get; set; }

    }

    public class CustomerResponseModel
    {
        public string FullName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string IdCard { get; set; }
        public string DriverLicense { get; set; }
        public string Address { get; set; }
    }
}
