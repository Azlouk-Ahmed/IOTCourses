using laundry.project.Business;
using laundry.project.Entities;
using laundry.project.Presentation.ConsoleUi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace laundry.project.Presentation
{
    class DisplayManager
    {
        private const int LEFT_MARGIN = 4;
        private const int CONTENT_WIDTH = 60;
        private const string HEADER_BORDER = "══════════════════════════════════════════════════════════";
        private const string SECTION_BORDER = "──────────────────────────────────────────────────────────";
        private const string MENU_BORDER = "──────────────────────────────────────────────────────────";


        private int _consoleWidth;
        private int _consoleHeight;


        public DisplayManager()
        {
            _consoleWidth = Console.LargestWindowWidth;
            _consoleHeight = Console.LargestWindowHeight;
            Console.CursorVisible = false; 
            SetupConsole();
        }



        private void SetupConsole()
        {
            try
            {
                Console.WindowWidth = Math.Min(120, Console.LargestWindowWidth);
                Console.WindowHeight = Math.Min(30, Console.LargestWindowHeight);
                Console.BufferWidth = Console.WindowWidth;
                Console.BufferHeight = Console.WindowHeight;

                ConsoleUI.DrawSplitScreenSeparator();
            }
            catch (Exception)
            {
                // Fallback if console resize fails
            }
        }

 

        private void SafeClearConsole(ConsoleZone zone = ConsoleZone.Left)
        {
            try
            {
                // Clear only the specified zone
                Uitility.ClearZone(zone);

                // Redraw the separator
                if (zone == ConsoleZone.Left)
                {
                    ConsoleUI.DrawSplitScreenSeparator();
                }

                // Check if console dimensions have changed
                if (_consoleWidth != Console.WindowWidth || _consoleHeight != Console.WindowHeight)
                {
                    _consoleWidth = Console.WindowWidth;
                    _consoleHeight = Console.WindowHeight;
                    ConsoleUI.DrawSplitScreenSeparator();
                }
            }
            catch (Exception)
            {
                // Fallback if Clear fails
                if (zone == ConsoleZone.Left)
                {
                    Console.SetCursorPosition(0, 0);
                    int halfWidth = Console.WindowWidth / 2;
                    string blankLine = new string(' ', halfWidth);
                    for (int i = 0; i < _consoleHeight; i++)
                    {
                        Console.SetCursorPosition(0, i);
                        Console.Write(blankLine);
                    }
                }
                else
                {
                    int halfWidth = Console.WindowWidth / 2;
                    string blankLine = new string(' ', halfWidth);
                    for (int i = 0; i < _consoleHeight; i++)
                    {
                        Console.SetCursorPosition(halfWidth + 1, i);
                        Console.Write(blankLine);
                    }
                }
                ConsoleUI.DrawSplitScreenSeparator();
            }
        }

      

       
        

       

        private void WaitForKey(string message = "Press any key to continue...")
        {
            " ".WriteLineLeft(ConsoleColor.White);
            message.WriteLineLeft(ConsoleColor.DarkGray);
            Console.CursorVisible = false;
            Console.ReadKey(true);
            Console.CursorVisible = true;
        }

        private string ReadUserInput(string prompt, ConsoleColor promptColor)
        {
            " ".WriteLineLeft(ConsoleColor.White);
            prompt.WriteLeft(promptColor);
            Console.CursorVisible = true;
            string input = Uitility.ReadLineLeft(ConsoleColor.White);
            Console.CursorVisible = false;
            return input.Trim();
        }

        public static void DisplayMachineStateChange(Machine machine, MachineState newState, DateTime timestamp)
        {
            // Format timestamp nicely
            string timeStr = timestamp.ToString("HH:mm:ss");

            // Show state change in right panel
            $"[{timeStr}] Machine: {machine.IdMachine}".WriteLineRight(ConsoleColor.Cyan);
            $"State changed: {GetStateDescription(machine.CurrentState)} → {GetStateDescription(newState)}".WriteLineRight(ConsoleUI.GetStateColor(newState));

            // Add separator line
            string separator = new string('·', Console.WindowWidth / 2 - 6);
            separator.WriteLineRight(ConsoleColor.DarkGray);
        }

        public static void DisplayRunningCycle(Machine machine, Cycle cycle, int remainingSeconds)
        {
            // Only update on certain intervals to avoid too much output
            if (remainingSeconds % 5 == 0 || remainingSeconds <= 10)
            {
                string timeStr = DateTime.Now.ToString("HH:mm:ss");
                $"[{timeStr}] Machine: {machine.IdMachine}".WriteLineRight(ConsoleColor.Magenta);
                $"Running: {cycle.NameCycle} - {remainingSeconds}s remaining".WriteLineRight(ConsoleColor.Yellow);
            }
        }

        public static void DisplayCycleCompleted(Machine machine, Cycle cycle)
        {
            string timeStr = DateTime.Now.ToString("HH:mm:ss");
            $"[{timeStr}] Machine: {machine.IdMachine}".WriteLineRight(ConsoleColor.Green);
            $"✓ COMPLETED: {cycle.NameCycle} cycle".WriteLineRight(ConsoleColor.Green);

            // Add separator line
            string separator = new string('═', Console.WindowWidth / 2 - 6);
            separator.WriteLineRight(ConsoleColor.Green);
        }

        public void DisplayConfiguration(List<Machine> machines, Dictionary<int, string> map)
        {
            SafeClearConsole(ConsoleZone.Left);
            ConsoleUI.WriteHeaderLeft("LAUNDRY MANAGEMENT SYSTEM", ConsoleColor.Cyan);
            ConsoleUI.WriteSectionTitleLeft("MACHINE CONFIGURATION", ConsoleColor.Yellow);

            if (machines.Count == 0)
            {
                "No machines configured in the system.".WriteLineLeft(ConsoleColor.Red);
                WaitForKey();
                return;
            }

            foreach (var machine in machines)
            {
                " ".WriteLineLeft(ConsoleColor.White);
                $"Machine ID: {machine.IdMachine}".WriteLineLeft(ConsoleColor.White);
                $"Input Channel: {machine.Input}".WriteLineLeft(ConsoleColor.Gray);
                $"Current State: {GetStateDescription(machine.CurrentState)}".WriteLineLeft(ConsoleUI.GetStateColor(machine.CurrentState));

                if (machine.Cycles != null && machine.Cycles.Any())
                {
                    "Available Cycles:".WriteLineLeft(ConsoleColor.Yellow);
                    foreach (var cycle in machine.Cycles)
                    {
                        $"  • {cycle.NameCycle} - {cycle.DureeCycle} seconds".WriteLineLeft(ConsoleColor.Gray);
                    }
                }
                else
                {
                    "  No cycles configured for this machine".WriteLineLeft(ConsoleColor.DarkGray);
                }

                string separator = new string('─', Console.WindowWidth / 2 - 6);
                separator.WriteLineLeft(ConsoleColor.DarkGray);
            }

            WaitForKey();
        }

        public void DisplayMenuPrincipale(List<Machine> machines, Dictionary<int, string> map)
        {
            // Initialize right side panel once
            SafeClearConsole(ConsoleZone.Right);
            ConsoleUI.WriteHeaderRight("MACHINE ACTIVITY LOG", ConsoleColor.Magenta);
            ConsoleUI.WriteSectionTitleRight("REAL-TIME UPDATES", ConsoleColor.Yellow);
            "System is monitoring machine states...".WriteLineRight(ConsoleColor.Gray);
            "Updates will appear here automatically.".WriteLineRight(ConsoleColor.Gray);

            string choice;
            do
            {
                SafeClearConsole(ConsoleZone.Left);
                ConsoleUI.WriteHeaderLeft("LAUNDRY MANAGEMENT SYSTEM", ConsoleColor.Cyan);

                // Display machine status
                DisplayMachineStatusSummary(machines);

                // Display menu options
                ConsoleUI.WriteSectionTitleLeft("MAIN MENU", ConsoleColor.White);
                string menuBorder = new string('─', Console.WindowWidth / 2 - 6);
                menuBorder.WriteLineLeft(ConsoleColor.DarkGray);
                "1. Turn OFF Machine".WriteLineLeft(ConsoleColor.White);
                "2. Turn ON Machine".WriteLineLeft(ConsoleColor.White);
                "3. Start Machine Cycle".WriteLineLeft(ConsoleColor.White);
                "4. View Machine Configuration".WriteLineLeft(ConsoleColor.White);
                "Q. Exit Application".WriteLineLeft(ConsoleColor.White);
                menuBorder.WriteLineLeft(ConsoleColor.DarkGray);

                choice = ReadUserInput("Enter your choice: ", ConsoleColor.Green).ToLower();

                switch (choice)
                {
                    case "1":
                        HandleTurnOffMachine(machines);
                        break;
                    case "2":
                        HandleTurnOnMachine(machines);
                        break;
                    case "3":
                        HandleStartCycle(machines);
                        break;
                    case "4":
                        DisplayConfiguration(machines, map);
                        break;
                    case "q":
                        if (ConfirmExit())
                            return;
                        choice = "";
                        break;
                    default:
                        "Invalid choice. Please try again.".WriteLineLeft(ConsoleColor.Red);
                        Thread.Sleep(1500);
                        break;
                }

            } while (choice != "q");
        }

        private void DisplayMachineStatusSummary(List<Machine> machines)
        {
            ConsoleUI.WriteSectionTitleLeft("MACHINE STATUS", ConsoleColor.Magenta);

            int offCount = machines.Count(m => m.CurrentState == MachineState.A);
            int onCount = machines.Count(m => m.CurrentState == MachineState.D);
            int cycleCount = machines.Count(m => m.CurrentState == MachineState.C);

            $"Total Machines: {machines.Count}".WriteLineLeft(ConsoleColor.White);
            $"OFF: {offCount} | ON: {onCount} | RUNNING CYCLE: {cycleCount}".WriteLineLeft(ConsoleColor.Gray);

            // List all machines with their states
            " ".WriteLineLeft(ConsoleColor.White);
            foreach (var machine in machines)
            {
                ConsoleColor stateColor = ConsoleUI.GetStateColor(machine.CurrentState);
                $"  • {machine.IdMachine}: {GetStateDescription(machine.CurrentState)}".WriteLineLeft(stateColor);
            }
        }

        private void HandleTurnOffMachine(List<Machine> machines)
        {
            SafeClearConsole(ConsoleZone.Left);
            ConsoleUI.WriteHeaderLeft("TURN OFF MACHINE", ConsoleColor.Yellow);

            var availableMachines = machines.Where(m => m.CurrentState == MachineState.D).ToList();

            if (!availableMachines.Any())
            {
                "No machines are currently ON to turn off.".WriteLineLeft(ConsoleColor.Red);
                WaitForKey();
                return;
            }

            ConsoleUI.WriteSectionTitleLeft("AVAILABLE MACHINES", ConsoleColor.White);
            foreach (var machine in availableMachines)
            {
                $"  • {machine.IdMachine}".WriteLineLeft(ConsoleColor.White);
            }

            string machineId = ReadUserInput("Enter Machine ID to turn OFF (or 'c' to cancel): ", ConsoleColor.Green);

            if (machineId.ToLower() == "c")
                return;

            Machine selectedMachine = availableMachines.FirstOrDefault(m => m.IdMachine.Equals(machineId, StringComparison.OrdinalIgnoreCase));

            if (selectedMachine == null)
            {
                $"Machine '{machineId}' not found or not available to turn off.".WriteLineLeft(ConsoleColor.Red);
                WaitForKey();
                return;
            }

            $"Turning OFF machine {selectedMachine.IdMachine}...".WriteLineLeft(ConsoleColor.Yellow);
            Thread.Sleep(1000); // Simulate processing time

            // Update machine state
            Structure.MapInputTension[selectedMachine.Input] = 0;

            $"Machine {selectedMachine.IdMachine} has been turned OFF successfully.".WriteLineLeft(ConsoleColor.Green);
            WaitForKey();
        }

        private void HandleTurnOnMachine(List<Machine> machines)
        {
            SafeClearConsole(ConsoleZone.Left);
            ConsoleUI.WriteHeaderLeft("TURN ON MACHINE", ConsoleColor.Yellow);

            var availableMachines = machines.Where(m => m.CurrentState == MachineState.A).ToList();

            if (!availableMachines.Any())
            {
                "No machines are currently OFF to turn on.".WriteLineLeft(ConsoleColor.Red);
                WaitForKey();
                return;
            }

            ConsoleUI.WriteSectionTitleLeft("AVAILABLE MACHINES", ConsoleColor.White);
            foreach (var machine in availableMachines)
            {
                $"  • {machine.IdMachine}".WriteLineLeft(ConsoleColor.White);
            }

            string machineId = ReadUserInput("Enter Machine ID to turn ON (or 'c' to cancel): ", ConsoleColor.Green);

            if (machineId.ToLower() == "c")
                return;

            Machine selectedMachine = availableMachines.FirstOrDefault(m => m.IdMachine.Equals(machineId, StringComparison.OrdinalIgnoreCase));

            if (selectedMachine == null)
            {
                $"Machine '{machineId}' not found or not available to turn on.".WriteLineLeft(ConsoleColor.Red);
                WaitForKey();
                return;
            }

            $"Turning ON machine {selectedMachine.IdMachine}...".WriteLineLeft(ConsoleColor.Yellow);
            Thread.Sleep(1000); // Simulate processing time

            // Update machine state
            Structure.MapInputTension[selectedMachine.Input] = 5;

            $"Machine {selectedMachine.IdMachine} has been turned ON successfully.".WriteLineLeft(ConsoleColor.Green);
            WaitForKey();
        }

        private void HandleStartCycle(List<Machine> machines)
        {
            SafeClearConsole(ConsoleZone.Left);
            ConsoleUI.WriteHeaderLeft("START MACHINE CYCLE", ConsoleColor.Yellow);

            var availableMachines = machines.Where(m => m.CurrentState == MachineState.D).ToList();

            if (!availableMachines.Any())
            {
                "No machines are currently ON to start a cycle.".WriteLineLeft(ConsoleColor.Red);
                WaitForKey();
                return;
            }

            ConsoleUI.WriteSectionTitleLeft("AVAILABLE MACHINES", ConsoleColor.White);
            foreach (var machine in availableMachines)
            {
                $"  • {machine.IdMachine}".WriteLineLeft(ConsoleColor.White);
            }

            string machineId = ReadUserInput("Enter Machine ID (or 'c' to cancel): ", ConsoleColor.Green);

            if (machineId.ToLower() == "c")
                return;

            Machine selectedMachine = availableMachines.FirstOrDefault(m => m.IdMachine.Equals(machineId, StringComparison.OrdinalIgnoreCase));

            if (selectedMachine == null)
            {
                $"Machine '{machineId}' not found or not available.".WriteLineLeft(ConsoleColor.Red);
                WaitForKey();
                return;
            }

            if (selectedMachine.Cycles == null || !selectedMachine.Cycles.Any())
            {
                $"Machine {selectedMachine.IdMachine} has no cycles configured.".WriteLineLeft(ConsoleColor.Red);
                WaitForKey();
                return;
            }

            SafeClearConsole(ConsoleZone.Left);
            ConsoleUI.WriteHeaderLeft($"SELECT CYCLE FOR {selectedMachine.IdMachine}", ConsoleColor.Yellow);

            ConsoleUI.WriteSectionTitleLeft("AVAILABLE CYCLES", ConsoleColor.White);
            foreach (var cycle in selectedMachine.Cycles)
            {
                $"  • {cycle.NameCycle} - {cycle.DureeCycle} seconds".WriteLineLeft(ConsoleColor.White);
            }

            string cycleName = ReadUserInput("Enter Cycle Name (or 'c' to cancel): ", ConsoleColor.Green);

            if (cycleName.ToLower() == "c")
                return;

            Cycle selectedCycle = selectedMachine.Cycles.FirstOrDefault(c =>
                c.NameCycle.Equals(cycleName, StringComparison.OrdinalIgnoreCase));

            if (selectedCycle == null)
            {
                $"Cycle '{cycleName}' not found for this machine.".WriteLineLeft(ConsoleColor.Red);
                WaitForKey();
                return;
            }

            // Confirmation screen
            SafeClearConsole(ConsoleZone.Left);
            ConsoleUI.WriteHeaderLeft("CONFIRM CYCLE START", ConsoleColor.Yellow);

            $"Machine: {selectedMachine.IdMachine}".WriteLineLeft(ConsoleColor.White);
            $"Cycle: {selectedCycle.NameCycle}".WriteLineLeft(ConsoleColor.White);
            $"Duration: {selectedCycle.DureeCycle} seconds".WriteLineLeft(ConsoleColor.White);
            " ".WriteLineLeft(ConsoleColor.White);

            string confirm = ReadUserInput("Start cycle? (y/n): ", ConsoleColor.Green).ToLower();

            if (confirm != "y")
            {
                "Cycle start cancelled.".WriteLineLeft(ConsoleColor.Yellow);
                WaitForKey();
                return;
            }

            $"Starting {selectedCycle.NameCycle} cycle on {selectedMachine.IdMachine}...".WriteLineLeft(ConsoleColor.Yellow);
            Thread.Sleep(1000); // Simulate processing time

            // Update machine state and start cycle
            Structure.MapInputTension[selectedMachine.Input] = 10;
            StateMachineManager.StartCycle(selectedMachine, selectedCycle);

            $"Cycle {selectedCycle.NameCycle} has been started successfully.".WriteLineLeft(ConsoleColor.Green);
            $"Duration: {selectedCycle.DureeCycle} seconds".WriteLineLeft(ConsoleColor.Green);
            WaitForKey();
        }

        private bool ConfirmExit()
        {
            SafeClearConsole(ConsoleZone.Left);
            ConsoleUI.WriteHeaderLeft("EXIT APPLICATION", ConsoleColor.Yellow);

            "Are you sure you want to exit the application?".WriteLineLeft(ConsoleColor.White);
            "Any running cycles will continue to run in the background.".WriteLineLeft(ConsoleColor.Yellow);
            " ".WriteLineLeft(ConsoleColor.White);

            string confirm = ReadUserInput("Exit? (y/n): ", ConsoleColor.Green).ToLower();

            return confirm == "y";
        }



        private static string GetStateDescription(MachineState state)
        {
            return state switch
            {
                MachineState.A => "OFF",
                MachineState.D => "ON",
                MachineState.C => "RUNNING CYCLE",
                _ => "UNKNOWN"
            };
        }
    }
}