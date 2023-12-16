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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            modelBuilder.Entity<User>()
                .HasKey(u => u.Id);
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();
            modelBuilder.Entity<User>()
                .Property(u => u.Password)
                .IsRequired();
            modelBuilder.Entity<User>()
                .HasMany(u => u.Posts)
                .WithOne(p => p.User)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<User>()
                .HasMany(u => u.Comments)
                .WithOne(c => c.User)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<User>()
                .HasMany(u => u.Interactions)
                .WithOne(i => i.User)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Post>()
                .HasKey(p => p.Id);
            modelBuilder.Entity<Post>()
                .HasMany(p => p.Comments)
                .WithOne(c => c.Post)
                .OnDelete(DeleteBehavior.Cascade);
          

            modelBuilder.Entity<Comment>()
                .HasKey(c => c.Id);
         

            modelBuilder.Entity<Interaction>()
                .HasKey(i => i.Id);
            
            modelBuilder.Entity<Interaction>()
                .HasOne(i => i.Post)
                .WithMany(p => p.Interactions)
                .HasForeignKey("PostId")
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired(false);

            modelBuilder.Entity<Interaction>()
                .HasOne(i => i.Comment)
                .WithMany(c => c.Interactions)
                .HasForeignKey("CommentId")
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired(false);
        }
    }
}
