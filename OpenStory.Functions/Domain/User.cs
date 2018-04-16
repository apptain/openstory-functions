using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Apptain.Functions.Domain
{
    public class User
    {
        public User()
        {
            Id = Guid.NewGuid();
            DateCreated = DateTime.Now;
        }
    
        [JsonProperty(PropertyName = "id")]
        public Guid Id { get; }
        [JsonProperty(PropertyName = "dateCreated")]
        public DateTime DateCreated { get; }

        [JsonProperty(PropertyName = "fistName")]
        public string FirstName { get; set; }
        [JsonProperty(PropertyName = "lastName")]
        public string LastName { get; set; }
        [JsonProperty(PropertyName = "email")]
        public string Email { get; set; }

        [JsonProperty(PropertyName = "handle")]
        public string Handle { get; set; }

        [JsonProperty(PropertyName = "facebookId")]
        public string FacebookId { get; set; }
        [JsonProperty(PropertyName = "facebookHandle")]
        public string FacebookHandle { get; set; }

        [JsonProperty(PropertyName = "twitterId")]
        public string TwitterId { get; set; }
        [JsonProperty(PropertyName = "twitterHandle")]
        public string TwitterHandle { get; set; }
    }
}
