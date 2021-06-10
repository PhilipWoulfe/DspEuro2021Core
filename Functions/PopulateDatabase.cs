using System;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using System.IO;
using System.Runtime.Serialization;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Microsoft.Azure.WebJobs;

namespace DspEuro2021.Functions
{
    public static class PopulateDatabase
    {

        private static readonly HttpClient client = new HttpClient();
        private static ILogger _logger;

        [FunctionName("PopulateDatabase")]
        public static async void Run(
            [TimerTrigger("0 */1 * * * *")] TimerInfo myTimer,
            [CosmosDB(
                databaseName: "Euro",
                collectionName: "Match",
                ConnectionStringSetting = "CosmosDbConnectionString")]IAsyncCollector<dynamic> documentsOut,
            ILogger log)
        {

            _logger = log;
            var config = new ConfigurationBuilder()
                .SetBasePath(Environment.CurrentDirectory)
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            var footBallDataApiKey = config["fotballDataApiKey"];
            var footBallDataMatchEndpoint = config["footBallDataMatchEndpoint"];
            var footBallDataTeamEndpoint = config["footBallDataTeamEndpoint"];
            var cosmosDbConnection = config.GetConnectionString("CosmosDbConnectionString");

            // _logger = context.GetLogger("PopulateDatabase");
            _logger.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            var v = await DeserializeOptimizedFromStreamCallAsync(footBallDataApiKey, footBallDataMatchEndpoint);

            // get cosmos input 
            // https://docs.microsoft.com/en-us/azure/azure-functions/functions-bindings-cosmosdb-v2-input?tabs=csharp

            Match[] matches = v.Matches;

            if (matches != null)
            {
                foreach (var match in matches)
                {

                    var id = match.Id;
                    var utcDate = match.UtcDate;
                    var status = match.Status;
                    var stage = match.Stage;
                    var group = match.Group;
                    var lastUpdated = match.LastUpdated;
                    var score = match.Score;
                    var homeTeam = match.HomeTeam;
                    var awayTeam = match.AwayTeam;

                    await documentsOut.AddAsync(new
                    {
                        id,
                        utcDate,
                        status,
                        stage,
                        group,
                        lastUpdated,
                        score,
                        homeTeam,
                        awayTeam
                    });
                }
            }

            _logger.LogInformation($"Next timer schedule at: {myTimer.ScheduleStatus.Next}");
        }

        private static async Task<JsonResponse> DeserializeOptimizedFromStreamCallAsync(string apiKey, string endPoint)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("X-Auth-Token", apiKey);
                using (var request = new HttpRequestMessage(HttpMethod.Get, endPoint))
                using (var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead))
                {
                    var stream = await response.Content.ReadAsStreamAsync();

                    if (response.IsSuccessStatusCode)
                        return DeserializeJsonFromStream<JsonResponse>(stream);

                    var content = await StreamToStringAsync(stream);
                    throw new ApiException
                    {
                        StatusCode = (int)response.StatusCode,
                        Content = content
                    };
                }
            }


        }


        // private static async Task ProcessRepositories(string apiKey, string endPoint)
        // {
        //     client.DefaultRequestHeaders.Accept.Clear();
        //     client.DefaultRequestHeaders.Add("X-Auth-Token", apiKey);
        //     client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        //     string stringTask;

        //     try
        //     {
        //         stringTask = await client.GetStringAsync(endPoint);
        //     }
        //     catch (Exception ex)
        //     {
        //         stringTask = null;
        //         _logger.LogError(ex.Message);
        //     }

        //     var msg = stringTask;
        // }


        private static async Task<string> StreamToStringAsync(Stream stream)
        {
            string content = null;

            if (stream != null)
                using (var sr = new StreamReader(stream))
                    content = await sr.ReadToEndAsync();

            return content;
        }

        private static T DeserializeJsonFromStream<T>(Stream stream)
        {
            if (stream == null || stream.CanRead == false)
                return default(T);

            using (var sr = new StreamReader(stream))
            using (var jtr = new JsonTextReader(sr))
            {
                var js = new JsonSerializer();
                var searchResult = js.Deserialize<T>(jtr);
                return searchResult;
            }
        }
    }

    public class ApiException : Exception
    {
        public int StatusCode { get; set; }

        public string Content { get; set; }
    }

    public class JsonResponse
    {

        [JsonProperty(PropertyName = "matches")]
        public Match[] Matches;
    }

    public class Match
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "utcDate")]
        public DateTime UtcDate { get; set; }

        [JsonProperty(PropertyName = "status")]
        public Status Status { get; set; }

        [JsonProperty(PropertyName = "stage")]
        public Stage Stage { get; set; }

        [JsonProperty(PropertyName = "group")]
        public Group? Group { get; set; }

        [JsonProperty(PropertyName = "lastUpdated")]
        public DateTime LastUpdated { get; set; }

        [JsonProperty(PropertyName = "score")]
        public Score Score { get; set; }

        [JsonProperty(PropertyName = "homeTeam")]
        public Team HomeTeam { get; set; }

        [JsonProperty(PropertyName = "awayTeam")]
        public Team AwayTeam { get; set; }
    }

    public class Team
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
    }

    public class Score
    {
        [JsonProperty(PropertyName = "winner")]
        public string Winner { get; set; }

        [JsonProperty(PropertyName = "fulltime")]
        public Time FullTime { get; set; }

        [JsonProperty(PropertyName = "extraTime")]
        public Time ExtraTime { get; set; }

        [JsonProperty(PropertyName = "penalties")]
        public Time Penalties { get; set; }
    }

    public class Time
    {
        [JsonProperty(PropertyName = "homeTeam")]
        public int? HomeTeam { get; set; }

        [JsonProperty(PropertyName = "awayTeam")]
        public int? AwayTeam { get; set; }
    }

    public enum Group
    {
        [EnumMember(Value = "Group A")]
        GROUP_A,
        [EnumMember(Value = "Group B")]
        GROUP_B,
        [EnumMember(Value = "Group C")]
        GROUP_C,
        [EnumMember(Value = "Group D")]
        GROUP_D,
        [EnumMember(Value = "Group E")]
        GROUP_E,
        [EnumMember(Value = "Group F")]
        GROUP_F
    }

    public enum Stage
    {
        GROUP_STAGE,
        LAST_16,
        QUARTER_FINAL,
        SEMI_FINAL,
        FINAL
    }

    public enum Status
    {
        SCHEDULED,
        LIVE,
        IN_PLAY,
        PAUSED,
        FINISHED,
        POSTPONED,
        SUSPENDED,
        CANCELED
    }


    public class MyInfo
    {
        public MyScheduleStatus ScheduleStatus { get; set; }

        public bool IsPastDue { get; set; }
    }

    public class MyScheduleStatus
    {
        public DateTime Last { get; set; }

        public DateTime Next { get; set; }

        public DateTime LastUpdated { get; set; }
    }
}
