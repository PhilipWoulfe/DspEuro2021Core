using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web;
using System.Threading.Tasks;
using TodoListClient.Models;
using TodoListClient.Services;
using TodoListClient.Interfaces.Services;
using System;
using TodoListClient.Dtos;
using System.Collections.Generic;
//using MatchService.Models;

namespace MatchClient.Controllers
{
    [AllowAnonymous]
    public class PlayerDtoController : Controller
    {
        private IUserService _userService;
        private readonly IHttpContextAccessor _contextAccessor;
        private IPlayerDtoService _playerService;

        public PlayerDtoController(IPlayerDtoService playerService, IHttpContextAccessor contextAccessor, IUserService userService)
        {
            _playerService = playerService;
            _userService = userService;
            _contextAccessor = contextAccessor;
        }

        // GET: Match
        public async Task<ActionResult> Index()
        {
            return View(await _playerService.GetAsync());
        }

        // GET: Match/Details/5
        public async Task<ActionResult> Details(string id)
        {
            return View(await _playerService.GetAsync(id));
        }

        // GET: User/Edit/5
        public async Task<ActionResult> Edit(string id)
        {
            var isAdmin = await IsAdministrator();

            if (!isAdmin)
            {
                return NotFound();
            }

            PlayerDto player = await this._playerService.GetAsync(id);

            if (player == null)
            {
                return NotFound();
            }

            return View(player);
        }

        // POST: User/Edit/5
        [HttpPost]
        [ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(string id, PlayerDto player)
        {
            var isAdmin = await IsAdministrator();

            if (!isAdmin)
            {
                return NotFound();
            }

            await _playerService.EditAsync(id, player);
            return RedirectToAction("Index");
        }

        private async Task<bool> IsAdministrator()
        {
            var identity = _contextAccessor.HttpContext.User.Identity as System.Security.Claims.ClaimsIdentity;
            var user = await _userService.GetAsync(identity.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value);
            return user.IsAdmin;
        }

    }
}