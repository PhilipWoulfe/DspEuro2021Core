// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Web;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using TodoListClient.Models;
using TodoListClient.Interfaces.Services;
//using TodoListService.Models;

namespace TodoListClient.Services
{
    public static class MatchServiceExtensions
    {
        public static void AddMatchService(this IServiceCollection services, IConfiguration configuration)
        {
            // https://docs.microsoft.com/en-us/dotnet/standard/microservices-architecture/implement-resilient-applications/use-httpclientfactory-to-implement-resilient-http-requests
            services.AddHttpClient<IMatchService, MatchService>();
        }
    }

    public class MatchService : IMatchService
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly HttpClient _httpClient;
        private readonly string _match = string.Empty;
        private readonly string _matchBaseAddress = string.Empty;
        private readonly ITokenAcquisition _tokenAcquisition;

        public MatchService(ITokenAcquisition tokenAcquisition, HttpClient httpClient, IConfiguration configuration, IHttpContextAccessor contextAccessor)
        {
            _httpClient = httpClient;
            _tokenAcquisition = tokenAcquisition;
            _contextAccessor = contextAccessor;
            _match = configuration["User:UserScope"];
            _matchBaseAddress = configuration["User:UserBaseAddress"];
        }

        public async Task<Match> AddAsync(Match match)
        {
            await PrepareAuthenticatedClient();

            var jsonRequest = JsonConvert.SerializeObject(match);
            var jsoncontent = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

            var response = await this._httpClient.PostAsync($"{ _matchBaseAddress}/api/match", jsoncontent);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var content = await response.Content.ReadAsStringAsync();
                match = JsonConvert.DeserializeObject<Match>(content);

                return match;
            }

            throw new HttpRequestException($"Invalid status code in the HttpResponseMessage: {response.StatusCode}.");
        }

        public async Task DeleteAsync(string id)
        {
            await PrepareAuthenticatedClient();

            var response = await this._httpClient.DeleteAsync($"{ _matchBaseAddress}/api/match/{id}");

            if (response.StatusCode == HttpStatusCode.OK)
            {
                return;
            }

            throw new HttpRequestException($"Invalid status code in the HttpResponseMessage: {response.StatusCode}.");
        }

        public async Task<Match> EditAsync(string id, Match match)
        {
            await PrepareAuthenticatedClient();

            var jsonRequest = JsonConvert.SerializeObject(match);
            var jsoncontent = new StringContent(jsonRequest, Encoding.UTF8, "application/json-patch+json");

            var response = await _httpClient.PatchAsync($"{ _matchBaseAddress}/api/match/{id}", jsoncontent);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var content = await response.Content.ReadAsStringAsync();
                match = JsonConvert.DeserializeObject<Match>(content);

                return match;
            }

            throw new HttpRequestException($"Invalid status code in the HttpResponseMessage: {response.StatusCode}.");
        }

        public async Task<IEnumerable<Match>> GetAsync()
        {
            await PrepareAuthenticatedClient();

            var response = await _httpClient.GetAsync($"{ _matchBaseAddress}/api/match");
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var content = await response.Content.ReadAsStringAsync();
                IEnumerable<Match> match = JsonConvert.DeserializeObject<IEnumerable<Match>>(content);

                return match;
            }

            throw new HttpRequestException($"Invalid status code in the HttpResponseMessage: {response.StatusCode}.");
        }

        public async Task<Match> GetAsync(string id)
        {
            await PrepareAuthenticatedClient();

            var response = await _httpClient.GetAsync($"{ _matchBaseAddress}/api/match/{id}");
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var content = await response.Content.ReadAsStringAsync();
                Match match = JsonConvert.DeserializeObject<Match>(content);

                return match;
            }

            throw new HttpRequestException($"Invalid status code in the HttpResponseMessage: {response.StatusCode}.");
        }

        public async Task<bool> GetMatchByOid(string oid)
        {
            await PrepareAuthenticatedClient();

            var response = await _httpClient.GetAsync($"{ _matchBaseAddress}/api/match/{oid}");
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var content = await response.Content.ReadAsStringAsync();
                bool match = JsonConvert.DeserializeObject<bool>(content);

                return match;
            }

            throw new HttpRequestException($"Invalid status code in the HttpResponseMessage: {response.StatusCode}.");
        }

        private async Task PrepareAuthenticatedClient()
        {
            var accessToken = await _tokenAcquisition.GetAccessTokenForUserAsync(new[] { _match });
            Debug.WriteLine($"access token-{accessToken}");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
    }
}