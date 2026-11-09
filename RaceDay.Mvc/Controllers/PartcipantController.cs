namespace RaceDay.Mvc_.Controllers //7.2
using global::RaceDay.Contracts;
using global::RaceDay.Mvc_.Models;
using Microsoft.AspNetCore.Mvc;
using RaceDay.Mvc.Models;
using System.Net.Http.Headers;

namespace RaceDay.Mvc.Controllers
{
    public class ParticipantController : Controller
    {
        private readonly RaceDayApiClient _api; 

        public ParticipantController(RaceDayApiClient api)
        {
            _api = api;
        }

        private bool IsParticipant()
        {
            return HttpContext.Session.GetString("Role") == "Participant";
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? type, string? search)
        {
            if (!IsParticipant()) return RedirectToAction("Login", "Auth");

            var response = await _api.SendAsync(HttpMethod.Get, "api/events");
            var events = await _api.ReadAsync<List<EventDto>>(response) ?? new();

            events = events.Where(e => e.EventDate >= DateTime.Today).ToList();

            if (!string.IsNullOrWhiteSpace(type))
                events = events.Where(e => e.EventType == type).ToList();

            if (!string.IsNullOrWhiteSpace(search))
                events = events.Where(e =>
                    e.Name.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    e.Location.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();

            ViewBag.SelectedType = type;
            ViewBag.SearchQuery = search;

            return View(events);
        }
      
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            if (!IsParticipant()) return RedirectToAction("Login", "Auth");

            var eventRes = await _api.SendAsync(HttpMethod.Get, $"api/events/{id}");
            if (!eventRes.IsSuccessStatusCode) return RedirectToAction(nameof(Index));

            var ev = await _api.ReadAsync<EventDto>(eventRes);
            if (ev == null) return RedirectToAction(nameof(Index));

            var catRes = await _api.SendAsync(HttpMethod.Get, $"api/events/{id}/categories");
            var categories = await _api.ReadAsync<List<CategoryDto>>(catRes) ?? new();

            return View(new EventDetailsVm
            {
                Event = ev,
                Categories = categories
            });
        }
        [HttpPost]
        public async Task<IActionResult> EnterEvent(int eventId, int categoryId)
        {
            if (!IsParticipant()) return RedirectToAction("Login", "Auth");

            var request = new CreateEnrolmentRequest
            {
                EventId = eventId,
                CategoryId = categoryId
            };

            var response = await _api.SendAsync(HttpMethod.Post, "api/enrolments", request);

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = await response.Content.ReadAsStringAsync();
                return RedirectToAction("Details", "Participant", new { id = eventId });
            }

            TempData["Success"] = "You have entered the event successfully.";
            return RedirectToAction(nameof(MyEvents));
        }
        
        [HttpGet]
        public async Task<IActionResult> MyEvents()
        {
            if (!IsParticipant()) return RedirectToAction("Login", "Auth");

            var response = await _api.SendAsync(HttpMethod.Get, "api/enrolments/mine");
            var enrolments = await _api.ReadAsync<List<ParticipantEnrolmentDto>>(response) ?? new();

            return View(enrolments);
        }
        
        [HttpGet]
        public async Task<IActionResult> MyResults()
        {
            if (!IsParticipant()) return RedirectToAction("Login", "Auth");

            var response = await _api.SendAsync(HttpMethod.Get, "api/results/mine");
            var results = await _api.ReadAsync<List<ResultDto>>(response) ?? new();

            return View(results);
        }

        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            if (!IsParticipant()) return RedirectToAction("Login", "Auth");

            var response = await _api.SendAsync(HttpMethod.Get, "api/profile");
            var profile = await _api.ReadAsync<ParticipantProfileVm>(response) ?? new();

            return View(profile);
        }
        [HttpPost]
        public async Task<IActionResult> Profile(ParticipantProfileVm model)
        {
            if (!IsParticipant()) return RedirectToAction("Login", "Auth");
            if (!ModelState.IsValid) return View(model);

            var updatePayload = new
            {
                model.FullName,
                model.Email,
                model.PhoneNumber
            };

            var putRes = await _api.SendAsync(HttpMethod.Put, "api/profile", updatePayload);
            if (!putRes.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", await putRes.Content.ReadAsStringAsync());
                return View(model);
            }
            
            if (model.ProfilePicture != null && model.ProfilePicture.Length > 0)
            {
                using var formData = new MultipartFormDataContent();
                using var fileStream = model.ProfilePicture.OpenReadStream();
                using var fileContent = new StreamContent(fileStream);
                fileContent.Headers.ContentType = new MediaTypeHeaderValue(model.ProfilePicture.ContentType);

                formData.Add(fileContent, "file", model.ProfilePicture.FileName);

                var picRes = await _api.SendMultipartAsync("api/profile/picture", formData);
                if (!picRes.IsSuccessStatusCode)
                {
                    ModelState.AddModelError("", "Profile saved, but picture upload failed: " + await picRes.Content.ReadAsStringAsync());
                    return View(model);
                }
            }

            TempData["Success"] = "Profile updated successfully.";
            return RedirectToAction(nameof(Profile));
        }
        //step8.7
        [HttpPost]
        public async Task<IActionResult> UploadProfilePicture(IFormFile? profileImage)
        {
            if (!IsParticipant()) return RedirectToAction("Login", "Auth");

            if (profileImage != null && profileImage.Length > 0)
            {
                var response = await _api.UploadFileAsync("api/users/profile-image", profileImage);
                if (!response.IsSuccessStatusCode)
                {
                    TempData["Error"] = "Failed to upload profile picture.";
                }
                else
                {
                    TempData["Success"] = "Profile picture updated.";
                }
            }

            return RedirectToAction(nameof(Profile));
        }

    }
}