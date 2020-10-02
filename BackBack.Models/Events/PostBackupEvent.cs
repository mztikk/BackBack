using System;

namespace BackBack.Models.Events
{
    public class PostBackupEvent : TimeEventArgs
    {
        public PostBackupEvent(DateTime time, BackupItem backupItem) : base(time)
        {
            BackupItem = backupItem;
            Name = backupItem.Name;
        }

        public BackupItem BackupItem { get; }

        public string Name { get; set; }
    }
}
