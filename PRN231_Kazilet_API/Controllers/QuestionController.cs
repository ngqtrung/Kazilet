using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using PRN231_Kazilet_API.Models.Dto;
using PRN231_Kazilet_API.Services;

namespace PRN231_Kazilet_API.Controllers
{
    public class QuestionController : ODataController
    {
        private readonly IQuestionService _questionService;

        public QuestionController(IQuestionService questionService)
        {
            _questionService = questionService;
        }

        [EnableQuery]
        [HttpGet]
        [Route("odata/Question/{courseId:int}")]
        public IActionResult GetAllQuestions(int courseId)
        {
            return Ok(_questionService.GetAllQuestionsByCourse(courseId));
        }

        [EnableQuery]
        [HttpGet]
        [Route("odata/Question")]
        public IActionResult GetQuestionById([FromQuery]int questionNumber, [FromQuery] int courseId) { 
            QuestionDto questionDto = _questionService.GetById(questionNumber, courseId);
            if(questionDto == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(questionDto);
            }
        }

        [EnableQuery]
        [HttpGet]
        [Route("odata/Question/GetRandom/{id:int}/{numOfQues:int}")]
        public IActionResult GetRandomQuestion(int id, int numOfQues)
        {
            List<QuestionDto> list = _questionService.GetRandom(id, numOfQues);
            if (list == null)
            {
                return NotFound(ModelState);
            }
            else
            {
                return Ok(list);
            }
        }
        [EnableQuery]
        [HttpGet]
        [Route("odata/Question/GetQuestionsByIds")]
        public IActionResult GetQuestionsByIds([FromQuery] List<int> ids)
        {
            List<QuestionDto> list = _questionService.GetQuestionsByIds(ids);
            if (list == null)
            {
                return NotFound(ModelState);
            }
            else
            {
                return Ok(list);
            }
        }
    }
}
