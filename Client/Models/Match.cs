using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TodoListClient.Enums;

namespace TodoListClient.Models
{
    public class Match
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "matchApiId")]
        public int MatchApiId { get; set; }

        [JsonProperty(PropertyName = "homeTeam")]
        public Team HomeTeam { get; set; }

        [JsonProperty(PropertyName = "awayTeam")]
        public Team AwayTeam { get; set; }

        [JsonProperty(PropertyName = "homeTeamScore")]
        public int HomeTeamScore { get; set; }

        [JsonProperty(PropertyName = "AwayTeamScore")]
        public int AwayTeamScore { get; set; }

        [JsonProperty(PropertyName = "status")]
        public Status Status { get; set; }

        [JsonProperty(PropertyName = "matchType")]
        public MatchType MatchType { get; set; }

        [JsonProperty(PropertyName = "group")]
        public Group Group { get; set; }
    }
}
