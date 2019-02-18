using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplicationAPI.DBHandler;
using WebApplicationAPI.Extension;
using WebApplicationAPI.Model;
using WebApplicationAPI.Models;
using WebApplicationAPI.Services;

namespace WebApplicationAPI.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class RegisteredUsersController : ControllerBase
    {
        private readonly WebApplicationAPIContext _context;
        private readonly UserService User_Service = UserService.Get;

        public RegisteredUsersController(WebApplicationAPIContext context)
        {
            _context = context;
        }

        // GET: api/RegisteredUsers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RegisteredUser>>> GetRegisteredUsers()
        {
            return User_Service.GetRegisteredUsers;
        }

        // GET: api/RegisteredUsers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<RegisteredUser>> GetRegisteredUser(string id)
        {

            var result = User_Service.GetRegisteredUser(id);

            if(result != null)
            {
                return Ok(new
                {
                    id = result.Id,
                    username = result.UserName,
                    normalizedUser = result.NormalizedUser,
                    email = result.Email,
                    securityStamp = result.SecurityStamp,
                    concurrencyStamp = result.ConcurrencyStamp,
                    role = result.Role,
                    registration_date = result.RegistrationDate,
                    last_update = result.LastUpdated
                });
            }

            //var registeredUser = await _context.RegisteredUsers.FindAsync(id);

            //if (registeredUser == null)
            //{
            //    return NotFound();
            //}
            Response.StatusCode = 401;
            return Content("User not found".ToResponseMessage());
        }

        // PUT: api/RegisteredUsers/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRegisteredUser(long id, RegisteredUser registeredUser)
        {
            //if (id != registeredUser.Id)
            //{
            //    return BadRequest();
            //}

            _context.Entry(registeredUser).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RegisteredUserExists(id))
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

        // POST: api/RegisteredUsers
        [HttpPost]
        public async Task<ActionResult<RegisteredUser>> PostRegisteredUser(RegisteredUser registeredUser)
        {
            _context.RegisteredUsers.Add(registeredUser);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetRegisteredUser", new { id = registeredUser.Id }, registeredUser);
        }

        // DELETE: api/RegisteredUsers/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<RegisteredUser>> DeleteRegisteredUser(long id)
        {
            var registeredUser = await _context.RegisteredUsers.FindAsync(id);
            if (registeredUser == null)
            {
                return NotFound();
            }

            _context.RegisteredUsers.Remove(registeredUser);
            await _context.SaveChangesAsync();

            return registeredUser;
        }

        private bool RegisteredUserExists(long id)
        {
            return _context.RegisteredUsers.Any(e => e.Id.Equals(id));
        }
    }
}
