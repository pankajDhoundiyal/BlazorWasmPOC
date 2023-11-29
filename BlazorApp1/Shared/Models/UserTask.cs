﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlazorApp1.Shared.Enum;

namespace BlazorApp1.Shared.Models
{
    public class UserTask
    {
        public UserTask()
        {
            //UserTaskComment = new List<UserTaskComment>();
        }
        public int Id { get; set; }
        [Required]
        public string Tasks { get; set; }
        public string? Description { get; set; }
        public DTaskStatus TaskStatus { get; set; }
        [ForeignKey("Employees")]
        public int UserId { get; set; }
        public string? Remarks { get; set; }
        [NotMapped]
        public int TaskStatusId { get; set; }
        [NotMapped]
        public string? Comment { get; set; }
        public Employee User { get; set; }
        
        public ICollection<UserTaskComment> UserTaskComment { get; set; }
    }
    public class UserTaskCls 
    {
        public int Id { get; set; }
        public string Tasks { get; set; }
        public string? Description { get; set; }
        public int UserId { get; set; }
        public int TaskStatusId { get; set; }
        public string? Comment { get; set; }
        public string? Remarks { get; set; }
    }
}
