using CloudinaryDotNet.Actions;
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
        UploadResult UploadPhotoToCloudinary(string photoName, IFormFile photoStream);
    }
}
