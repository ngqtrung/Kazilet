using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PRN231_Kazilet_API.Models.Dto;
using PRN231_Kazilet_API.Models.Entities;

namespace PRN231_Kazilet_API.Services.Impl
{
    public class FolderService : IFolderService
    {
        private readonly PRN231_Kazilet_v2Context _context;
        private readonly IMapper _mapper;

        public FolderService(PRN231_Kazilet_v2Context context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public bool AddCourseToFolder(FolderCourseDto folderCourseDto)
        {
            var course = _context.Courses.Find(folderCourseDto.CourseId);
            var folder = _context.Folders.Find(folderCourseDto.FolderId);

            if (course == null || folder == null)
                return false;

            course.Folders.Add(folder); // Thêm folder vào collection Folders của course
            return _context.SaveChanges() > 0;
        }

        public bool AddFolder(FolderDto folderDto)
        {
            Folder f = new Folder()
            {
                Id = 0,
                Name = folderDto.Name,
                CreatedAt = DateTime.Now.Date,
                CreatedBy = folderDto.CreatedBy
            };
            _context.Folders.Add(f);
            return _context.SaveChanges() > 0;
        }

        public List<CourseDto> GetCoursesByFolder(int folderId)
        {
            // Lấy folder cùng với các courses liên quan
            var folder = _context.Folders
                .Include(f => f.Courses)  // Đưa các courses vào khi lấy Folder
                .FirstOrDefault(f => f.Id == folderId);

            // Nếu không tìm thấy folder, trả về danh sách rỗng
            if (folder == null)
            {
                return new List<CourseDto>();
            }

            // Map các đối tượng Course từ entity sang DTO và trả về danh sách
            var courseDtos = _mapper.Map<List<CourseDto>>(folder.Courses);

            return courseDtos;
        }

        public bool RemoveCourseInFolder(FolderCourseDto folderCourseDto)
        {
            var course = _context.Courses.Include(c => c.Folders).FirstOrDefault(c => c.Id == folderCourseDto.CourseId);
            var folder = _context.Folders.Find(folderCourseDto.FolderId);

            if (course == null || folder == null)
                return false;

            if (course.Folders.Contains(folder))
            {
                course.Folders.Remove(folder);
                return _context.SaveChanges() > 0;
            }

            return false;
        }

        public bool RemoveFolder(int folderId)
        {
            var folder = _context.Folders.Include(f => f.Courses).FirstOrDefault(f => f.Id == folderId);
            if (folder == null)
            {
                return false;
            }

            // Xóa tất cả các quan hệ với các khóa học trước khi xóa Folder
            folder.Courses.Clear();
            _context.Folders.Remove(folder);

            return _context.SaveChanges() > 0;
        }
    }
}
