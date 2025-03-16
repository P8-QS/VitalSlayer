using System;

namespace Data.Models
{
    [Serializable]
    public class StepsRecord
    {
        public DateTime StartTime { get; set; }
        public string StartZoneOffset { get; set; }
        public DateTime EndTime { get; set; }
        public string EndZoneOffset { get; set; }
        public int Count { get; set; }
        public Metadata Metadata { get; set; }
    }
}