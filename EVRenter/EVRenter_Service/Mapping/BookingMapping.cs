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
            CreateMap<Booking, BookingResponseModel>()
                .ForMember(dest => dest.StationID, opt => opt.MapFrom(src => src.Vehicle.StationID))
                .ForMember(dest => dest.StationName, opt => opt.MapFrom(src => src.Vehicle.Station.Name))
                .ForMember(dest => dest.Overdue, opt => opt.MapFrom(src => src.Overdue))
                ;
            
            CreateMap<Booking, StaffBookingResponseModel>()
                .ForMember(dest => dest.Overdue, opt => opt.MapFrom(src => src.Overdue))

                .ForMember(dest => dest.Customer, otp => otp.MapFrom(src => src.User))

                .ForMember(dest => dest.RequestTime, otp => otp.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.RentalTime, opt => opt.MapFrom(src => (int)(src.EndDate - src.StartDate).TotalDays))

                .ForMember(dest => dest.StationID, opt => opt.MapFrom(src => src.Vehicle.StationID))
                .ForMember(dest => dest.StationName, opt => opt.MapFrom(src => src.Vehicle.Station.Name))

                .ForMember(dest => dest.Vehicle, opt => opt.MapFrom(src => src.Vehicle));

            CreateMap<Booking, CarBookingResponseModel>()
                .ForMember(dest => dest.RequestTime, otp => otp.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.RentalTime, opt => opt.MapFrom(src => (int)(src.EndDate - src.StartDate).TotalDays));
            CreateMap<BookingRequestModel, Booking>();
        }
    }
}
