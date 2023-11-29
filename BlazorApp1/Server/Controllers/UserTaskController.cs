using BlazorApp1.Shared.DBContexts;
using BlazorApp1.Shared.Enum;
using BlazorApp1.Shared.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace BlazorApp1.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserTaskController : ControllerBase
    {
        private readonly SQLDBContext _dbContext;

        public UserTaskController(SQLDBContext context)
        {
            _dbContext = context;
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
            UserTask userTask = new UserTask();
            userTask.Tasks = userTaskCls.Tasks;
            userTask.Description = userTaskCls.Description;
            userTask.Id = userTaskCls.Id;
            userTask.UserId = userTaskCls.UserId;
            userTask.TaskStatus = (DTaskStatus)userTaskCls.TaskStatusId;
            userTask.Remarks = userTaskCls.Remarks;
             
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
    }
}
