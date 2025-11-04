using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVRenter_Data.Entities
{
    public class Feedback : BaseEntity
    {
        public int UserID { get; set; }
        public int ModelID { get; set; }
        public string Comment {  get; set; }
        public DateTime CreatedDate { get; set; }
        public virtual User User { get; set; }
        public virtual Model Model { get; set; }
    }
}
