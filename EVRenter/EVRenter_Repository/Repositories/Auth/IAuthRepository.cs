using EVRenter_Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVRenter_Repository.Repositories.Auth
{
    public interface IAuthRepository
    {
        Task<User> GetUserByEmailAsync(string email);
        Task AddUserAsync(User user);
        Task<bool> CheckEmail(string email);
        Task<bool> CheckPhoneAsync(string phone); 
        Task<User> GetUserByToken(string token);
        Task UpdateUserAsync(User user);
        Task<User> GetUserById(int id);
    }
}
