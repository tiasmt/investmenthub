using System;

namespace App.API.Models
{
    public record DepositReq
    {
        public string User { get; set; }
        public int Amount { get; set; }
        public DateTime DateTime { get; set; }
    }

    public record DepositResp
    { }
}
