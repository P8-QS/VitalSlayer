using System;
using Newtonsoft.Json;

namespace Data.Models
{
    [Serializable]
    public class ActiveCaloriesBurnedRecord
    {
        [JsonProperty("startTime")]
        public DateTime StartTime { get; set; }
        
        [JsonProperty("startZoneOffset")]
        public string StartZoneOffset { get; set; }
        
        [JsonProperty("endTime")]
        public DateTime EndTime { get; set; }
        
        [JsonProperty("endZoneOffset")]
        public string EndZoneOffset { get; set; }
        
        [JsonProperty("energy")]
        public object Energy { get; set; } // TODO: Implement specific 'Energy' type
        
        [JsonProperty("metadata")]
        public Metadata Metadata { get; set; }
    }
}