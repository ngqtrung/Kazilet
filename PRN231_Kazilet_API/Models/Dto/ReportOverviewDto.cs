namespace PRN231_Kazilet_API.Models.Dto
{
    public class ReportOverviewDto
    {
        public int Rank { get; set; }

        public string Avatar { get; set; }  
        public string Username { get; set; }
        public double Duration { get; set; }

        public int Score { get; set; }

        public ReportOverviewDto(int rank, string avatar, string username, double duration, int score)
        {
            Rank = rank;
            Avatar = avatar;
            Username = username;
            Duration = duration;
            Score = score;
        }
    }
}
