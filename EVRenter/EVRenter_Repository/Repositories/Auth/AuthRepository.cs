using Azure.Core;
using EVRenter_Data.Entities;
using EVRenter_Repository.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVRenter_Repository.Repositories.Auth
{
    public class AuthRepository : IAuthRepository
    {
        private readonly IUnitOfWork _unitOfWork;

        public AuthRepository(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task AddUserAsync(User user)
        {
            await _unitOfWork.Repository<User>().AddAsync(user);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<bool> CheckEmail(string email)
        {
            return await _unitOfWork.Repository<User>()
                .AsQueryable().AnyAsync(u => u.Email.Trim() == email.Trim() && !u.IsDelete);
        }

        public async Task<bool> CheckPhoneAsync(string phone)
        {
            return await _unitOfWork.Repository<User>().AsQueryable()
                .AnyAsync(u => u.Phone.Trim() == phone.Trim() && !u.IsDelete);
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _unitOfWork.Repository<User>()
                .AsQueryable()
                .FirstOrDefaultAsync(u => u.Email.Trim() == email.Trim() && !u.IsDelete);
        }

        public async Task<User> GetUserById(int id)
        {
            return await _unitOfWork.Repository<User>().AsQueryable()
                .FirstOrDefaultAsync(u => u.Id == id && !u.IsDelete);
        }

        public async Task<User> GetUserByToken(string token)
        {
            return await _unitOfWork.Repository<User>().AsQueryable()
                .FirstOrDefaultAsync(u =>
                    u.EmailVerificationToken == token &&
                    !u.IsDelete);
        }

        public async Task UpdateUserAsync(User user)
        {
            var userRepo = _unitOfWork.Repository<User>();
            await userRepo.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();

        }
    }


}
