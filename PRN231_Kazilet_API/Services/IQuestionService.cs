using PRN231_Kazilet_API.Models.Dto;

namespace PRN231_Kazilet_API.Services
{
    public interface IQuestionService
    {
        public List<QuestionDto> GetAllQuestionsByCourse(int courseId);

        public int GetNumberOfQuestionsInCourse(int courseId);

        public QuestionDto GetById(int questionNumber, int courseId);

        public QuestionDto GetById(int questionId);

        public List<QuestionDto> GetRandom(int courseId, int numOfQues);
        public List<QuestionDto> GetQuestionsByIds(List<int> ids);
    }
}
