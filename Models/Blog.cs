using System.ComponentModel.DataAnnotations;

namespace OnlineCourse.Models
{
    public class Blog
    {
        [Key]
        public int BlogId { get; set; }
        public string? Title { get; set; }
        public string? Content { get; set; }
        public string? Image { get; set; }
        public string? Author { get; set; }
        public bool Status { get; set; }
        public int BlogCategoryId { get; set; }
        public BlogCategory? BlogCategory { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}