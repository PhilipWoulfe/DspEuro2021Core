using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoListService.Models;

namespace TodoListService.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    public class PlayerDashboardController : ControllerBase
    {
        [HttpGet]
        public ActionResult<IEnumerable<UserStats>> GetAll()
        {
            return new[]
            {
                new UserStats {Score = 6},
                new UserStats {Score = 23},
                new UserStats {Score = 89}
            };
        }
    }

}