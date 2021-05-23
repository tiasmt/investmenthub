using System;
using System.Collections.Generic;
using System.Linq;
using App.DataLayer.Entities;
using App.DataLayer.Events;

namespace App.API.App.Services
{
    public class PortfolioService
    {
        public string Username { get; }
        private readonly IList<IEvent> _events = new List<IEvent>();
        private readonly IList<IEvent> _uncommittedevents = new List<IEvent>();
        public int Version { get; protected set; }
        //Projection (Current State)
        private readonly Portfolio _portfolioState;

        public PortfolioService()
        {

        }

        public void Deposit(int quantity)
        {
            ApplyEvent(new DepositMade(Username, quantity, DateTime.UtcNow));
        }

        public void Withdrawal(int quantity)
        {
            ApplyEvent(new WithdrawalMade(Username, quantity, DateTime.UtcNow));
        }

        public void BuyShares(string stock, int quantity, double price)
        {
            ApplyEvent(new SharesBought(Username, stock, quantity, price, DateTime.UtcNow));
        }
        public void SellShares(string stock, int quantity, double price)
        {
            ApplyEvent(new SharesSold(Username, stock, quantity, price, DateTime.UtcNow));
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
                var share = new ShareTicker {Name = evnt.Stock, Price = evnt.Price, Amount = evnt.Amount};
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
        }
    }

}
