using Newtonsoft.Json;
using System;
using TodoListService.Enums;
using TodoListService.Interfaces.Models;

namespace TodoListService.Models
{
    public class Match : IEntity
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

        [JsonProperty(PropertyName = "createdDate")]
        public DateTime CreatedDate { get; set; }

        [JsonProperty(PropertyName = "lastAmendedDate")]
        public DateTime LastAmendedDate { get; set; }

        [JsonProperty(PropertyName = "updatedBy")]
        public int UpdatedBy { get; set; }

    }
}
