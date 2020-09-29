using System;

namespace BackBack.Models.Events
{
    public class PostBackupEvent : EventArgs
    {
        public PostBackupEvent(BackupItem backupItem)
        {
            BackupItem = backupItem;
            Name = backupItem.Name;
        }

        public BackupItem BackupItem { get; }

        public string Name { get; set; }
    }
}
