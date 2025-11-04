
using AutoMapper;
using AutoMapper.QueryableExtensions;
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
        Task<BookingResponseModel> CreateBookingAsync(BookingRequestModel request);
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

        public async Task<BookingResponseModel?> GetBookingByIdAsync(int id)
        {
            // Get the user with basic information
            var booking = await _unitOfWork.Repository<Booking>().AsQueryable()
                .Where(u => u.Id == id)
                .ProjectTo<BookingResponseModel>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();

            return booking;
        }

        public async Task<BookingResponseModel> CreateBookingAsync(BookingRequestModel request)
        {
            if (request == null)
                throw new ArgumentException("Invalid request data.");

            var vehicle = await _unitOfWork.Repository<Vehicle>().AsQueryable()
               .Where(u => u.Id == request.VehicleID)
               .FirstOrDefaultAsync();
            if (vehicle == null)
            {
                throw new Exception("Car not found");
            }

            var user = await _unitOfWork.Repository<User>().AsQueryable()
               .Where(u => u.Id == request.RenterID)
               .FirstOrDefaultAsync();
            if (user == null)
            {
                throw new Exception("Renter not found");
            }

            var booking = _mapper.Map<Booking>(request);

            if (booking.StartDate.AddDays(7) > booking.EndDate) booking.RentalType = 1; //Daily
            else if (booking.StartDate.AddMonths(1) > booking.EndDate) booking.RentalType = 2; //Weekly 
            else if (booking.StartDate.AddYears(1) > booking.EndDate) booking.RentalType = 3; // Monthly
            else booking.RentalType = 4; //Yearly

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
            booking.BaseCost = booking.RetalCost + booking.Deposit;


            await _unitOfWork.Repository<Booking>().InsertAsync(booking);
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
