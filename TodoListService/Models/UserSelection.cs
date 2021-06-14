using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TodoListService.Models
{
    public class UserSelection
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "homeTeam")]
        public Team HomeTeam { get; set; }

        [JsonProperty(PropertyName = "awayTeam")]
        public Team AwayTeam { get; set; }

        [JsonProperty(PropertyName = "homeTeamScore")]
        public int? HomeTeamScore { get; set; }

        [JsonProperty(PropertyName = "awayTeamScore")]
        public int? AwayTeamScore { get; set; }
    }
}
