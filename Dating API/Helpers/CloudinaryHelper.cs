using DatingAPI.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingAPI.Helpers
{
    public class CloudinaryHelper
    {
        private CloudinarySettings _cloudinarySettings;
        public CloudinaryHelper(CloudinarySettings cloudinarySettings)
        {
            _cloudinarySettings = cloudinarySettings;
        }
    }
}
