using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BackBack.Models;
using Microsoft.Extensions.Logging;
using RF.WPF;

namespace BackBack.Storage.Settings
{
    public class BackupData : Storage<Dictionary<string, BackupItem>>
    {
        public BackupData(Func<Type, ILogger> loggerCreator) : base(loggerCreator(typeof(BackupData))) { }

        public BackupItem this[string key]
        {
            get => Data[key];
            set => Data[key] = value;
        }

        public Dictionary<string, BackupItem> Data { get; set; }

        public override void ILoad()
        {
            _logger.LogDebug("Loading {backupdata}", nameof(BackupData));
            Data = Load();
            _logger.LogDebug("Loaded {backupdata}", nameof(BackupData));
        }

        public override async Task SaveAsync()
        {
            _logger.LogDebug("Asynchronously saving {backupdata}", nameof(BackupData));
            await SaveAsync(Data);
            _logger.LogDebug("Asynchronously saved {backupdata}", nameof(BackupData));
        }

        public override void Save()
        {
            _logger.LogDebug("Saving {backupdata}", nameof(BackupData));
            Save(Data);
            _logger.LogDebug("Saved {backupdata}", nameof(BackupData));
        }
    }
}
