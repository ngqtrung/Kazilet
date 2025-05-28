namespace PRN231_Kazilet_API.Models.Dto
{
    public class GameplaySettingDto
    {
        public int? Id { get; set; }
        public string? Code { get; set; } = null!;
        public int? CourseId { get; set; }
        public int? NoQuestion { get; set; }
        public int? TimeLimit { get; set; }
        public bool? IsSkillEnabled { get; set; }
        public bool? IsStarted { get; set; }
        public DateTime? CreatedAt { get; set; }
        public int? CreatedBy { get; set; }
        public bool? IsHostPlay { get; set; }
        public bool? IsCompleted { get; set; }

        public string? CourseName { get; set; }

        public int? NoPlayers { get;set; }  
    }
}
