using RaceDay.API.Data;
using RaceDayDBContext;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
FallbackEndpointRouteBuilderExtensions
builder.Services.AddControllers();

//step 11: Add the DbContext service
builder.Services.AddDbContext<RaceDayDBContext>(options =>
    options.UseSqlServer("Server=.\\SQLEXPRESS; Database = RaceDayDbContext; Trusted_Connection = True; TrustServerCertificate = True;"));

builder.Services.AddOpenApi();

//  SECTION 23: Add Swagger Generator Services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "RaceDay API",
        Version = "v1"
    });
});

//step 14 - adding a time sesssion
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(45);//session is 45 mins
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "RaceDay API v1");// swagger added st23
        options.RoutePrefix = string.Empty; 
    });
}

app.UseHttpsRedirection();

app.UseRouting

app.UseAuthorization();

app.UseSession();//step 14

app.MapControllers();

app.Run();


