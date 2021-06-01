using System;
using System.Threading.Tasks;
using App.Core.Hubs;
using App.Core.Services;
using App.DataLayer;
using App.DataLayer.Entities;
using App.DataLayer.Repository;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace tests
{

    public class SeedDepositDataFixture : IDisposable
    {
        public InvestmentHubContext InvestmentHubContext { get; private set; }
        public SeedDepositDataFixture()
        {
            var options = new DbContextOptionsBuilder<InvestmentHubContext>()
                    .UseInMemoryDatabase("Deposit")
                    .Options;
            InvestmentHubContext = new InvestmentHubContext(options);
        }

        public void Dispose()
        {
            InvestmentHubContext.Dispose();
        }
    }
    public class DepositTests : IClassFixture<SeedDepositDataFixture>
    {
        SeedDepositDataFixture _fixture;
        public DepositTests(SeedDepositDataFixture fixture)
        {
            _fixture = fixture;
        }
        
        [Fact]
        public async Task SingleDeposit()
        {
            
            var mockHub = new Mock<IHubContext<PortfolioHub, IPortfolio>>();
            var all = new Mock<IPortfolio>();
            all.Setup(_ => _.UpdatePortfolio(new Portfolio())).Verifiable();
            mockHub.Setup(_ => _.Clients.All).Returns(all.Object);
            
            var repo = new SQLRepository(_fixture.InvestmentHubContext);
            
            var portfolioService = new PortfolioService(repo, mockHub.Object);

            await portfolioService.Deposit("Matthias", 10);
            var result = await portfolioService.GetState("Matthias");
            Assert.Equal(10, result.Money);
        }
    }
}
