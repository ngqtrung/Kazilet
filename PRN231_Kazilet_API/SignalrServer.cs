
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using PRN231_Kazilet_API.Models.Dto;
using PRN231_Kazilet_API.Models.Entities;
using PRN231_Kazilet_API.Services;
using PRN231_Kazilet_API.Utils;

namespace PRN231_Kazilet_API
{
    public class SignalrServer : Hub
    {

        private readonly PRN231_Kazilet_v2Context _context;

        private readonly IHttpContextAccessor _contextAccessor;

        private readonly IAuthService _authService;

        private readonly IQuestionService _questionService;

        private bool isSumitted = false;

        private readonly IGameplayService _gameplayService;

        public SignalrServer(PRN231_Kazilet_v2Context context, IHttpContextAccessor contextAccessor, IAuthService authService, IQuestionService questionService, IGameplayService gameplayService)
        {
            _context = context;
            _contextAccessor = contextAccessor;
            _authService = authService;
            _questionService = questionService;
            _gameplayService = gameplayService;
        }

        public override async Task OnConnectedAsync()
        {
            var token = _contextAccessor.HttpContext.Request.Query["token"];
            string username = _authService.GetUsernameFromToken(token);
            string code = _authService.CheckGameplayCodeValid(token);
            string avatar = _gameplayService.GetPlayerAvatar(code, username);
            if (!string.IsNullOrEmpty(code))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, code);
                await Clients.Group(code).SendAsync("UserJoined", new{
                    Username = username,
                    Avatar = avatar
                });
            }
            else
            {
                Context.Abort();

            }
            await base.OnConnectedAsync();
        }

        public async Task GetQuestion()
        {
            var token = _contextAccessor.HttpContext.Request.Query["token"];
            string username = _authService.GetUsernameFromToken(token);
            string code = _authService.CheckGameplayCodeValid(token);
            int courseId = (int)_context.GameplaySettings.FirstOrDefault(c => c.Code == code).CourseId;
            List<QuestionDto> questionDtos = _questionService.GetAllQuestionsByCourse(courseId);
            int questionId = GameplayUtils.GenerateRandom(questionDtos.Count);
            while (_gameplayService.GetQuestionAlreadyAnswer(code).Contains(questionId))
            {
                questionId = GameplayUtils.GenerateRandom(questionDtos.Count);
            }
            QuestionDto questionDto = _questionService.GetById(questionId);
            List<GameplayAddI> gameplayAdds = new List<GameplayAddI>();
            List<PlayerInformationDto> players = _gameplayService.GetPlayerInRoom(code);
            for (int i = 0; i < players.Count; i++)
            {
                int currentQuestion = _gameplayService.GetCurrentQuestion(code);
                int totalQuestion = _gameplayService.GetTotalQuestions(code);
                int streak = _gameplayService.GetAnswerStreak(code, players[i].Username);
                int timeLimit = _gameplayService.GetTimeLimit(code);
                int point = _gameplayService.GetPoint(code, players[i].Username);
                gameplayAdds.Add(new GameplayAddI(players[i].Username, currentQuestion, totalQuestion, streak, timeLimit, point, DateTime.Now));
            }
            GameplayQuestionDto gameplayQuestionDto = new GameplayQuestionDto(questionDto, gameplayAdds);            //await _signalRHub.Clients.Group(code).SendAsync("GetQuestion", questionDto);
            string json = JsonConvert.SerializeObject(gameplayQuestionDto);
            await Clients.Group(code).SendAsync("GetQuestion", json);
        }

        public async Task CheckSubmitted()
        {
            
            var token = _contextAccessor.HttpContext.Request.Query["token"];
            string username = _authService.GetUsernameFromToken(token);
            string code = _authService.CheckGameplayCodeValid(token);
            PlayerAnswerDto playerAnswerDto = _gameplayService.CheckSubmittedLatest(code, username);
            if (playerAnswerDto != null)
            {
                List<GameplayResultDto> gameplayResultDtos = _gameplayService.GetGameplayResultForTurn(code, playerAnswerDto.Turn);
                _gameplayService.UpdateGetResult(code, playerAnswerDto.Turn);
                await Clients.Group(code).SendAsync("GetResult", JsonConvert.SerializeObject(gameplayResultDtos));
                await Task.Delay(3000);

                List<GameplayReportDto> gameplayReportDtos = _gameplayService.GetGameplayReportForTurn(code, playerAnswerDto.Turn);
                await Clients.Group(code).SendAsync("GetReport", JsonConvert.SerializeObject(gameplayReportDtos));

                await Task.Delay(3000);
                GameplayRankingDto gameplayRankingDto = _gameplayService.GetGameplayRankingForTurn(code, playerAnswerDto.Turn);
                await Clients.Group(code).SendAsync("GetRanking", JsonConvert.SerializeObject(gameplayRankingDto));

                await Task.Delay(4500);
                GameplaySettingDto gameplaySettingDto = _gameplayService.GetGameplaySettingDtoByCode(code);
                if (gameplaySettingDto != null)
                {
                    if (gameplaySettingDto.NoQuestion == playerAnswerDto.Turn)
                    {
                        await Clients.Group(code).SendAsync("GetFinalReport", code);
                    }
                    else
                    {
                        await GetQuestion();
                    }
                }
            }

            
        }

        public async Task Answer(string json)
        {
            await Console.Out.WriteLineAsync("JSON: " + json);
            var token = _contextAccessor.HttpContext.Request.Query["token"];
            string username = _authService.GetUsernameFromToken(token);
            string code = _authService.CheckGameplayCodeValid(token);
            PlayerAnswerDto playerAnswerDto = JsonConvert.DeserializeObject<PlayerAnswerDto>(json);
            int numberSubmitted = _gameplayService.AddPlayerAnswer(code, username, playerAnswerDto, _contextAccessor.HttpContext);
            if (numberSubmitted == _gameplayService.GetPlayerInRoom(code).Count)
            {
                List<GameplayResultDto> gameplayResultDtos = _gameplayService.GetGameplayResultForTurn(code, playerAnswerDto.Turn);
                _gameplayService.UpdateGetResult(code, playerAnswerDto.Turn);
                await Clients.Group(code).SendAsync("GetResult", JsonConvert.SerializeObject(gameplayResultDtos));
                await Task.Delay(3000);

                List<GameplayReportDto> gameplayReportDtos = _gameplayService.GetGameplayReportForTurn(code, playerAnswerDto.Turn);
                await Clients.Group(code).SendAsync("GetReport", JsonConvert.SerializeObject(gameplayReportDtos));

                await Task.Delay(3500);
                GameplayRankingDto gameplayRankingDto = _gameplayService.GetGameplayRankingForTurn(code, playerAnswerDto.Turn);
                await Clients.Group(code).SendAsync("GetRanking", JsonConvert.SerializeObject(gameplayRankingDto));

                await Task.Delay(4500);
                GameplaySettingDto gameplaySettingDto = _gameplayService.GetGameplaySettingDtoByCode(code);
                if (gameplaySettingDto != null)
                {
                    if(gameplaySettingDto.NoQuestion == playerAnswerDto.Turn)
                    {
                        await Clients.Group(code).SendAsync("GetFinalReport", code);
                    }
                    else
                    {
                        await GetQuestion();
                    }
                }
            }
            else
            {
                await Clients.Group(code).SendAsync("Submitted", numberSubmitted);
            }
            // Xử lý hoặc phản hồi lại client nếu cần thiết
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var token = _contextAccessor.HttpContext.Request.Query["token"];
            string username = _authService.GetUsernameFromToken(token);
            string code = _authService.CheckGameplayCodeValid(token);
            _gameplayService.DeleteFromLobby(code, username);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, code);
            await Clients.Group(code).SendAsync("UserLeft", username);
            await base.OnDisconnectedAsync(exception);
        }
    }
}
