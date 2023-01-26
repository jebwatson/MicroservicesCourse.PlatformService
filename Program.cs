using PlatformService.Data;
using Microsoft.EntityFrameworkCore;
using PlatformService.SyncDataServices.Http;

var builder = WebApplication.CreateBuilder(args);

// Local types
if (builder.Environment.IsProduction())
{
    System.Console.WriteLine("--> Using MySql DB");
    builder.Services
      .AddDbContext<AppDbContext>(opt => opt.UseMySql(
            builder.Configuration.GetConnectionString("PlatformsConn"),
            new MySqlServerVersion(new Version(8, 0, 32))));
}
else
{
    System.Console.WriteLine("--> Using InMem DB");
    builder.Services.AddDbContext<AppDbContext>(opt => opt.UseInMemoryDatabase("InMem"));
}

builder.Services.AddScoped<IPlatformRepo, PlatformRepo>();
builder.Services.AddHttpClient<ICommandDataClient, HttpCommandDataClient>();

// Other
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

PrepDb.PrepPopulation(app, builder.Environment.IsProduction());

app.Run();
