using BlazorApp1.Shared.DBContexts;
using BlazorApp1.Shared.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace BlazorApp1.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly SQLDBContext _dbContext;

        public EmployeeController(SQLDBContext context)
        {
            _dbContext = context;
        }
        [Route("GetEmployees/{userName}")]
        [HttpGet]
        public async Task<IList<Employee>> GetEmployees(string userName)
        {
            try
            {
                //var _data = await _dbContext.Employees.ToListAsync();
                string? uname = JsonConvert.DeserializeObject<string>(userName);
                var uid = await _dbContext.Employees.Where(_ => _.EmailId == uname).FirstOrDefaultAsync();
                int userid = uid.Id;
                var _data = await (from emp in _dbContext.Employees
                                   join pic in this._dbContext.EmployeeProfilePics on emp.Id equals pic.EmployeeId into p
                                   from pic in p.DefaultIfEmpty()
                                   where emp.Id != userid
                                   select new Employee
                                   {
                                       Id = emp.Id,
                                       Code = emp.Code,
                                       FullName = emp.FullName,
                                       DOB = emp.DOB,
                                       Address = emp.Address,
                                       City = emp.City,
                                       PostalCode = emp.PostalCode,
                                       State = emp.State,
                                       Country = emp.Country,
                                       EmailId = emp.EmailId,
                                       PhoneNo = emp.PhoneNo,
                                       JoiningDate = emp.JoiningDate,
                                       ImageType = pic.ImageType,
                                       thumbnail = pic.Thumbnail,
                                       EmployeeProfilePicId = pic.Id
                                   }).ToListAsync();
                return _data;
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }
        [Route("GetEmployee/{id}")]
        [HttpGet]
        public async Task<Employee> GetEmployee(int id)
        {
            try
            {
                //var _data = await _dbContext.Employees.FindAsync(id);
                var _data = await (from emp in _dbContext.Employees
                                   join pic in this._dbContext.EmployeeProfilePics on emp.Id equals pic.EmployeeId into p
                                   from pic in p.DefaultIfEmpty()
                                   where (emp.Id.Equals(id))
                                   select new Employee
                                   {
                                       Id = emp.Id,
                                       Code = emp.Code,
                                       FullName = emp.FullName,
                                       DOB = emp.DOB,
                                       Address = emp.Address,
                                       City = emp.City,
                                       PostalCode = emp.PostalCode,
                                       State = emp.State,
                                       Country = emp.Country,
                                       EmailId = emp.EmailId,
                                       PhoneNo = emp.PhoneNo,
                                       JoiningDate = emp.JoiningDate,
                                       ImageType = pic.ImageType,
                                       thumbnail = pic.Thumbnail,
                                       EmployeeProfilePicId = pic.Id
                                   }).FirstOrDefaultAsync();
                return _data;
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }
        [Route("SaveEmployee")]
        [HttpPost]
        public async Task<IActionResult> SaveEmployee(Employee employee)
        {
            try
            {
                if (_dbContext.Employees == null)
                {
                    return Problem("Entity set 'AppDbContext.Employee'  is null.");
                }

                if (employee != null)
                {
                    employee.Password = "123456";
                    _dbContext.Add(employee);
                    await _dbContext.SaveChangesAsync();

                    if (!string.IsNullOrEmpty(employee.thumbnail) && employee.Id > 0)
                    {
                        EmployeeProfilePic employeeProfilePic = new EmployeeProfilePic();
                        employeeProfilePic.ImageType = employee.ImageType;
                        employeeProfilePic.Thumbnail = employee.thumbnail;
                        employeeProfilePic.EmployeeId = employee.Id;
                        employeeProfilePic.ImageUrl = "localhost";

                        _dbContext.Add(employeeProfilePic);
                        await _dbContext.SaveChangesAsync();
                    }

                    return Ok("Save Successfully!!");
                }
            }
            catch (Exception ex)
            {
                throw (ex);
            }

            return NoContent();
        }
        [Route("UpdateEmployee")]
        [HttpPost]
        public async Task<IActionResult> UpdateEmployee(Employee employee)
        {
            _dbContext.Entry(employee).State = EntityState.Modified;

            try
            {
                if (!string.IsNullOrEmpty(employee.thumbnail))
                {
                    EmployeeProfilePic employeeProfilePic = await _dbContext.EmployeeProfilePics.Where(m => m.EmployeeId == employee.Id).FirstOrDefaultAsync();
                    employeeProfilePic.ImageType = employee.ImageType;
                    employeeProfilePic.Thumbnail = employee.thumbnail;
                    //employeeProfilePic.EmployeeId = employee.Id;
                    //employeeProfilePic.ImageUrl = "localhost";

                    if (employee.EmployeeProfilePicId > 0)
                        _dbContext.Entry(employeeProfilePic).State = EntityState.Modified;
                    else
                        _dbContext.Add(employeeProfilePic);
                }
                await _dbContext.SaveChangesAsync();
                return Ok("Update Successfully!!");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmployeeExists(employee.Id))
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
        [HttpDelete("DeleteEmployee/{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            if (_dbContext.Employees == null)
            {
                return NotFound();
            }
            var product = await _dbContext.Employees.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            _dbContext.Employees.Remove(product);
            await _dbContext.SaveChangesAsync();

            return NoContent();
        }
        private bool EmployeeExists(int id)
        {
            return (_dbContext.Employees?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
