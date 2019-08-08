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
            logger.LogInformation("Getting all posts for blog {blogId}", blogId);
            var posts = await context.Blogs
                                     .Where(b => b.BlogId == blogId)
                                     .SelectMany(b => b.BlogPosts)
                                     .Include(p => p.Comments)
                                     .ToListAsync();

            var apiBlogPostList = mapper.Map<List<FullBlogPost>>(posts);
            logger.LogInformation("Successfully got all posts for blog {blogId}", blogId);
            return apiBlogPostList;
        }

        // GET: api/BlogPosts/5
        [HttpGet("{id}")]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Any)]
        public async Task<ActionResult<FullBlogPost>> GetBlogPost(int id)
        {
            logger.LogInformation("Getting post by id {id}", id);
            var blogPost = await GetBlogPostWithCommentsById(id);
            if (blogPost == null)
            {
                logger.LogWarning("Post with id {id} doesn't exist", id);
                return NotFound("Post id doesn't exist");
            }

            var apiBlogPost = mapper.Map<FullBlogPost>(blogPost);
            logger.LogWarning("Succesfully fetched post with id {id}", id);
            return apiBlogPost;
        }

        // PUT: api/BlogPosts/5
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutBlogPost(int id, BlogPost blogPost)
        {
            logger.LogInformation("Editing post by id {id}", id);
            var (errorResult, efblogPost) = await CheckEditingRights(id);
            if (errorResult != null)
            {
                logger.LogInformation("Editing post {id} not possible/allowed", id);
                return errorResult;
            }

            var putBlogPost = mapper.Map<EFBlogPost>(blogPost);
            putBlogPost.BlogPostId = id;
            putBlogPost.BlogId = efblogPost.BlogId;

            var entry = context.Entry(efblogPost);
            entry.CurrentValues.SetValues(putBlogPost);
            entry.State = EntityState.Modified;

            logger.LogInformation("Edited post {id}, writing to the database", id);
            try
            {
                await context.SaveChangesAsync();
                logger.LogInformation("Post {id} writing to the database succeeded", id);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BlogPostExists(id))
                {
                    logger.LogInformation("Post {id} has been deleted concurrently during update", id);
                    return NotFound("Post doesn't exist any more");
                }
                logger.LogInformation("Post {id} editing has encountered concurrency problem", id);
                throw;
            }

            return NoContent();
        }

        // POST: api/BlogPosts
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<int>> PostBlogPost(BlogPost blogPost)
        {
            logger.LogInformation("Adding new post");
            var apiBlogPost = mapper.Map<EFBlogPost>(blogPost);

            var blog = await GetBlog();
            if (blog.BlogPosts == null)
                blog.BlogPosts = new List<EFBlogPost>();
            blog.BlogPosts.Add(apiBlogPost);

            logger.LogInformation("Post added, writing to the database");
            await context.SaveChangesAsync();

            logger.LogInformation("Post with id {id} successfully created", apiBlogPost.BlogPostId);
            return apiBlogPost.BlogPostId;
        }

        // DELETE: api/BlogPosts/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult<BlogPost>> DeleteBlogPost(int id)
        {
            logger.LogInformation("Deleting post by id {id}", id);
            var (errorResult, blogPost) = await CheckEditingRights(id);
            if (errorResult != null)
            {
                logger.LogInformation("Deleting post {id} not possible/allowed", id);
                return errorResult;
            }

            var deleteBlogPost = mapper.Map<BlogPost>(blogPost);
            context.BlogPosts.Remove(blogPost);

            logger.LogInformation("Post deleted, writing to the database");
            await context.SaveChangesAsync();

            logger.LogInformation("Post deleting succeeded");
            return deleteBlogPost;
        }

        bool BlogPostExists(int id) =>
            context.BlogPosts.Any(e => e.BlogPostId == id);

        async Task<(ActionResult, EFBlogPost)> CheckEditingRights(int id)
        {
            var post = await context.BlogPosts.FindAsync(id);
            if (post == null)
            {
                logger.LogWarning("Post with id {id} doesn't exist", id);
                return (NotFound("No post with this id"), null);
            }

            var entry = context.Entry(post);
            var editorBlogId = GetBlogId();
            if (post.BlogId != editorBlogId)
            {
                logger.LogWarning("Post with id {id} belongs to {ActualBlogId}, " +
                    "not modifyable by {EditorBlogId}", id, post.BlogId, editorBlogId);
                return (Forbid(), null);
            }

            logger.LogInformation("Security checks for modification of post {id} passed", id);
            return (null, post);
        }
    }
}
