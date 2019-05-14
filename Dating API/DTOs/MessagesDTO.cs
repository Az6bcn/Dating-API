using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingAPI.DTOs
{
    public class MessagesDTO
    {
        public int SenderID { get; set; }
        public int RecipientID { get; set; }
        public DateTime SentDate { get; set; }
        public string Content { get; set; }

        public MessagesDTO()
        {
            SentDate = DateTime.Now;
        }
    }
}
