namespace PRN231_Kazilet_API.Models.Dto
{
    public class GameplayResultDto
    {
        public string Username { get;set; }

        public int Score { get;set; }   

        public int Streak { get;set; }

        public bool IsCorrect { get;set; }

        public int Place { get;set; }   
    }
}
