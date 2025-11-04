using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVRenter_Service.ResponseModel
{
    public class RentalPriceResponse
    {
        public int Id { get; set; }
        public int ModelID { get; set; }
        public decimal Price { get; set; }
        public decimal Deposit { get; set; }
    }
}
