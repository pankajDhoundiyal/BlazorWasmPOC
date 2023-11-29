using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorApp1.Shared.Models
{
    public class TaskComment
    {
        public int Id { get; set; }
        public string? Comment { get; set; }
        [ForeignKey("Task")]
        public int TaskId { get; set; }
        //[ForeignKey("User")]
        //public int UserId { get; set; }
        //public DTask Task { get; set; }

        //public User User { get; set; }
    }
}
