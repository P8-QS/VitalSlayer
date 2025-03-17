using System;
using System.Collections.Generic;

namespace Data.Models
{
    [Serializable]
    public class SleepSessionRecord
    {
        public TimeSpan SleepTime => EndTime - StartTime;
        public DateTime StartTime { get; set; }
        public string StartZoneOffset { get; set; }
        public DateTime EndTime { get; set; }
        public string EndZoneOffset { get; set; }
        public Metadata Metadata { get; set; }
        public string Title { get; set; }
        public string Notes { get; set; }
        public IReadOnlyCollection<SleepStage> Stages { get; set; }
    }

    [Serializable]
    public class SleepStage
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int Stage { get; set; }
    }
}