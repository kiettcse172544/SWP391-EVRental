using AutoMapper;
using AutoMapper.QueryableExtensions;
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
    public interface IAmenitiesService
    {
        Task<IEnumerable<AmenitiesResponseModel>> GetAllAmenitiesByModel(int modelID);
        Task<IEnumerable<AmenitiesResponseModel>> CreateAmenities(AmenitiesRequestModel request);
    }
    public class AmenitiesService : IAmenitiesService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AmenitiesService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<AmenitiesResponseModel>> GetAllAmenitiesByModel(int modelID)
        {
            return await _unitOfWork.Repository<Amenities>()
                .GetQueryable()
                .Where(x => !x.IsDelete && x.ModelID == modelID)
                .ProjectTo<AmenitiesResponseModel>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task<IEnumerable<AmenitiesResponseModel>> CreateAmenities(AmenitiesRequestModel request)
        {
            if (request == null)
                throw new ArgumentException("Invalid request data.");

            var model = await _unitOfWork.Repository<Model>().AsQueryable()
                .Where(u => u.Id == request.ModelID && !u.IsDelete)
                .FirstOrDefaultAsync();
            if (model == null)
            {
                throw new Exception("Model not found");
            }

            foreach (var item in request.Amenities)
            {
                var existingAmenity = await _unitOfWork.Repository<Amenities>().AsQueryable()
                .Where(u => !u.IsDelete && u.ModelID == request.ModelID && u.Name == item.Name).FirstOrDefaultAsync();
                if(existingAmenity != null)
                {
                    throw new InvalidOperationException($"Amenity: {item.Name} already exists.");
                }
            }

            var newAmenities = _mapper.Map<IEnumerable<Amenities>>(request.Amenities);
            foreach (var amenity in newAmenities)
            {
                amenity.ModelID = request.ModelID;
                amenity.IsDelete = false;
                amenity.Status = 1;

                await _unitOfWork.Repository<Amenities>().InsertAsync(amenity);
            }

            await _unitOfWork.CommitAsync();

            var createdAmenities = await _unitOfWork.Repository<Amenities>()
                .GetQueryable()
                .Where(x => !x.IsDelete && x.ModelID == request.ModelID)
                .ProjectTo<AmenitiesResponseModel>(_mapper.ConfigurationProvider)
                .ToListAsync();

            if (createdAmenities == null)
            {
                throw new Exception("Failed to retrieve created Amenities.");
            }

            return createdAmenities;
        }

    }
}
