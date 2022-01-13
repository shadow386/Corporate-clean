using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FallenMoon.Library.Database;

namespace FallenMoon.Library
{
    public class BlogHelper
    {
        public List<BlogPost> Stories { get; set; }
        public BlogPost Story { get; set; }
        public BlogPostComment Comment { get; set; }
        public bool HasNextPage { get; set; }
        public bool HasPrevPage { get; set; }
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
    }
}
