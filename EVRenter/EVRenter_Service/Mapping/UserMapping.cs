using AutoMapper;
using EVRenter_Data.Entities;
using EVRenter_Service.RequestModel;
using EVRenter_Service.ResponseModel;
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

            CreateMap<UserCreateRequest, User>();
            CreateMap<UserUpdateRequest, User>();
        }
    }
}
