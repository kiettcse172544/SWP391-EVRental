using EVRenter_Data.Entities;
using EVRenter_Repository.UnitOfWork;
using EVRenter_Service.RequestModel;
using EVRenter_Service.ResponseModel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EVRenter_Service.IService;

namespace EVRenter_Service.Service
{
    

    public class ImageService : IImageService
    {
        private readonly IUnitOfWork _unitOfWork;
        private const string  CONTENT_1 = "Căn cước công dân mặt trước";
        private const string CONTENT_2 = "Căn cước công dân mặt sau";
        private const string CONTENT_3 = "Bằng lái xe mặt trước";
        private const string CONTENT_4 = "Bằng lái xe mặt sau";
        public ImageService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        
        public async Task<ModelImageResponse> UploadModelImageAsync(UploadModelImageRequest request)
        {
            bool modelExists = await _unitOfWork.Repository<Model>()
                .AsQueryable()
                .AnyAsync(m => m.Id == request.ModelID && !m.IsDelete);

            if (!modelExists)
                throw new KeyNotFoundException("Model not found.");

            var image = new Image
            {
                ContentType = request.ContentType,
                Base64Image = request.ImageData
            };

            await _unitOfWork.Repository<Image>().AddAsync(image);
            await _unitOfWork.SaveChangesAsync();

            var relation = new ModelImage
            {
                ModelID = request.ModelID,
                ImageID = image.Id
            };

            await _unitOfWork.Repository<ModelImage>().AddAsync(relation);
            await _unitOfWork.SaveChangesAsync();

            return new ModelImageResponse
            {
                ModelID = request.ModelID,
                ImageID = image.Id,
                ImageContentType = image.ContentType
            };
        }



        public async Task<VehicleImageResponse> UploadVehicleImageAsync(UploadVehicleImageRequest request)
        {
            bool vehicleExists = await _unitOfWork.Repository<Vehicle>()
                .AsQueryable()
                .AnyAsync(v => v.Id == request.VehicleID && !v.IsDelete);

            if (!vehicleExists)
                throw new KeyNotFoundException("Vehicle not found.");

            var image = new Image
            {
                ContentType = request.ContentType,
                Base64Image = request.ImageData 
            };

            await _unitOfWork.Repository<Image>().AddAsync(image);
            await _unitOfWork.SaveChangesAsync();

            var relation = new VehicleImage
            {
                VehicleID = request.VehicleID,
                ImageID = image.Id
            };

            await _unitOfWork.Repository<VehicleImage>().AddAsync(relation);
            await _unitOfWork.SaveChangesAsync();

            return new VehicleImageResponse
            {
                ImageData = image.Base64Image
            };
        }





        public async Task<IDImageResponse> UploadIDImageAsync(UploadIDImageRequest request)
        {
            bool renterExists = await _unitOfWork.Repository<RenterProfile>()
                .AsQueryable()
                .AnyAsync(r => r.Id == request.RenterID);

            if (!renterExists)
                throw new KeyNotFoundException("Renter not found.");

            var image = new Image
            {
                ContentType = request.ContentType,
                Base64Image = request.ImageData
            };

            await _unitOfWork.Repository<Image>().AddAsync(image);
            await _unitOfWork.SaveChangesAsync();

            var relation = new IDImage
            {
                RenterID = request.RenterID,
                ImageID = image.Id
            };

            await _unitOfWork.Repository<IDImage>().AddAsync(relation);
            await _unitOfWork.SaveChangesAsync();

            return new IDImageResponse
            {
                RenterID = request.RenterID,
                ImageID = image.Id,
                ImageContentType = image.ContentType
            };
        }


        
        public async Task<DriverLicenseImageResponse> UploadDriverLicenseImageAsync(UploadDriverLicenseImageRequest request)
        {
            bool renterExists = await _unitOfWork.Repository<RenterProfile>()
                .AsQueryable()
                .AnyAsync(r => r.Id == request.RenterID);

            if (!renterExists)
                throw new KeyNotFoundException("Renter not found.");

            var image = new Image
            {
                ContentType = request.ContentType,
                Base64Image = request.ImageData
            };

            await _unitOfWork.Repository<Image>().AddAsync(image);
            await _unitOfWork.SaveChangesAsync();

            var relation = new DriverLicenseImage
            {
                RenterID = request.RenterID,
                ImageID = image.Id
            };

            await _unitOfWork.Repository<DriverLicenseImage>().AddAsync(relation);
            await _unitOfWork.SaveChangesAsync();

            return new DriverLicenseImageResponse
            {
                RenterID = request.RenterID,
                ImageID = image.Id,
                ImageContentType = image.ContentType
            };
        }

        public async Task<List<ImageResponseModel>> GetImagesByModelIdAsync(int modelId)
        {
            var images = await _unitOfWork.Repository<ModelImage>()
                .AsQueryable()
                .Where(mi => mi.ModelID == modelId)
                .Include(mi => mi.Image)
                .Select(mi => new ImageResponseModel
                {
                    ImageID = mi.Image.Id,
                    ContentType = mi.Image.ContentType,
                    ImageData = mi.Image.Base64Image
                })
                .ToListAsync();

            return images;
        }

        public async Task<List<string>> GetImagesByVehicleIdAsync(int vehicleId)
        {
            var images = await _unitOfWork.Repository<VehicleImage>()
                .AsQueryable()
                .Where(vi => vi.VehicleID == vehicleId)
                .Include(vi => vi.Image)
                .Select(vi => Convert.ToBase64String(vi.Image.Base64Image))
                .ToListAsync();

            return images;   
        }



        public async Task<List<ImageResponseModel>> GetIDImagesByRenterIdAsync(int renterId)
        {
            var images = await _unitOfWork.Repository<IDImage>()
                .AsQueryable()
                .Where(i => i.RenterID == renterId)
                .Include(i => i.Image)
                .Select(i => new ImageResponseModel
                {
                    ImageID = i.Image.Id,
                    ContentType = i.Image.ContentType,
                    ImageData = i.Image.Base64Image
                })
                .ToListAsync();

            return images;
        }

        public async Task<List<ImageResponseModel>> GetDriverLicenseImagesByRenterIdAsync(int renterId)
        {
            var images = await _unitOfWork.Repository<DriverLicenseImage>()
                .AsQueryable()
                .Where(d => d.RenterID == renterId)
                .Include(d => d.Image)
                .Select(d => new ImageResponseModel
                {
                    ImageID = d.Image.Id,
                    ContentType = d.Image.ContentType,
                    ImageData = d.Image.Base64Image
                })
                .ToListAsync();

            return images;
        }

        public async Task<ImageResponseModel?> GetImageByIdAsync(int imageId)
        {
            var image = await _unitOfWork.Repository<Image>()
                .AsQueryable()
                .FirstOrDefaultAsync(i => i.Id == imageId);

            if (image == null) return null;

            return new ImageResponseModel
            {
                ImageID = image.Id,
                ContentType = image.ContentType,
                ImageData = image.Base64Image
            };
        }

        public async Task<bool> DeleteVehicleImageByBase64Async(int vehicleId, string base64Image)
        {
            var vehicleImageRepo = _unitOfWork.Repository<VehicleImage>();
            var imageRepo = _unitOfWork.Repository<Image>();

            
            byte[] incomingBytes;
            try
            {
                incomingBytes = Convert.FromBase64String(base64Image);
            }
            catch
            {
                throw new Exception("Ảnh không hợp lệ (base64 decode failed).");
            }

            
            var vehicleImages = await vehicleImageRepo
                .AsQueryable()
                .Where(v => v.VehicleID == vehicleId)
                .Include(v => v.Image)
                .ToListAsync();

            if (!vehicleImages.Any())
                throw new KeyNotFoundException("Xe này không có ảnh nào.");

            
            VehicleImage? matchedVehicleImage = null;

            foreach (var vi in vehicleImages)
            {
                if (vi.Image.Base64Image.SequenceEqual(incomingBytes))
                {
                    matchedVehicleImage = vi;
                    break;
                }
            }

            if (matchedVehicleImage == null)
                throw new KeyNotFoundException("Không tìm thấy ảnh cần xóa.");

            int imageId = matchedVehicleImage.ImageID;

            
            await vehicleImageRepo.DeleteAsync(matchedVehicleImage);

            
            bool isUsedElsewhere =
                await vehicleImageRepo.AsQueryable().AnyAsync(v => v.ImageID == imageId) ||
                await _unitOfWork.Repository<ModelImage>().AsQueryable().AnyAsync(m => m.ImageID == imageId) ||
                await _unitOfWork.Repository<IDImage>().AsQueryable().AnyAsync(i => i.ImageID == imageId) ||
                await _unitOfWork.Repository<DriverLicenseImage>().AsQueryable().AnyAsync(d => d.ImageID == imageId);

            
            if (!isUsedElsewhere)
            {
                var img = await imageRepo.GetByIdAsync(imageId);
                if (img != null)
                {
                    await imageRepo.DeleteAsync(img);
                }
            }

            await _unitOfWork.SaveChangesAsync();
            return true;
        }





    }
}
