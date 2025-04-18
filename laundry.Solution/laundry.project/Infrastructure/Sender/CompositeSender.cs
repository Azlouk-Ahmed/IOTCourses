using laundry.project.Entities;
using laundry.project.Interfaces;
using System.Collections.Generic;

namespace laundry.project.Infrastructure.Sender
{
    internal class CompositeSender : ISender
    {
        private readonly List<ISender> _senders = new List<ISender>();

        public void AddSender(ISender sender)
        {
            _senders.Add(sender);
        }

        public void SendMessage(Message message)
        {
            foreach (var sender in _senders)
            {
                sender.SendMessage(message);
            }
        }
    }
}