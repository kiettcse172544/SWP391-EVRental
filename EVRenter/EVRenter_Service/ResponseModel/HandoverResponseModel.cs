using System;
using System.Collections.Generic;

namespace EVRenter_Service.ResponseModel
{
    public class HandoverResponseModel
    {
        public int Id { get; set; }

        public int BookingID { get; set; }

        public int VehicleID { get; set; }

        public int StationID { get; set; }

        public int Type { get; set; }

        public DateTime CheckDate { get; set; }

        public int Status { get; set; }

        public string? Description { get; set; }

        public List<HandoverItemStatusResponse> Items { get; set; } = new();
    }


    public class HandoverItemStatusResponse
    {
        public int CarItemID { get; set; }
        public string CarItemName { get; set; }
        public string CategoryName { get; set; }
        public int Condition { get; set; }     // 1 = Tốt, 0 = Hư hỏng
        public string? Note { get; set; }
    }
}
