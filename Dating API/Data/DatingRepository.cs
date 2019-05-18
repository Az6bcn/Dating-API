using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using DatingAPI.DTOs;
using DatingAPI.Helpers;
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
            var user = await _dbContext.Users
                                .Include(p => p.Photos)
                                .Where(u => u.Id == userID).FirstOrDefaultAsync();
            return user;
        }

        public async Task<User> GetLikersLinq(int userID)
        {
            var user = await _dbContext.Users.Include(p => p.Photos)
                                .Include(l => l.Likees)
                                .Include(lr => lr.Likers)
                                .Where(u => u.Id == userID).FirstOrDefaultAsync();
            return user;
        }

        public async Task<PagedList<User>> GetUsers(UserParams userParams)
        {
            var users = _dbContext.Users.Include(p => p.Photos);

            var pagedUsers = await PagedList<User>.CreateAsync(users, userParams.PageNumber, userParams.PageSize);

            return pagedUsers;
        }

        public TEntity GetByID<TEntity>(int ID) where TEntity : class {

            var response = _dbContext.Set<TEntity>().Find(ID);

            return response;
        }

        public async Task<bool> SaveAll()
        {
            //SaveChangesAsync => return £s of rows affected, @@ROWSCOUNT
            var response = await _dbContext.SaveChangesAsync() > 0;
            return response;
        }


        public async Task<User> Update(User user)
        {
            _dbContext.Users.Update(user);

            var isUpdated = await SaveAll();


            if (isUpdated) return user;

            return null;

        }


        public async Task<Photo> UpdateMainPhoto(Photo photo, Photo currentMainPhoto)
        {
            // update 
            _dbContext.Photos.UpdateRange(photo, currentMainPhoto);

            var isUpdated = await SaveAll();

            if (isUpdated) return photo;

            return null;
        }


        public async Task<Like> SaveLike(Like like)
        {
           
            _dbContext.Likes.Add(like);

            var isAdded = await SaveAll();

            if (isAdded) return like;

            return null;
        } 

        public async Task<IEnumerable<User>> GetLikers(int UserID)
        {
            var likers = await _dbContext.Likes
                                   .Include(lkru => lkru.LikerUser)
                                   .ThenInclude(p => p.Photos)
                                   .Include(lkeeu => lkeeu.LikeeUser)
                                   .ThenInclude(p1 => p1.Photos)
                                   .Where(u => u.LikerUserID == UserID)
                                   .Select(like => new User
                                   {
                                       Id = like.LikeeUserID,
                                       Username = like.LikeeUser.Username,
                                       Gender = like.LikeeUser.Gender,
                                       DateOfBirth = like.LikeeUser.DateOfBirth,
                                       Country = like.LikeeUser.Country,
                                       City = like.LikeeUser.City,
                                       CreatedAt = like.LikeeUser.CreatedAt,
                                       LastActive = like.LikeeUser.LastActive,
                                       Photos = like.LikeeUser.Photos
                                   })
                                   .ToListAsync();

            return likers;
        }

        public async Task<IEnumerable<User>> GetLikees(int UserID)
        {
            var likers = await _dbContext.Likes
                                   .Include(lkru => lkru.LikerUser)
                                   .ThenInclude(p => p.Photos)
                                   .Include(lkeeu => lkeeu.LikeeUser)
                                   .ThenInclude(p1 => p1.Photos)
                                   .Where(u => u.LikeeUserID == UserID)
                                   .Select(like => new User
                                   {
                                       Id = like.LikerUserID,
                                       Username = like.LikerUser.Username,
                                       Gender = like.LikerUser.Gender,
                                       DateOfBirth = like.LikerUser.DateOfBirth,
                                       Country = like.LikerUser.Country,
                                       City = like.LikerUser.City,
                                       CreatedAt = like.LikerUser.CreatedAt,
                                       LastActive = like.LikerUser.LastActive,
                                       Photos = like.LikerUser.Photos
                                   })
                                   .ToListAsync();

            return likers;
        }


        public async Task<IEnumerable<Message>> GetMessageForUserInbox(int userID)
        {
            var response =  await _dbContext.Messages
                                            .Include(sender => sender.Sender)
                                            .ThenInclude(p => p.Photos)
                                            .Include(recipient => recipient.Recipient)
                                            .ThenInclude(p => p.Photos)
                                            .Where(m => m.RecipientID == userID)
                                            .ToListAsync();
            return response;
        }

        public async Task<IEnumerable<Message>> GetMessageForUserOutbox(int userID)
        {
            var response = await _dbContext.Messages
                                           .Include(sender => sender.Sender)
                                           .ThenInclude(p => p.Photos)
                                           .Include(recipient => recipient.Recipient)
                                           .ThenInclude(p => p.Photos)
                                           .Where(m => m.SenderID == userID)
                                           .ToListAsync();
            return response;
        }



        public async Task<Message> GetUserMessage(int messageID)
        {
            var response = await _dbContext.Messages
                                     .Include(sender => sender.Sender)
                                     .ThenInclude(p => p.Photos)
                                     .Include(recipient => recipient.Recipient)
                                     .ThenInclude(p => p.Photos)
                                     .Where(m => m.ID == messageID)
                                     .FirstOrDefaultAsync();

            return response;
        }

        public async Task<IEnumerable<Message>> GetMessageThread(int userID, int recipientID)
        {
            var response = await _dbContext.Messages
                                     .Include(sender => sender.Sender)
                                     .ThenInclude(p => p.Photos)
                                     .Include(recipient => recipient.Recipient)
                                     .ThenInclude(p => p.Photos)
                                     .Where(m => (m.SenderID == userID && m.RecipientID == recipientID)
                                                    || (m.SenderID == recipientID && m.RecipientID == userID))
                                     .ToListAsync();

            return response;
        }

        #region DB Operations that don't return an exact/specific entity: Stored Procedure
        /********** db operations that don't return a entity */
        public async Task<bool> IsThereMainPhotoForUser(int userID)
        {
            bool response;
            using (var command = _dbContext.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = "[dbo].[IsThereMainPhotoForUser]"; // stored procedure name
                command.CommandType = CommandType.StoredProcedure;
                // stored procedure parameter
                command.Parameters.Add(new SqlParameter("@UserID", userID));


                _dbContext.Database.OpenConnection();

                using (var result = await command.ExecuteReaderAsync())
                {
                    if (result.HasRows)
                    {
                        // read the result
                        while (result.Read())
                        {
                            // get the returned column value
                            response = (bool)result["Response"];
                            return response;
                        }
                    }
                }
                return false;
            }

        }


        

    }

    #endregion

}
                                                                                        
                                                                                           
                                                                                          
                                                                                           
                                                                                           