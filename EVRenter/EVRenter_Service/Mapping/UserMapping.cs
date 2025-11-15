using AutoMapper;
using EVRenter_Data.Entities;
using EVRenter_Service.RequestModel;
using EVRenter_Service.ResponseModel;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVRenter_Service.Mapping
{
    public class UserMapping : Profile
    {
        public UserMapping()
        {
            CreateMap<User, UserResponseModel>();
            CreateMap<User, CustomerResponseModel>()
                .ForMember(dest => dest.IdCard, otp => otp.MapFrom(src => src.RenterProfile.IDNumber))
                .ForMember(dest => dest.DriverLicense, otp => otp.MapFrom(src => src.RenterProfile.DriverLicenseNo));

            CreateMap<User, RenterResponseModel>()
                .ForMember(dest => dest.IdCard, otp => otp.MapFrom(src => src.RenterProfile.IDNumber))
                .ForMember(dest => dest.DriverLicense, otp => otp.MapFrom(src => src.RenterProfile.DriverLicenseNo))
                .ForMember(dest => dest.BookingCount, otp => otp.MapFrom(src => src.Bookings.Select(x => x.RenterID == src.Id).Count()))
                .ForMember(dest => dest.CusType, otp => otp.MapFrom(src => src.RenterProfile.Type));

            CreateMap<UserCreateRequest, User>();
            CreateMap<UserUpdateRequest, User>();
            CreateMap<RenterProfileRequest, RenterProfile>();
            CreateMap<RenterUpdateRequest, User>();
        }


    }
}
