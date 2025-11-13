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

    }

    public class ImageService : IImageService
    {
        private readonly IUnitOfWork _unitOfWork;

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
                ImageID = image.Id,
                ImageContentType = image.ContentType
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

    }
}
