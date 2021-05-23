using System;

namespace App.API.Models
{
    public record WithdrawalReq
    {  
        public string User { get; set; }
        public int Amount { get; set; }
        public DateTime DateTime { get; set; }
    }
    public record WithdrawalResp
    {  }
}