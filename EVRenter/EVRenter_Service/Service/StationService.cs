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
    public interface IStationService
    {
        Task<IEnumerable<StationResponseModel>> GetAllStation();
        Task<StationResponseModel?> GetStationByIdAsync(int id);
        Task<StationResponseModel> CreateStationAsync(StationRequestModel request);
        Task<StationResponseModel?> UpdateStationAsync(int id, StationUpdateRequest request);
        Task<bool> DeleteStationAsync(int id);
    }
    public class StationService : IStationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public StationService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<StationResponseModel>> GetAllStation()
        {
            return await _unitOfWork.Repository<Station>()
                .GetQueryable()
                .Where(x => !x.IsDelete)
                .ProjectTo<StationResponseModel>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task<StationResponseModel?> GetStationByIdAsync(int id)
        {
            // Get the user with basic information
            var station = await _unitOfWork.Repository<Station>().AsQueryable()
                .Where(u => u.Id == id)
                .ProjectTo<StationResponseModel>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();

            return station;
        }

        public async Task<StationResponseModel> CreateStationAsync(StationRequestModel request)
        {
            if (request == null)
                throw new ArgumentException("Invalid request data.");

            var station = _mapper.Map<Station>(request);

            await _unitOfWork.Repository<Station>().InsertAsync(station);
            await _unitOfWork.SaveChangesAsync();

            var createdStation = await _unitOfWork.Repository<Station>()
                .AsQueryable()
                .Where(x => x.Id == station.Id)
                .ProjectTo<StationResponseModel>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();

            if (createdStation == null)
            {
                throw new Exception("Failed to retrieve created station.");
            }

            return createdStation;
        }

        public async Task<StationResponseModel?> UpdateStationAsync(int id, StationUpdateRequest request)
        {
            var existingStation = await _unitOfWork.Repository<Station>()
                .AsQueryable()
                .Where(s => s.Id == id && !s.IsDelete)
                .FirstOrDefaultAsync();

            if(existingStation == null) return null;

            // Kiểm tra xem có bất kỳ trường nào được cập nhật không
            bool hasUpdates = false;

            // Cập nhật từng trường nếu có giá trị mới
            if (!string.IsNullOrEmpty(request.Name))
            {
                existingStation.Name = request.Name;
                hasUpdates = true;
            }
            if (!string.IsNullOrEmpty(request.Location))
            {
                existingStation.Location = request.Location;
                hasUpdates = true;
            }
            if (request.Capacity.HasValue)
            {
                existingStation.Capacity = request.Capacity.Value;
                hasUpdates = true;
            }

            if (hasUpdates)
            {
                await _unitOfWork.Repository<Station>().Update(existingStation, id);
                await _unitOfWork.SaveChangesAsync();
            }

            return _mapper.Map<StationResponseModel>(existingStation);
        }

        public async Task<bool> DeleteStationAsync(int id)
        {
            var user = await _unitOfWork.Repository<Station>().GetById(id);
            if (user == null) return false;

            user.IsDelete = true;
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}
