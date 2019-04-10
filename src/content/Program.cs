using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace AspNetCoreTemplateExtended
{
  public class Program
  {
    public static void Main(string[] args)
    {
      CreateWebHostBuilder(args).Build().Run();
    }

    public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
      WebHost.CreateDefaultBuilder(args)
        .UseStartup<Startup>()
        .ConfigureServices(services => services.AddAutofac())
        .ConfigureAppConfiguration((hostingContext, config) =>
        {
          config.AddJsonFile("AppSettings/appsettings.json", false, true);
          config.AddJsonFile("AppSettings/appsettings." + hostingContext.HostingEnvironment.EnvironmentName + ".json", true, true);
        })
        .UseUrls("http://localhost:5000", "https://localhost:5001")
        .CaptureStartupErrors(true);
  }
}