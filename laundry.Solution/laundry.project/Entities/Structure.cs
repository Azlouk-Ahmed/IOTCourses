using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace laundry.project.Entities
{
    enum MachineState { A, D, C,I }//I-> indéfini
    public static class Structure {
        public static Dictionary<int, string> MapInputMachine = new Dictionary<int, string>();
        public static Dictionary<int, int> MapInputTension = new Dictionary<int, int>();
       
    }
}   
