namespace EVRenter_Service.ResponseModel
{
    public class ExtraFeeResponseModel
    {
        public int Id { get; set; }
        public int BookingId { get; set; }
        public int HandoverAndReturnId { get; set; }
        public decimal Deposit { get; set; }
        public decimal Amount { get; set; }
        public decimal Cost { get; set; }
        public bool IsRefunded { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<FeeTypeResponseModel> FeeTypes { get; set; } = new();
    }

    public class FeeTypeResponseModel
    {
        public string Name { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
    }
}
