using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVRenter_Data.Entities
{
    public class Image : BaseEntity
    {
        public string ContentType { get; set; }
        public string Base64Image { get; set; }
        public virtual ICollection<ModelImage> ModelImages { get; set; }
        public virtual ICollection<IDImage> IDImages { get; set; }
        public virtual ICollection<DriverLicenseImage> DriverLicenseImages { get; set; }
    }
}
