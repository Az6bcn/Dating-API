using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using DatingAPI.Model;
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

        public UploadResult UploadPhotoToCloudinary(string photoName, Stream photoStream)
        {
            Account cloudinarAaccount = new Account(_cloudinarySettings.CloudName, _cloudinarySettings.ApiKey,
                _cloudinarySettings.ApiSecret );

            Cloudinary cloudinary = new Cloudinary(cloudinarAaccount);


            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(photoName, photoStream)
            };

            var uploadResult = cloudinary.Upload(uploadParams);

            return uploadResult;
        }
    }
}
