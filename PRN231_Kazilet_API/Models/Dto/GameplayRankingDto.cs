namespace PRN231_Kazilet_API.Models.Dto
{
    public class GameplayRankingDto
    {
        public List<PlayerDto> oldRank { get; set; }

        public List<PlayerDto> newRank { get; set; }

        public GameplayRankingDto(List<PlayerDto> oldRank, List<PlayerDto> newRank)
        {
            this.oldRank = oldRank;
            this.newRank = newRank;
        }
    }
}
