using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Extensions.Logging;

namespace AbroadConcepts.Configuration;
public static class SetupExtension
{
    private static string ApplicationName { get; set; } = string.Empty;

    public static IConfigurationBuilder SetupConfiguration(this IConfigurationBuilder config)
    {
        ApplicationName = Assembly.GetEntryAssembly()?.GetName().Name!;
        Environment.SetEnvironmentVariable("ApplicationName", ApplicationName);

        var configPath = Environment.ExpandEnvironmentVariables("%CONFIG_PATH%");
        if (configPath != "%CONFIG_PATH%")
        {
            config.AddJsonFile($"{configPath}{Path.DirectorySeparatorChar}{ApplicationName}.appsettings.json", true, true);
        }
        return config;
    }

    public static IConfigurationBuilder SetupSeriLog(this IConfigurationBuilder config, IServiceCollection services)
    {
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(config.Build())
            .CreateLogger();
        Log.Information("Starting {ApplicationName}", ApplicationName);
        Log.Information("Running from {Directory}", Directory.GetCurrentDirectory());
        services.AddSerilog();
        return config;
    }
}
