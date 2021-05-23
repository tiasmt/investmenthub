using System;

namespace App.API.Models
{
    public record SellStockReq
    {
        public string Stock { get; set; }
        public string User { get; set; }
        public int Amount { get; set; }
        public double Price { get; set; }
        public DateTime DateTime { get; set; }
    }

    public record SellStockResp
    { }
}