namespace PRN231_Kazilet_WebApp.Models.Dto
{
    public class AnswerDto
    {
        public int Id { get; set; }
        public int? QuestionId { get; set; }
        public string? Content { get; set; }
        public bool? IsCorrect { get; set; }
    }
}
