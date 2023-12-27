using BlazorApp1.Shared.DBContexts;
using BlazorApp1.Shared.Enum;
using BlazorApp1.Shared.Models;
using MailKit.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MimeKit.Text;
using MimeKit;
using Newtonsoft.Json;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BlazorApp1.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserTaskController : ControllerBase
    {
        private readonly SQLDBContext _dbContext;
        private readonly IConfiguration configuration;
        public UserTaskController(SQLDBContext context, IConfiguration configuration)
        {
            _dbContext = context;
            this.configuration = configuration;
        }
        [Route("GetUsers/{userName}")]
        [HttpGet]
        public async Task<IList<Employee>> GetUsers(string userName)
        {
            try
            {
                string? uname = JsonConvert.DeserializeObject<string>(userName);
                List<Employee> emps = await _dbContext.Employees.Where(m => m.Role != 1)
                                    .ToListAsync();

                return emps;
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }
        [Route("GetExperts/{userName}")]
        [HttpGet]
        public async Task<IList<Employee>> GetExperts(string userName)
        {
            try
            {
                string? uname = JsonConvert.DeserializeObject<string>(userName);
                List<Employee> emps = await _dbContext.Employees.Where(m => m.Role != 1 && m.Role != 2)
                                    .ToListAsync();
                foreach (var item in emps)
                {
                    var res = await _dbContext.UserCategories.FindAsync(item.ExpertCategoryId);
                    if (res != null && item.ExpertCategoryId > 0)
                    {
                        item.ExpertFulllName = string.Format("{0} {1}", item.FullName + " - ", res.CategoryName);
                    }
                }

                return emps;
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }
        [Route("GetTasks/{userName}")]
        [HttpGet]
        public async Task<IList<UserTask>> GetTasks(string userName)
        {
            try
            {
                string? uname = JsonConvert.DeserializeObject<string>(userName);
                var uid = await _dbContext.Employees.Where(_ => _.EmailId == uname).FirstOrDefaultAsync();
                int userid = uid.Id;
                List<UserTask> tasks = await _dbContext.UserTask
                                    //.Include(m => m.UserTaskComment)
                                    .Include(m => m.User)
                                    .Where(m => m.UserId == userid)
                                    .ToListAsync();
                foreach (var item in tasks)
                {
                    //item.UserTaskComment = _dbContext.UserTaskComment.Where(m => m.TaskId == item.Id).ToList();
                    item.TaskStatusId = (int)item.TaskStatus;
                }

                return tasks;
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }
        [Route("AddTask")]
        [HttpPost]
        public async Task<IActionResult> AddTask(UserTaskCls task)
        {
            try
            {
                UserTask userTask = new UserTask();
                userTask.Tasks = task.Tasks;
                userTask.Description = task.Description;
                userTask.TaskStatus = Shared.Enum.DTaskStatus.Active;
                userTask.UserId = task.UserId;

                if (_dbContext.UserTask == null)
                {
                    return Problem("Entity set 'AppDbContext.UserTask'  is null.");
                }

                if (task != null)
                {
                    _dbContext.Add(userTask);
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
        [Route("UpdateTask")]
        [HttpPost]
        public async Task<IActionResult> UpdateTask(UserTaskCls userTaskCls)
        {
            bool sendEmailtoAdmin = false;
            bool sendEmailtoExpert = false;
            if (userTaskCls.IsExpertHelpRequired)
            {
                sendEmailtoExpert = true;
            }
            if (!userTaskCls.IsAgreeWithDueDate)
            {
                sendEmailtoAdmin = true;
            }
            UserTask userTask = new UserTask();
            userTask.Tasks = userTaskCls.Tasks;
            userTask.Description = userTaskCls.Description;
            userTask.Id = userTaskCls.Id;
            userTask.UserId = userTaskCls.UserId;
            userTask.TaskStatus = (DTaskStatus)userTaskCls.TaskStatusId;
            userTask.Remarks = userTaskCls.Remarks;
            userTask.IsAgreeWithDueDate = userTaskCls.IsAgreeWithDueDate;
            userTask.DueDate = userTaskCls.DueDate;
            userTask.Reason = userTaskCls.Reason;
            userTask.IsExpertHelpRequired = userTaskCls.IsExpertHelpRequired;
            userTask.ExpertId = userTaskCls.ExpertId;
             
            _dbContext.Entry(userTask).State = EntityState.Modified;
            if (userTaskCls.Comment != null)
            {
                UserTaskComment taskComment = new UserTaskComment();
                taskComment.TaskId = userTaskCls.Id;
                taskComment.Comment = userTaskCls.Comment;
                taskComment.UserId = userTaskCls.UserId;
                _dbContext.UserTaskComment.Add(taskComment);
            }
            try
            {
                await _dbContext.SaveChangesAsync();
                if (sendEmailtoAdmin)
                {
                    // send email to Admin
                    var usr = await _dbContext.Employees.Where(m => m.Role == 1).FirstOrDefaultAsync();
                    List<string> emails = new List<string>();
                    emails.Add(usr.EmailId);
                    string baseUrl = configuration.GetValue<string>("BaseUrl");
                    StringBuilder body = new StringBuilder();
                    body.AppendFormat("<h1>DueDate change Request</h1>");
                    body.AppendFormat("Dear {0},", usr.FullName);
                    body.AppendFormat("<br />");
                    body.AppendFormat("<p>You have been requested to change the Task Duedate, please click on the below link to login to portal to view the Task Details!</p>");
                    body.AppendFormat("TaskName - {0}", userTaskCls.Tasks);
                    body.AppendFormat("<br />");
                    body.AppendFormat("Reason mentioned - {0}", userTaskCls.Reason);
                    body.AppendFormat("<br />");
                    body.AppendFormat("<br />");
                    body.AppendFormat("<a href=" + baseUrl + "> Click here to Login</a>");
                    body.AppendFormat("<br />");
                    body.AppendFormat("<br />");
                    body.AppendFormat("Warm Regards,");
                    body.AppendFormat("<br />");
                    // send email
                    await SendEmail("DueDate change Request", body, emails, null, null);
                }
                if (sendEmailtoExpert)
                {
                    // send email to expert
                    var usr = await _dbContext.Employees.Where(m => m.Id == userTaskCls.ExpertId).FirstOrDefaultAsync();
                    var tsk = await _dbContext.Employees.Where(m => m.Id == userTaskCls.UserId).FirstOrDefaultAsync();
                    List<string> emails = new List<string>();
                    emails.Add(usr.EmailId);
                    string baseUrl = configuration.GetValue<string>("BaseUrl");
                    StringBuilder body = new StringBuilder();
                    body.AppendFormat("<h1>Assistance Required on Task</h1>");
                    body.AppendFormat("Dear {0},", usr.FullName);
                    body.AppendFormat("<br />");
                    body.AppendFormat("<p>You have been requested by the user '" + tsk.EmailId + "' to help him on the Task Assigned by the Admin, please click on the below link to login to portal to view the Task Details!</p>");
                    body.AppendFormat("TaskName - {0}", userTaskCls.Tasks);
                    body.AppendFormat("<br />");
                    body.AppendFormat("<br />");
                    body.AppendFormat("<a href=" + baseUrl + "> Click here to Login</a>");
                    body.AppendFormat("<br />");
                    body.AppendFormat("<br />");
                    body.AppendFormat("Warm Regards,");
                    body.AppendFormat("<br />");
                    //body.AppendFormat("Admin");
                    // send email
                    await SendEmail("Assistance Required on Task", body, emails, null, null);
                }
                return Ok("Update Successfully!!");
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }

            return NoContent();
        }
        [HttpDelete("DeleteTask/{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            if (_dbContext.UserTask == null)
            {
                return NotFound();
            }
            var userTask = await _dbContext.UserTask.FindAsync(id);
            if (userTask == null)
            {
                return NotFound();
            }

            _dbContext.UserTask.Remove(userTask);
            await _dbContext.SaveChangesAsync();

            return Ok("Delete Successfully!!");
        }
        public async Task<bool> checkFun()
        {
            return true;
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
