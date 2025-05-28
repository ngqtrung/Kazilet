using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace PRN231_Kazilet_WebApp.Pages.Gameplay
{
    public class HostModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly string GameplayUrl = "https://localhost:7024/api/Gameplay";

        [BindProperty]
        public int CourseId { get; set; }

        [BindProperty]
        public string Code { get; set; }

        [BindProperty]
        public string Token { get; set; }

        [BindProperty]
        public string Username { get; set; }

        public HostModel()
        {
            _httpClient = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            _httpClient.DefaultRequestHeaders.Accept.Add(contentType);
        }
        public async Task OnGetAsync(int courseId)
        {
            
            Username = "Mast";
            HttpResponseMessage response = await _httpClient.PostAsync(GameplayUrl + "/host?courseId=" + courseId + "&username=Mast", null);
            await Console.Out.WriteLineAsync(response.ToString());
            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                dynamic jsonValue = JsonConvert.DeserializeObject<dynamic>(json);
                Code = jsonValue.code.ToString();
                Token = jsonValue.token.ToString();
            }
            
        }
    }
}
