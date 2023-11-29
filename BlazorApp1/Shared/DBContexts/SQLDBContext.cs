using BlazorApp1.Shared.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorApp1.Shared.DBContexts
{
    public partial class SQLDBContext : DbContext
    {
        public SQLDBContext()
        {
        }
        public SQLDBContext(DbContextOptions<SQLDBContext> options)
        : base(options)
        {
        }
        public virtual DbSet<Employee> Employees { get; set; }
        public virtual DbSet<EmployeeProfilePic> EmployeeProfilePics { get; set; }
        public virtual DbSet<UserTask> UserTask { get; set; }
        public virtual DbSet<UserTaskComment> UserTaskComment { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=DESKTOP-NBE5CHF\\SQLEXPRESS;Database=BlazorServerDB;integrated security=True; Trusted_Connection=True; MultipleActiveResultSets=true;");
            }
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

        }
    }
}
