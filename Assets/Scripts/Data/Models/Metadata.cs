using System;

namespace Data.Models
{
    [Serializable]
    public class Metadata
    {
        public int ClientRecordVersion { get; set; }
        public DataOrigin DataOrigin { get; set; }
        public Device Device { get; set; }
        public string Id { get; set; }
        public object LastModifiedTime { get; set; }
        public int RecordingMethod { get; set; }
    }
    
    [Serializable]
    public class DataOrigin
    {
        public string PackageName { get; set; }
    }
    
    [Serializable]
    public class Device
    {
        public int Type { get; set; }
    }
}