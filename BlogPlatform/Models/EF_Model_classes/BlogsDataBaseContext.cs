using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BlogPlatform.Models.EF_Model_classes
{
    public class BlogsDataBaseContext : IdentityDbContext<User, IdentityRole, string>
    {
        public BlogsDataBaseContext(DbContextOptions<BlogsDataBaseContext> options)
               : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                        .HasOne(u => u.Blog)
                        .WithOne(b => b.User)
                        .HasForeignKey<Blog>(b => b.UserId)
                        .IsRequired();

            modelBuilder.Entity<Blog>()
                        .HasMany(b => b.BlogPosts)
                        .WithOne(p => p.Blog)
                        .IsRequired();

            modelBuilder.Entity<BlogPost>()
                        .HasMany(p => p.Comments)
                        .WithOne()
                        .IsRequired();

            modelBuilder.Entity<Comment>()
                        .HasOne(p => p.User);
        }

        public DbSet<Blog> Blogs { get; set; }
        public DbSet<BlogPost> BlogPosts { get; set; }
        public DbSet<Comment> Comments { get; set; }
    }
}

       
