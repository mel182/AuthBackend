using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplicationAPI.Extension;
using WebApplicationAPI.Model;
using WebApplicationAPI.Models;
using WebApplicationAPI.Utility;
using System.Web.Http.Cors;
using Microsoft.AspNetCore.Authorization;
using WebApplicationAPI.Controllers.CustomBaseController;

namespace WebApplicationAPI.Controllers
{
    [Authorize]
    [EnableCors(origins: "AllowSpecificOrigin", headers: "*", methods: "*")]
    [Route("api/post")]
    [ApiController]
    public class PostsController : BaseSecurityController
    {
        private readonly WebApplicationAPIContext _context;

        public PostsController(WebApplicationAPIContext context)
        {
            _context = context;
        }

        // GET: api/post/all
        [Route("all")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Post>>> GetPost()
        {
            if(IsAdmin())
            {
                return await _context.Post.ToListAsync();
            }

            Response.StatusCode = 401;
            return Content("You are not authorized".ToErrorMessage());  
        }

        // GET: api/post/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Post>> GetPost(long id)
        {
            var post = await _context.Post.FindAsync(id);

            if (post == null)
            {
                Response.StatusCode = 404;
                return Content("Record not found".ToErrorMessage());
            }

            return post;
        }

        // PUT: api/Posts/5
        [Route("update/{id}")]
        [HttpPut]
        public async Task<IActionResult> PutPost(long id, Post post)
        {
            post.Publish_date = TimeStamp.GetCurrent;

            if (id != post.Id)
            {
                Response.StatusCode = 400;
                return Content("Missing request argument".ToErrorMessage());
            }

            _context.Entry(post).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PostExists(id))
                {
                    Response.StatusCode = 404;
                    return Content("Record not found".ToErrorMessage());
                }
                else
                {
                    Response.StatusCode = 500;
                    return Content("Error updating post record".ToErrorMessage());
                }
            }

            Response.StatusCode = 200;
            return Content("Record updated".ToErrorMessage());
        }

        // POST: api/post/add
        [Route("add")]
        [HttpPost]
        public async Task<ActionResult<Post>> PostPost(Post post)
        {
            if(post.Content.Equals("") || post.Title.Equals("") || post.Publish_date == 0)
            {
                Response.StatusCode = 400;
                return Content("Missing body argument".ToErrorMessage());
            }

            Post refinedInstance = post.Refine();

            _context.Post.Add(refinedInstance);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPost", new { id = refinedInstance.Id }, refinedInstance);
        }

        // DELETE: api/post/delete
        [Route("delete/{id}")]
        [HttpDelete]
        public async Task<ActionResult<Post>> DeletePost(long id)
        {
            var post = await _context.Post.FindAsync(id);
            if (post == null)
            {
                Response.StatusCode = 404;
                return Content("Post record not found".ToErrorMessage());
            }

            _context.Post.Remove(post);
            await _context.SaveChangesAsync();

            Response.StatusCode = 200;
            return Content("Post record deleted".ToErrorMessage()); ;
        }

        private bool PostExists(long id)
        {
            return _context.Post.Any(e => e.Id == id);
        }
    }
}
