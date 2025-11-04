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
    public class PriceMapping : Profile
    {
        public PriceMapping()
        {
            CreateMap<RentalPrice, RentalPriceResponse>();
            CreateMap<PriceRequestModel, RentalPrice>();
            CreateMap<PriceUpdateRequest,  RentalPrice>();
        }
    }
}
