using System;
using Newtonsoft.Json;

namespace Data.Models
{
    [Serializable]
    public class Energy
    {
        [JsonProperty("type")]
        public string Type { get; set; }
        
        [JsonProperty("value")]
        public decimal Value { get; set; }
    }
}