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
using TodoListService.Models;

namespace TodoListService.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [RequiredScope(scopeRequiredByAPI)]
    public class UserController : Controller
    {
        const string scopeRequiredByAPI = "access_as_user";
        // In-memory TodoList
        private static readonly Dictionary<int, User> UserStore = new Dictionary<int, User>();

        private readonly IHttpContextAccessor _contextAccessor;

        public UserController(IHttpContextAccessor contextAccessor)
        {
            this._contextAccessor = contextAccessor;

            // Pre-populate with sample data
            if (UserStore.Count == 0)
            {
                UserStore.Add(1, new User() { 
                    Id = 1, 
                    Oid = $"3fd3c66b-2957-4869-9559-9927ad0c7577",
                    FirstName = "Philip",
                    Surname = "Woulfe",
                    UserName = "PhilipWoulfe",
                    IsPaid = true,
                    IsAdmin = true,
                    CreatedDate = DateTime.Today,
                    LastAmendedDate = DateTime.Today,
                    UpdatedBy = 1

                });
            }
        }

        private string GetId()
        {
            var identity = _contextAccessor.HttpContext.User.Identity as System.Security.Claims.ClaimsIdentity;
            return identity.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value;
        }

        // GET: api/values
        [HttpGet]
        public IEnumerable<User> Get()
        {
            return UserStore.Values;
        }

        // GET: api/values
        [HttpGet("{id}", Name = "Get")]
        public User Get(int id)
        {
            return UserStore.Values.FirstOrDefault(t => t.Id == id);
        }

        // GET: api/values
        [HttpGet("{oid}", Name = "UserExistsByOid")]
        public bool GetIdByOid(string oid)
        {
            var exists = UserStore.Values.Select(t => t)
                    .Where(t => t.Oid == oid)
                    .Count() > 0;

            return exists;
        }

        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            UserStore.Remove(id);
        }

        // POST api/values
        [HttpPost]
        public IActionResult Post([FromBody] User user)
        {
            if (Get(user.Id) != null)
            {
                int id = UserStore.Values.OrderByDescending(x => x.Id).FirstOrDefault().Id + 1;
                User userNew = new User()
                {
                    Id = id,
                    Oid = user.Oid,
                    FirstName = user.FirstName,
                    Surname = user.Surname,
                    UserName = user.UserName,
                    IsPaid = user.IsPaid,
                    IsAdmin = user.IsAdmin,
                    CreatedDate = DateTime.Today,
                    LastAmendedDate = DateTime.Today,
                    UpdatedBy = 1

                };

                UserStore.Add(id, userNew);
            }
            return Ok(user);
        }

        // PATCH api/values
        [HttpPatch("{id}")]
        public IActionResult Patch(int id, [FromBody] User user)
        {
            if (id != user.Id)
            {
                return NotFound();
            }

            if (UserStore.Values.FirstOrDefault(x => x.Id == id) == null)
            {
                return NotFound();
            }

            UserStore.Remove(id);
            UserStore.Add(id, user);

            return Ok(user);
        }
    }
}