using BlazorApp1.Shared.DBContexts;
using BlazorApp1.Shared.Models;
using MailKit.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MimeKit.Text;
using MimeKit;
using Newtonsoft.Json;
using System.Text;
using BlazorApp1.Shared.Enum;

namespace BlazorApp1.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly SQLDBContext _dbContext;
        private readonly IConfiguration configuration;
        public EmployeeController(SQLDBContext context, IConfiguration configuration)
        {
            _dbContext = context;
            this.configuration = configuration;
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
                                       EmployeeProfilePicId = pic.Id,
                                       Role = emp.Role,
                                       ExpertCategoryId = emp.ExpertCategoryId
                                   }).OrderByDescending(m=>m.Id).ToListAsync();
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
                                       Password = emp.Password,
                                       PhoneNo = emp.PhoneNo,
                                       JoiningDate = emp.JoiningDate,
                                       ImageType = pic.ImageType,
                                       thumbnail = pic.Thumbnail,
                                       EmployeeProfilePicId = pic.Id,
                                       Role = emp.Role,
                                       ExpertCategoryId = emp.ExpertCategoryId
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
                    //employee.Role = employee.Role;
                    //if ((Role)employee.Role == Role.User)
                    //{
                    //    employee.ExpertCategoryId = 0;
                    //}
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
                    List<string> emails = new List<string>();
                    emails.Add(employee.EmailId);
                    string baseUrl = configuration.GetValue<string>("BaseUrl");
                    StringBuilder body = new StringBuilder();
                    body.AppendFormat("<h1>User Registration</h1>");
                    body.AppendFormat("Dear {0},", employee.FullName);
                    body.AppendFormat("<br />");
                    body.AppendFormat("<p>Thank you for registering with us!</p>");
                    body.AppendFormat("<p>Please find the details below to login </p>");
                    body.AppendFormat("UserName - {0}", employee.EmailId);
                    body.AppendFormat("<br />");
                    body.AppendFormat("Password - {0}", employee.Password);
                    body.AppendFormat("<br />");
                    body.AppendFormat("<br />");
                    body.AppendFormat("<a href=" + baseUrl + "> Click here to Login</a>");
                    body.AppendFormat("<br />");
                    body.AppendFormat("<br />");
                    body.AppendFormat("Warm Regards,");
                    body.AppendFormat("<br />");
                    body.AppendFormat("Admin");
                    // send email
                    await SendEmail("User Registration", body, emails, null, null);

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
            //employee.Role = employee.Role;
            //if ((Role)employee.Role == Role.User)
            //{
            //    employee.ExpertCategoryId = 0;
            //}
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
        }
        [HttpDelete("DeleteEmployee/{id}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            if (_dbContext.Employees == null)
            {
                return NotFound();
            }
            var emp = await _dbContext.Employees.FindAsync(id);

            var empProfile = await _dbContext.EmployeeProfilePics.Where(m=>m.EmployeeId == emp.Id).FirstOrDefaultAsync();
            var utask = await _dbContext.UserTask.Where(m => m.UserId == emp.Id).FirstOrDefaultAsync();
            if (emp == null)
            {
                return NotFound();
            }
            if(empProfile!=null)
                _dbContext.EmployeeProfilePics.Remove(empProfile);
            if (utask != null)
                _dbContext.UserTask.Remove(utask);
            _dbContext.Employees.Remove(emp);
            try
            {
                await _dbContext.SaveChangesAsync();
                return Ok("Delete Successfully!!");
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }
        private bool EmployeeExists(int id)
        {
            return (_dbContext.Employees?.Any(e => e.Id == id)).GetValueOrDefault();
        }
        [NonAction]
        public async Task<bool> SendEmail(string subject, StringBuilder Body, List<string> To, List<string> Cc, List<string> Bcc)
        {
            try
            {
                var email = new MimeMessage();
                email.From.Add(MailboxAddress.Parse(configuration.GetValue<string>("EmailSettings:From")));
                foreach (var toAddress in To)
                {
                    email.To.Add(MailboxAddress.Parse(toAddress));
                }
                // Add CC and BCC recipients if provided
                if (Cc != null)
                {
                    foreach (var ccAddress in Cc)
                    {
                        email.Cc.Add(MailboxAddress.Parse(ccAddress));
                    }
                }
                if (Bcc != null)
                {
                    foreach (var bccAddress in Bcc)
                    {
                        email.Bcc.Add(MailboxAddress.Parse(bccAddress));
                    }
                }
                email.Subject = subject;
                email.Body = new TextPart(TextFormat.Html) { Text = Body.ToString() };
                // send email
                using (var client = new MailKit.Net.Smtp.SmtpClient())
                {
                    try
                    {
                        client.ServerCertificateValidationCallback = (s, c, h, e) => true;
                        client.CheckCertificateRevocation = false;
                        client.Connect(configuration.GetValue<string>("EmailSettings:Host"), configuration.GetValue<int>("EmailSettings:Port"), SecureSocketOptions.Auto);
                        client.AuthenticationMechanisms.Remove("XOAUTH2");
                        await client.AuthenticateAsync(configuration.GetValue<string>("EmailSettings:From"), configuration.GetValue<string>("EmailSettings:Password"));
                        await client.SendAsync((MimeKit.MimeMessage)email);
                    }
                    catch (Exception ex)
                    {
                        throw;
                    }
                    finally
                    {
                        await client.DisconnectAsync(true);
                        client.Dispose();
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
