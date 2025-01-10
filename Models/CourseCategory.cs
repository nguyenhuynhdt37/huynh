using System;
using System.Collections.Generic;

namespace OnlineCourse.Models;

public partial class CourseCategory
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public string? Slug { get; set; }

    public bool? Status { get; set; }

}
