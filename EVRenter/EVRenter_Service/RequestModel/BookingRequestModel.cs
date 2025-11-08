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
        public int StationID { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Status { get; set; }
    }

    public class BookingStatusUpdateRequest
    {
        public int Status { get; set; }
    }

    public class BookingUpdateRequest
    {
        public int? Status { get; set; }
        public int? VehicleID { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

    }
}
