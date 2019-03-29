using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingAPI.Model
{
    public class Photo
    {
        public int ID { get; set; }
        public string Url { get; set; }
        public string Description { get; set; }
        public bool IsMain { get; set; }
        public string CloudinaryID { get; set; }
        public DateTime DateAdded { get; set; }
        // Navigation Property
        public User User { get; set; }
        //FK
        public int UserId { get; set; }
    }
}
