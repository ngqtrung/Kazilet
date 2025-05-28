using System;
using System.Collections.Generic;

namespace PRN231_Kazilet_API.Models.Entities
{
    public partial class QuestionStatus
    {
        public QuestionStatus()
        {
            Questions = new HashSet<Question>();
        }

        public int Id { get; set; }
        public string? Status { get; set; }

        public virtual ICollection<Question> Questions { get; set; }
    }
}
