using System;
using System.Collections.Generic;
using System.Linq;
using App.DataLayer.Entities;
using App.DataLayer.Events;
using App.DataLayer.Repository;

namespace App.API.Services
{
    public class PortfolioService : IPortfolioService
    {
        
        private readonly List<IEvent> _events = new List<IEvent>();
        private readonly List<IEvent> _uncommittedevents = new List<IEvent>();
        public int Version { get; protected set; }
        //Projection (Current State)
        private readonly Portfolio _portfolioState;
        private readonly IRepository _repository;

        public PortfolioService(IRepository repository)
        {
            _repository = repository;
            //TODO get latest Snapshot and update events
            _portfolioState = new Portfolio();
        }

        public void Deposit(string username, int quantity)
        {
            ApplyEvent(new DepositMade(username, quantity, DateTime.UtcNow));
        }

        public void Withdrawal(string username, int quantity)
        {
            ApplyEvent(new WithdrawalMade(username, quantity, DateTime.UtcNow));
        }

        public void BuyStock(string username, string stock, int quantity, double price)
        {
            ApplyEvent(new SharesBought(username, stock, quantity, price, DateTime.UtcNow));
        }
        public void SellStock(string username, string stock, int quantity, double price)
        {
            ApplyEvent(new SharesSold(username, stock, quantity, price, DateTime.UtcNow));
        }

        private void Apply(SharesSold evnt)
        {
            _portfolioState.Money += (evnt.Amount * evnt.Price);
            var share = new ShareTicker { Amount = evnt.Amount, Price = evnt.Price };
            if (_portfolioState.Shares == null) _portfolioState.Shares = new List<ShareTicker>();
            if (_portfolioState.Shares.Exists(s => s.Name == evnt.Stock))
            {
                _portfolioState.Shares.Where(s => s.Name == evnt.Stock).First().Amount -= evnt.Amount;
                _portfolioState.Profit = (evnt.Price - _portfolioState.Shares.Where(s => s.Name == evnt.Stock).First().Price) * evnt.Amount;
            }
            else
            {
                //do nothing
            }
        }
        private void Apply(SharesBought evnt)
        {
            _portfolioState.Money -= (evnt.Amount * evnt.Price);
            if (_portfolioState.Shares == null) _portfolioState.Shares = new List<ShareTicker>();
            if (_portfolioState.Shares.Exists(s => s.Name == evnt.Stock))
            {
                _portfolioState.Shares.Where(s => s.Name == evnt.Stock).First().Price = ((_portfolioState.Shares.Where(s => s.Name == evnt.Stock).First().Amount * _portfolioState.Shares.Where(s => s.Name == evnt.Stock).First().Price) + (evnt.Amount * evnt.Price)) / (_portfolioState.Shares.Where(s => s.Name == evnt.Stock).First().Amount + evnt.Amount);
                _portfolioState.Shares.Where(s => s.Name == evnt.Stock).First().Amount += evnt.Amount;
            }
            else
            {
                var share = new ShareTicker { Name = evnt.Stock, Price = evnt.Price, Amount = evnt.Amount };
                _portfolioState.Shares.Add(share);
            }

        }
        private void Apply(DepositMade evnt)
        {
            _portfolioState.Money += evnt.Amount;
        }
        private void Apply(WithdrawalMade evnt)
        {
            _portfolioState.Money -= evnt.Amount;
        }

        public IList<IEvent> GetEvents()
        {
            return _events;
        }

        public IList<IEvent> GetUncommittedEvents()
        {
            var uncommittedEvents = _uncommittedevents.ToArray();
            _uncommittedevents.Clear();
            return uncommittedEvents;
        }

        public void ApplyEvent(IEvent evnt, bool isFastForward = false)
        {
            switch (evnt)
            {
                case SharesBought sharesBought:
                    Apply(sharesBought);
                    break;
                case SharesSold sharesSold:
                    Apply(sharesSold);
                    break;
                case DepositMade depositMade:
                    Apply(depositMade);
                    break;
                case WithdrawalMade withdrawalMade:
                    Apply(withdrawalMade);
                    break;
            }


            if (isFastForward == false)
                _uncommittedevents.Add(evnt);
            else
                _events.Add(evnt);

            _repository.Save(_uncommittedevents);
        }
    }

}
