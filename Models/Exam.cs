using System;
using System.Collections.Generic;

namespace OnlineCourse.Models;

public partial class Exam
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? MetaTitle { get; set; }

    public string? Code { get; set; }

    public string? QuestionsList { get; set; }

    public string? AnswersList { get; set; }

    public int? CourseId { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public short? TotalScore { get; set; }

    public short? Time { get; set; }

    public short? TotalQuestions { get; set; }

    public string? Type { get; set; }

    public bool? Status { get; set; }

    public string? QuestionsEssay { get; set; }

    public string? UsersList { get; set; }

    public string? ScoreList { get; set; }

    public virtual Course? Course { get; set; }

    public virtual ICollection<Result> Results { get; set; } = new List<Result>();
}
