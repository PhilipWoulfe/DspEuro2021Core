using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using TodoListService.Interfaces.Services;
using TodoListService.Models;

namespace TodoListService.Services
{
    public class CosmosPlayerStatsDbService
    {
        private Container _container;
        
        public CosmosPlayerStatsDbService(
            CosmosClient dbClient,
            string databaseName,
            string containerName
            )
        {
            this._container = dbClient.GetContainer(databaseName, containerName);
        }
        
        public async Task<UserStats> GetUserStats()
        {
            var returnMe = new UserStats();

            returnMe.name = "Ryan";
            returnMe.Score = 35;

            await Task.Delay(2000);
            
            // var response = await this._container.ReadContainerAsync();
            //
            // return response;

            return returnMe;
        }
    }
}
