using AutoMapper;
using BlogPlatform;
using BlogPlatform.Controllers;
using BlogPlatform.Models;
using BlogPlatform.Models.EF_Model_classes;
using BlogPlatformTest.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace BlogPlatformUnitTest
{
    public class BlogPostControllerUnitTest
    {
        [Fact]
        public async Task TestGetExistingBlogPost()
        {
            var options = DbCreator.CreateInMemoryDatabase();
            var loggerMock = new Mock<ILogger<BlogPostsController>>();
            var mapper = TestMapper.CreateMapper();

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
            Assert.Equal(DbCreator.postText, post.BodyText);
            Assert.Equal(DbCreator.postHeader, post.HeaderText);
        }
    }
}
