using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;


namespace Doubble.Models
{
    public class DoubbleDBContext : DbContext
    {
        public DbSet<DoubbleRequest> DoubbleRequests { get; set; }
        public DbSet<ForSterling> ForSterlings { get; set; }
        public DbSet<ForNonSterling> ForNonSterlings { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<State> States { get; set; }
        public DbSet<Branches> Branch { get; set; }
    }
}