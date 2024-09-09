using AutoMapper;
using LightSensorBLL.Automapper;
using LightSensorBLL.Interfaces;
using LightSensorBLL.Services;
using LightSensorDAL.Data;
using LightSensorDAL.Interfaces;
using LightSensorDAL.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

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

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();
