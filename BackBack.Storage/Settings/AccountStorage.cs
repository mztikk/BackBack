using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BackBack.Models;
using Microsoft.Extensions.Logging;
using RF.WPF;

namespace BackBack.Storage.Settings
{
    public class AccountStorage : EncryptedStorage<List<Account>>
    {
        public AccountStorage(Func<Type, ILogger> loggerCreator, IEntropy entropy) : base(loggerCreator(typeof(AccountStorage)), entropy) { }

        public List<Account> Data { get; set; }

        public override void ILoad()
        {
            _logger.LogDebug("Loading {storage}", nameof(AccountStorage));
            Data = Load();
            _logger.LogDebug("Loaded {storage}", nameof(AccountStorage));
        }

        public override async Task SaveAsync()
        {
            _logger.LogDebug("Asynchronously saving {storage}", nameof(AccountStorage));
            await SaveAsync(Data);
            _logger.LogDebug("Asynchronously saved {storage}", nameof(AccountStorage));
        }

        public override void Save()
        {
            _logger.LogDebug("Saving {storage}", nameof(AccountStorage));
            Save(Data);
            _logger.LogDebug("Saved {storage}", nameof(AccountStorage));
        }
    }
}
