using DatingAPI.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingAPI.Data
{
    public class SeedData
    {
        private DataDbContext _dbContext;
        public SeedData(DataDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        public void Seed()
        {
            // read users from json
            var usersJsonString = System.IO.File.ReadAllText("Data/UserSeedData.json");
            // convert this json read string to object 
            var Users = JsonConvert.DeserializeObject<List<User>>(usersJsonString);


            //save to DB with DBcontext
            foreach (var user in Users)
            {
                byte[] passwordHash, passwordSalt;
                createPassword("password", out passwordHash, out passwordSalt);

                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;
                user.Username = user.Username.ToLower();

                _dbContext.Add(user);
                _dbContext.SaveChanges();


            }
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
    }
}
