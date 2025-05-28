using PRN231_Kazilet_API.Models.Dto;

namespace PRN231_Kazilet_API.Services
{
    public interface IFolderService
    {

        public bool AddFolder(FolderDto folderDto);
        public bool RemoveFolder(int folderId);
        public bool RemoveCourseInFolder(FolderCourseDto folderCourseDto);//Remove course inside folder
        public bool AddCourseToFolder(FolderCourseDto folderCourseDto);//Add Coures into folder
        public List<CourseDto> GetCoursesByFolder(int folderId);

    }
}
