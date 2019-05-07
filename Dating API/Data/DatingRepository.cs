using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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

        public async Task<User> GetLikersLinq(int userID)
        {
            var user = await _dbContext.Users.Include(p => p.Photos)
                                .Include(l => l.Likees)
                                .Include(lr => lr.Likers)
                                .Where(u => u.Id == userID).FirstOrDefaultAsync();
            return user;
        }

        public async Task<IEnumerable<User>> GetUsers()
        {
            var users = await _dbContext.Users.Include(p => p.Photos)
                                .ToListAsync();
            return users;
        }

        public TEntity GetByID<TEntity>(int ID) where TEntity : class {

            var response = _dbContext.Set<TEntity>().Find(ID);

            return response;
        }

        public async Task<bool> SaveAll()
        {
            var response = await _dbContext.SaveChangesAsync() > 0;
            return response;
        }

        public async Task<int> SaveOrUpdateReturnAffectedRowID()
        {
            var response = await _dbContext.SaveChangesAsync();
            return response;
        }

        public async Task<User> Update(User user)
        {
            var updatedUser = new User
            {
                Id = user.Id,
                City = user.City,
                Country = user.Country,
                Interests = user.Interests,
                Introduction = user.Introduction,
                LookingFor = user.LookingFor
            };

            _dbContext.Users.Update(user);

            var updatedUserID = _dbContext.SaveChanges();

            var response =await  _dbContext.Users
                                                .Include( p => p.Photos)    
                                                .Where(u => u.Id == updatedUserID).FirstOrDefaultAsync();

            return response;

        }


        public Photo UpdateMainPhoto(Photo photo, int UserID)
        {
            photo.IsMain = true;
            photo.UserId = UserID;

            // update 
            _dbContext.Photos.Update(photo);

            _dbContext.SaveChanges();

            return photo;
        }

        #region DB Operations that return an Entity : Stored Procedure


        public async Task<Like> SaveLike(int likerUserID, int likeeUserID)
        {
            var response = await _dbContext.Likes
                                           .FromSql("EXEC [dbo].[SaveLike] {0}, {1}, {2}",
                                            likerUserID, likeeUserID, DateTime.Now)
                                           .FirstOrDefaultAsync();

            return response;
        }

        #endregion

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

        public async Task<bool> PhotoExists(int photoID)
        {
            bool response;
            using (var command = _dbContext.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = "[dbo].[PhotoExist]"; // stored procedure name
                command.CommandType = CommandType.StoredProcedure;
                // stored procedure parameter
                command.Parameters.Add(new SqlParameter("@PhotoID", photoID));


                _dbContext.Database.OpenConnection();

                using (var result = await command.ExecuteReaderAsync())
                {
                    if (result.HasRows)
                    {
                        // read the result
                        while (result.Read())
                        {
                            // get the returned column value
                            response = (bool)result["PhotoExist"];
                            return response;
                        }
                    }
                }
                return false;
            }

        }

        public async Task<bool> DeletePhoto(int photoID)
        {
            bool response;
            using (var command = _dbContext.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = "[dbo].[DeletePhoto]"; // stored procedure name
                command.CommandType = CommandType.StoredProcedure;
                // stored procedure parameter
                command.Parameters.Add(new SqlParameter("@PhotoID", photoID));


                _dbContext.Database.OpenConnection();

                using (var result = await command.ExecuteReaderAsync())
                {
                    if (result.HasRows)
                    {
                        // read the result
                        while (result.Read())
                        {
                            // get the returned column value
                            response = (bool)result["Deleted"];
                            return response;
                        }
                    }
                }
                return false;
            }

        }

        public async Task<IEnumerable<User>> GetLikers(int UserID)
        {
            var response = new List<User>();
            using (var command = _dbContext.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = "[dbo].[GetLikers]"; // stored procedure name
                command.CommandType = CommandType.StoredProcedure;
                // stored procedure parameter
                command.Parameters.Add(new SqlParameter("@UserID", UserID));


                _dbContext.Database.OpenConnection();

                using (var result = await command.ExecuteReaderAsync())
                {
                    if (result.HasRows)
                    {
                        // read the result
                        while (result.Read())
                        {
                            // get the returned column value
                            response.Add(
                            new User
                            {
                                Id = (int)result["Id"],
                                Username = (string)result["Username"],
                                Gender = (string)result["Gender"],
                                DateOfBirth = (DateTime)result["DateOfBirth"],
                                Country = (string)result["Country"],
                                City = (string)result["City"],
                                CreatedAt = (DateTime)result["CreatedAt"],
                                LastActive = (DateTime)result["LastActive"],
                                Photos = new List<Photo> {
                                    new Photo {
                                        ID = (int)result["ID"],
                                        UserId = (int)result["UserId"],
                                        CloudinaryID = (string)result["CloudinaryID"],
                                        DateAdded = (DateTime)result["DateAdded"],
                                        Deleted = !result.IsDBNull(16) ? (DateTime)result["Deleted"] : (DateTime?)null,
                                        Description = (string)result["Description"],
                                        Url = (string)result["Url"],
                                        IsMain = (bool)result["IsMain"]
                                    }
                                }
                            });
                        }
                    }
                    return response;
                }
            }
        }

        public async Task<IEnumerable<User>> GetLikees(int UserID)
        {
            var response = new List<User>();
            using (var command = _dbContext.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = "[dbo].[GetLikees]"; // stored procedure name
                command.CommandType = CommandType.StoredProcedure;
                // stored procedure parameter
                command.Parameters.Add(new SqlParameter("@UserID", UserID));


                _dbContext.Database.OpenConnection();

                using (var result = await command.ExecuteReaderAsync())
                {
                    if (result.HasRows)
                    {
                        // read the result
                        while (result.Read())
                        {
                            // get the returned column value
                            response.Add(
                            new User
                            {
                                Id = (int)result["Id"],
                                Username = (string)result["Username"],
                                Gender = (string)result["Gender"],
                                DateOfBirth = (DateTime)result["DateOfBirth"],
                                Country = (string)result["Country"],
                                City = (string)result["City"],
                                CreatedAt = (DateTime)result["CreatedAt"],
                                LastActive = (DateTime)result["LastActive"],
                                Photos = new List<Photo> {
                                    new Photo {
                                        ID = (int)result["ID"],
                                        UserId = (int)result["UserId"],
                                        CloudinaryID = (string)result["CloudinaryID"],
                                        DateAdded = (DateTime)result["DateAdded"],
                                        Deleted = !result.IsDBNull(16) ? (DateTime)result["Deleted"] : (DateTime?)null,
                                        Description = (string)result["Description"],
                                        Url = (string)result["Url"],
                                        IsMain = (bool)result["IsMain"]
                                    }
                                }
                            });
                        }
                    }
                    return response;
                }
            }
        }
    }

    #endregion

}
                                                                                        
                                                                                           
                                                                                          
                                                                                           
                                                                                           