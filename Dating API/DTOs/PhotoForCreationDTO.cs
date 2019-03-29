using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingAPI.DTOs
{
    public class PhotoForCreationDTO
    {
        public string CloudinaryUrl { get; set; }
        public IFormFile File { get; set; }
        public string Description { get; set; }
        public DateTime DateAdded { get; set; }
        public string CloudinaryID { get; set; }
        public int UserID { get; set; }

        public PhotoForCreationDTO()
        {
            DateAdded = DateTime.Now;
        }
    }
}
