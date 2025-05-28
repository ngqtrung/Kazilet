namespace PRN231_Kazilet_WebApp.Models.Dto
{
    public class IncorrectAnswerDto
    {
        public int QuestionId { get; set; }
        public string QuestionText { get; set; }
        public List<int> UserAnswers { get; set; }
        public List<int> CorrectAnswers { get; set; }
        public ICollection<AnswerDto> AnswerDtos { get; set; }
    }
}
