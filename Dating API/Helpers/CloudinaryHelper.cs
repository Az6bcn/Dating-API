using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using DatingAPI.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DatingAPI.Helpers
{
    public class CloudinaryHelper: ICloudinaryHelper
    {
        private CloudinarySettings _cloudinarySettings;
     
        public CloudinaryHelper(IOptions<CloudinarySettings> cloudinarySettings)
        {
            _cloudinarySettings = cloudinarySettings.Value;
        }

        /// <summary>
        /// Upload image File to Cloudinary
        /// </summary>
        /// <param name="photoName"></param>
        /// <param name="photoStream"></param>
        /// <returns></returns>
        public async Task<UploadResult> UploadPhotoToCloudinary(string photoName, IFormFile formFile)
        {
            Account cloudinarAaccount = new Account(_cloudinarySettings.CloudName, _cloudinarySettings.ApiKey,
                _cloudinarySettings.ApiSecret );

            Cloudinary cloudinary = new Cloudinary(cloudinarAaccount);

            ImageUploadResult uploadResult;
            using (var fileStream = formFile.OpenReadStream())
            {
                var uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(photoName, fileStream)
                };

                 uploadResult = cloudinary.Upload(uploadParams);
            }

            

            return uploadResult;
        }

        private Stream FileToUploadStreamAsync(IFormFile formFile)
        {
            Stream stream;
            using (var fileStream = formFile.OpenReadStream())
            {
                stream = fileStream;
            }

            return stream;
        }

        public Photo ParsePhoto(string cloudinaryID, string cloudinaryUrl, int userID, string description, DateTime dateAdded, bool isMain)
        {
            var photo = new Photo
            {
                CloudinaryID = cloudinaryID,
                DateAdded = dateAdded,
                Description = description,
                Url = cloudinaryUrl,
                UserId = userID,
                IsMain = isMain
            };
            return photo;
        }
    }
}
