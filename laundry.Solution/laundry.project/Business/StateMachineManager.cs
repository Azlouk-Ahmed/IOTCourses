using laundry.project.Entities;
using laundry.project.Infrastructure.Sender;
using laundry.project.Interfaces;
using laundry.project.Presentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace laundry.project.Business
{
    public class StateMachineManager
    {
        static int left= Console.WindowWidth / 2;
        static int top=0;
        static Thread CreateStateMAchineThread(Machine machine, SensorManager sensorManager, ISender sender)
        {
            Thread t = new Thread(() => CreateStateMachine(machine, sensorManager, sender));
            return t;

        }
        static void CreateStateMachine(Machine machine, SensorManager sensorManager, ISender sender)
        {
            Cycle? currentCycle = null;
            DateTime? cycleStartTime = null;
            int cycleDuration = 0;

            while (true)
            {
                MachineState newState = sensorManager.GetMachineState(machine.Input);

                switch (machine.CurrentState)
                {
                    case MachineState.A:
                    case MachineState.D:
                    case MachineState.C:
                        if (machine.CurrentState == newState) break;


                        DisplayManager.DisplayMachineStateChange(machine, newState, DateTime.Now);

                        if (newState == MachineState.C && machine.CurrentState != MachineState.C)
                        {
                            if (machine.Cycles != null && machine.Cycles.Any())
                            {
                                currentCycle = machine.Cycles.FirstOrDefault();
                                cycleStartTime = DateTime.Now;
                                cycleDuration = currentCycle?.DureeCycle ?? 0;

                                DisplayManager.DisplayRunningCycle(machine, currentCycle, cycleDuration);
                            }
                        }

                        if (machine.CurrentState == MachineState.C && newState != MachineState.C)
                        {
                            if (currentCycle != null)
                            {
                                DisplayManager.DisplayCycleCompleted(machine, currentCycle);
                            }
                            currentCycle = null;
                            cycleStartTime = null;
                        }

                        sender.SendMessage(new Message(machine.IdMachine, DateTime.Now, newState));
                        machine.CurrentState = newState;
                        break;
                }

                Thread.Sleep(1000);
            }
        }
        internal static void LancerStateMachine(List<Machine> machines, SensorManager sensorManager, ISender sender)
        {
            foreach (Machine machine in machines)
            {
                Thread t = CreateStateMAchineThread(machine, sensorManager, sender);
                t.Start();

            }
        }
        internal static void StartCycle(Machine machine,Cycle cycle)
        {
            
            machine.Timer_timer= new System.Timers.Timer(cycle.DureeCycle*1000);
            
            machine.Timer_timer.Elapsed += (sender,e ) => OnTimerElapsed(sender, e, machine);
            machine.Timer_timer.AutoReset = false; // Le timer ne se répète pas
            machine.Timer_timer.Enabled = true; // Démarrer le timer

        }
        private static void OnTimerElapsed(object? sender, ElapsedEventArgs e,Machine machine )
        {

            Structure.MapInputTension[machine.Input] = 5;
            
            
        }
    }
}
