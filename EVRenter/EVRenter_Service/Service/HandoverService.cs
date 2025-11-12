using AutoMapper;
using EVRenter_CM.Enums;
using EVRenter_Data.Entities;
using EVRenter_Repository.UnitOfWork;
using EVRenter_Service.RequestModel;
using EVRenter_Service.ResponseModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EVRenter_Service.Service
{
    public interface IHandoverService
    {
        Task<HandoverResponseModel> CreateHandoverAsync(HandoverCreateRequest request);
        Task<bool> ConfirmHandoverAsync(int handoverId);
        Task<IEnumerable<HandoverResponseModel>> GetAllHandoversAsync();
        Task<HandoverResponseModel?> GetHandoverByIdAsync(int id);
    }

    public class HandoverService : IHandoverService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<HandoverService> _logger;

        public HandoverService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<HandoverService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        
        public async Task<HandoverResponseModel> CreateHandoverAsync(HandoverCreateRequest request)
        {
            
            var booking = await _unitOfWork.Repository<Booking>()
                .AsQueryable()
                .Include(b => b.Vehicle)
                .FirstOrDefaultAsync(b => b.Id == request.BookingID && !b.IsDelete);

            bool isExist = await _unitOfWork.Repository<User>()
                            .AsQueryable()
                            .AnyAsync(stf => stf.Id == request.StaffID 
                                            && !stf.IsDelete 
                                            && stf.RoleID == RoleType.Staff);

            if (!isExist)
            {
                throw new KeyNotFoundException("Staff not found.");
            }    

            if (booking == null)
                throw new KeyNotFoundException("Booking not found.");


            var handover = new HandoverAndReturn
            {
                BookingID = booking.Id,
                VehicleID = booking.VehicleID,
                StaffID = request.StaffID,
                StationID = booking.Vehicle.StationID,
                CheckDate = DateTime.UtcNow,
                Type = 1,
                Status = 1,
                Decription = request.Description,
                IsDelete = false,
                Exterior = "",
                Interior = "",
                Technical = "",
                Accessories = ""
            };

            var vehicle = booking.Vehicle;

            if (vehicle == null)
            {
                throw new KeyNotFoundException("Vehicle not found.");
            }

            vehicle.Status = 4;
            booking.Status = 4;
            await _unitOfWork.Repository<HandoverAndReturn>().AddAsync(handover);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation($"Handover #{handover.Id} created for Booking #{booking.Id}");

            
            var carItems = await _unitOfWork.Repository<CarItem>()
                .AsQueryable()
                .Include(c => c.Category)
                .Where(c => c.VehicleID == booking.VehicleID && !c.IsDelete)
                .ToListAsync();

            var itemResponses = carItems.Select(c => new HandoverItemStatusResponse
            {
                CarItemID = c.Id,
                CarItemName = c.Name,
                CategoryName = c.Category.Name,
                Condition = c.Status,
                Note = null
            }).ToList();
            
            return new HandoverResponseModel
            {
                Id = handover.Id,
                BookingID = handover.BookingID,
                VehicleID = handover.VehicleID,
                StationID = handover.StationID,
                Type = handover.Type,
                CheckDate = handover.CheckDate,
                Status = handover.Status,
                Description = handover.Decription,
                Items = itemResponses
            };
        }

        
        public async Task<bool> ConfirmHandoverAsync(int handoverId)
        {
            var handover = await _unitOfWork.Repository<HandoverAndReturn>()
                .GetByIdAsync(handoverId);

            

            if (handover == null || handover.IsDelete)
                throw new KeyNotFoundException("Handover record not found.");

            var booking = await _unitOfWork.Repository<Booking>()
                .AsQueryable()
                .FirstOrDefaultAsync(b => b.Id == handover.BookingID && !b.IsDelete);

            if (booking == null)
                throw new KeyNotFoundException("Booking not found for this handover.");


            var vehicle = await _unitOfWork.Repository<Vehicle>()
                .AsQueryable()
                .FirstOrDefaultAsync(v => v.Id == handover.VehicleID && !v.IsDelete);

            if (vehicle == null)
                throw new KeyNotFoundException("Vehicle not found for this handover.");

            if(handover.Status == 1)
            {
                throw new Exception("Handover was confirmed!");
            }

            handover.Status = 1;
            booking.Status = 4;
            vehicle.Status = 4;

            _unitOfWork.Repository<HandoverAndReturn>().Update(handover);
            
            _unitOfWork.Repository<Booking>().Update(booking);
        
            _unitOfWork.Repository<Vehicle>().Update(vehicle);

            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<HandoverResponseModel>> GetAllHandoversAsync()
        {
            var handovers = await _unitOfWork.Repository<HandoverAndReturn>()
                .AsQueryable()
                .Include(h => h.Booking)
                .Include(h => h.Vehicle)
                .Include(h => h.Station)
                .Where(h => !h.IsDelete)
                .OrderByDescending(h => h.CheckDate)
                .ToListAsync();

            var results = new List<HandoverResponseModel>();

            foreach (var handover in handovers)
            {
                var carItems = await _unitOfWork.Repository<CarItem>()
                    .AsQueryable()
                    .Include(c => c.Category)
                    .Where(c => c.VehicleID == handover.VehicleID && !c.IsDelete)
                    .ToListAsync();

                var items = carItems.Select(c => new HandoverItemStatusResponse
                {
                    CarItemID = c.Id,
                    CarItemName = c.Name,
                    CategoryName = c.Category.Name,
                    Condition = c.Status,
                    Note = null
                }).ToList();

                results.Add(new HandoverResponseModel
                {
                    Id = handover.Id,
                    BookingID = handover.BookingID,
                    VehicleID = handover.VehicleID,
                    StationID = handover.StationID,
                    Type = handover.Type,
                    CheckDate = handover.CheckDate,
                    Status = handover.Status,
                    Description = handover.Decription,
                    Items = items
                });
            }

            return results;
        }

        public async Task<HandoverResponseModel?> GetHandoverByIdAsync(int id)
        {
            var handover = await _unitOfWork.Repository<HandoverAndReturn>()
                .AsQueryable()
                .Include(h => h.Booking)
                .Include(h => h.Vehicle)
                .Include(h => h.Station)
                .FirstOrDefaultAsync(h => h.Id == id && !h.IsDelete);

            if (handover == null)
                return null;

            var carItems = await _unitOfWork.Repository<CarItem>()
                .AsQueryable()
                .Include(c => c.Category)
                .Where(c => c.VehicleID == handover.VehicleID && !c.IsDelete)
                .ToListAsync();

            var items = carItems.Select(c => new HandoverItemStatusResponse
            {
                CarItemID = c.Id,
                CarItemName = c.Name,
                CategoryName = c.Category.Name,
                Condition = c.Status,
                Note = null
            }).ToList();

            return new HandoverResponseModel
            {
                Id = handover.Id,
                BookingID = handover.BookingID,
                VehicleID = handover.VehicleID,
                StationID = handover.StationID,
                Type = handover.Type,
                CheckDate = handover.CheckDate,
                Status = handover.Status,
                Description = handover.Decription,
                Items = items
            };
        }

    }
}
