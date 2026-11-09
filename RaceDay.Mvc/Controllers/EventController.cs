using Microsoft.AspNetCore.Mvc;
using RaceDay.Contracts; //6.2

public async Task<IActionResult> Events()
{
    if (!IsOrganiser()) return RedirectToAction("Login", "Auth");
    var response = await _api.SendAsync(HttpMethod.Get, "api/events/mine");
    var events = await _api.ReadAsync<List<EventDto>>(response) ?? new();
    return View(events);
}
[HttpGet]
public IActionResult CreateEvent()
{
    if (!IsOrganiser()) return RedirectToAction("Login", "Auth");
    return View();
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
