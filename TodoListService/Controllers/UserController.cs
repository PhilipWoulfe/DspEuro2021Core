/*
 The MIT License (MIT)

Copyright (c) 2018 Microsoft Corporation

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
 */

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TodoListService.Interfaces.Services;
using TodoListService.Models;

namespace TodoListService.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [RequiredScope(scopeRequiredByAPI)]
    public class UserController : Controller
    {
        const string scopeRequiredByAPI = "access_as_user";

        private readonly IHttpContextAccessor _contextAccessor;
        private readonly ICosmosUserDbService _cosmosDbService;
        private readonly ICosmosMatchDbService _cosmosMatchDbService;

        public UserController(IHttpContextAccessor contextAccessor, ICosmosUserDbService cosmosDbService, ICosmosMatchDbService cosmosMatchDbService)
        {
            this._contextAccessor = contextAccessor;
            _cosmosDbService = cosmosDbService;
            _cosmosMatchDbService = cosmosMatchDbService;
        }

        private string GetId()
        {
            var identity = _contextAccessor.HttpContext.User.Identity as System.Security.Claims.ClaimsIdentity;
            return identity.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value;
        }

        // GET: api/values
        [HttpGet]
        //[ActionName("Index")]
        public async Task<IEnumerable<User>> Get()
        {
            return await _cosmosDbService.GetUsersAsync("SELECT * FROM c");
        }

        // GET: api/values
        [HttpGet("{id}")]
        public async Task<User> Get(string id)
        {
            return await _cosmosDbService.GetUserAsync(id);
        }

        

        // POST api/values
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<User> Post([FromBody] User user)
        {
            if (ModelState.IsValid)
            {
                
                //return RedirectToAction("Index");
                var matches = await _cosmosMatchDbService.GetMatchesAsync("SELECT * FROM c");
                user.UserSelection = new();
                foreach (var match in matches)
                {
                    
                    user.UserSelection.Add(new UserSelection()
                    {
                        Id = match.Id,
                        HomeTeam = match.HomeTeam,
                        AwayTeam = match.AwayTeam
                    });
                }

                await _cosmosDbService.AddUserAsync(user);
            }

            return user;

        }

        // PATCH api/values
        [HttpPatch("{id}")]
        //[ValidateAntiForgeryToken]
        public async Task<User> Patch(string id, [FromBody] User user)
        {
            if (ModelState.IsValid)
            {
                await _cosmosDbService.UpdateUserAsync(id, user);
                //return RedirectToAction("Index");
            }

            return user;
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete([Bind("Id")] string id)
        {
            await _cosmosDbService.DeleteUserAsync(id);
            return Ok();
        }
    }
}