using laundry.project.Entities;
using laundry.project.Interfaces;
using laundry.project.Presentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace laundry.project.Infrastructure.Sender
{
    internal class ConsoleSender : ISender
    {
        public void SendMessage(Message message)
        {
            ConsoleColor color = ConsoleColor.White;
            string stateDescription = "";

            if (message.State == MachineState.D)
            {
                color = ConsoleColor.Cyan;
                stateDescription = "démarre";
            }
            else if (message.State == MachineState.A)
            {
                color = ConsoleColor.Yellow;
                stateDescription = "arrêté";
            }
            else if (message.State == MachineState.C)
            {
                color = ConsoleColor.Red;
                stateDescription = "en cycle";
            }

            string formattedMessage = $"Machine ID: {message.IdMachine} | Date: {message.Date:G} | État: {stateDescription}";
            formattedMessage.WriteLineRight(color);
        }

    }
}
