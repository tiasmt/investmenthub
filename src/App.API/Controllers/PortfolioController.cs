using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.API.Models;
using App.API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace App.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class PortfolioController : ControllerBase
    {
        private readonly ILogger<PortfolioController> _logger;
        private readonly IPortfolioService _portfolioService;

        public PortfolioController(ILogger<PortfolioController> logger, IPortfolioService portfolioService)
        {
            _logger = logger;
            _portfolioService = portfolioService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok();
        }

        [HttpPost]
        [Route("deposit")]
        public async Task<IActionResult> Deposit([FromBody] DepositReq deposit)
        {
            await _portfolioService.Deposit(deposit.User, deposit.Amount);
            return Ok();
        }
        [HttpPost]
        [Route("withdraw")]
        public async Task<IActionResult> Withdrawal([FromBody] WithdrawalReq withdrawal)
        {
            await _portfolioService.Deposit(withdrawal.User, withdrawal.Amount);
            return Ok();
        }
        [HttpPost]
        [Route("buy")]
        public async Task<IActionResult> BuyStock([FromBody] BuyStockReq buyRequest)
        {
            await _portfolioService.BuyStock(buyRequest.User, buyRequest.Stock, buyRequest.Amount, buyRequest.Price);
            return Ok();
        }
        [HttpPost]
        [Route("sell")]
        public async Task<IActionResult> SellStock([FromBody] SellStockReq sellRequest)
        {
            await _portfolioService.SellStock(sellRequest.User, sellRequest.Stock, sellRequest.Amount, sellRequest.Price);
            return Ok();
        }
    }
}
