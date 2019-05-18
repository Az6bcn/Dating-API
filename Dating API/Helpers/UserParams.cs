using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingAPI.Helpers
{
    public class UserParams
    {
        // FixedMaxPageSize we can return from our API
        private const int MaxPageSize = 10;
        public int PageNumber { get; set; } = 1;
        
        //default pageSize
        private int pageSize = 5; 
        public int PageSize
        {
            get { return pageSize; }

            // control the MaxPageSize we can return from our API, if the PageSize been requested is > the MaxSize(10) 
            // return the default MaxPageSize else whatever has beeen requested.
            // if client doesn't specify we would return the default pagesize(5)
            set
            {
                pageSize = (value > MaxPageSize) ? MaxPageSize : value;
            }
        }

        public int UserID { get; set; }
        public string Gender { get; set; }
        public int MinAge { get; set; } = 18;
        public int MaxAge { get; set; } = 99;
    }
}
