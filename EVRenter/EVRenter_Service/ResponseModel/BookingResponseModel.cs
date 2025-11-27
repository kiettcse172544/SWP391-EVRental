using EVRenter_Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVRenter_Service.ResponseModel
{
    public class BookingResponseModel
    {
        public int Id { get; set; }
        public int RentalType { get; set; }
        public int VehicleID { get; set; }
        public int StationID { get; set; }
        public string StationName { get; set; }
        public int RenterID { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        [Precision(18, 0)]
        public decimal Deposit { get; set; }
        [Precision(18, 0)]
        public decimal RetalCost { get; set; }
        public int? VoucherID { get; set; }
        [Precision(18, 0)]
        public decimal BaseCost { get; set; }
        [Precision(18, 0)]
        public decimal? FinalCost { get; set; }
        public int Status { get; set; }
    }

    public class CarBookingResponseModel
    {
        public int Id { get; set; }
        public int Status { get; set; }
        public DateTime RequestTime { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        [Precision(18, 0)]
        public decimal Deposit { get; set; }
        [Precision(18, 0)]
        public decimal RetalCost { get; set; }
        [Precision(18, 0)]
        public decimal BaseCost { get; set; }
        public int RentalTime { get; set; }
        public int RentalType { get; set; }
        public int? VoucherID { get; set; }
        [Precision(18, 0)]
        public decimal? FinalCost { get; set; }
    }

    public class StaffBookingResponseModel
    {
        public int Id { get; set; }
        public int RentalType { get; set; }
        public int Status { get; set; }
        public int VehicleID { get; set; }
        public int RenterID { get; set; }
        public int StationID { get; set; }
        public string StationName { get; set; }
        public DateTime RequestTime { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int RentalTime { get; set; }
        [Precision(18, 0)]
        public decimal Deposit { get; set; }
        [Precision(18, 0)]
        public decimal RetalCost { get; set; }
        public int? VoucherID { get; set; }
        [Precision(18, 0)]
        public decimal BaseCost { get; set; }
        [Precision(18, 0)]
        public decimal? FinalCost { get; set; }
        public CustomerResponseModel Customer { get; set; }
        public VehicleForBookingResponseModel Vehicle {  get; set; }

    }
}
