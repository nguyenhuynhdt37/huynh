using System.Collections;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OnlineCourse.Models;

namespace OnlineCourse.Areas.Admin.Repository
{
    public interface ICourseRepository
    {
        Task<IEnumerable<Course>> GetAllAsync();
        IEnumerable GetCategories();
        IEnumerable<CourseCategory> GetCategoriesNew();
        Task CreateAsync(Course course);
        Task<Course> GetByIdAsync(int id);
        Task EditAsync(Course course);
        Task DeleteAsync(int id);
    }
    public class CourseRepository : ICourseRepository
    {
        private readonly DataContext _context;
        public CourseRepository(DataContext context)
        {
            _context = context;
        }

        // Display all courses
        public async Task<IEnumerable<Course>> GetAllAsync()
        {
            var courses = _context.Courses.Include(c => c.Lessons).Include(c => c.Chapters).ToList();
            
            return await _context.Courses.OrderBy(c => c.Id).ToListAsync();
        }

        // Get all categories
        public IEnumerable GetCategories()
        {
            return _context.CourseCategories.Select(c => new
            {
                Id = c.Id,
                Name = c.Name
            }).ToList();
        }
        public IEnumerable<CourseCategory> GetCategoriesNew()
        {
            return _context.CourseCategories.ToList(); // Trả về danh sách các CourseCategory
        }
        // Create a new course
        public async Task CreateAsync(Course course)
        {
            try
            {
                _context.Courses.Add(course);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        // Get course by id
        public async Task<Course> GetByIdAsync(int id)
        {
            return await _context.Courses.FindAsync(id);
        }

        // Edit course
        public async Task EditAsync(Course course)
        {
            try
            {
                _context.Courses.Update(course);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        // Delete course
        public async Task DeleteAsync(int id)
        {
            try
            {
                var course = await _context.Courses.FindAsync(id);
                _context.Courses.Remove(course);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

    }
}