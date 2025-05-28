using PRN231_Kazilet_API.Models.Dto;

namespace PRN231_Kazilet_API.Services
{
    public interface ICourseService
    {
        public bool AddCourse(CourseDto courseDto);
        public CourseDto GetCourse(int courseId);
        public bool UpdateCourse(CourseDto courseDto);
        public bool DeleteCourse(int courseId);
        public List<CourseDto> GetOwnedCourses(int userId);

        public List<CourseDto> SearchCourses(string search);
        public List<CourseDto> GetCourseByUser(int created_by);
        public bool VerifyPassword(int courseId, string password);

        bool IsCoursePublic(int courseId);
    }
}
