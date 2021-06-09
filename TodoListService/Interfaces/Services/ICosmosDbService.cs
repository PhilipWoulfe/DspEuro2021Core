using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TodoListService.Models;

namespace TodoListService.Interfaces.Services
{
    public interface ICosmosDbService
    {
        Task<IEnumerable<User>> GetUsersAsync(string query);
        Task<User> GetUserAsync(string id);
        Task AddUserAsync(User user);
        Task UpdateUserAsync(string id, User user);
        Task DeleteUserAsync(string id);
    }
}
