using laundry.project.Business;
using laundry.project.Entities;
using laundry.project.Infrastructure;
using laundry.project.Infrastructure.Sender;
using laundry.project.Interfaces;
using laundry.project.Presentation;
using laundry.project.Presentation.ConsoleUi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace laundry.project
{
    class Program
    {
        static void Main()
        {
            ConsoleUI.SetConsoleSizeToMax();
            ConsoleUI.ShowWelcomeScreen();

            var configManager = new ConfigurationManager();
            var displayManager = new DisplayManager();
            var machines = LoadConfiguration(configManager);

            InitializeSystemComponents(machines);

            displayManager.DisplayConfiguration(machines, Structure.MapInputMachine);
            displayManager.DisplayMenuPrincipale(machines, Structure.MapInputMachine);
        }

        

        

        static List<Machine> LoadConfiguration(ConfigurationManager configService)
        {
            string path, fileName;
            List<Machine> machines = null;

            while (true)
            {
                try
                {
                    PromptInput("Please enter configuration details:");

                    path = Prompt("Config File Path: ");
                    fileName = Prompt("Config File Name: ");
                    string fullPath = Path.Combine(path, fileName);

                    if (!File.Exists(fullPath))
                    {
                        ConsoleUI.ShowError("File not found! Please check the path and filename.");
                        continue;
                    }

                    ConsoleUI.ShowLoading("Loading configuration...");

                    machines = configService.SetConfig(fullPath);

                    if (machines == null || machines.Count == 0)
                    {
                        ConsoleUI.ShowError("No machines found in configuration file.");
                        continue;
                    }

                    ConsoleUI.ShowSuccess($"Configuration loaded successfully! {machines.Count} machines found.");
                    return machines;
                }
                catch (Exception ex)
                {
                    ConsoleUI.ShowError($"Error loading configuration: {ex.Message}");
                }
            }
        }

        static void InitializeSystemComponents(List<Machine> machines)
        {
            ConsoleUI.ShowLoading("Initializing system components...");
            SensorManager sensor = new();

            string connectionString = "Votre chaîne de connexion IoT Hub à récupérer depuis le portail Azure";


            var compositeSender = new CompositeSender();
            compositeSender.AddSender(new ConsoleSender());
            compositeSender.AddSender(new IoTHubSender(connectionString));


            ConsoleUI.ShowLoading("Starting machine monitoring...");
            StateMachineManager.LancerStateMachine(machines, sensor, compositeSender);
            ConsoleUI.ShowSuccess("System initialization complete.");
            Thread.Sleep(1500);
        }




        static void PromptInput(string message)
        {
            message.WriteLineLeft(ConsoleColor.White);
            "".WriteLineLeft(ConsoleColor.White);
        }

        static string Prompt(string label)
        {
            label.WriteLeft(ConsoleColor.White);
            return Console.ReadLine();
        }
    }
}
