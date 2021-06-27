using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoListClient.Interfaces.Services;

namespace TodoListClient.Controllers
{
    [AllowAnonymous]
    public class PlayerDashboardController : Controller
    {
        private IPlayerDtoService _playerService;

        public PlayerDashboardController (IPlayerDtoService playerService)
        {
            _playerService = playerService;
        }

        public async Task<ActionResult> Index()
        { 
            return View(await _playerService.GetAsync());
        }
    }
}