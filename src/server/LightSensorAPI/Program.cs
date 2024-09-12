using System.Text;
using AutoMapper;
using LightSensorAPI.Middlewares;
using LightSensorBLL.Automapper;
using LightSensorBLL.Interfaces;
using LightSensorBLL.Services;
using LightSensorDAL.Data;
using LightSensorDAL.Interfaces;
using LightSensorDAL.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
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
});

builder.Services.AddDbContext<LightSensorDbContext>(opts =>
{
    opts.UseSqlServer(builder.Configuration.GetConnectionString("LightSensorDbConnection"));
});

builder.Services.AddSingleton(new MapperConfiguration(cfg =>
{
    cfg.AddProfile(new AutoMapperProfile());
}).CreateMapper());

builder.Services.AddTransient<IClientRepository, ClientRepository>();
builder.Services.AddTransient<IClientService, ClientService>();
builder.Services.AddTransient<IJwtService, JwtService>();
builder.Services.AddTransient<ITelemetryRepository, TelemetryRepository>();
builder.Services.AddTransient<ITelemetryService, TelemetryService>();

// Configure JWT authentication
builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration["Jwt:Key"])),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"]
    };
});

var app = builder.Build();

Console.WriteLine("Starting LightSensor server...");

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
});
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();
