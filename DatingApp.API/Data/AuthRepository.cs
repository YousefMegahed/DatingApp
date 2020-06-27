using System;
using System.Linq;
using System.Threading.Tasks;
using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Data
{
    public class AuthRepository : IAuthRepository
    {
        private readonly DataContext _dbcontext;

        public AuthRepository(DataContext dbcontext)
        {
            _dbcontext = dbcontext;
        }
        public async Task<User> Login(string username, string password)
        {
            var User = await _dbcontext.Users.FirstOrDefaultAsync(z=>z.Username ==username);

            if(User == null) return null;

            if(!VerifyPasswordHash(password,User.PasswordHash,User.PasswordSalt))return null;

            return User;
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
             using(var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                 var ComputedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < ComputedHash.Length; i++)
                {
                    if(ComputedHash[i] != passwordHash[i]) return false;
                }
            }
            return true;
        }

        public async Task<User> Register(User user, string password)
        {
           byte[] passwordHash, passwordSalt;
           // out this passing by reference
           CreatePasswordHash(password,out passwordHash, out passwordSalt);
           user.PasswordHash = passwordHash;
           user.PasswordSalt = passwordSalt;
             
           await _dbcontext.AddAsync(user);
           await _dbcontext.SaveChangesAsync();
           return user; 
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using(var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                 passwordSalt = hmac.Key;
                 passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        public async Task<bool> UserExists(string username)
        {
            if(await _dbcontext.Users.AnyAsync(z=>z.Username == username)) return true;

            return false;
        }
    }
}