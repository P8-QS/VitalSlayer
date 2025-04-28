using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Data.Models
{
    [Serializable]
    public class ExerciseSessionRecord
    {
        [JsonIgnore]
        public TimeSpan Duration => EndTime - StartTime;
        
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
        
        [JsonProperty("exerciseType")]
        public int ExerciseType { get; set; }
                
        [JsonProperty("title")]
        public string Title { get; set; }
        
        [JsonProperty("notes")]
        public string Notes { get; set; }
        
        [JsonProperty("segments")]
        public IReadOnlyCollection<ExerciseSegment> Segments { get; set; }
        
        [JsonProperty("laps")]
        public IReadOnlyCollection<ExerciseLap> Laps { get; set; }
        
        [JsonProperty("exerciseRoute")]
        public object ExerciseRoute { get; set; } // Needs separate perm, always null without
        
        [JsonProperty("plannedExerciseSessionId")]
        public string PlannedExerciseSessionId { get; set; }
    }
    
    [Serializable]
    public class ExerciseSegment
    {
        [JsonProperty("startTime")]
        public DateTime StartTime { get; set; }
        
        [JsonProperty("endTime")]
        public DateTime EndTime { get; set; }
        
        [JsonProperty("segmentType")]
        public int SegmentType { get; set; }
        
        [JsonProperty("repetitions")]
        public int Repetitions { get; set; }
    }
    
    [Serializable]
    public class ExerciseLap
    {
        [JsonProperty("startTime")]
        public DateTime StartTime { get; set; }
        
        [JsonProperty("endTime")]
        public DateTime EndTime { get; set; }
        
        [JsonProperty("length")]
        public object Length { get; set; } // TODO: add length object
    }
}