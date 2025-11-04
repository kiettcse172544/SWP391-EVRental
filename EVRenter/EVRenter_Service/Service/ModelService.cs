using AutoMapper;
using AutoMapper.QueryableExtensions;
using Azure.Core;
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
    public interface IModelService
    {
        Task<IEnumerable<ModelResponseModel>> GetAllModel();
        Task<ModelResponseModel?> GetModelByIdAsync(int id);
        Task<ModelResponseModel> CreateModelAsync(ModelRequestModel request);
        Task<ModelResponseModel?> UpdateModelAsync(int id, ModelUpdateRequest request);
        Task<bool> DeleteModelAsync(int id);
        Task<IEnumerable<ModelResponseModel>> GetModelByStationAsync(int stationId);
        Task RebootModelQuantitiesAsync();
    }
    public class ModelService : IModelService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ModelService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ModelResponseModel>> GetAllModel()
        {
            return await _unitOfWork.Repository<Model>()
                .GetQueryable()
                .Where(x => !x.IsDelete)
                .ProjectTo<ModelResponseModel>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task<ModelResponseModel?> GetModelByIdAsync(int id)
        {
            // Get the user with basic information
            var model = await _unitOfWork.Repository<Model>().AsQueryable()
                .Where(u => u.Id == id && !u.IsDelete)
                .ProjectTo<ModelResponseModel>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();

            return model;
        }

        public async Task<IEnumerable<ModelResponseModel>> GetModelByStationAsync(int stationId)
        {
            // Get the user with basic information
            var model = await _unitOfWork.Repository<Model>().GetQueryable()
                .Where(m => m.Vehicles.Any(v => v.StationID == stationId))
                .ProjectTo<ModelResponseModel>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return model;
        }

        public async Task RebootModelQuantitiesAsync()
        {
            // Lấy danh sách Model + số lượng Vehicle thực tế
            var modelCounts = await _unitOfWork.Repository<Vehicle>()
                .AsQueryable()
                .Where(x => !x.IsDelete)
                .GroupBy(v => v.ModelID)
                .Select(g => new
                {
                    ModelID = g.Key,
                    VehicleCount = g.Count()
                })
                .ToListAsync();

            // Lấy toàn bộ Model
            var models = await _unitOfWork.Repository<Model>().AsQueryable()
                .Where(x => !x.IsDelete).ToListAsync();

            foreach (var model in models)
            {
                var count = modelCounts.FirstOrDefault(c => c.ModelID == model.Id)?.VehicleCount ?? 0;
                model.Quantity = count;
                await _unitOfWork.Repository<Model>().UpdateAsync(model);
            }

            await _unitOfWork.SaveChangesAsync();
        }



        public async Task<ModelResponseModel> CreateModelAsync(ModelRequestModel request)
        {
            if (request == null)
                throw new ArgumentException("Invalid request data.");

            var model = _mapper.Map<Model>(request);

            await _unitOfWork.Repository<Model>().InsertAsync(model);
            await _unitOfWork.SaveChangesAsync();

            var createdModel = await _unitOfWork.Repository<Model>()
                .AsQueryable()
                .Where(x => x.Id == model.Id)
                .ProjectTo<ModelResponseModel>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();

            if (createdModel == null)
            {
                throw new Exception("Failed to retrieve created model.");
            }

            return createdModel;
        }

        public async Task<ModelResponseModel?> UpdateModelAsync(int id, ModelUpdateRequest request)
        {
            var existingModel = await _unitOfWork.Repository<Model>()
                .AsQueryable()
                .Where(s => s.Id == id && !s.IsDelete)
                .FirstOrDefaultAsync();

            if (existingModel == null) return null;

            // Kiểm tra xem có bất kỳ trường nào được cập nhật không
            bool hasUpdates = false;

            // Cập nhật từng trường nếu có giá trị mới
            if (!string.IsNullOrEmpty(request.ModelName))
            {
                existingModel.ModelName = request.ModelName;
                hasUpdates = true;
            }
            if (!string.IsNullOrEmpty(request.Type))
            {
                existingModel.Type = request.Type;
                hasUpdates = true;
            }
            if (request.Seat.HasValue)
            {
                existingModel.Seat = request.Seat.Value;
                hasUpdates = true;
            }
            if (request.Range.HasValue)
            {
                existingModel.Range = request.Range.Value;
                hasUpdates = true;
            }
            if (request.TrunkCapatity.HasValue)
            {
                existingModel.TrunkCapatity = request.TrunkCapatity.Value;
                hasUpdates = true;
            }
            if (request.Hoursepower.HasValue)
            {
                existingModel.Hoursepower = request.Hoursepower.Value;
                hasUpdates = true;
            }
            if (request.MoveLimit.HasValue)
            {
                existingModel.MoveLimit = request.MoveLimit.Value;
                hasUpdates = true;
            }

            if (hasUpdates)
            {
                await _unitOfWork.Repository<Model>().Update(existingModel, id);
                await _unitOfWork.SaveChangesAsync();
            }

            return _mapper.Map<ModelResponseModel>(existingModel);
        }

        public async Task<bool> DeleteModelAsync(int id)
        {
            var model = await _unitOfWork.Repository<Model>().GetById(id);
            if (model == null || model.IsDelete) return false;

            var vehicles = await _unitOfWork.Repository<Vehicle>().AsQueryable()
               .Where(u => u.ModelID == id && !u.IsDelete)
               .ToListAsync();

            var price = await _unitOfWork.Repository<RentalPrice>().AsQueryable()
               .Where(u => u.ModelID == id && !u.IsDelete)
               .FirstOrDefaultAsync();

            var amanities = await _unitOfWork.Repository<Amenities>().AsQueryable()
                .Where(u => u.ModelID != id && !u.IsDelete)
                .ToListAsync();

            model.IsDelete = true;
            if (price != null) { price.IsDelete = false; }
            if (vehicles != null)
            {
                foreach (var vehicle in vehicles)
                {
                    vehicle.IsDelete = false;
                }
            }
            if (amanities != null)
            {
                foreach (var aman in amanities)
                {
                    aman.IsDelete = false;
                }
            }

            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}
