using System;
using System.Collections.Generic;

namespace PRN231_Kazilet_API.Models.Entities
{
    public class GameplayAnswer
    {
        public int GameplayId { get; set; }
        public int QuestionId { get; set; }
        public int? PlayerAnswer { get; set; }

        public virtual Gameplay Gameplay { get; set; } = null!;
        public virtual Question Question { get; set; } = null!;
    }
}
