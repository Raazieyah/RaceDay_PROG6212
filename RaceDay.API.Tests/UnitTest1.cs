using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore.InMemory
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System.Net;
using System.Net.Http.Json;
using Xunit;

public class RaceDayTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public RaceDayTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    // Helper to generate a clean client instance
    private HttpClient CreateNewClient() => _factory.CreateClient();

    #region PHASE 1: AUTHENTICATION TESTS

    [Fact]
    public async Task Register_ValidRegistration_Returns200Ok()
    {
        var client = CreateNewClient();
        var payload = new { Email = "testuser@raceday.com", Password = "SecurePassword123", Role = "Participant" };

        var response = await client.PostAsJsonAsync("/api/auth/register", payload);

        // Asserts: Valid registration succeeds
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Login_ValidCredentials_Returns200OkWithUserData()
    {
        var client = CreateNewClient();
        
        var payload = new { Email = "testuser@raceday.com", Password = "SecurePassword123" };

        var response = await client.PostAsJsonAsync("/api/auth/login", payload);

        
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Login_IncorrectCredentials_Returns401Unauthorized()
    {
        var client = CreateNewClient();
        var payload = new { Email = "registered@raceday.com", Password = "WrongPassword!!" };

        var response = await client.PostAsJsonAsync("/api/auth/login", payload);

        
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Register_IncorrectRoles_Returns400BadRequest()
    {
        var client = CreateNewClient();
        var payload = new { Email = "hacker@raceday.com", Password = "Password123", Role = "Administrator" }; // Invalid Role string

        var response = await client.PostAsJsonAsync("/api/auth/register", payload);

        
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    #endregion

    #region PHASE 2: ROLE ENFORCEMENT & SHIELDING TESTS

    [Fact]
    public async Task Profile_UnauthenticatedUser_Returns401Unauthorized()
    {
        var client = CreateNewClient(); 

        var response = await client.GetAsync("/api/profile/me");

        
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task EventCreation_ParticipantCreatesEvent_Returns403Forbidden()
    {
        var client = CreateNewClient();
        
        var loginPayload = new { Email = "participant@raceday.com", Password = "Password123" };
        await client.PostAsJsonAsync("/api/auth/login", loginPayload);

        var eventPayload = new { Name = "5K Run", Description = "Fun run", Date = DateTime.UtcNow, Location = "Park", Distance = 5.0, EventType = "Running" };
        var response = await client.PostAsJsonAsync("/api/events", eventPayload);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task EventCreation_OrganiserCreatesEvent_Returns21Created()
    {
        var client = CreateNewClient();

        var loginPayload = new { Email = "organiser@raceday.com", Password = "Password123" };
        await client.PostAsJsonAsync("/api/auth/login", loginPayload);

        var eventPayload = new { Name = "Marathon 2026", Description = "Full marathon", Date = DateTime.UtcNow.AddDays(10), Location = "City Center", Distance = 42.2, EventType = "Running" };
        var response = await client.PostAsJsonAsync("/api/events", eventPayload);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task Enrolment_ParticipantEnrolsInEvent_Returns200Ok()
    {
        var client = CreateNewClient();

        var loginPayload = new { Email = "participant@raceday.com", Password = "Password123" };
        await client.PostAsJsonAsync("/api/auth/login", loginPayload);

        var enrolmentPayload = new { EventId = 1, CategoryId = 2 };
        var response = await client.PostAsJsonAsync("/api/enrolments", enrolmentPayload);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    #endregion
}

