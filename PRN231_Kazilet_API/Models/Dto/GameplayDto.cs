using PRN231_Kazilet_API.Models.Entities;

namespace PRN231_Kazilet_API.Models.Dto
{
    public class GameplayDto
    {
        public int Id { get; set; }
        public string? Code { get; set; }
        public int? UserId { get; set; }
        public string? Username { get; set; }
        public int? QuestionId { get; set; }
        public string? PlayerAnswer { get; set; }
        public bool? IsCorrect { get; set; }
        public int? Turn { get; set; }
        public int? Score { get; set; }
        public double? Duration { get; set; }

        public string? Avatar { get; set; }
    }
}
