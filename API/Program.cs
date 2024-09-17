using Route = Core.Models.Route;
using System.Data;
using Core.Interfaces;
using Core.Models;
using Core.Services;
using DataAccess.Repositories;
using Microsoft.Data.SqlClient;

var builder = WebApplication.CreateBuilder(args);

// Services zone - BEGIN

builder.Services.AddTransient<IDbConnection>(_ =>
    new SqlConnection(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<ITrainRepository, TrainRepository>();
builder.Services.AddScoped<IStationRepository, StationRepository>();
builder.Services.AddScoped<IRouteRepository, RouteRepository>();
builder.Services.AddScoped<IService<Train>, TrainService>();
builder.Services.AddScoped<IService<Station>, StationService>();
builder.Services.AddScoped<IService<Route>, RouteService>();

builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(configuration =>
    {
        configuration.WithOrigins(builder.Configuration["allowedOrigins"]!).AllowAnyMethod()
            .AllowAnyHeader();
    });

    options.AddPolicy("free", configuration => { configuration.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod(); });
});

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

builder.Services.AddOutputCache();

builder.Services.AddHttpContextAccessor();

builder.Services.AddProblemDetails();

// Services zone - END

var app = builder.Build();

app.UseRouting();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();  // Ensure controller endpoints are mapped
});

// Middlewares zone - BEGIN

app.UseSwagger();
app.UseSwaggerUI();

app.UseStatusCodePages();

app.UseCors();

// Middlewares zone - END
app.Run();
