using AutoMapper;
using BlogPlatform;
using BlogPlatform.Controllers;
using BlogPlatform.Models;
using BlogPlatform.Models.EF_Model_classes;
using BlogPlatformUnitTest.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

using EFBlogPost = BlogPlatform.Models.EF_Model_classes.BlogPost;
using EFComment = BlogPlatform.Models.EF_Model_classes.Comment;

namespace BlogPlatformUnitTest
{
    public class BlogPostControllerUnitTest
    {
        [Fact]
        public async Task TestUnregisteredUserLogin()
        {
            var options = new DbContextOptionsBuilder<BlogsDataBaseContext>()
                .UseInMemoryDatabase(databaseName: "Mock DB")
                .Options;

            string postText = "Post";
            string postHeader = "Header";
            int blogId = 1;
            int postId = 1;

            // setup
            using (var ctx = new BlogsDataBaseContext(options))
            {
                ctx.BlogPosts.Add(
                    new EFBlogPost()
                    {
                        BlogId = blogId,
                        BlogPostId = postId,
                        BodyText = postText,
                        HeaderText = postHeader,
                        Comments = new List<EFComment>()
                    });
                await ctx.SaveChangesAsync();
            }

            var loggerMock = new Mock<ILogger<BlogPostsController>>();

            var mapperConf = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<EFBlogPost, FullBlogPost>();
            });
            var mapper = new Mapper(mapperConf);

            FullBlogPost post;
            using (var ctx = new BlogsDataBaseContext(options))
            {
                var controller = new BlogPostsController(ctx, mapper, loggerMock.Object);

                // act
                var result = await controller.GetBlogPost(1);
                post = result.Value;
            }

            // check
            Assert.NotNull(post);
            Assert.Equal(postText, post.BodyText);
            Assert.Equal(postHeader, post.HeaderText);
        }
    }
}
