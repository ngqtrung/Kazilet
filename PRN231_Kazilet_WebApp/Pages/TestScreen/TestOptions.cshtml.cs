using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using PRN231_Kazilet_WebApp.Models.Dto;

namespace PRN231_Kazilet_WebApp.Pages.TestScreen
{
    public class TestOptionsModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly string questionUrl = "https://localhost:7024/odata/Question";
        public TestOptionsModel()
        {
            _httpClient = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            _httpClient.DefaultRequestHeaders.Accept.Add(contentType);
        }
        public IList<QuestionDto> Questions { get; set; } = default!;
        [BindProperty]
        public int Id {  get; set; }
        public async Task OnGet(int id)
        {
            Id = id;
            HttpResponseMessage m = await _httpClient.GetAsync($"{questionUrl}/{id}");
            string jsonStr = await m.Content.ReadAsStringAsync();
            dynamic temp = JObject.Parse(jsonStr);
            var list = temp.value;
            Questions = JsonConvert.DeserializeObject<IList<QuestionDto>>(list.ToString());
        }
        public IActionResult OnPost(int duration, string random, int numOfQues)
        {
            if (random=="true")
            {

                return RedirectToPage("/TestScreen/Test", new
                {
                    id = Id,
                    duration = duration,
                    numOfQues = numOfQues,
                    random = true,
                    isRetake = 1
                });
            }
            else
            {
                return RedirectToPage("/TestScreen/SelectQuestionsScreen", new
                {
                    id = Id,
                    duration = duration,                   
                });
            }
        }
    }
}
