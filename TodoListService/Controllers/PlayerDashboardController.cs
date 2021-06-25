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
      
        private readonly CosmosPlayerStatsDbService _cosmosPlayerStatsService;

        public PlayerDashboardController(CosmosPlayerStatsDbService cosmosPlayerStatsService)
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