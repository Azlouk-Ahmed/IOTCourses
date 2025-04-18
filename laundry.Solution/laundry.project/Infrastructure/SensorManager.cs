using laundry.project.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace laundry.project.Infrastructure
{
    internal class SensorManager
    {
        void InitSensorMAp()
        {

        }

        int GetSensorValue(int input)
        {
            return Structure.MapInputTension[input];
        }
        public MachineState GetMachineState(int input)
        {
            int value = GetSensorValue( input);
            switch (value)
            {
                case 0:return MachineState.A;
                case 5: return MachineState.D; 
                case 10: return MachineState.C; 
                default : return MachineState.I; 
            }
        }
    }
}
