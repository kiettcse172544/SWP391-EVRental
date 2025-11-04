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
    public class ModelMapping : Profile
    {
        public ModelMapping()
        {
            CreateMap<Model, ModelResponseModel>()
                .ForMember(dest => dest.Vehicles, opt => opt.MapFrom(src => src.Vehicles.Where(v => !v.IsDelete)))
                .ForMember(dest => dest.Price,
                opt => opt.MapFrom(src => new PriceResponseModel
                {
                    Daily = src.RentalPrice != null ? src.RentalPrice.Price : 0,
                    Weekly = src.RentalPrice != null ? src.RentalPrice.Price * 7 * 0.9m : 0,
                    Monthly = src.RentalPrice != null ? src.RentalPrice.Price * 30 * 0.9m : 0
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
                }));
            CreateMap<ModelRequestModel, Model>();
            CreateMap<ModelUpdateRequest, Model>();
        }
    }
}
