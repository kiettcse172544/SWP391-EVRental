using EVRenter_Service.RequestModel;
using EVRenter_Service.ResponseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVRenter_Service.IService
{
    public interface IImageService
    {
        Task<ModelImageResponse> UploadModelImageAsync(UploadModelImageRequest request);
        Task<VehicleImageResponse> UploadVehicleImageAsync(UploadVehicleImageRequest request);
        Task<IDImageResponse> UploadIDImageAsync(UploadIDImageRequest request);
        Task<DriverLicenseImageResponse> UploadDriverLicenseImageAsync(UploadDriverLicenseImageRequest request);

        Task<List<ImageResponseModel>> GetImagesByModelIdAsync(int modelId);
        Task<List<string>> GetImagesByVehicleIdAsync(int vehicleId);
        Task<List<ImageResponseModel>> GetIDImagesByRenterIdAsync(int renterId);
        Task<List<ImageResponseModel>> GetDriverLicenseImagesByRenterIdAsync(int renterId);
        Task<ImageResponseModel?> GetImageByIdAsync(int imageId);

        Task<bool> DeleteVehicleImageByBase64Async(int vehicleId, string base64Image);


    }
}
