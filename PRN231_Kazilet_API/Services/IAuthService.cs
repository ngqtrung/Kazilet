using PRN231_Kazilet_API.Models.Entities;

namespace PRN231_Kazilet_API.Services
{
    public interface IAuthService
    {
        public string GetGameplayToken(string code, string username);
        public string CheckGameplayCodeValid(string token);
        public string GetUsernameFromToken(string token);
        public string GenerateJwtToken(User authenticatedUser);
        public string GetValueFromJwtToken(string field, string token);
        public User? GetUserFromJwtToken(string token);

    }
}
