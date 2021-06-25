using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoListService.Interfaces.Services;
using TodoListService.Models;
using TodoListService.Services;

namespace TodoListService.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    public class PlayerDashboardController : Controller
    {
      
        private readonly ICosmosPlayerStatsDbService _cosmosPlayerStatsService;

        public PlayerDashboardController(ICosmosPlayerStatsDbService cosmosPlayerStatsService)
        {
            _cosmosPlayerStatsService = cosmosPlayerStatsService;
          
        }
        
        [HttpGet]
        public async Task<UserStats> Get()
        {
            return await _cosmosPlayerStatsService.GetUserStats();
        }
    }

}