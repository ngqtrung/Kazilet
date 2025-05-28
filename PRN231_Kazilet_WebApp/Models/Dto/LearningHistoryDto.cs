namespace PRN231_Kazilet_WebApp.Models.Dto
{
    public class LearningHistoryDto
    {
        public int UserId { get; set; }
        public int CourseId { get; set; }
        public DateTime? LearningDate { get; set; }
        public string CourseName { get; set; }
        public string Creator { get; set; }
        public int numOfQues { get; set; }
    }
}
