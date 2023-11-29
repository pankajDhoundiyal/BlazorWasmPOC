using BlazorApp1.Client.Models.Account;
using BlazorApp1.Shared.DBContexts;
using BlazorApp1.Shared.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlazorApp1.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly SQLDBContext _dbContext;

        public AccountController(SQLDBContext context)
        {
            _dbContext = context;
        }
        [Route("login")]
        [HttpPost]
        public async Task<IActionResult> LoginAsync(Login user)
        {
            try
            {
                if (string.IsNullOrEmpty(user.Username) || string.IsNullOrEmpty(user.Password))
                {
                    return null;
                }
                var data = await _dbContext.Employees.FirstOrDefaultAsync(_ => _.EmailId == user.Username && _.Password == user.Password);
                if (data != null) { return Ok(data); }
                else return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        
        [Route("register")]
        [HttpPost]
        public async Task<IActionResult> Register(Employee employee)
        {
            try
            {
                if (_dbContext.Employees == null)
                {
                    return Problem("Entity set 'AppDbContext.Employee'  is null.");
                }

                if (employee != null)
                {
                    _dbContext.Add(employee);
                    await _dbContext.SaveChangesAsync();

                    return Ok("Save Successfully!!");
                }
            }
            catch (Exception ex)
            {
                throw (ex);
            }

            return NoContent();
        }
    }
}
  