using FallenMoon.Library;
using FallenMoon.Library.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FallenMoon.Models
{
    public class BlogViewModel
    {
        public List<Tweet> Tweets { get; set; }
        public List<BlogPost> Stories { get; set; }
        public BlogPost Story { get; set; }
        public bool HasNextPage { get; set; }
        public bool HasPrevPage { get; set; }
        public int CurrentPage { get; set; }
    }
}