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
using TodoListClient.Dtos;
//using TodoListService.Models;

namespace TodoListClient.Services
{

    public static class PlayerDtoServiceExtensions
    {
        public static void AddPlayerService(this IServiceCollection services, IConfiguration configuration)
        {
            // https://docs.microsoft.com/en-us/dotnet/standard/microservices-architecture/implement-resilient-applications/use-httpclientfactory-to-implement-resilient-http-requests
            services.AddHttpClient<IPlayerDtoService, PlayerDtoService>();
        }
    }

    public class PlayerDtoService : IPlayerDtoService
    {
        //private readonly IHttpContextAccessor _contextAccessor;
        private readonly HttpClient _httpClient;
        private readonly string _user = string.Empty;
        private readonly string _userBaseAddress = string.Empty;
        private readonly ITokenAcquisition _tokenAcquisition;

        public PlayerDtoService(HttpClient httpClient, IConfiguration configuration, ITokenAcquisition tokenAcquisition)
        {
            _httpClient = httpClient;
            _tokenAcquisition = tokenAcquisition;
            _userBaseAddress = configuration["User:UserBaseAddress"];
        }

    
        public async Task<IEnumerable<PlayerDto>> GetAsync()
        {
            //await PrepareAuthenticatedClient();

            var response = await _httpClient.GetAsync($"{ _userBaseAddress}/api/playerdto");
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var content = await response.Content.ReadAsStringAsync();
                IEnumerable<PlayerDto> player = JsonConvert.DeserializeObject<IEnumerable<PlayerDto>>(content);

                return player;
            }

            throw new HttpRequestException($"Invalid status code in the HttpResponseMessage: {response.StatusCode}.");
        }

        private async Task PrepareAuthenticatedClient()
        {
            var accessToken = await _tokenAcquisition.GetAccessTokenForUserAsync(new[] { _user });
            Debug.WriteLine($"access token-{accessToken}");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<PlayerDto> GetAsync(string id)
        {
            //await PrepareAuthenticatedClient();

            var response = await _httpClient.GetAsync($"{ _userBaseAddress}/api/playerdto/{id}");
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var content = await response.Content.ReadAsStringAsync();
                PlayerDto player = JsonConvert.DeserializeObject<PlayerDto>(content);

                return player;
            }

            throw new HttpRequestException($"Invalid status code in the HttpResponseMessage: {response.StatusCode}.");
        }

        public async Task<PlayerDto> EditAsync(string id, PlayerDto user)
        {
            await PrepareAuthenticatedClient();

            var jsonRequest = JsonConvert.SerializeObject(user);
            var jsoncontent = new StringContent(jsonRequest, Encoding.UTF8, "application/json-patch+json");

            var response = await _httpClient.PatchAsync($"{ _userBaseAddress}/api/playerdto/{id}", jsoncontent);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var content = await response.Content.ReadAsStringAsync();
                user = JsonConvert.DeserializeObject<PlayerDto>(content);

                return user;
            }

            throw new HttpRequestException($"Invalid status code in the HttpResponseMessage: {response.StatusCode}.");
        }
    }
}