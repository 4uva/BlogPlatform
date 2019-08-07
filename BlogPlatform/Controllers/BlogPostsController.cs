using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using AutoMapper;

using BlogPlatform.Models;

using BlogsDataBaseContext = BlogPlatform.Models.EF_Model_classes.BlogsDataBaseContext;
using EFBlog = BlogPlatform.Models.EF_Model_classes.Blog;
using EFBlogPost = BlogPlatform.Models.EF_Model_classes.BlogPost;

namespace BlogPlatform.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogPostsController : ControllerCommon<BlogPostsController>
    {
        public BlogPostsController(
            BlogsDataBaseContext context,
            IMapper mapper,
            ILogger<BlogPostsController> logger) : base(context, mapper, logger)
        {
        }

        // GET: api/BlogPosts
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<FullBlogPost>>> GetBlogPosts()
        {
            var blog = await GetBlog();
            await context.Entry(blog).Collection(b => b.BlogPosts).LoadAsync();

            var apiBlogPostList = mapper.Map<List<FullBlogPost>>(blog.BlogPosts);

            return apiBlogPostList;
        }

        // GET: api/BlogPosts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<FullBlogPost>> GetBlogPost(int id)
        {
            var blogPost = await context.BlogPosts.FindAsync(id);

            if (blogPost == null)
                return NotFound();

            var apiBlogPost = mapper.Map<FullBlogPost>(blogPost);
            return apiBlogPost;
        }

        // PUT: api/BlogPosts/5
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutBlogPost(int id, BlogPost blogPost)
        {
            var putBlogPost = mapper.Map<EFBlogPost>(blogPost);
            putBlogPost.BlogPostId = id;

            var existingPost = await context.BlogPosts.FindAsync(id);
            if (existingPost == null)
                return NotFound();

            var entry = context.Entry(existingPost);
            await entry.Reference(p => p.Blog).LoadAsync();
            if (existingPost.Blog.BlogId != GetBlogId())
                return Forbid(); // wrong blog :-P

            entry.CurrentValues.SetValues(putBlogPost);
            entry.State = EntityState.Modified;

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BlogPostExists(id))
                    return NotFound();
                throw;
            }

            return NoContent();
        }

        // POST: api/BlogPosts
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<int>> PostBlogPost(BlogPost blogPost)
        {
            var apiBlogPost = mapper.Map<EFBlogPost>(blogPost);

            var blog = await GetBlog();
            if (blog.BlogPosts == null)
                blog.BlogPosts = new List<EFBlogPost>();
            blog.BlogPosts.Add(apiBlogPost);
            await context.SaveChangesAsync();

            return apiBlogPost.BlogPostId;
        }

        // DELETE: api/BlogPosts/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult<BlogPost>> DeleteBlogPost(int id)
        {
            var blogPost = await context.BlogPosts.FindAsync(id);
            if (blogPost == null)
                return NotFound();

            var entry = context.Entry(blogPost);
            await entry.Reference(p => p.Blog).LoadAsync();
            if (blogPost.Blog.BlogId != GetBlogId())
                return Forbid(); // wrong blog :-P

            var deleteBlogPost = mapper.Map<BlogPost>(blogPost);
            context.BlogPosts.Remove(blogPost);
            await context.SaveChangesAsync();

            return deleteBlogPost;
        }

        bool BlogPostExists(int id) =>
            context.BlogPosts.Any(e => e.BlogPostId == id);
    }
}
