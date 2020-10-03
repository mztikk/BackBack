using System;

namespace BackBack.Models
{
    public class TriggerInfo
    {
        public TriggerType Type { get; set; }
        public TimeSpan Interval { get; set; }
        public string BackupName { get; set; }
    }
}
