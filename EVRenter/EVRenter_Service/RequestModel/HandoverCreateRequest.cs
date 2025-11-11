using System;

namespace EVRenter_Service.RequestModel
{
    public class HandoverCreateRequest
    {
        
        public int BookingID { get; set; }
        public int StaffID {  get; set; }
        public string? Description { get; set; }
    }
}
