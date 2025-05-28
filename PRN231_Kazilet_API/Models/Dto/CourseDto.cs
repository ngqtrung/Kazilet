using PRN231_Kazilet_API.Models.Entities;
using System.Text.Json.Serialization;

namespace PRN231_Kazilet_API.Models.Dto
{
    public class CourseDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public DateTime? CreatedAt { get; set; }
        public int? CreatedBy { get; set; }
        public string? CoursePassword { get; set; }
        public bool? IsPublic { get; set; }
        public int? Status { get; set; }
        public int numOfQues { get; set; }

        public virtual User? CreatedByNavigation { get; set; }
        public virtual ICollection<GameplaySetting> GameplaySettings { get; set; }
        public virtual ICollection<LearningHistory> LearningHistories { get; set; }
        public virtual ICollection<Question> Questions { get; set; }
        public virtual ICollection<Folder> Folders { get; set; }
    }
}
