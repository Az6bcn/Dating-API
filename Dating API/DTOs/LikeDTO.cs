using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingAPI.DTOs
{
    public class LikeDTO
    {
        public int LikeID { get; set; }
        // PK _ FKs
        public int LikerUserID { get; set; }
        public int LikeeUserID { get; set; }
        public DateTime Date { get; set; }
        public UserDTO LikerUser { get; set; }
        public UserDTO LikeeUser { get; set; }
    }
}
