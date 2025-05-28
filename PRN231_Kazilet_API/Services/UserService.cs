using Microsoft.EntityFrameworkCore;
using PRN231_Kazilet_API.Models.Entities;
using PRN231_Kazilet_API.Utils;
using System;
using System.Security.Cryptography;
using System.Text;

namespace PRN231_Kazilet_API.Services
{
    public interface IUserService
    {
        Task<bool> UserExists(string email);
        int Register(User user);
        int RegisterGoogle(User user);
        User? GetUserByEmail(string email);
        bool UpdateUser(User u);
        bool ChangePassword(int uid, string newpwd);
        Task<User> Authenticate(string username, string password);
        User GetUser(int uid);
        Task<bool> ResetPasswordAsync(User user, string token, string newPassword);
        Task UpdatePasswordAsync(User user, string newPassword);
    }

    public class UserService : IUserService
    {
        private readonly Common utils = new Common();
        private readonly PRN231_Kazilet_v2Context _context = new PRN231_Kazilet_v2Context();

        public Task<bool> UserExists(string email)
        {
            return Task.FromResult(_context.Users.Any(u => u.Email == email));
        }

        public int Register(User user)
        {
            string hashPwd = utils.HashPassword(user.Password);
            user.Password = hashPwd;
            _context.Users.Add(user);
            int rs = _context.SaveChanges();
            return rs > 0 ? user.Id : -1;
        }

        public int RegisterGoogle(User user)
        {
            _context.Users.Add(user);
            int rs = _context.SaveChanges();
            return rs > 0 ? user.Id : -1;
        }

        public User? GetUserByEmail(string email)
        {
            return _context.Users
                .Include(u => u.RoleNavigation)
                .FirstOrDefault(u => u.Email == email);
        }

        public Task<User> Authenticate(string email, string password)
        {
            var user = _context.Users
                .Include(u => u.RoleNavigation)
                .FirstOrDefault(u => u.Email == email
                && u.Password == utils.HashPassword(password));
            return Task.FromResult(user);
        }

        public User GetUser(int uid)
        {
            User u = _context.Users.Include(u => u.RoleNavigation).FirstOrDefault(u => u.Id == uid);
            return u;
        }

        public Task<bool> ResetPasswordAsync(User user, string token, string newPassword)
        {
            throw new NotImplementedException();
        }

        public Task UpdatePasswordAsync(User user, string newPassword)
        {
            throw new NotImplementedException();
        }

        public bool UpdateUser(User u)
        {
            if (u.Id == null || _context.Users.Find(u.Id) == null) return false;
            User user = _context.Users.Find(u.Id);
            user.Username = u.Username;
            user.Email = u.Email;
            user.Role = u.Role;
            user.Type = u.Type;
            user.Gid = u.Gid;
            _context.Users.Update(user);
            return _context.SaveChanges() > 0;
        }

        public bool ChangePassword(int uid, string newpwd)
        {
            User user = _context.Users.Find(uid);
            if(user == null) return false;
            user.Password = newpwd;
            _context.Users.Update(user);
            return _context.SaveChanges() > 0;
        }
    }
}
