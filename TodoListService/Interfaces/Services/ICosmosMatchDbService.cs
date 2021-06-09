using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TodoListService.Models;

namespace TodoListService.Interfaces.Services
{
    public interface ICosmosMatchDbService
    {
        Task<IEnumerable<Match>> GetMatchesAsync(string query);
        Task<Match> GetMatchAsync(string id);
        Task AddMatchAsync(Match match);
        Task UpdateMatchAsync(string id, Match match);
        Task DeleteMatchAsync(string id);
    }
}
