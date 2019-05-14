using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingAPI.DTOs
{
    public class MessageInOutBoxDTO
    {
        public int ID { get; set; }
        public string Content { get; set; }
        public bool IsRead { get; set; }
        public DateTime DateRead { get; set; }
        public DateTime MessageSent { get; set; }
        public int SenderID { get; set; }
        public string SenderKnownAs { get; set; }
        public int RecipientID { get; set; }
        public string RecipientKnownAs { get; set; }
        public string SenderPhotoURL { get; set; }
        public string RecipientPhotoURL { get; set; }
    }
}
