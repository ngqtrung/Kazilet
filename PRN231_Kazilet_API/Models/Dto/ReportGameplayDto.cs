namespace PRN231_Kazilet_API.Models.Dto
{
    public class ReportGameplayDto
    {
        public string Code { get; set; }    
        public double CorrectPercent { get;set; }

        public double IncorrectPercent { get;set; }

        public int NoPlayers { get; set; }

        public int NoQuestions { get; set; }

        public List<ReportOverviewDto> Overview { get; set; }

        public List<ReportQuestionDto> Question { get; set; }

      
        public override string? ToString()
        {
            return Code + " " + CorrectPercent + " " + IncorrectPercent;
        }
    }
}
