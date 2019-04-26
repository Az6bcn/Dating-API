using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingAPI.Model
{
    public class Like
    {
        public int LikeID { get; set; }
        // PK _ FKs
        public int LikerUserID { get; set; }
        public int LikeeUserID { get; set; }
        public DateTime Date { get; set; }
        //navigation Property
        public User LikerUser { get; set; }
        public User LikeeUser { get; set; }
    }
}
