using System.Collections.Generic;
using System.Threading.Tasks;
using App.DataLayer.Entities;
using App.DataLayer.Events;

namespace App.API.Services
{
    public interface IPortfolioService
    {
         Task Deposit(string username, int amount);
         Task Withdrawal(string username, int amount);
         Task BuyStock(string username, string stock, int quantity, double price);
         Task SellStock(string username, string stock, int quantity, double price);
         IList<IEvent> GetEvents();
         IList<IEvent> GetUncommittedEvents();
         Task ApplyEvent(IEvent evnt, bool isFastForward = false);
         Task<Portfolio> GetState(string username);
    }
}