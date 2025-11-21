using AutoMapper;
using EVRenter_Data.Entities;
using EVRenter_Service.RequestModel;
using EVRenter_Service.ResponseModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EVRenter_Service.Mapping
{
    public class ModelMapping : Profile
    {
        public ModelMapping()
        {
            CreateMap<Model, ModelResponseModel>()
                .ForMember(dest => dest.Price,
                    opt => opt.MapFrom(src => new PriceResponseModel
                    {
                        Daily = src.RentalPrice != null ? src.RentalPrice.Price : 0,
                        Weekly = src.RentalPrice != null ? src.RentalPrice.Price * 7 : 0,
                        Monthly = src.RentalPrice != null ? src.RentalPrice.Price * 30 : 0
                    }))

                .ForMember(dest => dest.Deposit,
                    opt => opt.MapFrom(src => new DepositResponseModel
                    {
                        Daily = src.RentalPrice != null ? src.RentalPrice.Deposit : 0,
                        Weekly = src.RentalPrice != null ? src.RentalPrice.Deposit * 2 : 0,
                        Monthly = src.RentalPrice != null ? src.RentalPrice.Deposit * 3 : 0
                    }))

                .ForMember(dest => dest.Features,
                    opt => opt.MapFrom(src => new List<string>
                    {
                        src.Type,
                        $"{src.Range}km (NEDC)",
                        $"{src.Seat} chỗ",
                        $"Dung tích cốp {src.TrunkCapatity}L"
                    }))

                .ForMember(dest => dest.Specifications,
                    opt => opt.MapFrom(src => new SpecificationsModel
                    {
                        Seat = src.Seat,
                        Hoursepower = src.Hoursepower,
                        TrunkCapatity = src.TrunkCapatity,
                        Range = src.Range,
                        CarModel = src.Type,
                        MoveLimit = src.MoveLimit
                    }))

                .ForMember(dest => dest.Amenities,
                    opt => opt.MapFrom(src =>
                        src.Amenities
                            .Where(a => !a.IsDelete)
                            .Select(a => a.Name)
                            .ToList()
                    ))

                
                .ForMember(dest => dest.ImageBase64List,
                    opt => opt.MapFrom(src =>
                        src.ModelImages != null
                            ? src.ModelImages
                                .Where(mi => mi.Image != null && mi.Image.Base64Image != null)
                                .Select(mi => Convert.ToBase64String(mi.Image.Base64Image))
                                .ToList()
                            : new List<string>()
                    ));

            CreateMap<ModelRequestModel, Model>();
            CreateMap<ModelUpdateRequest, Model>();
        }
    }
}
