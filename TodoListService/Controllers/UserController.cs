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
        private readonly ICosmosDbService _cosmosDbService;

        public UserController(IHttpContextAccessor contextAccessor, ICosmosDbService cosmosDbService)
        {
            this._contextAccessor = contextAccessor;
            _cosmosDbService = cosmosDbService;
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
        [HttpGet("{id}", Name = "Get")]
        public async Task<User> Get(string id)
        {
            return await _cosmosDbService.GetUserAsync(id);
        }

        //// GET: api/values
        //[HttpGet("{oid}", Name = "UserExistsByOid")]
        //public bool GetIdByOid(string oid)
        //{
        //    var exists = UserStore.Values.Select(t => t)
        //            .Where(t => t.Oid == oid)
        //            .Count() > 0;

        //    return exists;
        //}



        //[ActionName("Create")]
        //public IActionResult Create()
        //{
        //    return View();
        //}

        // POST api/values
        [HttpPost]
        //[ActionName("Create")]
        //[ValidateAntiForgeryToken]
        public async Task<User> Post([FromBody] User user)
        {
            if (Get(user.Id) != null)
            {
                if (ModelState.IsValid)
                {
                    user.Id = Guid.NewGuid().ToString();
                    await _cosmosDbService.AddUserAsync(user);
                    //return RedirectToAction("Index");
                }

                return user;
            }
            return user;
        }

        // PATCH api/values
        [HttpPatch("{id}")]
        //[ActionName("Edit")]
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

        //[ActionName("Edit")]
        //public async Task<ActionResult> EditAsync(string id)
        //{
        //    if (id == null)
        //    {
        //        return BadRequest();
        //    }

        //    User user = await _cosmosDbService.GetUserAsync(id);
        //    if (user == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(user);
        //}

        //[ActionName("Delete")]
        //public async Task<ActionResult> DeleteAsync(string id)
        //{
        //    if (id == null)
        //    {
        //        return BadRequest();
        //    }

        //    User item = await _cosmosDbService.GetUserAsync(id);
        //    if (item == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(item);
        //}

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete([Bind("Id")] string id)
        {
            await _cosmosDbService.DeleteUserAsync(id);
            return Ok();
        }

        //[ActionName("Details")]
        //public async Task<ActionResult> DetailsAsync(string id)
        //{
        //    return View(await _cosmosDbService.GetUserAsync(id));
        //}
    }
}