using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using OnlineCourse.Models;

namespace OnlineCourse.Models
{
    public class Enrollments
    {
        [Key]
        public int id { get; set; }
        public string? userId { get; set; }
        public int CourseId { get; set; }
        public DateTime EnrollmentDate { get; set; }

        [ForeignKey("userId")]
        public virtual AppUserModel? User { get; set; }
        [ForeignKey("CourseId")]
        public virtual Course? Course { get; set; }
    }
}