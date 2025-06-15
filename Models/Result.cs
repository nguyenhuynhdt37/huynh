using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace OnlineCourse.Models;

public partial class Result
{
    [Key]
    public int ResultId { get; set; }

    public int UserId { get; set; }

    public int ExamId { get; set; }

    public string? ResultQuiz { get; set; }

    public string? ResultEssay { get; set; }

    public string? StartDateQuiz { get; set; }

    public string? StartTimeQuiz { get; set; }

    public string? FinishTimeQuiz { get; set; }

    public DateTime? StartDateEssay { get; set; }

    public string? StartTimeEssay { get; set; }

    public string? FinishTimeEssay { get; set; }

    public bool? Status { get; set; }

    public string? Score { get; set; }

    public virtual Exam Exam { get; set; } = null!; 

    public virtual AppUserModel User { get; set; } = null!;
}
