using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EFComment = BlogPlatform.Models.EF_Model_classes.Comment;
using BlogsDataBaseContext = BlogPlatform.Models.EF_Model_classes.BlogsDataBaseContext;
using AutoMapper;
using BlogPlatform.Models;
using Microsoft.Extensions.Logging;

namespace BlogPlatform.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentsController : ControllerCommon<CommentsController>
    {
        public CommentsController(
            BlogsDataBaseContext context,
            IMapper mapper,
            ILogger<CommentsController> logger) : base(context, mapper, logger)
        {
        }

        // GET: api/Comments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Comment>>> GetComments()
        {
            var commentlist = await context.Comments.ToListAsync();

            var apicommentlist = mapper.Map<List<Comment>>(commentlist);

            return apicommentlist;
        }

        // GET: api/Comments/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Comment>> GetComment(int id)
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

        // POST: api/Comments
        [HttpPost]
        public async Task<ActionResult<Comment>> PostComment(Comment comment)
        {
            var apicomment = mapper.Map<EFComment>(comment);
            context.Comments.Add(apicomment);
            await context.SaveChangesAsync();

            return CreatedAtAction("GetComment", new { id = apicomment.CommentId }, comment);
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
