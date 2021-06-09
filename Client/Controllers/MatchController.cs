using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web;
using System.Threading.Tasks;
using TodoListClient.Models;
using TodoListClient.Services;
using TodoListClient.Interfaces.Services;
//using MatchService.Models;

namespace MatchClient.Controllers
{
    [Authorize]
    public class MatchController : Controller
    {
        private IMatchService _matchService;

        public MatchController(IMatchService matchService)
        {
            _matchService = matchService;
        }

        // GET: Match
        [AuthorizeForScopes(ScopeKeySection = "User:UserScope")]
        public async Task<ActionResult> Index()
        {
            return View(await _matchService.GetAsync());
        }

        // GET: Match/Details/5
        public async Task<ActionResult> Details(string id)
        {
            return View(await _matchService.GetAsync(id));
        }

        // GET: User/Create
        public ActionResult Create()
        {
           return View(new Match());
        }

        // POST: Match/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind("MatchApiId,HomeTeam,AwayTeam,HomeTeamScore,AwayTeamScore,Status,MatchType,Group")] Match match)
        {
            await _matchService.AddAsync(match);
            return RedirectToAction("Index");
        }

        // GET: Match/Edit/5
        public async Task<ActionResult> Edit(string id)
        {
            Match match = await this._matchService.GetAsync(id);

            if (match == null)
            {
                return NotFound();
            }

            return View(match);
        }

        // POST: Match/Edit/5
        [HttpPost]
        [ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(string id, [Bind("Id,MatchApiId,HomeTeam,AwayTeam,HomeTeamScore,AwayTeamScore,Status,MatchType,Group")] Match match)
        {
            await _matchService.EditAsync(id, match);
            return RedirectToAction("Index");
        }

        // GET: Match/Delete/5
        public async Task<ActionResult> Delete(string id)
        {
            Match match = await this._matchService.GetAsync(id);

            if (match == null)
            {
                return NotFound();
            }

            return View(match);
        }

        // POST: Match/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(string id, [Bind("Id,MatchApiId,HomeTeam,AwayTeam,HomeTeamScore,AwayTeamScore,Status,MatchType,Group")] Match match)
        {
            await _matchService.DeleteAsync(id);
            return RedirectToAction("Index");
        }
    }
}