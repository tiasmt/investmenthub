using System;

namespace App.DataLayer.Events
{
    public class DepositMade : IEvent
    {
        public string EventType { get; } = nameof(DepositMade);
        public string User { get; }
        public int Amount { get; }
        public DateTime DateTime { get; }

        public DepositMade(string user, int amount, DateTime dateTime)
        {
            User = user;
            Amount = amount;
            DateTime = dateTime;
        }
        
    }
}