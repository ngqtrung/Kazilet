namespace PRN231_Kazilet_API.Models.Dto
{
    public class PlayerAnswerDto
    {
        public int QuestionId { get; set; } 

        public int? PlayerAnswer { get; set; }
        
        public int Turn { get; set; }

        public DateTime AnswerAt { get; set; }

        public DateTime CreatedAt { get; set; }

        public override string? ToString()
        {
            return QuestionId + " " + PlayerAnswer + " " + Turn + " " + AnswerAt.ToString() + " " + CreatedAt.ToString();
        }
    }
}
