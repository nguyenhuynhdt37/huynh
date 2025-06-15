using System.ComponentModel.DataAnnotations;

namespace OnlineCourse.Models
{
    public partial class Chapter
    {
        [Key]
        public int ChapterId { get; set; }

        public string? Name { get; set; }

        public int? Order { get; set; }

        public int? CourseId { get; set; }

        public virtual Course? Course { get; set; }

        public bool Status { get; set; }

        public ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();
    }
}