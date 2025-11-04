using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVRenter_Data.Entities
{
    public class Model : BaseEntity
    {
        public string ModelName { get; set; }
        public string Type { get; set; }
        public int Quantity { get; set; }
        public int Seat { get; set; }
        public int Range { get; set; }
        public int TrunkCapatity { get; set; }
        public int Hoursepower { get; set; }
        public int MoveLimit { get; set; }
        public virtual ICollection<Vehicle> Vehicles { get; set; }
        public virtual RentalPrice RentalPrice { get; set; }
        public virtual ICollection<Booking> Bookings { get; set; }
        public virtual ICollection<Feedback> Feedbacks { get; set; }
        public virtual ICollection<ModelImage> ModelImages { get; set; }
        public virtual ICollection<Amenities> Amenities { get; set; }
    }
}
