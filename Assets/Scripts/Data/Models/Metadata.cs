using System;
using Newtonsoft.Json;

namespace Data.Models
{
    [Serializable]
    public class Metadata
    {
        [JsonProperty("clientRecordVersion")]
        public int ClientRecordVersion { get; set; }
        
        [JsonProperty("dataOrigin")]
        public DataOrigin DataOrigin { get; set; }
        
        [JsonProperty("device")]
        public Device Device { get; set; }
        
        [JsonProperty("id")]
        public string Id { get; set; }
        
        [JsonProperty("lastModifiedTime")]
        public object LastModifiedTime { get; set; }
        
        [JsonProperty("recordingMethod")]
        public int RecordingMethod { get; set; }
    }
    
    [Serializable]
    public class DataOrigin
    {
        [JsonProperty("packageName")]
        public string PackageName { get; set; }
    }
    
    [Serializable]
    public class Device
    {
        [JsonProperty("type")]
        public int Type { get; set; }
    }
}