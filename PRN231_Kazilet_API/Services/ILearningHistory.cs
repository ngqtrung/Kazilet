using PRN231_Kazilet_API.Models.Dto;

namespace PRN231_Kazilet_API.Services
{
    public interface ILearningHistory
    {
        public List<LearningHistoryDto> GetAllLearningHistoriesByUserId(int userId);
        public bool AddLearningHistory(int userId,int courseId);
        public List<CourseDto> GetTop5Course();
    }
}
