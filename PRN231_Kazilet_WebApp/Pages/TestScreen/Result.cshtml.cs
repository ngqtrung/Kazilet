using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using PRN231_Kazilet_WebApp.Models.Dto;

namespace PRN231_Kazilet_WebApp.Pages.TestScreen
{
    public class ResultModel : PageModel
    {
        [BindProperty]
        public int Score { get; set; }

        [BindProperty]
        public int Percentage { get; set; }

        [BindProperty]
        public List<IncorrectAnswerDto> IncorrectAnswers { get; set; } = new List<IncorrectAnswerDto>();

        [BindProperty]
        public List<QuestionDto> UnansweredQuestions { get; set; } = new List<QuestionDto>();
        [BindProperty]
        public int TotalQuestion { get; set; }
        [BindProperty]
        public int Id { get; set; }
        public void OnGet(int id)
        {
            Id= id;
            if (TempData["TotalQuestion"] != null)
            {
                TotalQuestion = (int)TempData["TotalQuestion"];
            }
            if (TempData["Score"] != null)
            {
                Score = (int)TempData["Score"];
            }

            if (TempData["Percentage"] != null)
            {
                Percentage = (int)TempData["Percentage"];
            }

            if (TempData["IncorrectAnswers"] != null)
            {
                IncorrectAnswers = JsonConvert.DeserializeObject<List<IncorrectAnswerDto>>(TempData["IncorrectAnswers"].ToString());
            }
            else
            {
                IncorrectAnswers = new List<IncorrectAnswerDto>(); 
            }

            if (TempData["UnansweredQuestions"] != null)
            {
                UnansweredQuestions = JsonConvert.DeserializeObject<List<QuestionDto>>(TempData["UnansweredQuestions"].ToString());
            }
            else
            {
                UnansweredQuestions = new List<QuestionDto>();
            }
        }
    }
}
