using System;
using Newtonsoft.Json;

namespace Data.Models
{
    [Serializable]
    public class HeartRateVariabilityRmssdRecord
    {
        [JsonProperty("time")]
        public DateTime Time { get; set; }
        
        [JsonProperty("zoneOffset")]
        public string ZoneOffset { get; set; }
        
        [JsonProperty("heartRateVariabilityMillis")]
        public decimal HeartRateVariabilityMillis { get; set; }
        
        [JsonProperty("metadata")]
        public Metadata Metadata { get; set; }
    }
}