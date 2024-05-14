using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Threading.Tasks;
using System.Device.Gpio;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;


namespace CatracaControlClient.Services
{
    public static class SignalRClient
    {
        public static async Task Connect()
        {
            string token = "";
            using var httpClient = new HttpClient();
            var requestBody = new {
                identificacao = "", //Insira a sua matricula do IFCE aqui
                senha = "" //Insira a sua senha do qacademico aqui
            };

            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var json = JsonConvert.SerializeObject(requestBody);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync("https://api-h-2.intranet.maracanau.ifce.edu.br/auth/login", content);

            if (response.IsSuccessStatusCode)
            {
                var responseData = await response.Content.ReadAsStringAsync();
                dynamic result = JsonConvert.DeserializeObject(responseData);
                token = result.data.token;
            }
            else
            {
                Console.WriteLine("Erro ao fazer a solicitação: " + response.StatusCode);
            }
            string hubUrl = "wss://api.intranet-h.maracanau.ifce.edu.br/hubs/transito-catraca"; //servidor hub
            var connection = new HubConnectionBuilder()
                .WithUrl(hubUrl, options => {
                    options.AccessTokenProvider = () => Task.FromResult(token);
                })
                .Build();

            connection.On("UltimosTransitos", () =>
            {
                Console.WriteLine("transitos");
                // teste se recebe algo
            });

            connection.On("TransitoAutorizado", () =>
            {
                Console.WriteLine("Saída liberada");
                // lógica para liberar a saida da catraca aqui
            });
            connection.On("TransitoNaoAutorizado", () =>
            {
                Console.WriteLine("Saída liberada");
                // lógica para liberar a saida da catraca aqui
            });

            await connection.StartAsync();

            Console.WriteLine("Cliente conectado ao servidor Hub");
        }
    }
}
