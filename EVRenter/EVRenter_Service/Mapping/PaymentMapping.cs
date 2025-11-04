using AutoMapper;
using EVRenter_Data.Entities;
using EVRenter_Service.RequestModel;
using EVRenter_Service.ResponseModel;

namespace EVRenter_Service.Mapping
{
    public class PaymentMapping : Profile
    {
        public PaymentMapping()
        {
            // Map khi tạo payment mới
            CreateMap<PaymentCreateRequest, Payment>()
                .ForMember(dest => dest.PaymentType,
                    opt => opt.MapFrom(src =>
                        src.PaymentMethod.ToLower() == "vnpay" ? PaymentType.VnPay : PaymentType.Cash))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => PaymentStatus.Pending))
                .ForMember(dest => dest.PaymentTime, opt => opt.Ignore())
                .ForMember(dest => dest.TransactionId, opt => opt.Ignore())
                .ForMember(dest => dest.ReferenceCode, opt => opt.Ignore())
                .ForMember(dest => dest.PaymentUrl, opt => opt.Ignore());

            // Map từ entity ra response
            CreateMap<Payment, PaymentResponseModel>()
                .ForMember(dest => dest.UserEmail, opt => opt.MapFrom(src => src.User.Email))
                .ForMember(dest => dest.PaymentMethod, opt => opt.MapFrom(src => src.PaymentType.ToString()))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));
        }
    }
}
