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

namespace EVRenter_Service.Service
{
    public interface IImageService
    {
        Task<ModelImageResponse> UploadModelImageAsync(UploadModelImageRequest request);
        Task<VehicleImageResponse> UploadVehicleImageAsync(UploadVehicleImageRequest request);
        Task<IDImageResponse> UploadIDImageAsync(UploadIDImageRequest request);
        Task<DriverLicenseImageResponse> UploadDriverLicenseImageAsync(UploadDriverLicenseImageRequest request);

        Task<List<ImageResponseModel>> GetImagesByModelIdAsync(int modelId);
        Task<List<ImageResponseModel>> GetImagesByVehicleIdAsync(int vehicleId);
        Task<List<ImageResponseModel>> GetIDImagesByRenterIdAsync(int renterId);
        Task<List<ImageResponseModel>> GetDriverLicenseImagesByRenterIdAsync(int renterId);
        Task<ImageResponseModel?> GetImageByIdAsync(int imageId);
        
        Task<UploadRPResponseModel> CreateRenterProfileAsync(UploadRPRequestModel request);
        Task<UploadRPResponseModel> GetRenterProfileAsync(int userId);
    }

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
                VehicleID = request.VehicleID,
                ImageContentType = image.ContentType,
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

        public async Task<List<ImageResponseModel>> GetImagesByVehicleIdAsync(int vehicleId)
        {
            var images = await _unitOfWork.Repository<VehicleImage>()
                .AsQueryable()
                .Where(vi => vi.VehicleID == vehicleId)
                .Include(vi => vi.Image)
                .Select(vi => new ImageResponseModel
                {
                    ImageID = vi.Image.Id,
                    ContentType = vi.Image.ContentType,
                    ImageData = vi.Image.Base64Image
                })
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

        public async Task<UploadRPResponseModel> CreateRenterProfileAsync(UploadRPRequestModel request)
        {
            var user = await _unitOfWork.Repository<User>()
                .AsQueryable()
                .FirstOrDefaultAsync(u => u.Id == request.RenterId && !u.IsDelete);

            if (user == null)
                throw new KeyNotFoundException("User not found.");


            var existProfile = await _unitOfWork.Repository<RenterProfile>()
                .AsQueryable()
                .FirstOrDefaultAsync(p => p.UserID == request.RenterId && !p.IsDelete);

            if (existProfile != null)
                throw new InvalidOperationException("This user already has a renter profile.");


            var profile = new RenterProfile
            {
                UserID = request.RenterId,
                IDNumber = request.IDNumber,
                DriverLicenseNo = request.DriverLicenseNo,
                Type = 1,
                IsDelete = false
            };
                
            

            await _unitOfWork.Repository<RenterProfile>().InsertAsync(profile);
            await _unitOfWork.SaveChangesAsync();




            var idFrontImage = new Image
            {
                ContentType = "CCCD mặt trước",
                Base64Image = request.IDNumberImage1
            };
            await _unitOfWork.Repository<Image>().InsertAsync(idFrontImage);


            var idBackImage = new Image
            {
                ContentType = "CCD mặt sau",
                Base64Image = request.IDNumberImage2
            };
            await _unitOfWork.Repository<Image>().InsertAsync(idBackImage);

            
            var dlFrontImage = new Image
            {
                ContentType = "GPLX mặt trước",
                Base64Image = request.DriverLicenseImage1
            };
            await _unitOfWork.Repository<Image>().InsertAsync(dlFrontImage);

            
            var dlBackImage = new Image
            {
                ContentType = "GPLX mặt sau",
                Base64Image = request.DriverLicenseImage2
            };
            await _unitOfWork.Repository<Image>().InsertAsync(dlBackImage);

            
            await _unitOfWork.SaveChangesAsync();


            

            // CCCD mặt trước
            await _unitOfWork.Repository<IDImage>().InsertAsync(new IDImage
            {
                RenterID = request.RenterId,  
                ImageID = idFrontImage.Id,
                Type = 1, 
            });

            // CCCD mặt sau
            await _unitOfWork.Repository<IDImage>().InsertAsync(new IDImage
            {
                RenterID = request.RenterId,
                ImageID = idBackImage.Id,
                Type = 2, 
            });


            

            // GPLX mặt trước
            await _unitOfWork.Repository<DriverLicenseImage>().InsertAsync(new DriverLicenseImage
            {
                RenterID = request.RenterId,
                ImageID = dlFrontImage.Id,
                Type = 1, 
            });

            // GPLX mặt sau
            await _unitOfWork.Repository<DriverLicenseImage>().InsertAsync(new DriverLicenseImage
            {
                RenterID = request.RenterId,
                ImageID = dlBackImage.Id,
                Type = 2, 
            });

            user.IsVerified = 2;
            await _unitOfWork.Repository<User>().UpdateAsync(user);
            
            await _unitOfWork.SaveChangesAsync();



            return new UploadRPResponseModel
            {
                IDNumber = profile.IDNumber,
                DriverLicenseNo = profile.DriverLicenseNo,

                IDFrontImageId = idFrontImage.Id,
                IDBackImageId = idBackImage.Id,

                DriverLicenseFrontImageId = dlFrontImage.Id,
                DriverLicenseBackImageId = dlBackImage.Id
            };
        }

        public async Task<UploadRPResponseModel> GetRenterProfileAsync(int userId)
        {
            
            var profile = await _unitOfWork.Repository<RenterProfile>()
                .AsQueryable()
                .FirstOrDefaultAsync(p => p.UserID == userId && !p.IsDelete);

            if (profile == null)
                throw new KeyNotFoundException("Renter profile not found.");

            
            var idImages = await _unitOfWork.Repository<IDImage>()
                .AsQueryable()
                .Where(x => x.RenterID == userId)
                .ToListAsync();

            var idFront = idImages.FirstOrDefault(x => x.Type == 1); 
            var idBack = idImages.FirstOrDefault(x => x.Type == 2); 

            
            var dlImages = await _unitOfWork.Repository<DriverLicenseImage>()
                .AsQueryable()
                .Where(x => x.RenterID == userId)
                .ToListAsync();

            var dlFront = dlImages.FirstOrDefault(x => x.Type == 1);
            var dlBack = dlImages.FirstOrDefault(x => x.Type == 2);

            
            return new UploadRPResponseModel
            {
                IDNumber = profile.IDNumber,
                DriverLicenseNo = profile.DriverLicenseNo,

                IDFrontImageId = idFront?.ImageID ?? 0,
                IDBackImageId = idBack?.ImageID ?? 0,

                DriverLicenseFrontImageId = dlFront?.ImageID ?? 0,
                DriverLicenseBackImageId = dlBack?.ImageID ?? 0
            };
        }

    }
}
