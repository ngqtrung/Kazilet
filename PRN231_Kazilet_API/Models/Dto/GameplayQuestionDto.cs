namespace PRN231_Kazilet_API.Models.Dto
{
    public class GameplayQuestionDto
    {
        public QuestionDto QuestionDto { get; set; }

        public List<GameplayAddI> gameplayAdds { get; set; }

        public GameplayQuestionDto()
        {
        }

        public GameplayQuestionDto(QuestionDto questionDto, List<GameplayAddI> gameplayAdds)
        {
            QuestionDto = questionDto;
            this.gameplayAdds = gameplayAdds;
        }
    }
}
