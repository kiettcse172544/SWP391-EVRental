using EVRenter_Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVRenter_Service.RequestModel
{
    public class VehicleRequestModel
    {
        public int ModelID { get; set; }
        public int StationID { get; set; }
        public string Location { get; set; }
        public string PlateNumber { get; set; }
        public int BatteryLevel { get; set; }
        public int Odometer { get; set; }
        public string Color { get; set; }
    }

    public class VehicleUpdateRequest
    {
        public string? PlateNumber { get; set; }
        public int? ModelID { get; set; }
        public int? StationID { get; set; }
        public string? Location { get; set; }
        public int? BatteryLevel { get; set; }
        public int? Odometer { get; set; }
        public string? Color { get; set; }
        public int? Status { get; set; }
    }

    public class UpdateCarItemsRequest
    {
        public int VehicleID { get; set; }
        public List<CarItemUpdateModel> Items { get; set; }
    }

    public class CarItemUpdateModel
    {
        public string Name { get; set; }     // Tên CarItem (vd: "Đèn pha trước")
        public int Status { get; set; }      // Trạng thái mới
    }

}
