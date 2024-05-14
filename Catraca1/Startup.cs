using CatracaControlClient.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;

namespace CatracaControlClient
{
    public class Startup
    {
        private static RfidReader rfidReader;

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            // Inicializa o cliente SignalR e a leitura dos cartões RFID
            Task.Run(() => SignalRClient.Connect());
            rfidReader = new RfidReader();
            Task.Run(() => rfidReader.Read("Entrada", "/dev/ttyS0"));
            Task.Run(() => rfidReader.Read("Saida", "/dev/ttyUSB0"));
        }
    }
}
