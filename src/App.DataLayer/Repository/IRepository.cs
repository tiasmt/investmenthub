using System.Collections.Generic;
using System.Threading.Tasks;
using App.DataLayer.Entities;
using App.DataLayer.Events;
namespace App.DataLayer.Repository
{
    public interface IRepository
    {
        Task Save(List<IEvent> newEvents);
        Task<List<IEvent>> GetAllEvents(string username);
        Task<Snapshot> GetSnapshot(string username);
    }
}