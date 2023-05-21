using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Automarket.DAL.Interfaces;
using Automarket.Domain.Entity;
using Automarket.Domain.Extensions;
using Automarket.Domain.Response;
using Automarket.Domain.ViewModels.Profile;
using Automarket.Domain.ViewModels.Recommendation;
using Automarket.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using TeleSharp.TL;
using TeleSharp.TL.Messages;
using TLSharp.Core;

namespace Automarket.Controllers
{
    public class ProfileController : Controller
    {
        private readonly IProfileService _profileService;
        
        private readonly IRecommendationService _recommendation;
        
        private readonly IPerfectHealthService _perfectHealth;

        private static string _lastUserProfile;
        

        public ProfileController(IProfileService profileService, IRecommendationService recommendation, IPerfectHealthService perfectHealth)
        {
            _profileService = profileService;
            _recommendation = recommendation;
            _perfectHealth = perfectHealth;
        }
        
        [Authorize(Roles = "Admin,Doctor")]
        [HttpGet]
        public IActionResult GetProfiles()
        {
            var response = _profileService.GetProfiles();
            if (response.StatusCode == Domain.Enum.StatusCode.OK)
            {
                return View(response.Data);   
            }
            return View("Error", $"{response.Description}");
        }

        [HttpPost]
        public async Task<IActionResult> Save(ProfileViewModel model)
        {
            ModelState.Remove("Id");
            ModelState.Remove("UserName");
            ModelState.Remove("Recommendations");
            ModelState.Remove("Temperature");
            ModelState.Remove("BloodPressure");
            ModelState.Remove("GUrineAnalysis");
            ModelState.Remove("GBloodTest");
            ModelState.Remove("Cholesterol");
            if (ModelState.IsValid)
            {
                var response = await _profileService.Save(model);
                if (response.StatusCode == Domain.Enum.StatusCode.OK)
                {
                    return Json(new { description = response.Description });
                }
            }
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
        
        [HttpPost]
        public async Task<IActionResult> SaveAnalyzes(ProfileViewModel model)
        {
            ModelState.Remove("Id");
            ModelState.Remove("UserName");
            ModelState.Remove("Age");
            ModelState.Remove("Address");
            ModelState.Remove("Recommendations");
            if (ModelState.IsValid)
            {
                var response = await _profileService.SaveAnalyzes(model);
                if (response.StatusCode == Domain.Enum.StatusCode.OK)
                {
                    return Json(new { description = Compare() });
                }
            }
            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        public string Compare()
        {
            var analyzes = _profileService.GetProfile(_lastUserProfile).Result.Data;
            var healths = _perfectHealth.GetHealths();
            int allHealths = 0;
            int chekHealths = 0;

            string[] profileBorder = analyzes.BloodPressure.Split("/");
            foreach (var health in healths.Data)
            {
                if (!(analyzes.Temperature >= health.Temperature - 0.3 && analyzes.Temperature <= health.Temperature + 0.3))
                {
                    allHealths++;
                    continue;
                }

                string[] healthBorder = health.BloodPressure.Split("/");
                if (!(int.Parse(profileBorder[0]) >= int.Parse(healthBorder[0]) - 10 && int.Parse(profileBorder[0]) <= int.Parse(healthBorder[0]) + 10) ||
                    !(int.Parse(profileBorder[1]) >= int.Parse(healthBorder[1]) - 5 && int.Parse(profileBorder[1]) <= int.Parse(healthBorder[1]) + 5))
                {
                    allHealths++;
                    continue;
                }
                
                if (analyzes.GUrineAnalysis != "Норма")
                {
                    allHealths++;
                    continue;
                }
                
                if (analyzes.GBloodTest != "Норма")
                {
                    allHealths++;
                    continue;
                }
                
                if (!(analyzes.Cholesterol >= health.Cholesterol - 3 && analyzes.Cholesterol <= health.Cholesterol + 3))
                {
                    allHealths++;
                    continue;
                }

                allHealths++;
                chekHealths++;
            }

            string text = "";

            if (allHealths != chekHealths)
            {
                string recommed = string.Format("Ваши анализы прошлли проверку {0} из {1} из нашей базы данных, вам выслана рекомендация (см. раздел рекомендаций)", chekHealths, allHealths);
                if (analyzes.Temperature>= 37)
                    text =
                        "Если есть другие симптомы, такие как кашель, боль в горле или затрудненное дыхание, то следует немедленно обратиться к врачу.\n";
                
                if (int.Parse(profileBorder[0]) > 120)
                    text = text +
                           "При высоком давлении, следует обратиться к врачу для получения консультации и лечения. Можно порекомендовать изменение образа жизни, такие как здоровое питание, физическая активность и снижение стресса. В некоторых случаях может потребоваться лекарственное лечение.\t";
                
                
                if (int.Parse(profileBorder[1]) < 80)
                    text = text +
                           "При низком давлении в первую очередь следует обратить внимание на свой образ жизни. Важно правильно питаться, употреблять достаточное количество воды, избегать переутомления и стрессовых ситуаций, регулярно заниматься физической активностью. Если изменение образа жизни не помогает, то следует обратиться к врачу для получения консультации и назначения лечения. В некоторых случаях может потребоваться прием лекарственных препаратов для нормализации давления.\t";

                if (analyzes.GUrineAnalysis != "Норма")
                    text = text + "Возможно, потребуется провести дополнительные анализы мочи, крови или другие исследования для выявления причины отклонений от нормы. Лечение будет зависеть от выявленной причины и может включать в себя прием лекарственных препаратов, изменение образа жизни или другие методы.\t";

                if (analyzes.GBloodTest != "Норма")
                    text = text + "Возможно, потребуется провести дополнительные анализы крови, ультразвуковое исследование или другие методы для выявления причины отклонений от нормы.\t";

                if (analyzes.Cholesterol > 5.2)
                    text = text + "Есть риск развития сердечно-сосудистых заболеваний. В таком случае необходимо обратиться к врачу для проведения дополнительных исследований и получения рекомендаций по коррекции уровня холестерина.\t";

                RecommendationViewModel recommendation = new RecommendationViewModel()
                {
                    AuthorName = "MedicalAI",
                    PatientName = _lastUserProfile,
                    Description = text,
                    DateCreate = DateTime.Now.ToString()
                };
                _recommendation.Create(recommendation);
                return recommed;
            }
            else
            {
                return  "Ваши анализы соответствуют норме, вы здоровы";
            }
        }

        [HttpGet]
        public async Task<IActionResult> Detail(string userName)
        {
            
            // var client = new TelegramClient(22373081, "f7f72fce3ca7da530daaf6cca3db280e");
            // await client.ConnectAsync();
            //
            // // var result = await client.GetContactsAsync();
            //
            // var dialogsResult = (TLDialogs) await client.GetUserDialogsAsync();
            // var users = dialogsResult.Users.OfType<TLUser>();
            // var bot = users.FirstOrDefault(x => x.Username == "chatgpt_tgm_bot");
            // var peer = new TLInputPeerUser { UserId = bot.Id, AccessHash = bot.AccessHash.Value };
            // var result = await client.SendMessageAsync(peer, "message");
            
            // var user = result.Users
            //     .OfType<TLUser>()
            //     .FirstOrDefault(x => x.Username == "GPT4Telegrambot");
            //
            // if (user == null)
            // {
            //     throw new System.Exception("Number was not found in Contacts List of user: " + "79632354724");
            // }
            //
            // await client.SendTypingAsync(new TLInputPeerUser() { UserId = user.Id });
            // Thread.Sleep(3000);
            // await client.SendMessageAsync(new TLInputPeerUser() { UserId = user.Id }, "TEST");

            // var hash = await client.SendCodeRequestAsync("79632361218");
            // var code = "68150"; // you can change code in debugger
            // var user = await client.MakeAuthAsync("79632361218", hash, code);
            //
            //get user dialogs
            // var dialogs = (TLDialogs) await client.GetUserDialogsAsync();
            //
            // //find channel by title
            // var chat = dialogs.Chats
            //     .OfType<TLChannel>()
            //     .FirstOrDefault(c => c.Title == "@GPT4Telegrambot");
            //
            // //send message
            // await client.SendMessageAsync(new TLInputPeerChannel() { ChannelId = chat.Id, AccessHash = chat.AccessHash.Value }, "TEST MSG");
            
            // var userName = User.Identity.Name;
            var response = await _profileService.GetProfile(userName);
            if (response.StatusCode == Domain.Enum.StatusCode.OK)
            {
                _lastUserProfile = userName;
                return View(response.Data);   
            }

            return View();
        }

        [HttpGet]
        public IActionResult SaveRecommendation()
        {
            ViewBag.Message = _lastUserProfile;
            return PartialView();
        }


        [HttpPost]
        public async Task<IActionResult> SaveRecommendation(RecommendationViewModel model)
        {
            if (ModelState.IsValid)
            {
                var response = await _recommendation.Create(model);
                if (response.StatusCode == Domain.Enum.StatusCode.OK)
                {
                    return Json(new { description = response.Description });
                }
        
                return BadRequest(new { errorMessage = response.Description });
            }
        
            var errorMessage = ModelState.Values
                .SelectMany(v => v.Errors.Select(x => x.ErrorMessage)).ToList().Join();
            return StatusCode(StatusCodes.Status500InternalServerError, new { errorMessage });
        }
        
        public async Task<IActionResult> DeleteRecommendation(long id)
        {
            var response = await _recommendation.DeleteRecommendation(id);
            if (response.StatusCode == Domain.Enum.StatusCode.OK)
            {
                return RedirectToAction("Index", "Home");
                return RedirectToAction("Detail", "Profile", id);
            }
            return RedirectToAction("Index", "Home");
        }
    }
}