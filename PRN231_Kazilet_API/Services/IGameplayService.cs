using Microsoft.EntityFrameworkCore;
using PRN231_Kazilet_API.Models.Dto;
using PRN231_Kazilet_API.Models.Entities;
using PRN231_Kazilet_API.Services.Impl;

namespace PRN231_Kazilet_API.Services
{
    public interface IGameplayService
    {
        public string HostGame(int courseId, string username, HttpContext httpContext);

        public Task StartGame(string code, string username);

        public GameplaySettingDto UpdateGameplaySetting(GameplaySettingDto gameplaySettingDto);

        public bool CheckExistCode(string code);

        public string JoinGame(string code, string username, HttpContext httpContext);

        public List<PlayerInformationDto> GetPlayerInRoom(string code);

        public PlayerAvatarDto GetPlayerAvatarInformation(string code, string username);

        public int[] GetQuestionAlreadyAnswer(string code);

        public Task<bool> UpdatePlayerAvatar(string code, string username, string avatar);

        public int AddPlayerAnswer(string code, string username, PlayerAnswerDto playerAnswerDto, HttpContext httpContext);

        public List<GameplayResultDto> GetGameplayResultForTurn(string code, int turn);

        public List<GameplayReportDto> GetGameplayReportForTurn(string code, int turn);

        public GameplayRankingDto GetGameplayRankingForTurn(string code, int turn);

        public GameplaySettingDto GetGameplaySettingDtoByCode(string code);

        public GameplayFinalReportDto GetGameplayFinalReport(string code, string username);

        public int GetCurrentQuestion(string code);

        public int GetTotalQuestions(string code);

        public int GetAnswerStreak(string code, string username);

        public int GetTimeLimit(string code);

        public int GetPoint(string code, string username);

        public int GetLatestTurn(string code);

        public void DeleteFromLobby(string code, string username);

        public void UpdateGetResult(string code, int turn);

        public string[] GetAvatarInLobby(string code);

        public string GetPlayerAvatar(string code, string username);

        public PlayerAnswerDto CheckSubmittedLatest(string code, string username);

        public List<GameplaySettingDto> GetAllGameplayCompleted();

        public ReportGameplayDto GetReportGameplay(int id);

        public ReportDetailQuestionDto GetDetailQuestion(int id, int no);

        public List<DetailPlayerAnswer> GetDetailPlayerAnswers(string code, int turn);

    }
}
