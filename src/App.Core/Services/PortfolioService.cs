using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Core.Hubs;
using App.DataLayer.Entities;
using App.DataLayer.Events;
using App.DataLayer.Repository;
using Microsoft.AspNetCore.SignalR;

namespace App.Core.Services
{
    public class PortfolioService : IPortfolioService
    {

        private readonly List<IEvent> _events = new List<IEvent>();
        private readonly List<IEvent> _uncommittedevents = new List<IEvent>();
        public int Version { get; protected set; }
        //Projection (Current State)
        private Portfolio _portfolioState;
        private readonly IRepository _repository;
        private readonly IHubContext<PortfolioHub, IPortfolio> _portfolioHub;

        public PortfolioService(IRepository repository, IHubContext<PortfolioHub, IPortfolio> portfolioHub)
        {
            _repository = repository;
            _portfolioHub = portfolioHub;
        }

        public async Task Deposit(string username, int quantity)
        {
            await ApplyEvent(new DepositMade(username, quantity, DateTime.UtcNow));
        }

        public async Task Withdrawal(string username, int quantity)
        {
            await ApplyEvent(new WithdrawalMade(username, quantity, DateTime.UtcNow));
        }

        public async Task BuyStock(string username, string stock, int quantity, double price)
        {
            await ApplyEvent(new SharesBought(username, stock, quantity, price, DateTime.UtcNow));
        }
        public async Task SellStock(string username, string stock, int quantity, double price)
        {
            await ApplyEvent(new SharesSold(username, stock, quantity, price, DateTime.UtcNow));
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

        public async Task ApplyEvent(IEvent evnt, bool isFastForward = false)
        {
            if (_portfolioState == null)
                await GetPortfolio(evnt.User);

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
            {
                _uncommittedevents.Add(evnt);
                await _portfolioHub.Clients.All.UpdatePortfolio(_portfolioState);
            }
            else
            {
                _events.Add(evnt);
            }

            await _repository.Save(_uncommittedevents, _portfolioState);


        }

        public async Task<Portfolio> GetState(string username)
        {
            await GetPortfolio(username);
            return _portfolioState;
        }

        private async Task GetPortfolio(string username)
        {
            var snapshot = await _repository.GetSnapshot(username);
            _portfolioState = snapshot.State;
            var events = await _repository.GetEvents(username, snapshot.Version);
            foreach (var evnt in events)
            {
                await ApplyEvent(evnt, true);
            }
        }
    }

}
