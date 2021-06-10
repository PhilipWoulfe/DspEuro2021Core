using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TodoListService.Models
{
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
}
