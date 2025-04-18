using laundry.project.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace laundry.project.Interfaces
{
    internal interface ISender
    {
        void SendMessage(Message message);
    }
}
