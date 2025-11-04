using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace EVRenter_Data.Entities
{
    public class RenterProfile : BaseEntity
    {
        public int UserID { get; set; }
        public string IDNumber { get; set; }
        public string DriverLicenseNo { get; set; }
        public virtual User User { get; set; }
        public virtual ICollection<IDImage> IDImages { get; set; }
        public virtual ICollection<DriverLicenseImage> DriverLicenseImages { get; set; }

    }
}
