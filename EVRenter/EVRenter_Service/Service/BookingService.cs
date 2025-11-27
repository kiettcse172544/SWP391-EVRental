
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
        Task<StaffBookingResponseModel?> GetBookingByIdForStaffAsync(int id);
        Task<IEnumerable<StaffBookingResponseModel>> GetAllBookingsForStaff();
        Task<IEnumerable<BookingResponseModel>> GetBookingByRenter(int renterID);
        Task<BookingResponseModel> CreateBookingAsync(BookingRequestModel request);
        Task<StaffBookingResponseModel?> UpdateBookingStatsusAsync(int id, BookingUpdateRequest request);
        Task<IEnumerable<StaffBookingResponseModel>> GetStaffBookingsByStattion(int stationID);
        Task<StaffBookingResponseModel> AutoUpdateBookingStatusAsync(int bookingId);
        Task<StaffBookingResponseModel?> StaffRefusingAsync(int bookingId);
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

        public async Task<IEnumerable<StaffBookingResponseModel>> GetAllBookingsForStaff()
        {
            return await _unitOfWork.Repository<Booking>()
                .GetQueryable()
                .Where(x => !x.IsDelete && x.Status < 5 && x.Status > 0)
                //.Include(x => x.Vehicle.VehicleImages).ThenInclude(x => x.Image)
                //.Include(x => x.Vehicle).ThenInclude(x => x.Station)
                //.Include(x => x.Vehicle).ThenInclude(x => x.Model)
                //.Include(x => x.Vehicle).ThenInclude(x => x.CarItems).ThenInclude(x => x.Category)
                //.Include(x => x.User)
                .ProjectTo<StaffBookingResponseModel>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task<IEnumerable<StaffBookingResponseModel>> GetStaffBookingsByStattion(int stationID)
        {
            return await _unitOfWork.Repository<Booking>()
                .GetQueryable()
                .Where(x => !x.IsDelete && x.Status < 5 && x.Status > 0 && x.Vehicle.StationID == stationID)
                .ProjectTo<StaffBookingResponseModel>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task<BookingResponseModel?> GetBookingByIdAsync(int id)
        {
            // Get the user with basic information
            var booking = await _unitOfWork.Repository<Booking>().AsQueryable()
                .Where(u => u.Id == id && u.IsDelete == false)
                .ProjectTo<BookingResponseModel>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();

            return booking;
        }

        public async Task<StaffBookingResponseModel?> GetBookingByIdForStaffAsync(int id)
        {
            // Get the user with basic information
            var booking = await _unitOfWork.Repository<Booking>().AsQueryable()
                .Where(u => u.Id == id && u.IsDelete == false)
                .ProjectTo<StaffBookingResponseModel>(_mapper.ConfigurationProvider)
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

        public async Task<IEnumerable<BookingResponseModel>> GetBookingByRenter(int renterID)
        {
            return await _unitOfWork.Repository<Booking>()
                .GetQueryable()
                .Where(x => !x.IsDelete && x.RenterID == renterID)
                .ProjectTo<BookingResponseModel>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task<BookingResponseModel> CreateBookingAsync(BookingRequestModel request)
        {
            if (request == null)
                throw new ArgumentException("Invalid request data.");

            // Validate time
            if (request.StartDate >= request.EndDate)
                throw new Exception("StartDate must be earlier than EndDate.");

            var vehicle = await _unitOfWork.Repository<Vehicle>().AsQueryable()
               .Where(u => !u.IsDelete && u.ModelID == request.ModelID && u.StationID == request.StationID && u.Status == 0)
               .FirstOrDefaultAsync();
            if (vehicle != null)
            {
                vehicle.Status = 1;
            }
            else
            {
                vehicle = await _unitOfWork.Repository<Vehicle>().AsQueryable()
                    .Where(u => !u.IsDelete && u.ModelID == request.ModelID && u.StationID == request.StationID
                        && !u.Bookings.Any(b =>
                            b.Status < 5 &&
                            b.StartDate < request.EndDate && b.EndDate > request.StartDate
                        ))
                    .FirstOrDefaultAsync();

                if (vehicle == null) throw new Exception("Car is full");

            }


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

            if (request.RentalType == 1)
            {
                if (!request.EndDate.HasValue) throw new Exception("EndDate is require!");
            }
            else if (request.RentalType == 2)
            {
                if (request.RentTime.HasValue)
                {
                    booking.EndDate = booking.StartDate.AddDays(7 * request.RentTime.Value);
                }
                else
                {
                    throw new Exception("RentTime is require!");
                }
            }
            else if (request.RentalType == 3)
            {
                if (request.RentTime.HasValue)
                {
                    booking.EndDate = booking.StartDate.AddDays(30 * request.RentTime.Value);
                }
                else
                {
                    throw new Exception("RentTime is require!");
                }
            }
            else
            {
                throw new Exception("RentType is only 1...3!");
            }
            var totalDays = (int)Math.Ceiling((booking.EndDate - booking.StartDate).TotalDays);
            booking.RetalCost = price.Price * totalDays;
            booking.Deposit = price.Deposit * request.RentalType;

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

            if (request.EndDate.HasValue)
            {
                if (request.EndDate.Value < DateTime.UtcNow) throw new Exception("EndDate is lower now");
               
                var dateSpan = (int)(request.EndDate.Value - existingBooking.EndDate).TotalDays;
                existingBooking.EndDate = request.EndDate.Value;
                var price = await _unitOfWork.Repository<RentalPrice>().AsQueryable().Where(u => u.ModelID == existingBooking.Vehicle.ModelID).FirstOrDefaultAsync();
                existingBooking.Overdue = price.Price * dateSpan;
                existingBooking.FinalCost = existingBooking.BaseCost + existingBooking.Overdue;

                hasUpdates = true;
            }

            if (hasUpdates)
            {
                await _unitOfWork.Repository<Booking>().Update(existingBooking, id);
                await _unitOfWork.SaveChangesAsync();
            }

            return _mapper.Map<StaffBookingResponseModel>(existingBooking);

        }

        public async Task<StaffBookingResponseModel> AutoUpdateBookingStatusAsync(int bookingId)
        {
            var existingBooking = await _unitOfWork.Repository<Booking>()
                .AsQueryable()
                .Where(s => s.Id == bookingId && !s.IsDelete && s.Status < 5 && s.Status > 0)
                .FirstOrDefaultAsync();
            if (existingBooking == null) return null;

            var existingVehicle = await _unitOfWork.Repository<Vehicle>()
                .AsQueryable()
                .Where(s => s.Id == existingBooking.VehicleID && !s.IsDelete && s.Status < 5 && s.Status > 0)
                .FirstOrDefaultAsync();
            if (existingVehicle == null || existingVehicle.Status == 0) return null;

            if (existingBooking.Status < 4)
            {
                existingBooking.Status++;
                existingVehicle.Status++;
            }
            else if (existingBooking.Status == 4)
            {
                existingBooking.Status = 5;

                var checkBooking = await _unitOfWork.Repository<Booking>()
                .AsQueryable()
                .Where(s => s.VehicleID == existingBooking.VehicleID && !s.IsDelete && s.Id != existingBooking.Id && s.Status < 5 && s.Status > 0)
                .FirstOrDefaultAsync();

                if (checkBooking != null) existingVehicle.Status = 1;
                else existingVehicle.Status = 0;
            }

            await _unitOfWork.Repository<Vehicle>().Update(existingVehicle, existingBooking.VehicleID);
            await _unitOfWork.Repository<Booking>().Update(existingBooking, bookingId);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<StaffBookingResponseModel>(existingBooking);
        }

        public async Task<StaffBookingResponseModel?> StaffRefusingAsync(int bookingId)
        {
            var existingBooking = await _unitOfWork.Repository<Booking>()
                .AsQueryable()
                .Where(s => s.Id == bookingId && !s.IsDelete && s.Status > 0 && s.Status < 4)
                .FirstOrDefaultAsync();
            if (existingBooking == null) return null;

            var existingVehicle = await _unitOfWork.Repository<Vehicle>()
                .AsQueryable()
                .Where(s => s.Id == existingBooking.VehicleID && !s.IsDelete)
                .FirstOrDefaultAsync();
            if (existingVehicle == null || existingVehicle.Status == 0) return null;
            existingBooking.Status = 6;

            var checkBooking = await _unitOfWork.Repository<Booking>()
                .AsQueryable()
                .Where(s => s.VehicleID == existingBooking.VehicleID && !s.IsDelete && s.Id != existingBooking.Id && s.Status < 5 && s.Status > 0)
                .FirstOrDefaultAsync();

            if (checkBooking != null) existingVehicle.Status = 1;
            else existingVehicle.Status = 0;

            await _unitOfWork.Repository<Vehicle>().Update(existingVehicle, existingBooking.VehicleID);
            await _unitOfWork.Repository<Booking>().Update(existingBooking, bookingId);
            await _unitOfWork.SaveChangesAsync();

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
