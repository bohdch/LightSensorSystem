using LightSensorSimulator.Models;
using LightSensorSimulator.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateDefaultBuilder();
builder.ConfigureAppConfiguration((_, config) =>
{
    config.AddEnvironmentVariables();
    config.AddJsonFile("appsettings.json", optional: true);
});

builder.ConfigureServices((hostContext, services) =>
{
    var deviceConfig = hostContext.Configuration.GetSection("DeviceConfiguration").Get<DeviceConfiguration>();
    
    // Replace placeholder in ServerUrl
    deviceConfig.ServerUrl = deviceConfig.ServerUrl.Replace("{DeviceId}", deviceConfig.DeviceId.ToString());

    services.AddSingleton(deviceConfig);
    services.AddHttpClient();
    services.AddSingleton<LightSensorService>();
});

var host = builder.Build();
host.Services.GetRequiredService<LightSensorService>();

await Task.Delay(-1);