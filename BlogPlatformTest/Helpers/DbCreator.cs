using BlogPlatform.Models.EF_Model_classes;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BlogPlatformTest.Helpers
{
    static class DbCreator
    {
        static public readonly string postText = "Post";
        static public readonly string postHeader = "Header";
        static public readonly int blogId = 1;
        static public readonly int postId = 1;

        public static void AddItems(BlogsDataBaseContext ctx)
        {
            ctx.BlogPosts.Add(
                new BlogPost()
                {
                    BlogId = blogId,
                    BlogPostId = postId,
                    BodyText = postText,
                    HeaderText = postHeader,
                    Comments = new List<Comment>()
                });
            ctx.SaveChanges();
        }

        public static DbContextOptions<BlogsDataBaseContext> CreateInMemoryDatabase()
        {
            var options = new DbContextOptionsBuilder<BlogsDataBaseContext>()
                .UseInMemoryDatabase(databaseName: "Mock DB")
                .Options;

            // setup
            using (var ctx = new BlogsDataBaseContext(options))
                AddItems(ctx);

            return options;
        }
    }
}
