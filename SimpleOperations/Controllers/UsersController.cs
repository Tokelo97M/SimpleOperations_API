using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleOperations.Models;

namespace SimpleOperations.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UsersContext _dbContext;

        public UsersController(UsersContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Users>>> GetUsers()
        {
            if(_dbContext.Users == null)
            {
                return NotFound();
            }
            return await _dbContext.Users.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Users>> GetUsers(int id)
        {
            if (_dbContext.Users == null)
            {
                return NotFound();
            }
            var users = await _dbContext.Users.FindAsync(id);
            if(users == null)
            {
                return NotFound();
            }

            return users;
        }

        [HttpPost]
        public async Task<ActionResult<Users>> PostUsers(Users users)
        {
            _dbContext.Users.Add(users);
            await _dbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUsers), new {id = users.Id}, users);
        }


        [HttpPut]
        public async Task<IActionResult> PutUsers(int id, Users users)
        {
            if(id != users.Id)
            {
                return BadRequest();
            }

            _dbContext.Entry(users).State = EntityState.Modified;

            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch(DbUpdateConcurrencyException)
            {
                if (!UserAvailable(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok();
        }

        private bool UserAvailable(int id)
        {
            return (_dbContext.Users?.Any(x => x.Id == id)).GetValueOrDefault();
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUsers(int id)
        {
            if(_dbContext.Users == null)
            {
                return NotFound();
            }

            var user = await _dbContext.Users.FindAsync(id);
            if(user == null)
            {
                return NotFound();
            }

            _dbContext.Users.Remove(user);
            await _dbContext.SaveChangesAsync();
            return Ok();
        }
    }
}
