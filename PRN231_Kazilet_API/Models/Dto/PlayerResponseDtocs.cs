namespace PRN231_Kazilet_API.Models.Dto
{
    public class PlayerResponseDtocs
    {
        public int PlayerAnswer { get; set; }   

        public QuestionDto QuestionDto { get; set; }

        public PlayerResponseDtocs()
        {
        }

        public PlayerResponseDtocs(int playerAnswer, QuestionDto QuestionDto)
        {
            PlayerAnswer = playerAnswer;
            this.QuestionDto = QuestionDto;
        }
    }
}
