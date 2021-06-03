using System;
using System.Threading.Tasks;
using App.Core.Hubs;
using App.Core.Services;
using App.DataLayer;
using App.DataLayer.Entities;
using App.DataLayer.Repository;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;
using Xunit.Abstractions;

namespace tests
{

    public class PortfolioTests : IDisposable
    {
        private readonly InvestmentHubContext _dbContext;
        private readonly SQLRepository _repository;

        public PortfolioTests()
        {
            // The key to keeping the databases unique and not shared is 
            // generating a unique db name for each.
            string dbName = Guid.NewGuid().ToString();

            // Create a fresh service provider, and therefore a fresh 
            // InMemory database instance.
            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();

            // Create a new options instance telling the context to use an
            // InMemory database and the new service provider.
            var builder = new DbContextOptionsBuilder<InvestmentHubContext>();
            builder.UseInMemoryDatabase(dbName)
                   .UseInternalServiceProvider(serviceProvider);

            _dbContext = new InvestmentHubContext(builder.Options);
            _repository = new SQLRepository(_dbContext);
        }

        public void Dispose()
        {
            _dbContext.Dispose();
        }
        [Fact]
        public async Task SingleDeposit_SingleUser()
        {

            var mockHub = new Mock<IHubContext<PortfolioHub, IPortfolio>>();
            var all = new Mock<IPortfolio>();
            all.Setup(_ => _.UpdatePortfolio(new Portfolio())).Verifiable();
            mockHub.Setup(_ => _.Clients.All).Returns(all.Object);


            var portfolioService = new PortfolioService(_repository, mockHub.Object);
            var initialState = await portfolioService.GetState("User");
            Assert.Equal(0, initialState.Money);

            await portfolioService.Deposit("User", 10);
            var result = await portfolioService.GetState("User");
            Assert.Equal(10, result.Money);
        }

        [Fact]
        public async Task MultipleDeposit_SingleUser()
        {

            var mockHub = new Mock<IHubContext<PortfolioHub, IPortfolio>>();
            var all = new Mock<IPortfolio>();
            all.Setup(_ => _.UpdatePortfolio(new Portfolio())).Verifiable();
            mockHub.Setup(_ => _.Clients.All).Returns(all.Object);


            var portfolioService = new PortfolioService(_repository, mockHub.Object);
            var initialState = await portfolioService.GetState("User");
            Assert.Equal(0, initialState.Money);


            await portfolioService.Deposit("User", 10);
            var result = await portfolioService.GetState("User");
            Assert.Equal(10, result.Money);
        }
        [Fact]
        public async Task SingleDepositSingleWithdrawal_SingleUser()
        {

            var mockHub = new Mock<IHubContext<PortfolioHub, IPortfolio>>();
            var all = new Mock<IPortfolio>();
            all.Setup(_ => _.UpdatePortfolio(new Portfolio())).Verifiable();
            mockHub.Setup(_ => _.Clients.All).Returns(all.Object);


            var portfolioService = new PortfolioService(_repository, mockHub.Object);
            var initialState = await portfolioService.GetState("User");
            Assert.Equal(0, initialState.Money);

            await portfolioService.Deposit("User", 10);
            await portfolioService.Withdrawal("User", 5);
            var result = await portfolioService.GetState("User");
            Assert.Equal(5, result.Money);
        }

        [Fact]
        public async Task MultipleDepositMultipleWithdrawal_SingleUser()
        {

            var mockHub = new Mock<IHubContext<PortfolioHub, IPortfolio>>();
            var all = new Mock<IPortfolio>();
            all.Setup(_ => _.UpdatePortfolio(new Portfolio())).Verifiable();
            mockHub.Setup(_ => _.Clients.All).Returns(all.Object);


            var portfolioService = new PortfolioService(_repository, mockHub.Object);
            var initialState = await portfolioService.GetState("User");
            Assert.Equal(0, initialState.Money);

            await portfolioService.Deposit("User", 10);
            await portfolioService.Deposit("User", 5);
            await portfolioService.Withdrawal("User", 5);
            await portfolioService.Withdrawal("User", 10);

            var result = await portfolioService.GetState("User");
            Assert.Equal(0, result.Money);
        }

        [Fact]
        public async Task SingleDepositBuySingleStock_SingleUser()
        {

            var mockHub = new Mock<IHubContext<PortfolioHub, IPortfolio>>();
            var all = new Mock<IPortfolio>();
            all.Setup(_ => _.UpdatePortfolio(new Portfolio())).Verifiable();
            mockHub.Setup(_ => _.Clients.All).Returns(all.Object);


            var portfolioService = new PortfolioService(_repository, mockHub.Object);
            var initialState = await portfolioService.GetState("User");
            Assert.Equal(0, initialState.Money);

            await portfolioService.Deposit("User", 10);
            await portfolioService.BuyStock("User", "MSFT", 1, 1);

            var result = await portfolioService.GetState("User");
            Assert.Equal(9, result.Money);
            Assert.Equal(0, result.Profit);
            Assert.Equal(1, result.Shares[0].Amount);
            Assert.Equal("MSFT", result.Shares[0].Name);
            Assert.Equal(1, result.Shares[0].Price);
        }
        [Fact]
        public async Task SingleDepositBuyMultipleStocks_SingleUser()
        {

            var mockHub = new Mock<IHubContext<PortfolioHub, IPortfolio>>();
            var all = new Mock<IPortfolio>();
            all.Setup(_ => _.UpdatePortfolio(new Portfolio())).Verifiable();
            mockHub.Setup(_ => _.Clients.All).Returns(all.Object);


            var portfolioService = new PortfolioService(_repository, mockHub.Object);
            var initialState = await portfolioService.GetState("User");
            Assert.Equal(0, initialState.Money);

            await portfolioService.Deposit("User", 10);
            await portfolioService.BuyStock("User", "MSFT", 1, 1);
            await portfolioService.BuyStock("User", "MSFT", 1, 2);

            var result = await portfolioService.GetState("User");
            Assert.Equal(7, result.Money);
            Assert.Equal(0, result.Profit);
            Assert.Equal(2, result.Shares[0].Amount);
            Assert.Equal("MSFT", result.Shares[0].Name);
            Assert.Equal(1.5, result.Shares[0].Price);
        }
    }
}