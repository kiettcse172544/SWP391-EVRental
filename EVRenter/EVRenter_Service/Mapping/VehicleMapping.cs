using AutoMapper;
using EVRenter_Data.Entities;
using EVRenter_Service.RequestModel;
using EVRenter_Service.ResponseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace EVRenter_Service.Mapping
{
    public class VehicleMapping : Profile
    {
        public VehicleMapping()
        {
            CreateMap<Model, CarSpecifications>()
                .ForMember(dest => dest.ChargingTime, opt => opt.MapFrom(src => src.ChargingTime))
                .ForMember(dest => dest.ChargePower, opt => opt.MapFrom(src => src.ChargePower))
                .ForMember(dest => dest.Seat, opt => opt.MapFrom(src => src.Seat));
            CreateMap<Vehicle, VehicleResponseModel>()
                .ForMember(dest => dest.ModelName, opt => opt.MapFrom(src => src.Model.ModelName))
                .ForMember(dest => dest.StationName, opt => opt.MapFrom(src => src.Station.Name))
                .ForMember(dest => dest.Specifications,
                opt => opt.MapFrom(src => src.Model))

                .ForMember(dest => dest.Customer,
                otp => otp.MapFrom(src => src.Bookings
                .Where(x => x.VehicleID == src.Id && x.Status < 5 && !x.IsDelete)
                .Select(x => x.User)
                .FirstOrDefault()))

                .ForMember(dest => dest.Booking, otp => otp.MapFrom(src =>
                src.Bookings.Where(x => x.VehicleID == src.Id && !x.IsDelete && x.Status < 5).FirstOrDefault()))

                .ForMember(dest => dest.Categories, opt => opt.MapFrom(src => src.CarItems
                .GroupBy(i => new { i.CategoryID, i.Category.Name })
                .Select(g => new CategoryChecklistResponse
                {
                    CategoryName = g.Key.Name,
                    Items = g.Select(i => new CarItemResponse
                    {
                        Id = i.Id,
                        Name = i.Name,
                        Status = i.Status
                    }).ToList()
                }).ToList()
                ));


            CreateMap<Vehicle, VehicleDetailResponseModel>()
               .ForMember(dest => dest.ModelName, opt => opt.MapFrom(src => src.Model.ModelName))
               .ForMember(dest => dest.StationName, opt => opt.MapFrom(src => src.Station.Name))
               .ForMember(dest => dest.StationLocation, opt => opt.MapFrom(src => src.Station.Location))
               .ForMember(dest => dest.Categories, opt => opt.MapFrom(src => src.CarItems
                .GroupBy(i => new { i.CategoryID, i.Category.Name })
                .Select(g => new CategoryChecklistResponse
                {
                    CategoryName = g.Key.Name,
                    Items = g.Select(i => new CarItemResponse
                    {
                        Id = i.Id,
                        Name = i.Name,
                        Status = i.Status
                    }).ToList()
                }).ToList()
                ));
            CreateMap<VehicleRequestModel, Vehicle>();
            CreateMap<VehicleUpdateRequest, Vehicle>();
        }
    }
}
