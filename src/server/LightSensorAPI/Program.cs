using AutoMapper;
using LightSensorAPI.Middlewares;
using LightSensorBLL.Automapper;
using LightSensorBLL.Interfaces;
using LightSensorBLL.Services;
using LightSensorDAL.Data;
using LightSensorDAL.Interfaces;
using LightSensorDAL.Repositories;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

builder.Host.UseSerilog((context, _, configuration) =>
{
    configuration.ReadFrom.Configuration(context.Configuration)
        .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
        .Enrich.FromLogContext();

    var format = context.Configuration["Logging:OutputTemplate"];
    
    if (bool.Parse(context.Configuration["Logging:EnableConsole"]))
    {
        configuration.WriteTo.Console(outputTemplate: format);
    }
    if (bool.Parse(context.Configuration["Logging:EnableFile"]))
    {
        configuration.WriteTo.File("Logfile.txt", rollingInterval: RollingInterval.Day, outputTemplate: format);
    }
    if (bool.Parse(context.Configuration["Logging:EnableHttp"]))
    {
        // TODO: Implement logging via http endpoint 
    }
});

builder.Services.AddDbContext<LightSensorDbContext>(opts =>
{
    opts.UseSqlServer(builder.Configuration.GetConnectionString("LightSensorDbConnection"));
});

builder.Services.AddSingleton(new MapperConfiguration(cfg =>
{
    cfg.AddProfile(new AutoMapperProfile());
}).CreateMapper());

builder.Services.AddTransient<ITelemetryRepository, TelemetryRepository>();
builder.Services.AddTransient<ITelemetryService, TelemetryService>();


var app = builder.Build();

Console.WriteLine("Starting LightSensor server...");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();
app.UseRouting();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();
