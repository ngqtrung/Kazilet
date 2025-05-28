using MailKit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using PRN231_Kazilet_API.Models.Dto;
//using OfficeOpenXml;
using PRN231_Kazilet_API.Models.Entities;
using PRN231_Kazilet_API.Services;
using PRN231_Kazilet_API.Services.Impl;

namespace PRN231_Kazilet_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoursesController : ODataController
    {
        private readonly PRN231_Kazilet_v2Context _context;
        private readonly ICourseService _courseService;
        public CoursesController(PRN231_Kazilet_v2Context context, ICourseService courseService)
        {
            _context = context;
            _courseService = courseService;
        }


        //[HttpGet("by-folder/{folderid}")]
        //public IActionResult GetCourseByFolder(int folderid)
        //{
        //    var courses = _context.FolderCourses
        //        .Where(fc => fc.FolderId == folderid)
        //        .Include(fc => fc.Course)
        //        .ThenInclude(c => c.CreatedByNavigation)
        //        .Select(fc => fc.Course)
        //        .ToList();



        //    if (courses == null || courses.Count == 0)
        //    {
        //        return NotFound("This folder hasn't have any course");
        //    }
        //    return Ok(courses);
        //}

        [HttpPost("import")]
        public IActionResult ImportFromExcel(IFormFile file, int courseId)
        {
            if (file == null || file.Length <= 0)
            {
                return BadRequest("Please upload a valid Excel file.");
            }

            using (var stream = new MemoryStream())
            {
                file.CopyTo(stream);

                using (var package = new ExcelPackage(stream))
                {
                    var worksheet = package.Workbook.Worksheets[0];
                    var rowCount = worksheet.Dimension.Rows;

                    for (int row = 2; row <= rowCount; row++) // Bắt đầu từ hàng 2 để bỏ qua tiêu đề
                    {
                        var questionContent = worksheet.Cells[row, 1].Value?.ToString();
                        var correctAnswer = worksheet.Cells[row, 2].Value?.ToString();
                        var answer1 = worksheet.Cells[row, 3].Value?.ToString();
                        var answer2 = worksheet.Cells[row, 4].Value?.ToString();
                        var answer3 = worksheet.Cells[row, 5].Value?.ToString();

                        // Tạo câu hỏi mới
                        var question = new Question
                        {
                            CourseId = courseId,
                            Content = questionContent,
                            IsMarked = false,  // Bạn có thể thêm logic để xác định IsMarked
                        };

                        // Tạo các câu trả lời
                        var answers = new List<Answer>
                {
                    new Answer { Content = correctAnswer, IsCorrect = true },
                    new Answer { Content = answer1, IsCorrect = false },
                    new Answer { Content = answer2, IsCorrect = false },
                    new Answer { Content = answer3, IsCorrect = false }
                };

                        question.Answers = answers;

                        _context.Questions.Add(question);
                    }

                    _context.SaveChanges();
                }
            }

            return Ok("Questions imported successfully from Excel.");
        }

        [HttpGet("export")]
        public IActionResult ExportToExcel(int courseId)
        {
            var questions = _context.Questions
                                    .Where(q => q.CourseId == courseId)
                                    .Include(q => q.Answers)  // Bao gồm câu trả lời
                                    .ToList();

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Questions");
                worksheet.Cells[1, 1].Value = "Câu hỏi";
                worksheet.Cells[1, 2].Value = "Câu trả lời đúng";
                worksheet.Cells[1, 3].Value = "Câu trả lời 1";
                worksheet.Cells[1, 4].Value = "Câu trả lời 2";
                worksheet.Cells[1, 5].Value = "Câu trả lời 3";
                worksheet.Cells[1, 6].Value = "Câu trả lời 4";

                int row = 2;
                foreach (var question in questions)
                {
                    var correctAnswer = question.Answers.FirstOrDefault(a => (bool)a.IsCorrect)?.Content;

                    worksheet.Cells[row, 1].Value = question.Content;
                    worksheet.Cells[row, 2].Value = correctAnswer;
                    worksheet.Cells[row, 3].Value = question.Answers.ElementAtOrDefault(0)?.Content;
                    worksheet.Cells[row, 4].Value = question.Answers.ElementAtOrDefault(1)?.Content;
                    worksheet.Cells[row, 5].Value = question.Answers.ElementAtOrDefault(2)?.Content;
                    worksheet.Cells[row, 6].Value = question.Answers.ElementAtOrDefault(3)?.Content;
                    row++;
                }

                var stream = new MemoryStream();
                package.SaveAs(stream);
                stream.Position = 0;

                var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                var fileName = "Questions.xlsx";
                return File(stream, contentType, fileName);
            }
        }

        [HttpGet]
        [Route("{userId}")]
        public IActionResult GetOwnedCourses(int userId)
        {
            return Ok(_courseService.GetOwnedCourses(userId));
        }

        [EnableQuery]
        [HttpGet]
        [Route("Details/{courseId}")]
        public IActionResult GetCourseDetails(int courseId)
        {
            var rs = _courseService.GetCourse(courseId);
            return Ok(rs);
        }

        //[Authorize(Roles = "user,admin")]
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

        //[Authorize(Roles = "user,admin")]
        [HttpPost("Update")]
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

        [HttpDelete("{courseId}")]
        public IActionResult DeleteCourse(int courseId)
        {
            bool result = _courseService.DeleteCourse(courseId);

            if (result)
            {
                return Ok("Course deleted successfully.");
            }
            else
            {
                return NotFound("Course not found or could not be deleted.");
            }
        }

        [HttpGet("IsPublic/{courseId}")]
        public IActionResult IsCoursePublic(int courseId)
        {
            bool isPublic = _courseService.IsCoursePublic(courseId);
            return isPublic ? Ok("Course is public.") : NotFound("Course is private.");
        }

        [HttpPost("VerifyPassword/{courseId}")]
        public IActionResult VerifyPassword(int courseId, [FromQuery] string password)
        {
            if (_courseService.VerifyPassword(courseId, password))
            {
                return Ok();
            }
            else
            {
                return Content("Invalid password. Please try again.", "text/html", System.Text.Encoding.UTF8);
            }
        }


        [HttpGet("search")]
        public IActionResult SearchCourses(string search)
        {
            var courses = _courseService.SearchCourses(search);
            return Ok(courses);
        }
    }
}
