using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace CatracaControlClient
{
    public class Program
    {
        public static void Main(string[] args)
        {
            System.Console.WriteLine("Programa inicializado");
            
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
