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
    public class CloudinaryHelper
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
        public async Task<UploadResult> UploadPhotoToCloudinaryAsync(string photoName, IFormFile photoStream)
        {
            Account cloudinarAaccount = new Account(_cloudinarySettings.CloudName, _cloudinarySettings.ApiKey,
                _cloudinarySettings.ApiSecret );

            Cloudinary cloudinary = new Cloudinary(cloudinarAaccount);

            var stream = await FileToUploadStreamAsync(photoStream);

            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(photoName, stream)
            };

            var uploadResult = cloudinary.Upload(uploadParams);

            return uploadResult;
        }

        private async Task<Stream> FileToUploadStreamAsync(IFormFile formFile)
        {
            Stream stream;
            using (var memoryStream = new MemoryStream())
            {
                await formFile.CopyToAsync(memoryStream);
                stream = memoryStream;
            }

            return stream;
        }

        public Photo ParsePhoto(string cloudinaryID, string cloudinaryUrl, int userID, string description, DateTime dateAdded)
        {
            var photo = new Photo
            {
                CloudinaryID = cloudinaryID,
                DateAdded = dateAdded,
                Description = description,
                Url = cloudinaryUrl,
                UserId = userID
            };
        }
    }
}
