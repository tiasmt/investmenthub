using System.Threading.Tasks;
using App.DataLayer.Entities;
using Microsoft.AspNetCore.SignalR;

namespace App.Core.Hubs
{
    public class PortfolioHub : Hub<IPortfolio>
    {
        public async Task SendPortfolio(Portfolio portfolio)
        {
            await Clients.All.UpdatePortfolio(portfolio);
        }
    }
}