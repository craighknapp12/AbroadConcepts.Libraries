# ==AbroadConcepts.Configuration==

The ==AbroadConcepts.Configuration== package provides some functionality for working with Serilog.   

## Classes
{: style="color: Cyan;  opacity: 0.8;" }

```
public static class SetupExtension
{
    public static IConfigurationBuilder SetupConfiguration(this IConfigurationBuilder config);
    public static IConfigurationBuilder SetupLog(this IConfigurationBuilder config, IServiceCollection services);
    public static void ConfigureCrossSiteAccess(this WebApplication application);
}

```

# Usage
{: style="color: Lime; opacity: 0.80;" }
```

using AbroadConcepts.Configuration;

```
## Example Code C#

Adding to Program.cs:

{: style="color: Lime; opacity: 0.80;" }
```
builder.Configuration.SetupConfiguration().SetupLog(builder.Services);

```
Adding setting in the appsettings.json:

{: style="color: Lime; opacity: 0.80;" }
```
    "Serilog": {
        "Using": [],
        "MinimumLevel": {
            "Default": "Information",
            "Override": {
                "Microsoft": "Warning",
                "System": "Warning"
            }
        },
        "Enrich": [ "FromLogContext", "WithMachineName", "WithProcessId", "WithThreadId" ],
        "WriteTo": [
            {
                "Name": "Console"
            },
            {
                "Name": "File",
                "Args": {
                    "path": "/Logs/%ApplicationName%-.log",
                    "outputTemplate": "{Timestamp:G} {Level:u3} {Message}{NewLine:1}{Exception:1}",
                    "RollingInterval": "Day"
                }
            }
        ]
    }
```
