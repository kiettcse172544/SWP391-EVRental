using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVRenter_Data.Entities
{
    public class RentalPrice : BaseEntity
    {
        public int ModelID { get; set; }
        [Precision(18, 2)] 
        public decimal Price { get; set; }
        [Precision(18, 2)] 
        public decimal Deposit { get; set; }
        public virtual Model Model { get; set; }
    }
}
