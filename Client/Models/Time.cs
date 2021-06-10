using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TodoListClient.Models
{
    public class Time
    {
        [JsonProperty(PropertyName = "homeTeam")]
        public int? HomeTeam { get; set; }

        [JsonProperty(PropertyName = "awayTeam")]
        public int? AwayTeam { get; set; }
    }
}
