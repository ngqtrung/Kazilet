using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PRN231_Kazilet_API.Models.Dto;
using PRN231_Kazilet_API.Models.Entities;
using PRN231_Kazilet_API.Services;
using PRN231_Kazilet_API.Services.Impl;

namespace PRN231_Kazilet_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LearningHistoryController : ControllerBase
    {
        private readonly ILearningHistory _learningHistoryService;
        private readonly IAuthService _authService;
        public LearningHistoryController(ILearningHistory learningHistoryService, IAuthService authService)
        {
            _learningHistoryService = learningHistoryService;
            _authService = authService;
        }
        [HttpGet]
        public IActionResult GetLearningHistories() {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized(new { isValid = false, message = "No token provided." });
            }
            User? u = _authService.GetUserFromJwtToken(token);
            List<LearningHistoryDto> learningHistoryDtos = _learningHistoryService.GetAllLearningHistoriesByUserId(u.Id);
            if (learningHistoryDtos == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(learningHistoryDtos);
            }
        }
        [HttpPost("{courseId}")]
        public IActionResult AddLearningHistory(int courseId)
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized(new { isValid = false, message = "No token provided." });
            }
            User? u = _authService.GetUserFromJwtToken(token);
            var isSaved = _learningHistoryService.AddLearningHistory(u.Id,courseId);
            if (isSaved)
            {
                return Ok();              
            }
            else
            {
                return NotFound();
            }
        }
        [HttpGet("GetTopCourse")]
        public IActionResult GetTopCourse()
        {
            var courseList = _learningHistoryService.GetTop5Course();
            return courseList == null ? NotFound() : Ok(courseList);
        }
    }
}
