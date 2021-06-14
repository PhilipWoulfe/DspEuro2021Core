using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TodoListService.Models
{
    public class User
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "firstName")]
        public string FirstName { get; set; }

        [JsonProperty(PropertyName = "surname")]
        public string Surname { get; set; }

        [JsonProperty(PropertyName = "username")]
        public string Username { get; set; }

        [JsonProperty(PropertyName = "isPaid")]
        public bool IsPaid { get; set; }

        [JsonProperty(PropertyName = "isAdmin")]
        public bool IsAdmin { get; set; }

        [JsonProperty(PropertyName = "isDeleted")]
        public bool IsDeleted { get; set; }

        [JsonProperty(PropertyName = "createdDate")]
        public DateTime CreatedDate { get; set; }
        
        [JsonProperty(PropertyName = "lastAmendedDate")]
        public DateTime LastAmendedDate { get; set; }

        [JsonProperty(PropertyName = "updatedBy")]
        public int UpdatedBy { get; set; }

        [JsonProperty(PropertyName = "userSelection")]
        public List<UserSelection> UserSelection { get; set; }

        [JsonProperty(PropertyName = "goldenBoot")]
        public string GoldenBoot { get; set; }

    }
}
