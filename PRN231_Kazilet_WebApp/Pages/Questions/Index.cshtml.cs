using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PRN231_Kazilet_WebApp.Models.Dto;
using System.Net.Http.Headers;
using System.Text.Json.Serialization;

namespace PRN231_Kazilet_WebApp.Pages.Questions
{
    public class IndexModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly string QuestionUrl = "https://localhost:7024/odata/Question";

        [BindProperty]
        public List<QuestionDto> QuestionDtos { get; set; }

        [BindProperty]
        public int Total { get; set; }

        [BindProperty]
        public int CourseId { get; set; }

        public IndexModel()
        {
            _httpClient = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            _httpClient.DefaultRequestHeaders.Accept.Add(contentType);
            QuestionDtos = new List<QuestionDto>();
        }

        public async Task OnGetAsync(int courseId)
        {
            CourseId = courseId;
            string condition = "";
            HttpResponseMessage response = await _httpClient.GetAsync(QuestionUrl + "/" + courseId + "?$expand=Answers&$count=true&$top=100" + condition);
            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                dynamic jsonValue = JsonConvert.DeserializeObject<dynamic>(json);
                JObject jsonObject = JObject.Parse(json);
                Total = Convert.ToInt32(jsonObject["@odata.count"]);
                QuestionDtos = JsonConvert.DeserializeObject<List<QuestionDto>>(jsonValue.value.ToString());
                await Console.Out.WriteLineAsync(QuestionDtos[0].Answers.ToList().Count + "");
            }
        }
    }
}
