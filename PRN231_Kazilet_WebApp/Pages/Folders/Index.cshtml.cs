using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using PRN231_Kazilet_WebApp.Models.Dto;
using System.Net.Http.Headers;

namespace PRN231_Kazilet_WebApp.Pages.Folders
{
    public class IndexModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly string folderUrl = "https://localhost:7024/api/Folders/folders/";

        public IndexModel()
        {
            _httpClient = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            _httpClient.DefaultRequestHeaders.Accept.Add(contentType);
        }

        public List<FolderDto> Folders { get; set; } = new List<FolderDto>();

        public async Task OnGetAsync(int userId)
        {
            userId = 1;
            string requestUrl = $"{folderUrl}{userId}";

            HttpResponseMessage res = await _httpClient.GetAsync(requestUrl);
            if (res.IsSuccessStatusCode)
            {
                string json = await res.Content.ReadAsStringAsync();
                Folders = JsonConvert.DeserializeObject<List<FolderDto>>(json);
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Error loading folders from API.");
            }
        }
    }
}
