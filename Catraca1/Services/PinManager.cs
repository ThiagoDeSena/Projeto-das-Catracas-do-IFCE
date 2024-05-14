using System.Device.Gpio;
using System.Threading;
using System.Threading.Tasks;

namespace CatracaControlClient.Services
{
    public class PinManager
    {
        //inicializa variaveis dos pinos
        private GpioController gpio;
        private const int LED_VERDE_RELE_SAIDA = 21;
        private const int LED_VERDE_RELE_ENTRADA = 20;
        private const int LED_RGB_AZUL_ENTRADA = 22;
        private const int LED_RGB_AZUL_SAIDA = 27;
        private const int LED_RGB_VERMELHO_ENTRADA = 6;
        private const int LED_RGB_VERMELHO_SAIDA = 5;
        private const int LED_RGB_VERDE_ENTRADA = 19;
        private const int LED_RGB_VERDE_SAIDA = 13;
        private const int REED_SWITCH_ENTRADA = 23;
        private const int REED_SWITCH_SAIDA = 24;
        private const int RELE_ENTRADA = 12;
        private const int RELE_SAIDA = 16;

        public PinManager()
        {
            gpio = new GpioController();
            InitializeGpio();
        }
        //valores iniciais dos pinos
        public void InitializeGpio()
        {
            
            gpio.OpenPin(REED_SWITCH_ENTRADA, PinMode.InputPullUp);
            gpio.OpenPin(REED_SWITCH_SAIDA, PinMode.InputPullUp);
            gpio.OpenPin(RELE_SAIDA, PinMode.Output);
            gpio.OpenPin(RELE_ENTRADA, PinMode.Output);
            gpio.OpenPin(LED_VERDE_RELE_SAIDA, PinMode.Output);
            gpio.OpenPin(LED_VERDE_RELE_ENTRADA, PinMode.Output);
            gpio.OpenPin(LED_RGB_AZUL_ENTRADA, PinMode.Output);
            gpio.OpenPin(LED_RGB_AZUL_SAIDA, PinMode.Output);
            gpio.OpenPin(LED_RGB_VERDE_ENTRADA, PinMode.Output);
            gpio.OpenPin(LED_RGB_VERDE_SAIDA, PinMode.Output);
            gpio.OpenPin(LED_RGB_VERMELHO_ENTRADA, PinMode.Output);
            gpio.OpenPin(LED_RGB_VERMELHO_SAIDA, PinMode.Output);
            
            //Estado inicial
            gpio.Write(RELE_ENTRADA, PinValue.High);
            gpio.Write(RELE_SAIDA, PinValue.High);
            gpio.Write(LED_VERDE_RELE_ENTRADA, PinValue.High);
            gpio.Write(LED_VERDE_RELE_SAIDA, PinValue.High);
            gpio.Write(LED_RGB_AZUL_ENTRADA, PinValue.High);
            gpio.Write(LED_RGB_AZUL_SAIDA, PinValue.High);
            gpio.Write(LED_RGB_VERMELHO_ENTRADA, PinValue.Low);
            gpio.Write(LED_RGB_VERMELHO_SAIDA, PinValue.Low);
            gpio.Write(LED_RGB_VERDE_ENTRADA, PinValue.Low);
            gpio.Write(LED_RGB_VERDE_SAIDA, PinValue.Low);
        }
        //Define os pinos para o estado inicial caso seja feito leitura de cartão para saida
        public void ExitInitialState()
        {
            gpio.Write(RELE_SAIDA, PinValue.High);
            gpio.Write(LED_VERDE_RELE_SAIDA, PinValue.High);
            gpio.Write(LED_RGB_AZUL_SAIDA, PinValue.High);
            gpio.Write(LED_RGB_VERMELHO_SAIDA, PinValue.Low);
            gpio.Write(LED_RGB_VERDE_SAIDA, PinValue.Low);
        }
        //Define os pinos para o estado inicial caso seja feito leitura de cartão para entrada
        public void EntranceInitialState()
        {
            gpio.Write(RELE_ENTRADA, PinValue.High);
            gpio.Write(LED_VERDE_RELE_ENTRADA, PinValue.High);
            gpio.Write(LED_RGB_AZUL_ENTRADA, PinValue.High);
            gpio.Write(LED_RGB_VERMELHO_ENTRADA, PinValue.Low);
            gpio.Write(LED_RGB_VERDE_ENTRADA, PinValue.Low);
        }
        //Define os pinos para o estado de saida autorizada
        public void ExitAuthorizedState()
        {
            gpio.Write(RELE_SAIDA, PinValue.Low);
            gpio.Write(LED_VERDE_RELE_SAIDA, PinValue.Low);
            gpio.Write(LED_RGB_AZUL_SAIDA, PinValue.Low);
            gpio.Write(LED_RGB_VERMELHO_SAIDA, PinValue.Low);
            gpio.Write(LED_RGB_VERDE_SAIDA, PinValue.High);
        }
        //Define os pinos para o estado de saida não autorizada
        public void ExitUnauthorizedState()
        {
            gpio.Write(LED_RGB_AZUL_SAIDA, PinValue.Low);
            gpio.Write(LED_RGB_VERMELHO_SAIDA, PinValue.High);
            gpio.Write(LED_RGB_VERDE_SAIDA, PinValue.Low);
            Thread.Sleep(3000);
            gpio.Write(LED_RGB_AZUL_SAIDA, PinValue.High);
            gpio.Write(LED_RGB_VERMELHO_SAIDA, PinValue.Low);
        }
        //Define os pinos para o estado de entrada autorizada
        public void EntranceAuthorizedState()
        {
            gpio.Write(RELE_ENTRADA, PinValue.Low);
            gpio.Write(LED_VERDE_RELE_ENTRADA, PinValue.Low);
            gpio.Write(LED_RGB_AZUL_ENTRADA, PinValue.Low);
            gpio.Write(LED_RGB_VERMELHO_ENTRADA, PinValue.Low);
            gpio.Write(LED_RGB_VERDE_ENTRADA, PinValue.High);
        }
        //Define os pinos para o estado de entrada não autorizada
        public void EntranceUnauthorizedState()
        {
            gpio.Write(LED_RGB_AZUL_ENTRADA, PinValue.Low);
            gpio.Write(LED_RGB_VERMELHO_ENTRADA, PinValue.High);
            gpio.Write(LED_RGB_VERDE_ENTRADA, PinValue.Low);
            Thread.Sleep(3000);
            gpio.Write(LED_RGB_AZUL_ENTRADA, PinValue.High);
            gpio.Write(LED_RGB_VERMELHO_ENTRADA, PinValue.Low);
        }
        //Verifica os pinos do reed switch para verificar se usuario passou na saida
        public async Task<bool> WaitReedSwitchExit(int timeoutMilliseconds)
        {
            PinValue  initialPinValue = gpio.Read(REED_SWITCH_SAIDA);
            CancellationTokenSource cts = new CancellationTokenSource(timeoutMilliseconds);
            CancellationToken cancellationToken = cts.Token;
            
            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();

            cancellationToken.Register(() => tcs.TrySetResult(false));

            while(!cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(10);
                PinValue currentPinValue = gpio.Read(REED_SWITCH_SAIDA);

                if(currentPinValue != initialPinValue)
                {
                    tcs.TrySetResult(true);
                    break;
                }   

            }

            return await tcs.Task;
            
        }
        //Verifica os pinos do reed switch para verificar se usuario passou na entrada
        public async Task<bool> WaitReedSwitchEntrance(int timeoutMilliseconds)
        {
            PinValue  initialPinValue = gpio.Read(REED_SWITCH_ENTRADA);
            CancellationTokenSource cts = new CancellationTokenSource(timeoutMilliseconds);
            CancellationToken cancellationToken = cts.Token;

            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();

            cancellationToken.Register(() => tcs.TrySetResult(false));

            while(!cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(10);
                PinValue currentPinValue = gpio.Read(REED_SWITCH_ENTRADA);

                if(currentPinValue != initialPinValue)
                {
                    tcs.TrySetResult(true);
                    break;
                }   

            }

            return await tcs.Task;
        }
    }
}