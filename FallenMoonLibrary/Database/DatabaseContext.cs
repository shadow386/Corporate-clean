using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FallenMoon.Library.Database
{
    public class DatabaseContext : DEV_fallencoreEntities
    {
        public DatabaseContext(bool lazyLoading = true)
        {
            this.Configuration.LazyLoadingEnabled = lazyLoading;
        }
    }
}