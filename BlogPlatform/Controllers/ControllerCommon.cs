using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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

        protected async Task<Blog> GetBlog() =>
            await context.Blogs.FindAsync(GetBlogId());

        protected readonly BlogsDataBaseContext context;
        protected readonly IMapper mapper;
        protected readonly ILogger logger;
    }
}
