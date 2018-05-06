using DatingAPI.Model;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace DatingAPI.Data
{
    public class AuthRepository : IAuthRepository
    {
        private readonly DataDbContext _dataDbContext;

        // DI: Inject my DbContext into my Repository to use it to fetch data from DB
        public AuthRepository(DataDbContext dataDbContext)
        {
            _dataDbContext = dataDbContext;
        }


        public async Task<User> Login(string username, string password)
        {
            var user = await _dataDbContext.Users.FirstOrDefaultAsync(x => x.Username == username);

            if (user == null) return null;

            // verify that the hashed version of the password input by user matched the password hash in the DB
            if (!verifyPasswordHash(password, user.PasswordHash, user.PasswordSalt)) return null;

            // authsuccessful
            return user;

        }

        private bool verifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            // hash password and copared with one in DB
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                hmac.Key = passwordSalt;
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password)); // hash password

                // compare hash with one stored in DB
                var equal = computedHash.SequenceEqual(passwordHash);

                return equal;
            }
        }

        public async Task<User> Register(User user, string password)
        {
            byte[] passwordHash, passwordSalt;

            // out => to pass a refrence to the variables.
            createPassword(password, out passwordHash, out passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            // add new user 
            await _dataDbContext.Users.AddAsync(user);
            await _dataDbContext.SaveChangesAsync();

            return user;
        }


        private void createPassword(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            // salt and hash password
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                // assign values to the variables 'passwordHash, passwordSalt' through the references 
                passwordSalt = hmac.Key; // assign the randomly generated key as password salt (random generated characters);
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password)); // hash password: adds the randomly generated key(salt) to the password before hashing.
            }
            /* HMACSHA512: creates a randomly generated key.
                The key is our password salt
                The key is then internally added to our password and hash it.
             */
        }

        public async Task<bool> UserExists(string username)
        {
            var exists = await _dataDbContext.Users.AnyAsync(x => x.Username == username);

            return exists;

        }
    }
}
