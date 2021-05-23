using System.Collections.Generic;
using App.DataLayer.Events;

namespace App.API.Services
{
    public interface IPortfolioService
    {
         void Deposit(string username, int amount);
         void Withdrawal(string username, int amount);
         void BuyStock(string username, string stock, int quantity, double price);
         void SellStock(string username, string stock, int quantity, double price);
         IList<IEvent> GetEvents();
         IList<IEvent> GetUncommittedEvents();
         void ApplyEvent(IEvent evnt, bool isFastForward = false);
    }
}