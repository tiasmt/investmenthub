using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App.DataLayer.Entities;
using App.DataLayer.Events;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace App.DataLayer.Repository
{
    public class SQLRepository : IRepository
    {
        private readonly InvestmentHubContext _context;
        private readonly int _snapshotInterval;
        public SQLRepository(InvestmentHubContext context)
        {
            _context = context;
        }

        public async Task<List<IEvent>> GetAllEvents(string username)
        {
            var events = new List<IEvent>();
            var eventData = await _context.Events.Where(x => x.User == username).ToListAsync();
            foreach(var evnt in eventData)
            {
                IEvent resolvedEvent = DeserializeEvent(evnt);
                events.Add(resolvedEvent);
            }
            return events;
        }

        public Task<Snapshot> GetSnapshot(string username)
        {
            throw new System.NotImplementedException();
        }

        public async Task Save(List<IEvent> newEvents)
        {
            long version = 0;

            foreach (var evnt in newEvents)
            {
                var jsonData = JsonConvert.SerializeObject(evnt);
                var evt = new Event{Timestamp = DateTime.UtcNow, EventType = evnt.EventType, User = evnt.User, Data = jsonData};
                await _context.Events.AddAsync(evt);
                version = await _context.SaveChangesAsync();
                

                if ((version) % _snapshotInterval == 0)
                {
                    // await AppendSnapshot(portfolio, version);
                }
            }
        }

        private IEvent DeserializeEvent(Event evnt)
        {
            IEvent result = null;

            if (evnt.EventType == nameof(DepositMade))
            {
                result = JsonConvert.DeserializeObject<DepositMade>(evnt.Data);
            }
            else if (evnt.EventType == nameof(WithdrawalMade))
            {
                result = JsonConvert.DeserializeObject<WithdrawalMade>(evnt.Data);
            }
            else if (evnt.EventType == nameof(SharesBought))
            {
                result = JsonConvert.DeserializeObject<SharesBought>(evnt.Data);
            }
            else if (evnt.EventType == nameof(SharesSold))
            {
                result = JsonConvert.DeserializeObject<SharesSold>(evnt.Data);
            }

            return result;

        }
    }
}