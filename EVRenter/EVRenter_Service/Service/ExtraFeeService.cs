using AutoMapper;
using EVRenter_Data.Entities;
using EVRenter_Repository.UnitOfWork;
using EVRenter_Service.RequestModel;
using EVRenter_Service.ResponseModel;
using Microsoft.EntityFrameworkCore;
using EVRenter_Service.IService;

namespace EVRenter_Service.Service
{
    


    public class ExtraFeeService : IExtraFeeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ExtraFeeService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ExtraFeeResponseModel> CreateExtraFeeAsync(ExtraFeeCreateRequest request)
        {
            var booking = await _unitOfWork.Repository<Booking>()
                .AsQueryable()
                .FirstOrDefaultAsync(b => b.Id == request.BookingId && !b.IsDelete);

            if (booking == null)
                throw new KeyNotFoundException("Booking not found.");

            var extraFee = new ExtraFee
            {
                BookingID = request.BookingId,
                HandoverAndReturnID = request.HandoverAndReturnId,
                UserID = request.UserId,
                Deposit = booking.Deposit,
                IsRefunded = 0 
            };

            await _unitOfWork.Repository<ExtraFee>().InsertAsync(extraFee);
            await _unitOfWork.SaveChangesAsync();

            
            if (request.FeeTypes != null && request.FeeTypes.Any())
            {
                foreach (var ft in request.FeeTypes)
                {
                    await _unitOfWork.Repository<FeeType>().InsertAsync(new FeeType
                    {
                        ExtraFeeID = extraFee.Id,
                        Name = ft.Name,
                        Description = ft.Description,
                        UnitPrice = ft.UnitPrice
                    });
                }

                await _unitOfWork.SaveChangesAsync();
            }

            
            var feeTypes = await _unitOfWork.Repository<FeeType>()
                .AsQueryable()
                .Where(f => f.ExtraFeeID == extraFee.Id)
                .ToListAsync();

            var amount = feeTypes.Sum(f => f.UnitPrice);
            var deposit = booking.Deposit;
            var cost = Math.Abs(deposit - amount);
            var isRefunded = deposit > amount;

            extraFee.Amount = amount;
            extraFee.Cost = cost;
            extraFee.IsRefunded = isRefunded ? 1 : 0;

            await _unitOfWork.Repository<ExtraFee>().Update(extraFee, extraFee.Id);
            await _unitOfWork.SaveChangesAsync();

            
            return new ExtraFeeResponseModel
            {
                Id = extraFee.Id,
                BookingId = extraFee.BookingID,
                HandoverAndReturnId = extraFee.HandoverAndReturnID,
                Deposit = deposit,
                Amount = amount,
                Cost = cost,
                IsRefunded = isRefunded,
                Message = isRefunded
                    ? $"Hoàn lại khách {cost:N0} VND tiền cọc."
                    : $"Khách cần thanh toán thêm {cost:N0} VND.",
                FeeTypes = feeTypes.Select(f => new FeeTypeResponseModel
                {
                    Name = f.Name,
                    UnitPrice = f.UnitPrice
                }).ToList()
            };
        }

        public async Task<IEnumerable<ExtraFeeResponseModel>> GetAllExtraFeesAsync()
        {
            var extraFees = await _unitOfWork.Repository<ExtraFee>()
                .AsQueryable()
                .Include(e => e.Booking)
                .Include(e => e.User)
                .Include(e => e.FeeTypes)
                .Where(e => !e.IsDelete)
                .OrderByDescending(e => e.Id)
                .ToListAsync();

            var result = extraFees.Select(e => new ExtraFeeResponseModel
            {
                Id = e.Id,
                BookingId = e.BookingID,
                HandoverAndReturnId = e.HandoverAndReturnID,
                Deposit = e.Deposit,
                Amount = e.Amount,
                Cost = e.Cost,
                IsRefunded = e.IsRefunded == 1,
                Message = e.IsRefunded == 1
                    ? $"Hoàn lại khách {e.Cost:N0} VND tiền cọc."
                    : $"Khách cần thanh toán thêm {e.Cost:N0} VND.",
                FeeTypes = e.FeeTypes.Select(f => new FeeTypeResponseModel
                {
                    Name = f.Name,
                    UnitPrice = f.UnitPrice
                }).ToList()
            });

            return result;
        }

        public async Task<IEnumerable<ExtraFeeResponseModel>> GetExtraFeesByBookingAsync(int bookingId)
        {
            var extraFees = await _unitOfWork.Repository<ExtraFee>()
                .AsQueryable()
                .Include(e => e.FeeTypes)
                .Where(e => e.BookingID == bookingId && !e.IsDelete)
                .ToListAsync();

            var result = extraFees.Select(e => new ExtraFeeResponseModel
            {
                Id = e.Id,
                BookingId = e.BookingID,
                HandoverAndReturnId = e.HandoverAndReturnID,
                Deposit = e.Deposit,
                Amount = e.Amount,
                Cost = e.Cost,
                IsRefunded = e.IsRefunded == 1,
                Message = e.IsRefunded == 1
                    ? $"Hoàn lại khách {e.Cost:N0} VND tiền cọc."
                    : $"Khách cần thanh toán thêm {e.Cost:N0} VND.",
                FeeTypes = e.FeeTypes.Select(f => new FeeTypeResponseModel
                {
                    Name = f.Name,
                    UnitPrice = f.UnitPrice
                }).ToList()
            });

            return result;
        }

    }
}
