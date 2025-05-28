using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PRN231_Kazilet_API.Models.Dto;
using PRN231_Kazilet_API.Services;
using PRN231_Kazilet_API.Services.Impl;

namespace PRN231_Kazilet_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseController : Controller
    {
        private readonly ICourseService _courseService;

        public CourseController(ICourseService courseService) {
            _courseService = courseService;
        }

        [HttpGet]
        [Route("Details/{courseId}")]
        public IActionResult GetCourseDetails(int courseId)
        {
            return Ok(_courseService.GetCourse(courseId));
        }

        [HttpPost("Add")]
        public async Task<IActionResult> AddCourse([FromBody] CourseDto courseDto)
        {
            if (courseDto == null)
            {
                return BadRequest("Question list cannot be null or empty.");
            }

            bool result = _courseService.AddCourse(courseDto);

            if (result)
            {
                return Ok("Questions added successfully.");
            }
            else
            {
                return StatusCode(500, "An error occurred while adding questions.");
            }
        }

        [HttpPost("Update/{courseId}")]
        public IActionResult UpdateCourse([FromBody] CourseDto courseDto)
        {
            if (courseDto == null)
            {
                return BadRequest("Course data cannot be null.");
            }

            bool result = _courseService.UpdateCourse(courseDto);

            if (result)
            {
                return Ok("Course updated successfully.");
            }
            else
            {
                return StatusCode(500, "An error occurred while updating the course.");
            }
        }
    }
}
