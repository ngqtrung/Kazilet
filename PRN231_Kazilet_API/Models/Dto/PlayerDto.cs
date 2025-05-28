namespace PRN231_Kazilet_API.Models.Dto
{
    public class PlayerDto
    {
        public string Avatar { get; set; }  
        public string Username { get; set; }

        public int Score { get; set; }

        public PlayerDto(string avatar, string username, int score)
        {
            Avatar = avatar;
            Username = username;
            Score = score;
        }
    }
}
