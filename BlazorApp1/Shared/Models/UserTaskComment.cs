using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorApp1.Shared.Models
{
    public class UserTaskComment
    {
        public int Id { get; set; }
        public string? Comment { get; set; }
        [ForeignKey("UserTask")]
        public int TaskId { get; set; }
        [ForeignKey("Employees")]
        public int UserId { get; set; }
        public UserTask UserTask { get; set; }

        public Employee User { get; set; }
    }
}
