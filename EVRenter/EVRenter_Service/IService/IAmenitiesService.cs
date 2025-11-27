using EVRenter_Service.RequestModel;
using EVRenter_Service.ResponseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVRenter_Service.IService
{
    public interface IAmenitiesService
    {
        Task<IEnumerable<AmenitiesResponseModel>> GetAllAmenitiesByModel(int modelID);
        Task<IEnumerable<AmenitiesResponseModel>> CreateAmenities(AmenitiesRequestModel request);
    }
}
