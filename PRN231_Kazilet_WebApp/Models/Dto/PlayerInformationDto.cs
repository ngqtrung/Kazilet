namespace PRN231_Kazilet_WebApp.Models.Dto
{
    public class PlayerInformationDto
    {
        public string Username { get; set; }

        public string Avatar { get; set; }

        public PlayerInformationDto(string username, string avatar)
        {
            Username = username;
            Avatar = avatar;
        }
    }
}
