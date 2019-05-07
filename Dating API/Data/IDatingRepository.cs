using DatingAPI.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingAPI.Data
{
    public interface IDatingRepository
    {
        void Add<T>(T entity) where T: class;
        void Delete<T>(T entity) where T : class;
        Task<bool> SaveAll();

        Task<int> SaveOrUpdateReturnAffectedRowID();
        Task<IEnumerable<User>> GetUsers();
        TEntity GetByID<TEntity>(int ID) where TEntity : class;
        Task<User> GetUser(int userID);
        Task<User> Update(User user);
        Photo UpdateMainPhoto(Photo photo, int userID);
        Task<bool> IsThereMainPhotoForUser(int userID);
        Task<bool> PhotoExists(int photoID);
        Task<bool> DeletePhoto(int photoID);
        Task<Like> SaveLike(int likerUserID, int likeeUserID);
        Task<IEnumerable<User>> GetLikers(int userID);
        Task<IEnumerable<User>> GetLikees(int userID);
    }
}
