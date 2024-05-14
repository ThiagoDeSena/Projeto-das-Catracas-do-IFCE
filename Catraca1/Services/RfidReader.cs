using System;
using System.Device.Gpio;
using System.IO.Ports;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;


namespace CatracaControlClient.Services
{
    public class RfidReader
    {
        private static readonly HttpClient httpClient = new HttpClient();
        private static PinManager pinManager = new PinManager();
        private static ComunicationApi comunicationApi = new ComunicationApi();
        private static Timer timer;
        private static string numCatraca = "1";

        public RfidReader()
        {
            DateTime now = DateTime.Now;
            DateTime scheduledTime = new DateTime(now.Year, now.Month, now.Day, 5, 0, 0);
            if (scheduledTime < now)
            {
                scheduledTime = scheduledTime.AddDays(1);
            }

            TimeSpan initialDelay = scheduledTime - now;

            timer = new Timer(AtualizaBackupUsers, null, initialDelay, TimeSpan.FromDays(1));
        }


        //leitura cartão
        public async void Read(string tipoLeitor, string portPath)
        {
            //declara o porta serial
            SerialPort usb1 = new SerialPort(portPath);
            usb1.Encoding = Encoding.UTF8;
            usb1.BaudRate = 9600;
            usb1.Handshake = Handshake.None;
            usb1.ReadTimeout = 1;
            usb1.Open();

            while (true)
            {
                try
                {
                    usb1.DiscardInBuffer(); //limpa a entrada da porta serial
                    int usb_read_byte = usb1.ReadByte();
                    int[] DebugW = new int[10];
                    //leitura e conversão para o padrão wiegand do cartao
                    if (usb_read_byte == 0x02)
                    {
                        for (int counter = 0; counter < 10; counter++)
                        {
                            usb_read_byte = usb1.ReadByte();
                            if (usb_read_byte <= 57)
                            {
                                DebugW[counter] = usb_read_byte - 48;
                            }
                            else
                            {
                                DebugW[counter] = usb_read_byte - 55;
                            }
                        }
                        int facility = DebugW[4] * 16 + DebugW[5];
                        string facilityStr = facility.ToString();
                        while (facilityStr.Length < 3)
                        {
                            facilityStr = '0' + facilityStr;
                        }
                        int cardCode = DebugW[6] * 16 * 16 * 16 + DebugW[7] * 16 * 16 + DebugW[8] * 16 + DebugW[9];
                        string cardCodeStr = cardCode.ToString();
                        while (cardCodeStr.Length < 5)
                        {
                            cardCodeStr = '0' + cardCodeStr;
                        }
                        string dadosTagSaida = facilityStr + cardCodeStr;
                        Console.WriteLine($"{tipoLeitor}: {dadosTagSaida}");
                        
                        
                        // Chamar a API para validar entrada
                        bool autorizacao = await comunicationApi.GetUser(dadosTagSaida);

                        if (autorizacao)
                        {
                            //verifica se é entrada ou saida
                            if (tipoLeitor == "Entrada")
                            {
                                pinManager.EntranceAuthorizedState();
                                int timeoutMilliseconds = 5000;
                                bool esperaEntrada = await pinManager.WaitReedSwitchEntrance(timeoutMilliseconds);
                                //verifica se o usuario passou na catraca
                                if (esperaEntrada)
                                {
                                    pinManager.EntranceInitialState();
                                    Console.WriteLine("Entrada Passou");
                                    bool postResult = await comunicationApi.PostUser(dadosTagSaida, numCatraca, "1", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
                                    if (postResult)
                                    {
                                        Console.WriteLine("Dados entrada enviados com sucesso!");
                                    }
                                    else
                                    {
                                        Console.WriteLine("Erro ao enviar dados de entrada!");
                                    }
                                }
                                else
                                {
                                    pinManager.EntranceInitialState();
                                    Console.WriteLine("Entrada Não Passou");
                                }
                            }
                            if (tipoLeitor == "Saida")
                            {
                                pinManager.ExitAuthorizedState();
                                int timeoutMilliseconds = 5000;
                                bool esperaSaida = await pinManager.WaitReedSwitchExit(timeoutMilliseconds);
                                //verifica se o usuario passou na catraca
                                if (esperaSaida)
                                {
                                    pinManager.ExitInitialState();
                                    Console.WriteLine("Saida Passou");
                                    bool postResult = await comunicationApi.PostUser(dadosTagSaida, numCatraca, "2", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
                                    if (postResult)
                                    {
                                        Console.WriteLine("Dados saida enviados com sucesso!");
                                    }
                                    else
                                    {
                                        Console.WriteLine("Erro ao enviar dados de saida!");
                                    }

                                }
                                else
                                {
                                    pinManager.ExitInitialState();
                                    Console.WriteLine("Saida Não Passou");
                                }
                            }
                            Console.WriteLine($"API chamada com sucesso {tipoLeitor} resultado: {autorizacao}");
                            Thread.Sleep(2000);
                            Console.WriteLine("Fechou catraca");
                        }
                        //caso não seja autorizado
                        else
                        {
                            //verifica se é entrada ou saida
                            if (tipoLeitor == "Entrada")
                            {
                                pinManager.EntranceUnauthorizedState();
                                Console.WriteLine("Entrada Não autorizada");
                                try
                                {
                                    var backupUsers = await comunicationApi.GetBackupUsers();
                                    if (backupUsers != null)
                                    {
                                        Console.WriteLine("Usuarios atualizados");
                                    }
                                    else
                                    {
                                        Console.WriteLine("Erro ao atualizar os usuarios");
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine($"Excecao ao atualizar usuarios: {ex.Message}");
                                }
                            }
                            if (tipoLeitor == "Saida")
                            {
                                //pinManager.ExitAuthorizedState();
                                pinManager.ExitUnauthorizedState();
                                Console.WriteLine("Saida Não autorizada");
                                try
                                {
                                    var backupUsers = await comunicationApi.GetBackupUsers();
                                    if (backupUsers != null)
                                    {
                                        Console.WriteLine("Usuarios atualizados");
                                    }
                                    else
                                    {
                                        Console.WriteLine("Erro ao atualizar os usuarios");
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine($"Excecao ao atualizar usuarios: {ex.Message}");
                                }

                            }
                            Thread.Sleep(2000);
                            Console.WriteLine("Não autorizado");
                        }
                    }
                }
                catch (TimeoutException)
                {
                }

            }
        }
        private async void AtualizaBackupUsers(object state)
        {
            try
            {
                var backupUsers = await comunicationApi.GetBackupUsers();
                if (backupUsers != null)
                {
                    Console.WriteLine("Usuarios atualizados");
                }
                else
                {
                    Console.WriteLine("Erro ao atualizar os usuarios");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Excecao ao atualizar usuarios: {ex.Message}");
            }
        }
    }
}
