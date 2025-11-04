using EVRenter_Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVRenter_Service.ResponseModel
{
    public class VehicleResponseModel
    {
        public int Id { get; set; }
        public int ModelID { get; set; }
        public string ModelName { get; set; }
        public string PlateNumber { get; set; }
        public int BatteryLevel { get; set; }
        public int Odometer { get; set; }
        public int StationID { get; set; }
        public string StationName { get; set; }
        public string StationLocation { get; set; }
        public int Status { get; set; }
    }

    public class VehicleDetailResponseModel
    {
        public int Id { get; set; }
        public int ModelID { get; set; }
        public string ModelName { get; set; }
        public string PlateNumber { get; set; }
        public int BatteryLevel { get; set; }
        public int Odometer { get; set; }
        public int StationID { get; set; }
        public string StationName { get; set; }
        public string StationLocation { get; set; }
        public int Status { get; set; }
        public List<CategoryChecklistResponse> Categories { get; set; }
    }

    public class CategoryChecklistResponse
    {
        //public int CategoryID { get; set; }
        public string CategoryName { get; set; }
        public List<CarItemResponse> Items { get; set; }
    }

    public class CarItemResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Status { get; set; }
    }
}
