using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorApp1.Shared.Models
{
    public class Employee
    {
        public int Id { get; set; }

        [Required]
        public string Code { get; set; }

        [Required]
        public string FullName { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime DOB { get; set; } = DateTime.Today.AddYears(-25);

        public string? Address { get; set; }

        [Required]
        public string City { get; set; }

        [Required]
        public string State { get; set; }

        [Required]
        public string Country { get; set; }

        [Required]
        public string PostalCode { get; set; }

        [Required]
        public string EmailId { get; set; }
        
        public string? Password { get; set; }

        [Required]
        public string PhoneNo { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime JoiningDate { get; set; } = DateTime.Now;
        [NotMapped]
        public int? EmployeeProfilePicId { get; set; }
        [NotMapped]
        public string? thumbnail { get; set; }

        [NotMapped]
        public string? ImageType { get; set; }
        public int Role { get; set; } = 2;
    }
    public class StatusItem
    {
        public string Text { get; set; }
        public int Value { get; set; }
    }
}
