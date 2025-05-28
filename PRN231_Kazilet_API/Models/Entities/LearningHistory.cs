using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PRN231_Kazilet_API.Models.Entities
{
    public partial class LearningHistory
    {
        public int UserId { get; set; }
        public int CourseId { get; set; }
        public DateTime? LearningDate { get; set; }

        public virtual Course Course { get; set; } = null!;
        [JsonIgnore]
        public virtual User User { get; set; } = null!;
    }
}
