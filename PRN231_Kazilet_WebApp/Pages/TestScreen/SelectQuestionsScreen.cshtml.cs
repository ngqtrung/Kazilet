using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using PRN231_Kazilet_WebApp.Models.Dto;
using System.Diagnostics.Metrics;
using System.Net.Http.Headers;

namespace PRN231_Kazilet_WebApp.Pages.TestScreen
{
    public class SelectQuestionsScreenModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly string questionUrl = "https://localhost:7024/odata/Question";
        public SelectQuestionsScreenModel()
        {
            _httpClient = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            _httpClient.DefaultRequestHeaders.Accept.Add(contentType);
        }
        public IList<QuestionDto> Questions { get; set; } = default!;
        [BindProperty]
        public int Id {  get; set; }
        [BindProperty]
        public int Duration {  get; set; }
        [BindProperty]
        public List<int> SelectedQuestions { get; set; } = new List<int>();
        public async Task OnGet(int id, int duration)
        {
            Id = id;
            Duration = duration;
            HttpResponseMessage m = await _httpClient.GetAsync($"{questionUrl}/{id}");
            string jsonStr = await m.Content.ReadAsStringAsync();
            dynamic temp = JObject.Parse(jsonStr);
            var list = temp.value;
            Questions = JsonConvert.DeserializeObject<IList<QuestionDto>>(list.ToString());
        }
        public IActionResult OnPost()
        {
            if (SelectedQuestions.Count == 0)
            {
                return Page();
            }

            return RedirectToPage("/TestScreen/Test", new
            {
                id = Id,
                duration = Duration,
                selectedQuestions = string.Join(",", SelectedQuestions),
                isRetake = 1,
                numOfQues = SelectedQuestions.Count,
                random = false
            });
        }
    }
}
