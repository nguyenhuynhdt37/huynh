using System.ComponentModel.DataAnnotations;

namespace OnlineCourse.Models
{
    public partial class Document
    {
        [Key]
        public int Id { get; set; }
        
        public string? Name { get; set; }

        public string? Description { get; set; }

        public string? Type { get; set; }

        public string? FilePath { get; set; }

        public string? Link { get; set; }

        public bool? Status { get; set; }

        public int? CourseId { get; set; }

        public virtual Course? Course { get; set; }

        public int? CategoryId { get; set; }

        public virtual CourseCategory? Category { get; set; }

        public int? ViewCount { get; set; } = 0;

        public int? DownloadCount { get; set; } = 0;
    }
}