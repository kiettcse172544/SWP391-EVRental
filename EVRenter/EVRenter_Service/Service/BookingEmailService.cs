using EVRenter_Data.Entities;
using EVRenter_Repository.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

namespace EVRenter_Service.Service
{
    public interface IBookingEmailService
    {
        Task<bool> SendSignatureEmailAsync(int bookingId);
        Task<bool> ConfirmSignatureAsync(string token);
    }

    public class BookingEmailService : IBookingEmailService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _config;

        public BookingEmailService(IUnitOfWork unitOfWork, IEmailService emailService, IConfiguration config)
        {
            _unitOfWork = unitOfWork;
            _emailService = emailService;
            _config = config;
        }

<<<<<<< Updated upstream
        
=======

>>>>>>> Stashed changes
        public async Task<bool> SendSignatureEmailAsync(int bookingId)
        {
            var booking = await _unitOfWork.Repository<Booking>()
                .AsQueryable()
                .Include(b => b.User)
                .FirstOrDefaultAsync(b => b.Id == bookingId && !b.IsDelete);

            if (booking == null)
                throw new Exception("Booking not found.");

            if (booking.Status != 1)
                throw new Exception("Booking is not in 'waiting for signature' status.");

            // Tạo token và hạn sử dụng
            var token = Guid.NewGuid().ToString();
            booking.SignatureToken = token;
            booking.SignatureTokenExpiresAt = DateTime.UtcNow.AddHours(24);

            await _unitOfWork.Repository<Booking>().UpdateAsync(booking);
            await _unitOfWork.SaveChangesAsync();

            // URL xác nhận
            var frontendUrl = "https://swp-391-fawn.vercel.app";
            var confirmUrl = $"{frontendUrl}/confirm-signature?token={token}";

            // Soạn mail
            var subject = "Xác nhận ký hợp đồng thuê xe EVRenter";
            var body = $@"
                <p>Xin chào <b>{booking.User.FullName}</b>,</p>
                <p>Bạn có một đơn thuê xe cần xác nhận ký hợp đồng.</p>
                <p>Vui lòng nhấn vào liên kết sau để ký xác nhận:</p>
                <p><a href='{confirmUrl}' target='_blank'>{confirmUrl}</a></p>
                <p>Liên kết này sẽ hết hạn sau 24 giờ.</p>
                <br/>
                <p>Trân trọng,<br/>Đội ngũ EVRenter</p>
            ";

            await _emailService.SendEmailAsync(booking.User.Email, subject, body);
            return true;
        }

        public async Task<bool> ConfirmSignatureAsync(string token)
        {
            var booking = await _unitOfWork.Repository<Booking>()
                .AsQueryable()
                .FirstOrDefaultAsync(b => b.SignatureToken == token && !b.IsDelete);

            if (booking == null)
                throw new Exception("Invalid or used token.");

            if (booking.SignatureTokenExpiresAt < DateTime.UtcNow)
                throw new Exception("This signature link has expired.");

<<<<<<< Updated upstream
            
=======

>>>>>>> Stashed changes
            booking.Status = 2;
            booking.SignedAt = DateTime.UtcNow;
            booking.SignatureToken = null;
            booking.SignatureTokenExpiresAt = null;

            await _unitOfWork.Repository<Booking>().UpdateAsync(booking);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}

