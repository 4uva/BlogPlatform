using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using AutoMapper;

using BlogPlatform.Models;

using BlogsDataBaseContext = BlogPlatform.Models.EF_Model_classes.BlogsDataBaseContext;
using EFComment = BlogPlatform.Models.EF_Model_classes.Comment;

namespace BlogPlatform.Controllers
{
    [Route("api/BlogPosts/{blogPostId}/[controller]")]
    [ApiController]
    public class CommentsController : ControllerCommon<CommentsController>
    {
        public CommentsController(
            BlogsDataBaseContext context,
            IMapper mapper,
            ILogger<CommentsController> logger) : base(context, mapper, logger)
        {
        }

        // GET: api/BlogPost/5/Comments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FullComment>>> GetComments(int blogPostId)
        {
            var postWithComments = await GetBlogPostWithCommentsById(blogPostId);
            if (postWithComments == null)
                return NotFound();

            var apicommentlist = mapper.Map<List<FullComment>>(postWithComments.Comments);
            return apicommentlist;
        }

        // GET: api/Blogpost/5/Comments/7
        [HttpGet("{id}")]
        public async Task<ActionResult<FullComment>> GetComment(int blogPostId, int id)
        {
            var comment = await context.Comments.FindAsync(id);
            if (comment == null)
                return NotFound("Comment id doesn't exist");

            await context.Entry(comment).Reference(c => c.Author).LoadAsync();
            if (comment.BlogPostId != blogPostId)
                return BadRequest("Comment belongs to different post");

            var apicomment = mapper.Map<FullComment>(comment);
            return apicomment;
        }

        // PUT: api/Blogpost/5/Comments/7
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutComment(int blogPostId, int id, Comment comment)
        {
            var (errorResult, efcomment) = await CheckEditingRights(blogPostId, id);
            if (errorResult != null)
                return errorResult;

            var putcomment = mapper.Map<EFComment>(comment);
            putcomment.CommentId = id;
            putcomment.BlogPostId = efcomment.BlogPostId;

            // all checks are done, saving
            var entry = context.Entry(efcomment);
            entry.CurrentValues.SetValues(putcomment);
            entry.State = EntityState.Modified;

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CommentExists(id))
                    return NotFound("Comment doesn't exist any more");
                throw;
            }

            return NoContent();
        }

        // POST: api/BlogPost/5/Comments
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<int>> PostComment(int blogPostId, Comment comment)
        {
            var efcomment = mapper.Map<EFComment>(comment);
            efcomment.Author = await GetBlog();
            var blogPost = await context.BlogPosts.FindAsync(blogPostId);
            if (blogPost == null)
                return NotFound("Post doesn't exist");

            blogPost.Comments = new[] { efcomment };
            await context.SaveChangesAsync();

            return efcomment.CommentId;
        }

        // DELETE: api/BlogPosts/5/Comments/7
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult<FullComment>> DeleteComment(int blogPostId, int id)
        {
            var (errorResult, efcomment) = await CheckEditingRights(blogPostId, id);
            if (errorResult != null)
                return errorResult;

            var deleteComment = mapper.Map<FullComment>(efcomment);
            context.Comments.Remove(efcomment);
            await context.SaveChangesAsync();

            return deleteComment;
        }

        bool CommentExists(int id) =>
            context.Comments.Any(e => e.CommentId == id);

        async Task<(ActionResult, EFComment)> CheckEditingRights(int blogPostId, int id)
        {
            var comment = await context.Comments.FindAsync(id);
            if (comment == null)
                return (NotFound("No comment with this id"), null);

            if (comment.BlogPostId != blogPostId)
                return (BadRequest("Comment belongs to a different post"), null);

            var entry = context.Entry(comment);
            await entry.Reference(c => c.Author).LoadAsync();
            if (comment.Author.BlogId != GetBlogId())
                return (Forbid(), null);

            return (null, comment);
        }
    }
}
