using Aspire.Dashboard;
using Aspire.Dashboard.Model;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

namespace AspireDashboard.Extensions;

// stolen from: https://github.com/martinjt/aspire-app-extension
// tweaked to be how I like it

public static class AspireDashboardExtensions
{
    public static IServiceCollection AddAspireDashboard(this IServiceCollection services, 
        string aspireDashboardGuiUrl, int openTelemetryPort = 4317)
    {
        services.AddHostedService((sp) => {
            var applicationName = sp.GetRequiredService<IWebHostEnvironment>().ApplicationName;

            //ref: https://learn.microsoft.com/en-us/dotnet/aspire/fundamentals/dashboard/configuration?tabs=bash

            //NOTE: we tell OTLP to listen on "Any" (0.0.0.0) so remote machines can reach us
            Environment.SetEnvironmentVariable("DOTNET_DASHBOARD_OTLP_ENDPOINT_URL", $"http://0.0.0.0:{openTelemetryPort}");
            Environment.SetEnvironmentVariable("DOTNET_DASHBOARD_UNSECURED_ALLOW_ANONYMOUS", true.ToString());  //NOTE: Make this a parameter maybe?

            //NOTE: this thing seems to hijack the ASPNETCORE_URLS - which is a problem if your builder already has something running on that port, so watch out!
            var originalUrls = Environment.GetEnvironmentVariable("ASPNETCORE_URLS");
            Environment.SetEnvironmentVariable("ASPNETCORE_URLS", aspireDashboardGuiUrl);

            var application = new DashboardWebApplication(sp.GetRequiredService<ILogger<DashboardWebApplication>>(), serviceCollection =>
            {
                serviceCollection.AddSingleton<IDashboardViewModelService>(new DashboardViewModelService(applicationName));
            });

            //NOTE: put back the ASPNETCORE_URLS so your own host (or whatever) can use its own setting
            Environment.SetEnvironmentVariable("ASPNETCORE_URLS", originalUrls);    //restore the ASPNETCORE_URLS back for your "hosting" web app

            return application;
        });

        var oltpLocalUrl = new Uri($"http://localhost:{openTelemetryPort}"); // the Dashboard will log TO this address, which is ourselves (see DOTNET_DASHBOARD_OTLP_ENDPOINT_URL above)

        //NOTE: you are logging at yourself here!
        services.AddLogging(options =>
            options.AddOpenTelemetry(openTelemetry =>
            {
                openTelemetry.IncludeFormattedMessage = true;   //so the messages look nice in our web page GUI
                openTelemetry.AddOtlpExporter(o => o.Endpoint = oltpLocalUrl);
            }));
        services.AddOpenTelemetry()
            .WithTracing(tracerProviderBuilder => tracerProviderBuilder.AddOtlpExporter(o => o.Endpoint = oltpLocalUrl))
            .WithMetrics(metricProviderBuilder => metricProviderBuilder.AddOtlpExporter(o => o.Endpoint = oltpLocalUrl));
        // not sure if that gets too spammy or not, we'll see

        return services;
    }
}
