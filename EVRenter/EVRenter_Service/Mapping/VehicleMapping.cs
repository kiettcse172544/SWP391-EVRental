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
    public class VehicleMapping : Profile
    {
        public VehicleMapping()
        {
            CreateMap<Vehicle, VehicleResponseModel>()
                .ForMember(dest => dest.ModelName, opt => opt.MapFrom(src => src.Model.ModelName))
                .ForMember(dest => dest.StationName, opt => opt.MapFrom(src => src.Station.Name))
                .ForMember(dest => dest.StationLocation, opt => opt.MapFrom(src => src.Station.Location));

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
