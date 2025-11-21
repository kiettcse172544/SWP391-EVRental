using AutoMapper;
using EVRenter_Data.Entities;
using EVRenter_Service.RequestModel;
using EVRenter_Service.ResponseModel;

namespace EVRenter_Service.Mapping
{
    public class UserMapping : Profile
    {
        public UserMapping()
        {
            
            CreateMap<User, UserResponseModel>()
                .ForMember(dest => dest.Renter, opt => opt.MapFrom(src => src))
                .ForMember(dest => dest.Staff, opt => opt.MapFrom(src => src));


            CreateMap<User, IsRenter>()
                .ForMember(dest => dest.IsVerified, opt => opt.MapFrom(src => src.IsVerified))

                .ForMember(dest => dest.BookingCount,opt => opt.MapFrom(src => src.Bookings.Count(b => b.RenterID == src.Id)))
                .ForMember(dest => dest.CusType, opt => opt.MapFrom(src => src.RenterProfile.Type))

                .ForMember(dest => dest.IdCardFrontImage,
                    opt => opt.MapFrom(src =>
                        src.RenterProfile.IDImages
                        .Where(i => i.Type == 1)
                        .Select(i => i.Image.Base64Image)
                        .FirstOrDefault()
                    ))
                .ForMember(dest => dest.IdCardBackImage,
                    opt => opt.MapFrom(src =>
                        src.RenterProfile.IDImages
                        .Where(i => i.Type == 2)
                        .Select(i => i.Image.Base64Image)
                        .FirstOrDefault()
                    ))

                .ForMember(dest => dest.DriverLicenseFrontImage,
                    opt => opt.MapFrom(src =>
                        src.RenterProfile.DriverLicenseImages
                        .Where(i => i.Type == 1)
                        .Select(i => i.Image.Base64Image)
                        .FirstOrDefault()
                    ))

                .ForMember(dest => dest.DriverLicenseBackImage,
                    opt => opt.MapFrom(src =>
                        src.RenterProfile.DriverLicenseImages
                        .Where(i => i.Type == 2)
                        .Select(i => i.Image.Base64Image)
                        .FirstOrDefault()
                    ));

            CreateMap<User, IsStaff>()
                .ForMember(dest => dest.StationID,
                    opt => opt.MapFrom(src => src.StaffProfile.StationID))
                .ForMember(dest => dest.StationName,
                    opt => opt.MapFrom(src => src.StaffProfile.Station.Name));

            CreateMap<StaffProfileRequest, StaffProfile>();


            CreateMap<User, CustomerResponseModel>()
                .ForMember(dest => dest.Id,
                    opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.IdCard,
                    opt => opt.MapFrom(src => src.RenterProfile.IDNumber))
                .ForMember(dest => dest.DriverLicense,
                    opt => opt.MapFrom(src => src.RenterProfile.DriverLicenseNo));


            CreateMap<User, RenterResponseModel>()

               

                .ForMember(dest => dest.IdCard,
                    opt => opt.MapFrom(src => src.RenterProfile.IDNumber))
                .ForMember(dest => dest.DriverLicense,
                    opt => opt.MapFrom(src => src.RenterProfile.DriverLicenseNo))


                
                .ForMember(dest => dest.BookingCount,
                    opt => opt.MapFrom(src =>
                        src.Bookings.Count(b => b.RenterID == src.Id)))

                
                .ForMember(dest => dest.CusType,
                    opt => opt.MapFrom(src => src.RenterProfile.Type))

                
                .ForMember(dest => dest.IsVerified,
                    opt => opt.MapFrom(src => src.IsVerified))

                
                .ForMember(dest => dest.IdCardFrontImage,
                    opt => opt.MapFrom(src =>
                        src.RenterProfile.IDImages
                        .Where(i => i.Type == 1)
                        .Select(i => i.Image.Base64Image)
                        .FirstOrDefault()
                    ))

                .ForMember(dest => dest.IdCardBackImage,
                    opt => opt.MapFrom(src =>
                        src.RenterProfile.IDImages
                        .Where(i => i.Type == 2)
                        .Select(i => i.Image.Base64Image)
                        .FirstOrDefault()
                    ))

                
                .ForMember(dest => dest.DriverLicenseFrontImage,
                    opt => opt.MapFrom(src =>
                        src.RenterProfile.DriverLicenseImages
                        .Where(i => i.Type == 1)
                        .Select(i => i.Image.Base64Image)
                        .FirstOrDefault()
                    ))

                .ForMember(dest => dest.DriverLicenseBackImage,
                    opt => opt.MapFrom(src =>
                        src.RenterProfile.DriverLicenseImages
                        .Where(i => i.Type == 2)
                        .Select(i => i.Image.Base64Image)
                        .FirstOrDefault()
                    ));
            CreateMap<User, StaffResponseModel>()
                .ForMember(dest => dest.Staff, opt => opt.MapFrom(src => src));


            CreateMap<UserCreateRequest, User>();
            CreateMap<UserUpdateRequest, User>();
            CreateMap<RenterProfileRequest, RenterProfile>();
            CreateMap<RenterUpdateRequest, User>();
        }
    }
}
