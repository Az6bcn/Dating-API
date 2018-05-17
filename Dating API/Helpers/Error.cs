 using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingAPI.Model
{
    /// <summary>
    /// Establece el mensaje de error, sea una
    /// traducción, exception message o un string
    /// </summary>
    public class Error
    {
        public string ErrorMessage { get; set; }
    }
}
