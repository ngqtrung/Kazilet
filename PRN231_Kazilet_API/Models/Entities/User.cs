using System;
using System.Collections.Generic;

namespace PRN231_Kazilet_API.Models.Entities
{
    public partial class User
    {
        public User()
        {
            Courses = new HashSet<Course>();
            Folders = new HashSet<Folder>();
            GameplaySettings = new HashSet<GameplaySetting>();
            Gameplays = new HashSet<Gameplay>();
            LearningHistories = new HashSet<LearningHistory>();
            Notifications = new HashSet<Notification>();
        }

        public int Id { get; set; }
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public int? Role { get; set; }
        public string? Type { get; set; }
        public string? Gid { get; set; }

        public virtual UserRole? RoleNavigation { get; set; }
        public virtual ICollection<Course> Courses { get; set; }
        public virtual ICollection<Folder> Folders { get; set; }
        public virtual ICollection<GameplaySetting> GameplaySettings { get; set; }
        public virtual ICollection<Gameplay> Gameplays { get; set; }
        public virtual ICollection<LearningHistory> LearningHistories { get; set; }
        public virtual ICollection<Notification> Notifications { get; set; }
    }
}
