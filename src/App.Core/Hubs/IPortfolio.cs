using System.Threading.Tasks;
using App.DataLayer.Entities;

namespace App.Core.Hubs
{
    public interface IPortfolio
    {
        Task UpdatePortfolio(Portfolio portfolio);
    }
}