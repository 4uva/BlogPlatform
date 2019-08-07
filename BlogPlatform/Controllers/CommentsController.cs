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
        public async Task<ActionResult<IEnumerable<Comment>>> GetComments(int blogPostId)
        {
            var postWithComments = await GetBlogPostWithCommentsById(blogPostId);
            if (postWithComments == null)
                return NotFound();

            var apicommentlist = mapper.Map<List<Comment>>(postWithComments.Comments);
            return apicommentlist;
        }

        // GET: api/Blogpost/5/Comments/7
        [HttpGet("{id}")]
        public async Task<ActionResult<Comment>> GetComment(int blogPostId, int id)
        {
            var comment = await context.Comments.FindAsync(id);
            if (comment == null)
                return NotFound();

            var apicomment = mapper.Map<Comment>(comment);
            return apicomment;
        }

        // PUT: api/Comments/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutComment(int id, Comment comment)
        {
            var putcomment = mapper.Map<EFComment>(comment);
            context.Entry(putcomment).State = EntityState.Modified;

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CommentExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/BlogPost/5/Comments
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Comment>> PostComment(int blogPostId, Comment comment)
        {
            var efcomment = mapper.Map<EFComment>(comment);
            var blogPost = await context.BlogPosts.FindAsync(blogPostId);
            blogPost.Comments = new[] { efcomment };
            await context.SaveChangesAsync();

            return CreatedAtAction("GetComment", new { id = efcomment.CommentId }, comment);
        }

        // DELETE: api/Comments/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Comment>> DeleteComment(int id)
        {
            var comment = await context.Comments.FindAsync(id);
            if (comment == null)
                return NotFound();

            var deleteComment = mapper.Map<Comment>(comment);
            context.Comments.Remove(comment);
            await context.SaveChangesAsync();

            return deleteComment;
        }

        private bool CommentExists(int id) =>
            context.Comments.Any(e => e.CommentId == id);
    }
}
