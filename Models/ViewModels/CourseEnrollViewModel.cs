using System;
using System.Collections.Generic;
using OnlineCourse.Models;

namespace OnlineCourse.Models.ViewModels
{
    public class CourseEnrollViewModel
    {
        public Course Course { get; set; }
        public Lesson? FirstLesson { get; set; }
        public int CourseId { get; set; }
    }
}
