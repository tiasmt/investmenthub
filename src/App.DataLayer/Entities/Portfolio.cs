using System.Collections.Generic;

namespace App.DataLayer.Entities
{
    public class Portfolio
    {
        public double Money { get; set; }
        public Dictionary<string, ShareTicker> Shares { get; set; }
        public double Profit { get; set; }
    }

    public record ShareTicker
    {
        public int Amount { get; set; }
        public double Price { get; set; }
    }
}