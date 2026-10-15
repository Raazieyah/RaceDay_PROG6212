using RaceDay.API.Data;
using RaceDayDBContext;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddOpenApi();

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
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseSession();//step 14

app.MapControllers();

app.Run();
//step 11

// Step 11: Add the DbContext service
builder.Services.AddDbContext<RaceDayDbContext>(options =>
    options.UseSqlServer("Server=.\\SQLEXPRESS; Database = RaceDayDbContext; Trusted_Connection = True; TrustServerCertificate = True;"));

/*DefaultConnection": "Server=.\
\SQLEXPRESS; Database = RaceDayDBContext; Trusted_Connection = True; TrustServerCertificate = True; "*/