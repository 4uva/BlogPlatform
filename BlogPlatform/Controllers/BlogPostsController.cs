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
            // cannot explicitly load subitems so a full query is needed
            var blogId = GetBlogId();
            var posts = await context.Blogs
                                     .Where(b => b.BlogId == blogId)
                                     .SelectMany(b => b.BlogPosts)
                                     .Include(p => p.Comments)
                                     .ToListAsync();

            var apiBlogPostList = mapper.Map<List<FullBlogPost>>(posts);
            return apiBlogPostList;
        }

        // GET: api/BlogPosts/5
        [HttpGet("{id}")]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Any)]
        public async Task<ActionResult<FullBlogPost>> GetBlogPost(int id)
        {
            var blogPost = await GetBlogPostWithCommentsById(id);
            if (blogPost == null)
                return NotFound("Post id doesn't exist");

            var apiBlogPost = mapper.Map<FullBlogPost>(blogPost);
            return apiBlogPost;
        }

        // PUT: api/BlogPosts/5
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutBlogPost(int id, BlogPost blogPost)
        {
            var (errorResult, efblogPost) = await CheckEditingRights(id);
            if (errorResult != null)
                return errorResult;

            var putBlogPost = mapper.Map<EFBlogPost>(blogPost);
            putBlogPost.BlogPostId = id;
            putBlogPost.BlogId = efblogPost.BlogId;

            var entry = context.Entry(efblogPost);
            entry.CurrentValues.SetValues(putBlogPost);
            entry.State = EntityState.Modified;

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BlogPostExists(id))
                    return NotFound("Post doesn't exist any more");
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
            var (errorResult, blogPost) = await CheckEditingRights(id);
            if (errorResult != null)
                return errorResult;

            var deleteBlogPost = mapper.Map<BlogPost>(blogPost);
            context.BlogPosts.Remove(blogPost);
            await context.SaveChangesAsync();

            return deleteBlogPost;
        }

        bool BlogPostExists(int id) =>
            context.BlogPosts.Any(e => e.BlogPostId == id);

        async Task<(ActionResult, EFBlogPost)> CheckEditingRights(int id)
        {
            var post = await context.BlogPosts.FindAsync(id);
            if (post == null)
                return (NotFound("No post with this id"), null);

            var entry = context.Entry(post);
            if (post.BlogId != GetBlogId())
                return (Forbid(), null);

            return (null, post);
        }
    }
}
