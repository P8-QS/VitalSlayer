using System;
using Newtonsoft.Json;

namespace Data.Models
{
    [Serializable]
    public class Vo2MaxRecord
    {
        [JsonProperty("time")]
        public DateTime Time { get; set; }
        
        [JsonProperty("zoneOffset")]
        public string ZoneOffset { get; set; }
        
        [JsonProperty("metadata")]
        public Metadata Metadata { get; set; }
        
        [JsonProperty("vo2MillilitersPerMinuteKilogram")]
        public decimal Vo2MillilitersPerMinuteKilogram { get; set; }
        
        [JsonProperty("measurementMethod")]
        public int MeasurementMethod { get; set; }
    }
}