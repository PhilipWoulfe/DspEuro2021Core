using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web;
using System.Threading.Tasks;
using TodoListClient.Models;
using TodoListClient.Services;
using ToooListClient.Interfaces.Services;
//using UserService.Models;

namespace UserClient.Controllers
{
    public class UserController : Controller
    {
        private IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        // GET: User
        [AuthorizeForScopes(ScopeKeySection = "User:UserScope")]
        public async Task<ActionResult> Index()
        {
            return View(await _userService.GetAsync());
        }

        // GET: User/Details/5
        public async Task<ActionResult> Details(int id)
        {
            return View(await _userService.GetAsync(id));
        }

        // GET: User/Create
        public ActionResult Create()
        {
            var identity = HttpContext.User.Identity as System.Security.Claims.ClaimsIdentity;
            var userId = identity.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value;
            //var userFirstName = identity.FindFirst(System.Security.Claims.ClaimTypes.Name).Value;
            //var userSurname = identity.FindFirst(System.Security.Claims.ClaimTypes.Surname).Value;
            var username = identity.Name;
            var v = identity.Claims;
            User user = new User() { 
                Oid = userId,
                //FirstName = userFirstName,
                //Surname = userSurname,
                Username = username
            };
            return View(user);
        }

        // POST: User/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind("Oid,FirstName,Surname,Username")] User user)
        {
            await _userService.AddAsync(user);
            return RedirectToAction("Index");
        }

        // GET: User/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            User user = await this._userService.GetAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: User/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, [Bind("Id,Oid,FirstName,Surname,Username,IsPaid,IsAdmin,IsDeleted")] User user)
        {
            await _userService.EditAsync(user);
            return RedirectToAction("Index");
        }

        // GET: User/Delete/5
        public async Task<ActionResult> Delete(int id)
        {
            User user = await this._userService.GetAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: User/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id, [Bind("Id,Oid,FirstName,Surname")] User user)
        {
            await _userService.DeleteAsync(id);
            return RedirectToAction("Index");
        }
    }
}