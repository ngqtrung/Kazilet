namespace PRN231_Kazilet_API.Models.Dto
{
    public class ReportDetailQuestionDto
    {
        public QuestionDto QuestionDto { get;set; }

        public double CorrectPercent { get;set; }

        public double Duration { get; set; }

        public List<DetailPlayerAnswer> Details { get; set; }
    }

    public class DetailPlayerAnswer
    {
        public string Avatar { get; set; }

        public string Username { get; set; }

        public string PlayerAnswer { get; set; }

        public bool IsCorrect { get; set; } 

        public double Duration { get;set; }

        public int Score { get; set; }

        public DetailPlayerAnswer(string avatar, string username, string playerAnswer, bool isCorrect, double duration, int score)
        {
            Avatar = avatar;
            Username = username;
            PlayerAnswer = playerAnswer;
            IsCorrect = isCorrect;
            Duration = duration;
            Score = score;
        }
    }
}
