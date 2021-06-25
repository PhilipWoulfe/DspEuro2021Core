using System.Threading.Tasks;
using TodoListService.Models;

namespace TodoListService.Interfaces.Services
{
    public interface ICosmosPlayerStatsDbService
    {
        Task<UserStats> GetUserStats();
    }
}