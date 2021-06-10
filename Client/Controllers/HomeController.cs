using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using TodoListClient.Models;
using TodoListClient.Interfaces.Services;

namespace TodoListClient.Controllers
{
    
    public class HomeController : Controller
    {
        //private readonly ITokenAcquisition tokenAcquisition;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IUserService _userService;

        public HomeController(IHttpContextAccessor contextAccessor, IUserService userService)
        {
            //this.tokenAcquisition = tokenAcquisition;
            this._contextAccessor = contextAccessor;
            this._userService = userService;
        }

        [AllowAnonymous]
        public IActionResult Index()
        {
            var identity = _contextAccessor.HttpContext.User.Identity as System.Security.Claims.ClaimsIdentity;
            if (_contextAccessor.HttpContext.User.Identity.IsAuthenticated) { 
                var userFirstName = identity.FindFirst(System.Security.Claims.ClaimTypes.GivenName).Value;
                var userSurname = identity.FindFirst(System.Security.Claims.ClaimTypes.Surname).Value;
                var userId = identity.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value;
                var username = identity.Name;

                foreach (var claim in identity.Claims)
                {
                    if (claim.Type == "newUser")
                    {
                        if (claim.Value == "true")
                        {

                            //var x = await _userService.GetAsync(1);
                            //var v = await _userService.GetUserByOid(userId);

                            _userService.AddAsync(new User
                            {
                                Id = userId,
                                FirstName = userFirstName,
                                Surname = userSurname,
                                Username = username
                            });

                            break;
                        }
                    }
                }
            }

            return View();
        }

        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}