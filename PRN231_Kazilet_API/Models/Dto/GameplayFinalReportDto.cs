namespace PRN231_Kazilet_API.Models.Dto
{
    public class GameplayFinalReportDto
    {
        public string Avatar { get; set; }
        public string Code { get; set; }

        public string Username { get; set; }

        public int CorrectAnswer { get; set; }

        public int TotalQuestion { get; set; }

        public double Duration {  get; set; }    

        public int HighestStreak { get; set; }

        public int Score { get;set; }

        public int Place { get; set; }

        public int TotalPlayers { get; set; }    

        public List<PlayerResponseDtocs> PlayerResponses { get; set; }
        
        public List<PlayerDto> FinalRanking { get; set; }

    }
}
