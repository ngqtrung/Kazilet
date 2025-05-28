using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PRN231_Kazilet_API.Models.Entities
{
    public partial class Course
    {
        public Course()
        {
            GameplaySettings = new HashSet<GameplaySetting>();
            LearningHistories = new HashSet<LearningHistory>();
            Questions = new HashSet<Question>();
            Folders = new HashSet<Folder>();
        }

        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public DateTime? CreatedAt { get; set; }
        public int? CreatedBy { get; set; }
        public string? CoursePassword { get; set; }
        public bool? IsPublic { get; set; }
        public int? Status { get; set; }

        public virtual User? CreatedByNavigation { get; set; }
        [JsonIgnore]
        public virtual ICollection<GameplaySetting> GameplaySettings { get; set; }
        [JsonIgnore]
        public virtual ICollection<LearningHistory> LearningHistories { get; set; }
        public virtual ICollection<Question> Questions { get; set; }
        [JsonIgnore]
        public virtual ICollection<Folder> Folders { get; set; }
    }
}
