using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Shopf.Models.Data
{
    public class DB : DbContext
    {
        public DbSet<PagesDTO> Pages { get; set; }

        public DbSet<SidebarDTO> Sidebars { get; set; }
    }
}