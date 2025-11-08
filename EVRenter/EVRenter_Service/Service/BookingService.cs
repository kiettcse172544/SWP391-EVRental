
using AutoMapper;
using AutoMapper.QueryableExtensions;
using EVRenter_CM.Enums;
using EVRenter_Data.Entities;
using EVRenter_Repository.UnitOfWork;
using EVRenter_Service.RequestModel;
using EVRenter_Service.ResponseModel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVRenter_Service.Service
{
    public interface IBookingService
    {
        Task<IEnumerable<BookingResponseModel>> GetAllBooking();
        Task<BookingResponseModel?> GetBookingByIdAsync(int id);
        Task<BookingResponseModel?> GetBookingByVehicleAsync(int vehicleId);
        Task<IEnumerable<StaffBookingResponseModel>> GetUnapprovalBooking();
        Task<BookingResponseModel> CreateBookingAsync(BookingRequestModel request);
        Task<StaffBookingResponseModel?> UpdateBookingStatsusAsync(int id, BookingUpdateRequest request);
        Task<bool> DeleteUnpaidBookingAsync(int id);
    }
    public class BookingService : IBookingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public BookingService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<BookingResponseModel>> GetAllBooking()
        {
            return await _unitOfWork.Repository<Booking>()
                .GetQueryable()
                .Where(x => !x.IsDelete)
                .ProjectTo<BookingResponseModel>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task<IEnumerable<StaffBookingResponseModel>> GetUnapprovalBooking()
        {
            return await _unitOfWork.Repository<Booking>()
                .GetQueryable()
                .Where(x => !x.IsDelete && x.Status == 0)
                .ProjectTo<StaffBookingResponseModel>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task<BookingResponseModel?> GetBookingByIdAsync(int id)
        {
            // Get the user with basic information
            var booking = await _unitOfWork.Repository<Booking>().AsQueryable()
                .Where(u => u.Id == id)
                .ProjectTo<BookingResponseModel>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();

            return booking;
        }

        public async Task<BookingResponseModel?> GetBookingByVehicleAsync(int vehicleId)
        {
            // Get the user with basic information
            var booking = await _unitOfWork.Repository<Booking>().AsQueryable()
                .Where(u => u.VehicleID == vehicleId)
                .ProjectTo<BookingResponseModel>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();

            return booking;
        }

        public async Task<BookingResponseModel> CreateBookingAsync(BookingRequestModel request)
        {
            if (request == null)
                throw new ArgumentException("Invalid request data.");

            var vehicle = await _unitOfWork.Repository<Vehicle>().AsQueryable()
               .Where(u => !u.IsDelete && u.ModelID == request.ModelID && u.StationID == request.StationID && u.Status == 0)
               .FirstOrDefaultAsync();
            if (vehicle == null)
            {
                throw new Exception("Car is full");
            }

            vehicle.Status = 1;
            var user = await _unitOfWork.Repository<User>().AsQueryable()
               .Where(u => !u.IsDelete && u.Id == request.RenterID)
               .FirstOrDefaultAsync();
            if (user == null)
            {
                throw new Exception("Renter not found");
            }

            var booking = _mapper.Map<Booking>(request);

            var price = await _unitOfWork.Repository<RentalPrice>().AsQueryable()
               .Where(u => u.ModelID == vehicle.ModelID)
               .FirstOrDefaultAsync();
            if (price == null)
            {
                throw new Exception("price not found");
            }
            var totalDays = (int)Math.Ceiling((booking.EndDate - booking.StartDate).TotalDays);
            booking.RetalCost = price.Price * totalDays;
            booking.Deposit = price.Deposit;

            //if (booking.StartDate.AddDays(7) > booking.EndDate)
            //{
            //    booking.RentalType = 1; //Daily
            //}
            //else if (booking.StartDate.AddMonths(1) > booking.EndDate)
            //{
            //    booking.RentalType = 2; //Weekly 
            //    booking.RetalCost = booking.RetalCost * 0.9m;

            //}
            //else if (booking.StartDate.AddYears(1) > booking.EndDate)
            //{
            //    booking.RentalType = 3; // Monthly
            //    booking.RetalCost = booking.RetalCost * 0.9m;
            //}
            booking.RentalType = 1;

            booking.BaseCost = booking.RetalCost + booking.Deposit;

            booking.VehicleID = vehicle.Id;
            booking.RenterID = request.RenterID;
            booking.Status = 0;
            booking.CreatedAt = DateTime.Now;

            await _unitOfWork.Repository<Booking>().InsertAsync(booking);
            await _unitOfWork.Repository<Vehicle>().UpdateAsync(vehicle);
            await _unitOfWork.SaveChangesAsync();

            var createdBooking = await _unitOfWork.Repository<Booking>()
                .AsQueryable()
                .Where(x => x.Id == booking.Id)
                .ProjectTo<BookingResponseModel>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();

            if (createdBooking == null)
            {
                throw new Exception("Failed to retrieve created booking.");
            }

            return createdBooking;
        }

        public async Task<StaffBookingResponseModel?> UpdateBookingStatsusAsync(int id, BookingUpdateRequest request)
        {
            var existingBooking = await _unitOfWork.Repository<Booking>()
                .AsQueryable()
                .Where(u => u.Id == id && !u.IsDelete)
                .FirstOrDefaultAsync();

            if (existingBooking == null) return null;
            bool hasUpdates = false;

            if (request.Status.HasValue)
            {
                existingBooking.Status = request.Status.Value;
                hasUpdates = true;
            }

            if (request.VehicleID.HasValue)
            {
                var existingVehicle = await _unitOfWork.Repository<Vehicle>()
                .AsQueryable()
                .Where(u => u.Id == request.VehicleID && !u.IsDelete && u.Status == 0)
                .FirstOrDefaultAsync();
                if (existingVehicle == null) return null;

                existingBooking.VehicleID = request.VehicleID.Value;
                hasUpdates = true;
            }

            if (request.StartDate.HasValue)
            {
                if (request.EndDate.HasValue)
                {
                    if (request.EndDate.Value < request.StartDate.Value) throw new Exception("EndDate is lower StartDate");

                    existingBooking.StartDate = request.StartDate.Value;
                    existingBooking.EndDate = request.EndDate.Value;
                }
                else
                {
                    if (existingBooking.EndDate < request.StartDate.Value) throw new Exception("EndDate is lower StartDate");
                    existingBooking.StartDate = request.StartDate.Value;
                }
                hasUpdates = true;
            }

            if (request.EndDate.HasValue)
            {
                if (request.EndDate.Value < existingBooking.StartDate) throw new Exception("EndDate is lower StartDate");
                existingBooking.EndDate = request.EndDate.Value;
                hasUpdates = true;
            }

            if(request.StartDate.HasValue)
            {
                existingBooking.Status = request.Status.Value;
                hasUpdates = true;
            }

            if (hasUpdates)
            {
                await _unitOfWork.Repository<Booking>().Update(existingBooking, id);
                await _unitOfWork.SaveChangesAsync();
            }

            return _mapper.Map<StaffBookingResponseModel>(existingBooking);

        }

        public async Task<bool> DeleteUnpaidBookingAsync(int id)
        {
            var booking = await _unitOfWork.Repository<Booking>().GetById(id);
            if (booking == null) return false;

            //booking.status = ? patment.status = 1
            //...

            booking.IsDelete = true;
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}
