using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using PRN231_Kazilet_WebApp.Models.Dto;
using System.Net.Http.Headers;

namespace PRN231_Kazilet_WebApp.Pages.Gameplay
{
    public class LobbyModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly string GameplayUrl = "https://localhost:7024/api/Gameplay";
        [BindProperty]
        public string Code { get; set; }

        [BindProperty]
        public string Username { get; set; }

        [BindProperty]
        public string Token { get; set; }

        [BindProperty]
        public List<PlayerInformationDto> Players { get; set; }

        public LobbyModel()
        {
            _httpClient = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            _httpClient.DefaultRequestHeaders.Accept.Add(contentType);
        }

        public async Task OnGetAsync(string code, string username)
        {
            Code = code;
            Username = username;
            
            HttpResponseMessage response = await _httpClient.GetAsync(GameplayUrl + "/get-players?code=" + Code);
            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                Players = JsonConvert.DeserializeObject<List<PlayerInformationDto>>(json);
               
            }

            response = await _httpClient.PostAsync(GameplayUrl + "/join?code=" + Code + "&username=" + Username, null);
            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<JoinGameResponse>(json);
                Token = result.Token;
                await Console.Out.WriteLineAsync("Token: " + Token);

            }
        }

        private class JoinGameResponse
        {
            public string Token { get; set; }
        }
    }
}
