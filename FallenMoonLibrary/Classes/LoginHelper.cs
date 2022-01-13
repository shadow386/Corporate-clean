using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FallenMoon.Library.Database;

namespace FallenMoon.Library
{
    public class LoginHelper
    {
        public UserAccount UserAccount { get; set; }
        public bool LoginSuccess { get; set; }
        public string LoginMessage { get; set; }
    }
}
