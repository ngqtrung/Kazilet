namespace PRN231_Kazilet_API.Models.Dto
{
    public class GameplayAddI
    {
        public string Username { get; set; }
        public int CurrentQuestion { get; set; }

        public int TotalQuestion { get; set; }

        public int AnswerStreak { get; set; }

        public int TimeLimit { get; set; }

        public int Point { get; set; }

        public DateTime CreatedAt { get; set; }

        public GameplayAddI()
        {
        }

        public GameplayAddI(string username, int currentQuestion, int totalQuestion, int answerStreak, int timeLimit, int point, DateTime createdAt)
        {
            Username = username;
            CurrentQuestion = currentQuestion;
            TotalQuestion = totalQuestion;
            AnswerStreak = answerStreak;
            TimeLimit = timeLimit;
            Point = point;
            CreatedAt = createdAt;
        }
    }
}
