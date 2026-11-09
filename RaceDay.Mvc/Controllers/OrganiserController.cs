
using Microsoft.AspNetCore.Mvc;
using RaceDay.Contracts;
using RaceDay.Mvc.Models;
using static AspNetCoreGeneratedDocument.CaptureResults;


namespace RaceDay.Web.Controllers
{
    public class OrganiserController : Controller
    {
        private readonly public class RaceDayApiClient _api; 

        public OrganiserController(public class RaceDayApiClient
 api)
        {
            _api = api;
        }

        private bool IsOrganiser()
        {
            // Uses your existing session/claims check
            return HttpContext.Session.GetString("Role") == "Organiser";
        }

        //for dashboard
        [HttpGet]
        public async Task<IActionResult> Dashboard()
        {
            if (!IsOrganiser()) return RedirectToAction("Login", "Auth");

            var eventsRes = await _api.SendAsync(HttpMethod.Get, "api/events/mine");
            var events = await _api.ReadAsync<List<EventDto>>(eventsRes) ?? new();

            var vm = new OrganiserDashboardVm();

            foreach (var ev in events.OrderBy(e => e.EventDate))
            {
                var enrolRes = await _api.SendAsync(HttpMethod.Get, $"api/events/{ev.Id}/enrolments");
                var enrolments = await _api.ReadAsync<List<OrganiserEnrolmentVm>>(enrolRes) ?? new();

                vm.Events.Add(new OrganiserEventSummaryVm
                {
                    EventId = ev.Id,
                    Name = ev.Name,
                    EventDate = ev.EventDate,
                    EnrolmentCount = enrolments.Count
                });
            }

            return View(vm);
        }

        //events
        [HttpGet]
        public async Task<IActionResult> Events()
        {
            if (!IsOrganiser()) return RedirectToAction("Login", "Auth");

            var response = await _api.SendAsync(HttpMethod.Get, "api/events/mine");
            var events = await _api.ReadAsync<List<EventDto>>(response) ?? new();
            return View(events);
        }

        //create events
        [HttpGet]
        public IActionResult CreateEvent()
        {
            if (!IsOrganiser()) return RedirectToAction("Login", "Auth");
            return View(new CreateEventRequest());
        }

        [HttpPost]
        public async Task<IActionResult> CreateEvent(CreateEventRequest model)
        {
            if (!IsOrganiser()) return RedirectToAction("Login", "Auth");
            if (!ModelState.IsValid) return View(model);

            var response = await _api.SendAsync(HttpMethod.Post, "api/events", model);
            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", await response.Content.ReadAsStringAsync());
                return View(model);
            }

            return RedirectToAction(nameof(Events));
        }

        [HttpGet]
        public async Task<IActionResult> EditEvent(int id)
        {
            if (!IsOrganiser()) return RedirectToAction("Login", "Auth");

            var response = await _api.SendAsync(HttpMethod.Get, $"api/events/{id}");
            if (!response.IsSuccessStatusCode) return RedirectToAction(nameof(Events));

            var ev = await _api.ReadAsync<EventDto>(response);
            if (ev == null) return RedirectToAction(nameof(Events));

            var model = new UpdateEventRequest
            {
                Id = ev.Id,
                Name = ev.Name,
                Description = ev.Description,
                EventDate = ev.EventDate,
                Location = ev.Location,
                DistanceKm = ev.DistanceKm,
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditEvent(UpdateEventRequest model)
        {
            if (!IsOrganiser()) return RedirectToAction("Login", "Auth");
            if (!ModelState.IsValid) return View(model);

            var response = await _api.SendAsync(HttpMethod.Put, $"api/events/{model.Id}", model);
            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", await response.Content.ReadAsStringAsync());
                return View(model);
            }

            return RedirectToAction(nameof(Events));
        }

        //categories correctred and completed reference to categories view
        [HttpPost]
        public async Task<IActionResult> AddCategory(int eventId, CreateCategoryRequest model)
        {
            if (!IsOrganiser()) return RedirectToAction("Login", "Auth");

            if (ModelState.IsValid)
            {
                var response = await _api.SendAsync(HttpMethod.Post, $"api/events/{eventId}/categories", model);// make correction

                if (!response.IsSuccessStatusCode)
                {
                    TempData["ErrorMessage"] = "Failed to add category. Please try again.";
                }
            }

            return RedirectToAction(nameof(Categories), new { eventId = eventId });
        }

//enrolment
        [HttpGet]
        public async Task<IActionResult> Enrolments(int eventId)
        {
            if (!IsOrganiser()) return RedirectToAction("Login", "Auth");

            var response = await _api.SendAsync(HttpMethod.Get, $"api/events/{eventId}/enrolments");
            var enrolments = await _api.ReadAsync<List<OrganiserEnrolmentVm>>(response) ?? new();

            ViewBag.EventId = eventId;
            return View(enrolments);
        }
        //results
        [HttpGet]
        public IActionResult CaptureResult(int enrolmentId, int eventId)
        {
            if (!IsOrganiser()) return RedirectToAction("Login", "Auth");

            ViewBag.EventId = eventId;
            return View(new CaptureResultVm { EnrolmentId = enrolmentId });
        }

        [HttpPost]
        public async Task<IActionResult> CaptureResult(CaptureResultVm model, int eventId)
        {
            if (!IsOrganiser()) return RedirectToAction("Login", "Auth");
            if (!ModelState.IsValid)
            {
                ViewBag.EventId = eventId;
                return View(model);
            }

            var response = await _api.SendAsync(HttpMethod.Post, "api/results", model);
            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", await response.Content.ReadAsStringAsync());
                ViewBag.EventId = eventId;
                return View(model);
            }

            return RedirectToAction(nameof(Enrolments), new { eventId });
        }

        // step 8
        [HttpPost]
        public async Task<IActionResult> CreateEvent(CreateEventRequest model, IFormFile? banner)
        {
            if (!IsOrganiser()) return RedirectToAction("Login", "Auth");
            if (!ModelState.IsValid) return View(model);

            var response = await _api.SendAsync(HttpMethod.Post, "api/events", model);
            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", await response.Content.ReadAsStringAsync());
                return View(model);
            }

            var createdEvent = await _api.ReadAsync<EventDto>(response);
            if (createdEvent != null && banner != null && banner.Length > 0)
            {
                await _api.UploadFileAsync($"api/events/{createdEvent.Id}/banner", banner);
            }

            return RedirectToAction(nameof(Events));
        }

        [HttpPost]
        public async Task<IActionResult> EditEvent(UpdateEventRequest model, IFormFile? banner)
        {
            if (!IsOrganiser()) return RedirectToAction("Login", "Auth");
            if (!ModelState.IsValid) return View(model);

            var response = await _api.SendAsync(HttpMethod.Put, $"api/events/{model.Id}", model);
            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", await response.Content.ReadAsStringAsync());
                return View(model);
            }

            if (banner != null && banner.Length > 0)
            {
                await _api.UploadFileAsync($"api/events/{model.Id}/banner", banner);
            }

            return RedirectToAction(nameof(Events));
        }

    }
}

