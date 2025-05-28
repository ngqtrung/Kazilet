using Microsoft.AspNetCore.Mvc;
using PRN231_Kazilet_API.Models.Dto;
using PRN231_Kazilet_API.Services;
using PRN231_Kazilet_API.Services.Impl;

namespace PRN231_Kazilet_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GameplayController : Controller
    {
        private readonly IGameplayService _gameplayService;

        private readonly IAuthService _authService;

        private readonly IQuestionService _questionService;

        public GameplayController(IGameplayService gameplayService, IAuthService authService, IQuestionService questionService)
        {
            _gameplayService = gameplayService;
            _authService = authService;
            _questionService = questionService;
        }

        [HttpGet]
        [Route("get-all")]
        public IActionResult GetAllGameplayCompleted()
        {
            return Ok(_gameplayService.GetAllGameplayCompleted());
        }

        [HttpGet]
        [Route("get-detail-question")]
        public IActionResult GetDetailQuestion([FromQuery] int id, [FromQuery] int turn)
        {
            return Ok(_gameplayService.GetDetailQuestion(id, turn));
        }

        [HttpGet]
        [Route("report/{id}")]
        public IActionResult GetReportGameplay(int id)
        {
            return Ok(_gameplayService.GetReportGameplay(id));
        }

        [HttpPost]
        [Route("host")]
        public IActionResult HostGame([FromQuery] int courseId, [FromQuery] string username)
        {
            string code = _gameplayService.HostGame(courseId, username, HttpContext);
            return Ok(new
            {
                Code = code,
                Token = _authService.GetGameplayToken(code, username),
                NoQ = _questionService.GetNumberOfQuestionsInCourse(courseId)
            });
        }

        [HttpGet]
        [Route("find")]
        public IActionResult FindGame([FromQuery] string code)
        {
            if (_gameplayService.CheckExistCode(code))
            {
                return Ok();
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost]
        [Route("join")]
        public IActionResult JoinGame([FromQuery] string code, [FromQuery] string username)
        {
            string token = _gameplayService.JoinGame(code, username, HttpContext);
            if (!string.IsNullOrEmpty(token))
            {
                return Ok(new { token });
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpPatch]
        [Route("update-setting")]
        public IActionResult UpdateGameplaySetting([FromBody] GameplaySettingDto gameplaySettingDto)
        {
            if (_gameplayService.CheckExistCode(gameplaySettingDto.Code))
            {
                return Ok(_gameplayService.UpdateGameplaySetting(gameplaySettingDto));
            }
            else
            {
                return NotFound();
            }
        }

        [HttpGet]
        [Route("get-players")]
        public IActionResult GetAllPlayersInRoom([FromQuery] string code)
        {
            return Ok(_gameplayService.GetPlayerInRoom(code));
        }

        [HttpPost]
        [Route("update-avatar")]
        public async Task<IActionResult> UpdatePlayerAvatar([FromQuery] string code, [FromQuery] string username, [FromQuery] string avatar)
        {
            return Ok(_gameplayService.UpdatePlayerAvatar(code, username, avatar));
        }

        [HttpGet]
        [Route("get-avatars")]
        public IActionResult GetAvatarInformation([FromQuery] string code, [FromQuery] string username)
        {
            return Ok(_gameplayService.GetPlayerAvatarInformation(code, username));
        }

        [HttpPost]
        [Route("start")]
        public async Task<IActionResult> StartGame([FromQuery] string code, [FromQuery] string username)
        {
            await _gameplayService.StartGame(code, username);
            return Ok();
        }

        [HttpGet]
        [Route("final-report")]
        public IActionResult GetFinalReport([FromQuery] string code, [FromQuery] string username)
        {
            return Ok(_gameplayService.GetGameplayFinalReport(code, username)); 
        }
    }
}
