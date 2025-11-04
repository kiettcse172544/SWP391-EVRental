using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVRenter_Data.Entities
{
    public class ItemCategory : BaseEntity
    {
        public string Name { get; set; }
        public virtual ICollection<CarItem> Items { get; set; }
    }
}
