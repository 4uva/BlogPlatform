using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using BlogPlatform.Models.EF_Model_classes;
using AutoMapper;

namespace BlogPlatform.Controllers
{
    public class ControllerCommon<T> : ControllerBase
    {
        public ControllerCommon(
            BlogsDataBaseContext context,
            IMapper mapper,
            ILogger<T> logger)
        {
            this.context = context;
            this.mapper = mapper;
            this.logger = logger;
        }

        protected int GetBlogId() =>
            int.Parse(HttpContext.User.Claims.Single(c => c.Type == "BlogId").Value);

        protected async Task<Blog> GetBlogById(int id) =>
            await context.Blogs.FindAsync(id);

        protected Task<Blog> GetBlog() => GetBlogById(GetBlogId());

        protected async Task<BlogPost> GetBlogPostWithCommentsById(int id)
        {
            var blogpost =
                await context.BlogPosts
                             .Include(p => p.Comments)
                                 .ThenInclude(c => c.Author)
                             .SingleOrDefaultAsync(p => p.BlogPostId == id);
            return blogpost;
        }

        protected readonly BlogsDataBaseContext context;
        protected readonly IMapper mapper;
        protected readonly ILogger logger;
    }
}
