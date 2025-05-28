using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using PRN231_Kazilet_WebApp.Models.Dto;

namespace PRN231_Kazilet_WebApp.Pages.LearningHistory
{
    public class HistoryModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly string learningHistorynUrl = "https://localhost:7024/api/LearningHistory";
        public HistoryModel()
        {
            _httpClient = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            _httpClient.DefaultRequestHeaders.Accept.Add(contentType);
        }
        [BindProperty]
        public int UserId {  get; set; }
        [BindProperty]
        public List<LearningHistoryDto> LearningHistories { get; set; }
        public async Task OnGet()
        {
            string jwtToken = HttpContext.Request.Cookies["accessToken"];

            if (!string.IsNullOrEmpty(jwtToken))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
            }
            HttpResponseMessage response = await _httpClient.GetAsync($"{learningHistorynUrl}");

            if (response.IsSuccessStatusCode)
            {
                string jsonStr = await response.Content.ReadAsStringAsync();
                LearningHistories = JsonConvert.DeserializeObject<List<LearningHistoryDto>>(jsonStr);
            }
            else
            {
                Console.WriteLine("Error: " + response.StatusCode);
            }
        }
    }
}
