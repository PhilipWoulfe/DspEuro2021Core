using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TodoListService.Models;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Fluent;
using Microsoft.Extensions.Configuration;
using TodoListService.Interfaces.Services;
using System;

namespace TodoListService.Services
{
    public class CosmosDbMatchService : ICosmosMatchDbService
    {
        private Container _container;

        public CosmosDbMatchService(
            CosmosClient dbClient,
            string databaseName,
            string containerName)
        {
            this._container = dbClient.GetContainer(databaseName, containerName);
        }

        public async Task AddMatchAsync(Match match)
        {
            try
            {
                var v = await this._container.CreateItemAsync<Match>(match, new PartitionKey(match.Id));
            }
            catch(Exception e)
            {

            }
        }

        public async Task DeleteMatchAsync(string id)
        {
            await this._container.DeleteItemAsync<Match>(id, new PartitionKey(id));
        }

        public async Task<Match> GetMatchAsync(string id)
        {
            try
            {
                ItemResponse<Match> response = await this._container.ReadItemAsync<Match>(id, new PartitionKey(id));
                return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }

        }

        public async Task<IEnumerable<Match>> GetMatchesAsync(string queryString)
        {
            var query = this._container.GetItemQueryIterator<Match>(new QueryDefinition(queryString));
            List<Match> results = new List<Match>();
            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();

                results.AddRange(response.ToList());
            }

            return results;
        }

        public async Task UpdateMatchAsync(string id, Match Match)
        {
            await this._container.UpsertItemAsync<Match>(Match, new PartitionKey(id));
        }
    }
}
