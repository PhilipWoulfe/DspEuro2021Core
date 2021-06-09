using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TodoListService.Models;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Fluent;
using Microsoft.Extensions.Configuration;
using TodoListService.Interfaces.Services;
using User = TodoListService.Models.User;
using System;

namespace TodoListService.Services
{
    public class CosmosDbUserService : ICosmosUserDbService
    {
        private Container _container;

        public CosmosDbUserService(
            CosmosClient dbClient,
            string databaseName,
            string containerName)
        {
            this._container = dbClient.GetContainer(databaseName, containerName);
        }

        public async Task AddUserAsync(User user)
        {
            try
            {
                var v = await this._container.CreateItemAsync<User>(user, new PartitionKey(user.Id));
            }
            catch(Exception e)
            {

            }
        }

        public async Task DeleteUserAsync(string id)
        {
            await this._container.DeleteItemAsync<User>(id, new PartitionKey(id));
        }

        public async Task<User> GetUserAsync(string id)
        {
            try
            {
                ItemResponse<User> response = await this._container.ReadItemAsync<User>(id, new PartitionKey(id));
                return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }

        }

        public async Task<IEnumerable<User>> GetUsersAsync(string queryString)
        {
            var query = this._container.GetItemQueryIterator<User>(new QueryDefinition(queryString));
            List<User> results = new List<User>();
            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();

                results.AddRange(response.ToList());
            }

            return results;
        }

        public async Task UpdateUserAsync(string id, User User)
        {
            await this._container.UpsertItemAsync<User>(User, new PartitionKey(id));
        }
    }
}
