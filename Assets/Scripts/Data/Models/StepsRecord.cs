using System;
using Newtonsoft.Json;

namespace Data.Models
{
    [Serializable]
    public class StepsRecord
    {
        [JsonProperty("startTime")]
        public DateTime StartTime { get; set; }
        
        [JsonProperty("startZoneOffset")]
        public string StartZoneOffset { get; set; }
        
        [JsonProperty("endTime")]
        public DateTime EndTime { get; set; }
        
        [JsonProperty("endZoneOffset")]
        public string EndZoneOffset { get; set; }
        
        [JsonProperty("count")]
        public int StepsCount { get; set; }
        
        [JsonProperty("metadata")]
        public Metadata Metadata { get; set; }
    }
}