using EVRenter_Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVRenter_Repository.Repositories.HandoverRepo
{
    public interface IHandoverRepository
    {
        Task<Booking> GetBookingById(int id);
        Task<bool> CheckStaff(int id);
        Task UpdateVehicle(Vehicle vehicle);
        Task UpdateBooking(Booking booking);
        Task AddHandover(HandoverAndReturn handoverAndReturn);
        Task<List<CarItem>> GetCarItems(int vehicleId);
        Task<HandoverAndReturn> GetHandover(int handoverId);
        Task<Vehicle> GetVehicle(int vehicleId);
        Task<List<HandoverAndReturn>> GetAllHandovers();
        Task<HandoverAndReturn> GetHandoverById(int id);
    }
}
