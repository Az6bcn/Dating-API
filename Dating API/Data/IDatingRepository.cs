﻿using DatingAPI.Helpers;
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
        Task<PagedList<User>> GetUsers(UserParams userParams);
        TEntity GetByID<TEntity>(int ID) where TEntity : class;
        Task<User> GetUser(int userID);
        Task<User> Update(User user);
        Task<Photo> UpdateMainPhoto(Photo photo, Photo currentMainPhoto);
        Task<bool> IsThereMainPhotoForUser(int userID);
        Task<Like> SaveLike(Like like);
        Task<IEnumerable<User>> GetLikers(int userID);
        Task<IEnumerable<User>> GetLikees(int userID);
        Task<IEnumerable<Message>> GetMessageForUserInbox(int userID);
        Task<IEnumerable<Message>> GetMessageForUserOutbox(int userID);
        Task<Message> GetUserMessage(int messageID);
        Task<IEnumerable<Message>> GetMessageThread(int userID, int recipientID);


    }
}
