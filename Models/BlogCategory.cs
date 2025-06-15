using System.ComponentModel.DataAnnotations;

namespace OnlineCourse.Models
{
    public class BlogCategory
    {
        [Key]
        public int BlogCategoryId { get; set; }
        public string? Name { get; set; }
        public string? Slug { get; set; }
        public bool Status { get; set; }
        public ICollection<Blog>? Blogs { get; set; }

    }
}