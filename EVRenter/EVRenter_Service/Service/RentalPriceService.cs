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
    public interface IRentalPriceService
    {
        Task<IEnumerable<RentalPriceResponse>> GetAllRentalPrice();
        Task<RentalPriceResponse?> GetPriceByIdAsync(int id);
        Task<RentalPriceResponse?> GetPriceByModelAsync(int modelId);
        Task<RentalPriceResponse> CreatePriceAsync(PriceRequestModel request);
        Task<RentalPriceResponse> UpdatePriceByModelAsync(PriceUpdateRequest request);
        Task<bool> DeletePriceAsync(int id);

    }
    public class RentalPriceService : IRentalPriceService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public RentalPriceService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<RentalPriceResponse>> GetAllRentalPrice()
        {
            return await _unitOfWork.Repository<RentalPrice>()
                .GetQueryable()
                .Where(x => !x.IsDelete)
                .ProjectTo<RentalPriceResponse>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task<RentalPriceResponse?> GetPriceByIdAsync(int id)
        {
            // Get the user with basic information
            var rentalPrice = await _unitOfWork.Repository<RentalPrice>().AsQueryable()
                .Where(u => u.Id == id)
                .ProjectTo<RentalPriceResponse>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();

            return rentalPrice;
        }

        public async Task<RentalPriceResponse?> GetPriceByModelAsync(int modelId)
        {
            // Get the user with basic information
            var rentalPrice = await _unitOfWork.Repository<RentalPrice>().AsQueryable()
                .Where(u => u.ModelID == modelId)
                .ProjectTo<RentalPriceResponse>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();

            return rentalPrice;
        }

        public async Task<RentalPriceResponse> CreatePriceAsync(PriceRequestModel request)
        {
            if (request == null)
                throw new ArgumentException("Invalid request data.");

            var model = await _unitOfWork.Repository<Model>().AsQueryable()
               .Where(u => u.Id == request.ModelID)
               .FirstOrDefaultAsync();
            if (model == null)
            {
                throw new Exception("Model not found");
            }

            var price = _mapper.Map<RentalPrice>(request);

            await _unitOfWork.Repository<RentalPrice>().InsertAsync(price);
            await _unitOfWork.SaveChangesAsync();

            var createdPrice = await _unitOfWork.Repository<RentalPrice>()
                .AsQueryable()
                .Where(x => x.Id == model.Id)
                .ProjectTo<RentalPriceResponse>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();

            if (createdPrice == null)
            {
                throw new Exception("Failed to retrieve created price.");
            }

            return createdPrice;
        }

        public async Task<RentalPriceResponse> UpdatePriceByModelAsync(PriceUpdateRequest request)
        {
            if (request == null)
                throw new ArgumentException("Invalid request data.");

            var model = await _unitOfWork.Repository<Model>().AsQueryable()
               .Where(u => u.Id == request.ModelID)
               .FirstOrDefaultAsync();
            if (model == null)
            {
                throw new Exception("Model not found");
            }

            var existingPrice = await _unitOfWork.Repository<RentalPrice>()
                .AsQueryable()
                .Where(s => s.ModelID == request.ModelID && !s.IsDelete)
                .FirstOrDefaultAsync();
            if (existingPrice == null) { return null;  }

            // Kiểm tra xem có bất kỳ trường nào được cập nhật không
            bool hasUpdates = false;
            // Cập nhật từng trường nếu có giá trị mới
            if (request.Price.HasValue)
            {
                existingPrice.Price = request.Price.Value;
                hasUpdates = true;
            }
            if (request.Deposit.HasValue)
            {
                existingPrice.Price = request.Deposit.Value;
                hasUpdates = true;
            }

            if (hasUpdates)
            {
                await _unitOfWork.Repository<RentalPrice>().Update(existingPrice, existingPrice.Id);
                await _unitOfWork.SaveChangesAsync();
            }

            return _mapper.Map<RentalPriceResponse>(existingPrice);

        }

        public async Task<bool> DeletePriceAsync(int id)
        {
            var price = await _unitOfWork.Repository<RentalPrice>().GetById(id);
            if (price == null) return false;

            price.IsDelete = true;
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}
