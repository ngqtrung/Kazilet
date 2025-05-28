using System;
using System.Collections.Generic;

namespace PRN231_Kazilet_API.Models.Entities
{
    public partial class GameplaySetting
    {
        public GameplaySetting()
        {
            Gameplays = new HashSet<Gameplay>();
        }

        public int Id { get; set; }
        public string Code { get; set; } = null!;
        public int? CourseId { get; set; }
        public int? NoQuestion { get; set; }
        public int? TimeLimit { get; set; }
        public bool? IsSkillEnabled { get; set; }
        public bool? IsStarted { get; set; }
        public DateTime? CreatedAt { get; set; }
        public int? CreatedBy { get; set; }
        public bool? IsHostPlay { get; set; }
        public bool? IsCompleted { get; set; }

        public virtual Course? Course { get; set; }
        public virtual User? CreatedByNavigation { get; set; }
        public virtual ICollection<Gameplay> Gameplays { get; set; }

        public GameplaySetting(string code, DateTime? createdAt, int? createdBy)
        {
            Code = code;
            CreatedAt = createdAt;
            CreatedBy = createdBy;
        }
    }
}
