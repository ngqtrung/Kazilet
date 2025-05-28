namespace PRN231_Kazilet_API.Models.Dto
{
    public class ReportQuestionDto
    {
        public int No { get;set; }

        public string Content { get; set; } 

        public double CorrectPercent { get;set; }

        public ReportQuestionDto(int no, string content, double correctPercent)
        {
            No = no;
            Content = content;
            CorrectPercent = correctPercent;
        }
    }
}
