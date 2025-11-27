using EVRenter_Service.RequestModel;
using EVRenter_Service.ResponseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVRenter_Service.IService
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
}
