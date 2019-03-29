using CloudinaryDotNet.Actions;
using DatingAPI.Model;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DatingAPI.Helpers
{
    public interface ICloudinaryHelper
    {
        Task<UploadResult> UploadPhotoToCloudinary(string photoName, IFormFile photoStream);
        Photo ParsePhoto(string cloudinaryID, string cloudinaryUrl, int userID, string description, DateTime dateAdded, bool isMain);
    }
}
