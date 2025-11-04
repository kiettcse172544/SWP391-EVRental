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
        public int ModelID { get; set; }
        public virtual Model Model { get; set; }
        public int RenterID { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int RentalType { get; set; }
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
}
