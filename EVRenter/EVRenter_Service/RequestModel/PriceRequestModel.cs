using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVRenter_Service.RequestModel
{
    public class PriceRequestModel
    {
        public int ModelID { get; set; }
        public decimal Price { get; set; }
        public decimal Deposit { get; set; }
    }

    public class PriceUpdateRequest
    {
        public int ModelID { get; set; }
        public decimal? Price { get; set; }
        public decimal? Deposit { get; set; }
    }
}
