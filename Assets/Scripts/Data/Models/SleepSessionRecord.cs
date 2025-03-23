using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Data.Models
{
    [Serializable]
    public class SleepSessionRecord
    {
        [JsonIgnore]
        public TimeSpan SleepTime => EndTime - StartTime;
        
        [JsonProperty("startTime")]
        public DateTime StartTime { get; set; }
        
        [JsonProperty("startZoneOffset")]
        public string StartZoneOffset { get; set; }
        
        [JsonProperty("endTime")]
        public DateTime EndTime { get; set; }
        
        [JsonProperty("endZoneOffset")]
        public string EndZoneOffset { get; set; }
        
        [JsonProperty("metadata")]
        public Metadata Metadata { get; set; }
        
        [JsonProperty("title")]
        public string Title { get; set; }
        
        [JsonProperty("notes")]
        public string Notes { get; set; }
        
        [JsonProperty("stages")]
        public IReadOnlyCollection<SleepStage> Stages { get; set; }
    }

    [Serializable]
    public class SleepStage
    {
        [JsonProperty("startTime")]
        public DateTime StartTime { get; set; }
        
        [JsonProperty("endTime")]
        public DateTime EndTime { get; set; }
        
        [JsonProperty("stage")]
        public int Stage { get; set; }
    }
}