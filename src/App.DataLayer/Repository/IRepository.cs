using System.Collections.Generic;
using System.Threading.Tasks;
using App.DataLayer.Entities;
using App.DataLayer.Events;
namespace App.DataLayer.Repository
{
    public interface IRepository
    {
        Task Save(IList<IEvent> newEvents, Portfolio portfolioState = null);
        Task<IList<IEvent>> GetEvents(string username, long start = 0);
        Task<Snapshot> GetSnapshot(string username);
    }
}