using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App.DataLayer.Entities;
using App.DataLayer.Events;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace App.DataLayer.Repository
{
    public class SQLRepository : IRepository
    {
        private readonly InvestmentHubContext _context;
        private readonly int _snapshotInterval = 5;
        private readonly IDistributedCache _distributedCache;
        public SQLRepository(InvestmentHubContext context, IDistributedCache distributedCache)
        {
            _context = context;
            _distributedCache = distributedCache;
        }

        public async Task<IList<IEvent>> GetEvents(string username, long start = 0)
        {
            var cacheKey = "events" + username;
            var serializedEvents = string.Empty;
            var events = new List<IEvent>();
            var eventData = new List<Event>();
            var redisEvents = await _distributedCache.GetAsync(cacheKey);
            if (redisEvents != null)
            {
                serializedEvents = Encoding.UTF8.GetString(redisEvents);
                eventData = JsonConvert.DeserializeObject<List<Event>>(serializedEvents);
            }
            else
            {
                eventData = await _context.Events.Where(x => x.User == username).Skip((int)start).ToListAsync();
                serializedEvents = JsonConvert.SerializeObject(eventData);
                redisEvents = Encoding.UTF8.GetBytes(serializedEvents);
                var options = new DistributedCacheEntryOptions()
                    .SetAbsoluteExpiration(DateTime.Now.AddMinutes(10))
                    .SetSlidingExpiration(TimeSpan.FromMinutes(2));
                await _distributedCache.SetAsync(cacheKey, redisEvents, options);
            }
            foreach (var evnt in eventData)
            {
                IEvent resolvedEvent = DeserializeEvent(evnt);
                events.Add(resolvedEvent);
            }
            return events;
        }

        public async Task<Snapshot> GetSnapshot(string username)
        {
            var snapshotEvent = await _context.Snapshots.Where(e => e.User == username)
                                                        .OrderByDescending(s => s.Id)
                                                        .FirstOrDefaultAsync();
            var snapshot = snapshotEvent == null ? new Snapshot() : JsonConvert.DeserializeObject<Snapshot>(snapshotEvent.Data);
            return snapshot;
        }

        public async Task Save(IList<IEvent> newEvents, Portfolio portfolioState = null)
        {
            long version = 0;

            foreach (var evnt in newEvents)
            {
                var jsonData = JsonConvert.SerializeObject(evnt);
                var evt = new Event { Timestamp = DateTime.UtcNow, EventType = evnt.EventType, User = evnt.User, Data = jsonData };
                await _context.Events.AddAsync(evt);
                await _context.SaveChangesAsync();
                version = await _context.Events.Where(e => e.User == evnt.User).CountAsync();

                if ((version) % _snapshotInterval == 0 && portfolioState != null)
                {
                    await AppendSnapshot(portfolioState, (int)version, evnt.User);
                }
            }

            /*if (newEvents.Count != 0)
            {
                var user = newEvents.FirstOrDefault().User;
                await _distributedCache.RemoveAsync("events" + user);
            }*/
        }

        private async Task AppendSnapshot(Portfolio portfolio, int version, string user)
        {
            try
            {
                var jsonData = JsonConvert.SerializeObject(new Snapshot { State = portfolio, Version = version });
                await _context.Snapshots.AddAsync(new SnapshotEvent { Version = version, User = user, EventType = nameof(Snapshot), Timestamp = DateTime.UtcNow, Data = jsonData });
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
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