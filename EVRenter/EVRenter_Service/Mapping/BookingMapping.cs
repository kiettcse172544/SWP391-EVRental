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
    public class BookingMapping : Profile
    {
        public BookingMapping()
        {
            CreateMap<Booking, BookingResponseModel>();
                //.ForMember(dest => dest.Vehicle, opt => opt.MapFrom(src => src.Vehicle));
            CreateMap<Booking, StaffBookingResponseModel>()
                .ForMember(dest => dest.Customer, otp => otp.MapFrom(src => src.User))
                .ForMember(dest => dest.RequestTime, otp => otp.MapFrom(src => src.CreatedAt));
            CreateMap<BookingRequestModel, Booking>();
        }
    }
}
