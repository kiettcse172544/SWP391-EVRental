using EVRenter_Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVRenter_Service.ResponseModel
{
    public class ModelResponseModel
    {
        public int Id { get; set; }
        public string ModelName { get; set; }
        public string Type { get; set; }
        public int Quantity { get; set; }
        public PriceResponseModel Price { get; set; }
        public DepositResponseModel Deposit { get; set; }
        public List<string> Features { get; set; }
        //public List<string> Specifications { get; set; }
        public SpecificationsModel Specifications { get; set; }
        public List<string> Amenities { get; set; }
        //public virtual ICollection<Vehicle> Vehicles { get; set; }
    }

    public class PriceResponseModel
    {
        public decimal Daily { get; set; }
        public decimal Weekly { get; set; }
        public decimal Monthly { get; set; }
    }

    public class DepositResponseModel
    {
        public decimal Daily { get; set; }
        public decimal Weekly { get; set; }
        public decimal Monthly { get; set; }
    }

    public class SpecificationsModel
    {
        public int Seat { get; set; }
        public int Range { get; set; }
        public int TrunkCapatity { get; set; }
        public int Hoursepower { get; set; }
        public string CarModel { get; set; }
        public int MoveLimit { get; set; }
    }
}
