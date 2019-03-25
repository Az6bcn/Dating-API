using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatingAPI.DTOs;
using DatingAPI.Model;
using Microsoft.EntityFrameworkCore;

namespace DatingAPI.Data
{
    public class DatingRepository : IDatingRepository
    {
        private DataDbContext _dbContext;

        public DatingRepository(DataDbContext dataDbContext)
        {
            _dbContext = dataDbContext;
        }


        public void Add<T>(T entity) where T : class
        {
            _dbContext.Add<T>(entity);
        }

        public void Delete<T>(T entity) where T : class
        {
            _dbContext.Remove<T>(entity);
        }

        public async Task<User> GetUser(int userID)
        {
            var user = await _dbContext.Users.Include(p => p.Photos)
                                .Where(u => u.Id == userID).FirstOrDefaultAsync();
            return user;
        }

        public async Task<IEnumerable<User>> GetUsers()
        {
            var users = await _dbContext.Users.Include(p => p.Photos)
                                .ToListAsync();
            return users;
        }

        public async Task<bool> SaveAll()
        {
            var response = await _dbContext.SaveChangesAsync() > 0;
            return response;
        }

        public async Task<User> Update(User user)
        {
            var response = await _dbContext.Users
                                           .FromSql("EXEC [dbo].[UPDATE_USER_PROFILE] {0}, {1}, {2 },  {3}, {4},  {5}",
                                           user.Id, user.City, user.Country, user.Interests, user.Introduction, user.LookingFor)
                                           .FirstOrDefaultAsync();

            var photos = await GetUserPhotosByUserID(user.Id);

            response.Photos = photos as List<Photo>;

            return response;
        }

        public async Task<IEnumerable<Photo>> GetUserPhotosByUserID(int UserID)
        {
            var response = await _dbContext.Photos
                                           .FromSql("EXEC [dbo].[GetUserPhotoByUserID] {0}", UserID)
                                           .ToListAsync();

            return response;
        }

    }                                                                                          
}                                                                                         
                                                                                           
                                                                                          
                                                                                           
                                                                                           