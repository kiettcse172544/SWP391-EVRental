using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVRenter_Service.RequestModel
{
    public class ExtraFeeCreateRequest
    {
        public int BookingId { get; set; }             
        public int HandoverAndReturnId { get; set; }  
        public int UserId { get; set; }                 

        
        public List<FeeTypeCreateRequest>? FeeTypes { get; set; }
    }

    public class FeeTypeCreateRequest
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
