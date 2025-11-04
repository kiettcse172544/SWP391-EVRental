using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVRenter_Data.Entities
{
    public class ModelImage
    {
        public int ModelID { get; set; }
        public int ImageID { get; set; }
        public virtual Image Image { get; set; }
        public virtual Model Model { get; set; }
    }
}
