using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.SignalR.Client;

namespace console
{
    class Program
    {
        static void Main(string[] args)
        {
            //Set connection
            var signalRConnection = new SignalRConnection();
            signalRConnection.Start();

            string x = "";
            while (x != "q")
            {
                x = Console.ReadLine();

            }

        }

        public class SignalRConnection
        {
            private HubConnection _connection;
            public async void Start()
            {
                var url = "http://localhost:5100/portfoliohub";

                _connection = new HubConnectionBuilder()
                    .WithUrl(url)
                    .WithAutomaticReconnect()
                    .Build();

                // receive a message from the hub
                _connection.On<Portfolio>("UpdatePortfolio", (porfolio) => UpdatePortfolio(porfolio));

                await _connection.StartAsync();
            }

            private void UpdatePortfolio(Portfolio portfolio)
            {
                System.Console.Clear();
                System.Console.WriteLine($"Money: {portfolio.Money:0.##}");
                if (portfolio.Shares != null)
                {
                    foreach (var stockOwned in portfolio.Shares)
                    {
                        System.Console.WriteLine($"Stock Ticker: {stockOwned.Name} Quantity: {stockOwned.Amount} Average Price: {stockOwned.Price:0.##} Profit: {portfolio.Profit:0.##}");
                    }
                }
            }

        }

        public class Portfolio
        {
            public double Money { get; set; }
            public List<ShareTicker> Shares { get; set; }
            public double Profit { get; set; }
        }

        public record ShareTicker
        {
            public string Name { get; set; }
            public int Amount { get; set; }
            public double Price { get; set; }
        }
    }
}
