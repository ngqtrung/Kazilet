using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PRN231_Kazilet_API.Models.Dto;
using PRN231_Kazilet_API.Models.Entities;
using PRN231_Kazilet_API.Services;
using PRN231_Kazilet_API.Services.Impl;

namespace PRN231_Kazilet_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FoldersController : ControllerBase
    {
        private IFolderService _folderService;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly PRN231_Kazilet_v2Context _context;

        public FoldersController(IFolderService folderService, IHttpContextAccessor contextAccessor, PRN231_Kazilet_v2Context context)
        {
            _folderService = folderService;
            _contextAccessor = contextAccessor;
            _context = context;

        }

        [HttpPost("Add")]
        public IActionResult AddFolder([FromQuery] string folderName)
        {
            FolderDto folderDto = new FolderDto();
            folderDto.CreatedBy = 1;//TODO: Get current UserId
            folderDto.Name = folderName;
            _folderService.AddFolder(folderDto);
            return Ok(folderDto);
        }

        [HttpGet("folders/{userid}")]
        public IActionResult GetFoldersByUser(int userid)
        {
            var folders = _context.Folders.Include(f => f.CreatedByNavigation)
                                        .Where(c => c.CreatedByNavigation.Id == userid)
                                        .Select(f => new
                                        {
                                            Id = f.Id,
                                            Name = f.Name,
                                            Created_by = f.CreatedByNavigation,
                                            Created_at = f.CreatedAt
                                        })
                                        .ToList();
            if (folders == null || folders.Count == 0)
            {
                return NotFound("No folders found for user");
            }
            return Ok(folders);
        }

        [HttpGet("GetCourse/{folderId}")]
        public IActionResult AddCourseToFolder(int folderId)
        {

            var courses = _folderService.GetCoursesByFolder(folderId);
            
                return Ok(courses);
        }

        [HttpPost("AddCourse")]
        public IActionResult AddCourseToFolder([FromBody] FolderCourseDto folderCourseDto)
        {
           
            if(_folderService.AddCourseToFolder(folderCourseDto))
            {
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpPost("RemoveCourse")]
        public IActionResult RemoveCourseToFolder([FromBody] FolderCourseDto folderCourseDto)
        {

            if (_folderService.RemoveCourseInFolder(folderCourseDto))
            {
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpPost("RemoveFolder")]
        public IActionResult RemoveFolder([FromQuery] int folderId)
        {

            if (_folderService.RemoveFolder( folderId))
            {
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }
    }
}
