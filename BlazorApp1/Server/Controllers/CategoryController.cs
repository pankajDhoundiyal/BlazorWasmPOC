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
    public class CategoryController : ControllerBase
    {
        private readonly SQLDBContext _dbContext;
        public CategoryController(SQLDBContext context)
        {
            _dbContext = context;
        }
        [Route("GetCategories")]
        [HttpGet]
        public async Task<IList<Categories>> GetCategories()
        {
            try
            {
                List<Categories> categories = await _dbContext.UserCategories.OrderByDescending(m => m.Id).ToListAsync();
                return categories;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        [Route("AddUpdate")]
        [HttpPost]
        public async Task<bool> AddUpdate(Categories categories)
        {
            try
            {
                if (categories.Id > 0)
                {
                    _dbContext.UserCategories.Update(categories);
                }
                else
                {
                    _dbContext.UserCategories.Add(categories);

                }

                _dbContext.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        [Route("DeleteCategory/{id}")]
        [HttpDelete]
        public async Task<bool> DeleteCategory(int id)
        {
            try
            {
                var category = await _dbContext.UserCategories.FindAsync(id);
                if (category == null)
                    return false;
                _dbContext.UserCategories.Remove(category);
                _dbContext.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
