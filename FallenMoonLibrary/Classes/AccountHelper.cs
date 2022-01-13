using FallenMoon.Library.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FallenMoon.Library
{
    public class AccountHelper
    {
        public UserAccount Account { get; set; }
        public UserProfile Profile { get; set; }
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
    }
}
