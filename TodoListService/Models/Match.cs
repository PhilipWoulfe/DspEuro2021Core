using Newtonsoft.Json;
using System;
using TodoListService.Enums;

namespace TodoListService.Models
{
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
}
