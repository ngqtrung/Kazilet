using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PRN231_Kazilet_WebApp.Models.Dto;
using System.Collections.Generic;
using System.Net.Http.Headers;

namespace PRN231_Kazilet_WebApp.Pages.Test
{
    public class TestModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly string questionUrl = "https://localhost:7024/odata/Question";

        [BindProperty]
        public List<QuestionDto> QuestionList { get; set; }
        public static List<QuestionDto> SelectedQuestionList { get; set; }
        public static int SelectedDuration { get; set; }
        //public static int SelectedNumOfQues { get; set; }
        public static int SelectedCourseId { get; set; }
        [BindProperty]
        public int Duration { get; set; }

        //[BindProperty]
        //public int NumOfQues { get; set; }
        [BindProperty]
        public int IsRetake { get; set; }

        public TestModel()
        {
            _httpClient = new HttpClient
            {
                DefaultRequestHeaders = { Accept = { new MediaTypeWithQualityHeaderValue("application/json") } }
            };
        }

        public async Task OnGet(int id, bool random, int duration, string selectedQuestions, int numOfQues, int isRetake)
        {
            
            IsRetake = isRetake;
            
            if(isRetake==1)
            {
                Duration = duration;
                SelectedDuration = duration;
                //NumOfQues = numOfQues;
                //SelectedNumOfQues = numOfQues;
                SelectedCourseId = id;
                List<int> listId = new List<int> ();
                if (selectedQuestions != null)
                {
                    listId = selectedQuestions.Split(',')
                                     .Select(int.Parse)
                                     .ToList();
                }
                
                string url2 = "";
                if (random == false)
                {
                    for (int i = 0; i < listId.Count; i++)
                    {
                        url2 += "ids=" + listId[i];
                        if (i < listId.Count - 1)
                        {
                            url2 += "&";
                        }
                    }
                }           
                var url = random
                ? $"{questionUrl}/GetRandom/{SelectedCourseId}/{numOfQues}"
                : $"{questionUrl}/GetQuestionsByIds?"+url2;
                var jsonStr = await _httpClient.GetStringAsync(url);
                JArray jsonArray = JArray.Parse(jsonStr);
                QuestionList = jsonArray.ToObject<List<QuestionDto>>();
                if (SelectedQuestionList != null)
                {
                    SelectedQuestionList.Clear();
                }
                
                SelectedQuestionList = QuestionList;
            }
            else
            {
                Duration = SelectedDuration;
                //NumOfQues = SelectedNumOfQues;
                QuestionList = SelectedQuestionList;
            }          
            TempData["QuestionList"] = JsonConvert.SerializeObject(QuestionList);
        }

        public async Task<IActionResult> OnPostSubmitAsync()
        {
            if (TempData["QuestionList"] != null)
            {
                QuestionList = JsonConvert.DeserializeObject<List<QuestionDto>>(TempData["QuestionList"].ToString());
            }

            if (QuestionList == null || !QuestionList.Any())
            {
                return RedirectToPage("/Error");
            }

            var selectedAnswers = Request.Form
                .Where(f => f.Key.StartsWith("question-"))
                .ToDictionary(f => f.Key, f => f.Value.ToString().Split(',').Select(int.Parse).ToList());

            int score = 0;
            List<IncorrectAnswerDto> incorrectAnswers = new List<IncorrectAnswerDto>();

            foreach (var question in QuestionList)
            {
                var correctAnswerIds = question.Answers
                    .Where(a => a.IsCorrect == true)
                    .Select(a => a.Id)
                    .ToList();

                List<int> userAnswerIds = null;

                if (selectedAnswers.TryGetValue($"question-{question.Id}", out var userAnswers))
                {
                    userAnswerIds = userAnswers;
                }

                if (userAnswerIds == null || !userAnswerIds.Any())
                {
                    userAnswerIds = new List<int>(); 

                    var availableAnswers = question.Answers.Select(a => a.Id).ToList();

                    incorrectAnswers.Add(new IncorrectAnswerDto
                    {
                        QuestionId = question.Id,
                        QuestionText = question.Content,
                        UserAnswers = userAnswerIds, 
                        CorrectAnswers = correctAnswerIds,
                        AnswerDtos = question.Answers
                    });
                }
                else
                {
                    bool isCorrect = correctAnswerIds.Count == userAnswerIds.Count &&
                                     !correctAnswerIds.Except(userAnswerIds).Any();

                    if (isCorrect)
                    {
                        score++;
                    }
                    else
                    {
                        incorrectAnswers.Add(new IncorrectAnswerDto
                        {
                            QuestionId = question.Id,
                            QuestionText = question.Content,
                            UserAnswers = userAnswerIds,
                            CorrectAnswers = correctAnswerIds,
                            AnswerDtos = question.Answers
                        });
                    }
                }
            }

            int totalQuestions = QuestionList.Count;
            int percentage = (int)Math.Round(((double)score / totalQuestions) * 100);

            TempData["TotalQuestion"] = totalQuestions;
            TempData["Score"] = score;
            TempData["Percentage"] = percentage;
            TempData["IncorrectAnswers"] = JsonConvert.SerializeObject(incorrectAnswers);

            return RedirectToPage("/TestScreen/Result", new
            {
                id = SelectedCourseId
            });
        }

    }
}
