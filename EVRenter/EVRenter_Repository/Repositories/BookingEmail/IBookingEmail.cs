using EVRenter_Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVRenter_Repository.Repositories.BookingEmail
{
    public interface IBookingEmail
    {
        Task<Booking> GetBookingById(int id);
        Task<Vehicle> GetVehicleById(int id);
        Task<Booking> GetBookingByToken(string token);
        Task UpdateBooking(Booking booking);
        Task UpdateVehicle(Vehicle vehicle);
    }
}
