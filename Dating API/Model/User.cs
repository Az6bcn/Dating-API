using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingAPI.Model
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public string Gender { get; set; }
        public string KnownAs { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string Introduction { get; set; }
        public string LookingFor { get; set; }
        public string Interests { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastActive { get; set; }

        // navigation collection
        public IList<Photo> Photos { get; set; }
        public IList<Like> Likers { get; set; }
        public IList<Like> Likees { get; set; }


        public User()
        {
            Photos = new List<Photo>();
        }
    }
}
