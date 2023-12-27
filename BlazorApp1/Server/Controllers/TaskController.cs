using BlazorApp1.Shared.DBContexts;
using BlazorApp1.Shared.Enum;
using BlazorApp1.Shared.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Web;

namespace BlazorApp1.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        private readonly SQLDBContext _dbContext;

        public TaskController(SQLDBContext context)
        {
            _dbContext = context;
        }
        [Route("GetUsers")]
        [HttpGet]
        public async Task<IList<Employee>> GetUsers()
        {
            try
            {
                List<Employee> emps = await _dbContext.Employees.Where(m=>m.Role!=1)
                                    .ToListAsync();

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
                                    .Include(m => m.User)
                                    .Where(m=>m.UserId != userid)
                                    .ToListAsync();
                foreach (var item in tasks)
                {
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
                userTask.TaskStatus = (DTaskStatus)task.TaskStatusId;
                userTask.UserId = task.UserId;
                userTask.DueDate = task.DueDate;

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
            UserTask userTask = new UserTask();
            userTask.Tasks = userTaskCls.Tasks;
            userTask.Description = userTaskCls.Description;
            userTask.TaskStatus = (DTaskStatus)userTaskCls.TaskStatusId;
            userTask.Id = userTaskCls.Id;
            userTask.UserId = userTaskCls.UserId;
            userTask.DueDate = userTaskCls.DueDate;
            _dbContext.Entry(userTask).State = EntityState.Modified;

            try
            {
                await _dbContext.SaveChangesAsync();
                return Ok("Update Successfully!!");
            }
            catch (DbUpdateConcurrencyException)
            {
                return BadRequest();
            }

            return NoContent();
        }
        [HttpDelete("DeleteTask/{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            try
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
            catch(Exception ex)
            {
                return BadRequest();
            }            
        }
    }
}
