using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Web;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace TodoListClient.Services
{
    public class BaseService
    {
        protected readonly IHttpContextAccessor _contextAccessor;
        protected readonly string _UserBaseAddress = string.Empty;
        protected readonly ITokenAcquisition _tokenAcquisition;
        protected readonly HttpClient _httpClient;
        protected readonly string User = string.Empty;

        public BaseService(ITokenAcquisition tokenAcquisition, HttpClient httpClient, IConfiguration configuration, IHttpContextAccessor contextAccessor)
        {
            _tokenAcquisition = tokenAcquisition;
            _httpClient = httpClient;
            User = configuration["User:UserScope"];
            _contextAccessor = contextAccessor;
            _UserBaseAddress = configuration["User:UserBaseAddress"];
        }
        protected async Task PrepareAuthenticatedClient()
        {
            var accessToken = await _tokenAcquisition.GetAccessTokenForUserAsync(new[] { User });
            Debug.WriteLine($"access token-{accessToken}");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
    }
}
