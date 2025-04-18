using laundry.project.Interfaces;
using laundry.project.Entities;
using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using laundry.project.Presentation;

namespace laundry.project.Infrastructure.Sender
{
    internal class IoTHubSender : ISender
    {
        private readonly DeviceClient _deviceClient;


        public IoTHubSender(string connectionString)
        {
            _deviceClient = DeviceClient.CreateFromConnectionString(connectionString);
        }

        public void SendMessage(laundry.project.Entities.Message message)
        {
            var messageToSend = new
            {
                IdMachine = message.IdMachine,
                Date = message.Date,
                State = GetStateDescription(message.State) 
            };

            string messageJson = JsonConvert.SerializeObject(messageToSend);
            var iotMessage = new Microsoft.Azure.Devices.Client.Message(Encoding.UTF8.GetBytes(messageJson));

            var cts = new CancellationTokenSource();
            var loaderThread = new Thread(() => Presentation.ConsoleUi.ConsoleUI.ShowLoader("Sending to IoT Hub", cts.Token));
            loaderThread.Start();

            try
            {
                Task.Run(async () =>
                {
                    await _deviceClient.SendEventAsync(iotMessage);
                    cts.Cancel();            
                    loaderThread.Join();    

                    $" ===> Message sent to IoT Hub: Machine {message.IdMachine} is {GetStateDescription(message.State)}"
                        .WriteLineRight(ConsoleColor.Cyan);
                }).Wait();
            }
            catch (Exception ex)
            {
                cts.Cancel();
                loaderThread.Join();
                $"[Error] Failed to send message to IoT Hub: {ex.Message}".WriteLineRight(ConsoleColor.Red);
            }
        }

       



        private string GetStateDescription(laundry.project.Entities.MachineState state)
        {
            return state switch
            {
                laundry.project.Entities.MachineState.D => "démarre",
                laundry.project.Entities.MachineState.A => "arrêté",
                laundry.project.Entities.MachineState.C => "en cycle",
                _ => state.ToString()
            };
        }
    }
}
