using System;
using System.Collections.Generic;

namespace PRN231_Kazilet_API.Models.Entities
{
    public partial class FolderCourse
    {
        public int Id { get; set; }
        public int? CourseId { get; set; }
        public int? FolderId { get; set; }

        public virtual Course? Course { get; set; }
        public virtual Folder? Folder { get; set; }
    }
}
