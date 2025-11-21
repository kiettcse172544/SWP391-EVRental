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
        public IsRenter? Renter { get; set; }
        public IsStaff? Staff { get; set; }
    }

    public class IsStaff
    {
        public int? StationID { get; set; }
        public string? StationName { get; set; }
    }

    public class IsRenter
    {
        public int? IsVerified { get; set; }

        public int? BookingCount { get; set; }
        public int? CusType { get; set; }

        public string? IdCard { get; set; }
        public byte[]? IdCardFrontImage { get; set; }
        public byte[]? IdCardBackImage { get; set; }

        public string? DriverLicense { get; set; }
        public byte[]? DriverLicenseFrontImage { get; set; }
        public byte[]? DriverLicenseBackImage { get; set; }
    }

    public class RenterResponseModel
    {
        public int Id { get; set; }

        
        public string FullName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }

        public bool IsActive { get; set; }
        public bool IsEmailVerified { get; set; }

        
        public int BookingCount { get; set; }
        public int CusType { get; set; }

        
        public int IsVerified { get; set; }

        
        public string IdCard { get; set; }              
        public byte[] IdCardFrontImage { get; set; }    
        public byte[] IdCardBackImage { get; set; }      

        
        public string DriverLicense { get; set; }
        public byte[] DriverLicenseFrontImage { get; set; }
        public byte[] DriverLicenseBackImage { get; set; }
    }


    public class CustomerResponseModel
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string IdCard { get; set; }
        public string DriverLicense { get; set; }
        public string Address { get; set; }
    }

    public class StaffResponseModel
    {
        public int Id { get; set; }

        public string FullName { get; set; }
        public string Email { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }

        public bool IsActive { get; set; }
        public bool IsEmailVerified { get; set; }

        public IsStaff? Staff { get; set; }

    }
}
