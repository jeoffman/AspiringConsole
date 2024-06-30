using AspireDashboard.Extensions;

namespace Jkh.AspireDashboardHosting
{
    public class AspireDashboardHost
    {
        /// <summary>
        /// Be sure your hosting app has the Project Sdk (in your .csproj) set to "Microsoft.NET.Sdk.web" or the Web pages
        ///   in the Aspire Dashboard will look very weird (you are missing the AspireDashboardHosting.staticwebassets.runtime.json 
        ///   file in debug and the wwwroot in published)
        /// </summary>
        /// <param name="guiListenAddress">builds Aspire Dashboard's ASPNETCORE_URLS which had better be different than your app's ASPNETCORE_URLS!</param>
        /// <param name="openTelemetryListenPort">builds Aspire Dashboard's DOTNET_DASHBOARD_OTLP_ENDPOINT_URL</param>
        /// <param name="token"></param>
        /// <returns>the Async Task for the Dashboard - do not await this until after you cancel the token!</returns>
        public Task StartAspireDashboard(string guiListenAddress, int openTelemetryListenPort, CancellationToken token)
        {
            var builder = WebApplication.CreateBuilder();

            // note: when this thing starts up it is going to look for a file <Your.Application.exe>.staticwebassets.runtime.json 
            // in the "bin" (install) folder and if it doesn't find it then your Dashboard pages will look "weird"
            var applicationName = System.Reflection.Assembly.GetEntryAssembly().GetName().Name;

            //so IF you forget to put the .web on your console app's <Project Sdk=...> then this will fix you up in DEBUG
            //  BUT it doesn't translate to the "publish", so you really must be sure your console/winforms/whatever is an
            //  sdk=Microsoft.NET.Sdk.web to make this hack work at all
            //string hackRequiredStaticWebAssetsFilenameArgh = $"{applicationName}.staticwebassets.runtime.json";
            //if (!File.Exists(hackRequiredStaticWebAssetsFilenameArgh))
            //{
            //    //throw new InvalidOperationException("something about staticwebassets.runtime.json, is your console exe sdk set to Microsoft.NET.Sdk.Web?");
            //    File.Copy($"{nameof(AspireDashboardHosting)}.staticwebassets.runtime.json", hackRequiredStaticWebAssetsFilenameArgh);
            //}
            //NOTE: I was manually copying the AspireDashboardHosting.staticwebassets.runtime.json from its "bin" folder and renaming it to match the application name

            //NOTE: not sure if aspire dashboard even cares...
            builder.WebHost.UseSetting(WebHostDefaults.ApplicationKey, applicationName);

            builder.Services.AddAspireDashboard(guiListenAddress, openTelemetryListenPort);

            var app = builder.Build();

            return app.RunAsync(token);
        }
    }
}
