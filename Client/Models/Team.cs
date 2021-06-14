using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TodoListClient.Models
{
    public class Team
    {

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
    }
}
