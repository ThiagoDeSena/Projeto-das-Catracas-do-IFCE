 using System;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using dotenv.net;
using Newtonsoft.Json.Linq;
using System.Text;
using System.Text.Json;

namespace CatracaControlClient.Services
{
    public class ComunicationApi
    {
        private static readonly HttpClient httpClient = new HttpClient();
        private const string baseUri = ""; //Url da API de produção do Intranet - Encontrada na documentação do projeto passada para o professor
        private const string usuario = ""; //usuario para acessar a API - Encontrada na documentação do projeto passada para o professor
        private const string senha= ""; //senha para acessar a API - Encontrada na documentação do projeto passada para o professor
        
        //inicializa as variaveis de backup
        public class BackupUsers
        {
            public string Clearcode {get;set;}
        }
        //verifica se o cartão é válido
        public async Task<bool> GetUser(string dadosTag)
        {
            try
            {
                var requestUri = new Uri($"{baseUri}tag?cartao={dadosTag}&usuario={usuario}&senha={senha}");

                
                HttpResponseMessage response = await httpClient.GetAsync(requestUri);



                if(response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    Console.WriteLine(responseBody);
                    var autorizacao = JObject.Parse(responseBody);
                    if((int)autorizacao["autorizacao"] == 1)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                    
                }
                else
                {
                    Console.WriteLine($"Erro ao validar usuario API");
                    string fileContent = File.ReadAllText("backup_users.json");
                    List<BackupUsers> backup_users = JsonConvert.DeserializeObject<List<BackupUsers>>(fileContent);
                    if(backup_users.Any(user => user.Clearcode == dadosTag))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exceção autorizar usuario {ex.Message}");
                string fileContent = File.ReadAllText("backup_users.json");
                List<BackupUsers> backup_users = JsonConvert.DeserializeObject<List<BackupUsers>>(fileContent);
                if(backup_users.Any(user => user.Clearcode == dadosTag))
                {
                    return true;
                }
                else
                {
                    return false;
                }
                
            }
        }
        //Posta os dados usuario que passou na catraca no banco
        public async Task<bool> PostUser(string dadosTag, string numCatraca, string tipoMovimentacao, string dataHora)
        {
            try
            {
                var queryString = $"inserirtransito?cartao={dadosTag}&numeroCatraca={numCatraca}&tipoMovimentacao={tipoMovimentacao}&data_hora={dataHora}&usuario={usuario}&senha={senha}";
                
                var requestUri = new Uri($"{baseUri}{queryString}");

                var response = await httpClient.PostAsync(requestUri, null);
                
                if(response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    
                    Console.WriteLine($"Erro ao postar usuario");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exceção postar usuario: {ex.Message}");
                return false;
            }
        }
        // realiza o backup dos cartões dos usuarios
        public async Task<List<BackupUsers>> GetBackupUsers()
        {
            try
            {
                string requestUri = ($"{baseUri}backup?usuario={usuario}&senha={senha}");

                HttpResponseMessage response = await httpClient.GetAsync(requestUri);

                if(response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    List<BackupUsers> backupUsers = JsonConvert.DeserializeObject<List<BackupUsers>>(responseBody);
                    File.WriteAllText("backup_users.json", JsonConvert.SerializeObject(backupUsers));
                    return backupUsers;                    
                }
                else
                {
                    Console.WriteLine($"Erro ao fazer backup dos usuarios");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exceção ao fazer backup dos usuarios: {ex.Message}");
                return null;
            }
        }
    }
}

