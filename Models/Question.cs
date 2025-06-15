using System;
using System.Collections.Generic;

namespace OnlineCourse.Models;

public partial class Question
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Content { get; set; }

    public string? Answer { get; set; }

    public string? Type { get; set; }

    public int? CourseId { get; set; }

    public bool? Status { get; set; }

    public virtual Course? Course { get; set; }
}
