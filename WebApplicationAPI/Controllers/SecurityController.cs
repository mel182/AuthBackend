using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApplicationAPI.Models;

namespace WebApplicationAPI.Controllers
{
    [Route("api/security")]
    [ApiController]
    public class SecurityController : ControllerBase
    {
        private WebApplicationAPIContext WebApplicationAPIContext;

        public SecurityController(WebApplicationAPIContext webApplicationAPIContext)
        {
            this.WebApplicationAPIContext = webApplicationAPIContext;
        }

        // GET: api/Security
        [Route("all/users")]
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return WebApplicationAPIContext.Users.Select(u => u.SecurityStamp).ToArray();
            //return new string[] { "value1", "value2" };
        }

        // GET: api/Security/5
        [HttpGet("{id}", Name = "Get")]
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Security
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT: api/Security/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
