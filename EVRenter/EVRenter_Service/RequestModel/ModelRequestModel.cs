using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVRenter_Service.RequestModel
{
    public class ModelRequestModel
    {
        public string ModelName { get; set; }
        public string Type { get; set; }
        public int Seat { get; set; }
        public int Range { get; set; }
        public int TrunkCapatity { get; set; }
        public int Hoursepower { get; set; }
        public int MoveLimit { get; set; }
        public int ChargingTime { get; set; }
        public int ChargePower { get; set; }
    }

    public class ModelUpdateRequest
    {
        public string? ModelName { get; set; }
        public string? Type { get; set; }
        public int? Seat { get; set; }
        public int? Range { get; set; }
        public int? TrunkCapatity { get; set; }
        public int? Hoursepower { get; set; }
        public int? MoveLimit { get; set; }
        public int? ChargingTime { get; set; }
        public int? ChargePower { get; set; }
    }
}
