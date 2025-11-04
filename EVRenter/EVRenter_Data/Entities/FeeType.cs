using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVRenter_Data.Entities
{
    public class FeeType : BaseEntity
    {
        public int ExtraFeeID { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        [Precision(18, 2)] 
        public decimal UnitPrice { get; set; }
        public virtual ExtraFee ExtraFee { get; set; }

    }
}
