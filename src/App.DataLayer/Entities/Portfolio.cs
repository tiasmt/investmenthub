using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace App.DataLayer.Entities
{
    [Keyless]
    public class Portfolio
    {
        public double Money { get; set; }
        public List <ShareTicker> Shares { get; set; }
        public double Profit { get; set; }
    }

    public record ShareTicker
    {
        public string Name {get;set;}
        public int Amount { get; set; }
        public double Price { get; set; }
    }
}