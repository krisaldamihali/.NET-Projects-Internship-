using CreditSimulatorAPI.Data;
using CreditSimulatorAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CreditSimulatorAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly CreditDbContext _context;

        public UsersController(CreditDbContext context)
        {
            _context = context;
        }

        // POST: api/users
        [HttpPost]
        public IActionResult CreateUser([FromBody] UserCreateDto userDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = new User
            {
                FullName = userDto.FullName,
                Email = userDto.Email
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            return Ok(user); 
        }

        // GET: api/users
        [HttpGet]
        public IActionResult GetUsers()
        {
            var users = _context.Users.ToList(); 
            return Ok(users);
        }

        // GET: api/users/{id}
        [HttpGet("{id}")]
        public IActionResult GetUserById(int id)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == id);

            if (user == null)
                return NotFound();

            return Ok(user); 
        }

        // GET: api/users/withloans
        [HttpGet("withloans")]
        public IActionResult GetUsersWithLoans()
        {
            var users = _context.Users
                .Include(u => u.Loans) 
                .Select(u => new
                {
                    u.Id,
                    u.FullName,
                    u.Email,
                    Loans = u.Loans.Select(l => new
                    {
                        l.Id,
                        l.Amount,
                        l.InterestRate,
                        l.RemainingBalance
                    }).ToList()
                })
                .ToList();

            return Ok(users);
        }
    }
}
