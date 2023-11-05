using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MyGolb.Models;

namespace MyGolb.Data
{
    public class MyGolbContext : DbContext
    {
        public MyGolbContext (DbContextOptions<MyGolbContext> options)
            : base(options)
        {
        }

        public DbSet<MyGolb.Models.User> User { get; set; } = default!;
        public DbSet<MyGolb.Models.Interaction>? Interaction { get; set; }
        public DbSet<MyGolb.Models.Post>? Post { get; set; }
        public DbSet<MyGolb.Models.Comment>? Comment { get; set; }
    }
}
