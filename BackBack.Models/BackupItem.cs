using System;

namespace BackBack.Models
{
    public class BackupItem : IEquatable<BackupItem>
    {
        public string Name { get; set; }
        public string Source { get; set; }
        public string Destination { get; set; }
        public string Ignores { get; set; }
        public string PostCompletionScript { get; set; }
        public bool ZipFiles { get; set; }
        public string ZipFileDestination { get; set; }
        public bool LimitArchives { get; set; }
        public double NumberOfArchives { get; set; }
        public DateTime LastExecution { get; set; }
        public bool BackupPeriodically { get; set; }
        public TimeSpan Interval { get; set; }

        public bool Equals(BackupItem other) => other.Name.Equals(Name);
    }
}
