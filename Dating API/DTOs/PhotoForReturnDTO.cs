using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingAPI.DTOs
{
    public class PhotoForReturnDTO
    {
        public int ID { get; set; }
        public string Url { get; set; }
        public string Description { get; set; }
        public DateTime DateAdded { get; set; }
        public string CloudinaryID { get; set; }
        public bool IsMain { get; set; }
    }
}
