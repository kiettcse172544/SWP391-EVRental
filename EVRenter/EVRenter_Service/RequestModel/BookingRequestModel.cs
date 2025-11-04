using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVRenter_Service.RequestModel
{
    public class BookingRequestModel
    {
        public int ModelID { get; set; }
        public int RenterID { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int? RentalType { get; set; }
    }

    public class BookingUpdateRequest
    {

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
