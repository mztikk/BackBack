using System.Collections.Generic;
using System.Threading.Tasks;
using BackBack.ViewModel;
using RF.WPF;

namespace BackBack
{
    public class BackupData : Storage<Dictionary<string, BackupItem>>
    {
        public BackupData() { }

        public Dictionary<string, BackupItem> Data { get; set; }

        public override void ILoad() => Data = Load();
        public override async Task SaveAsync() => await SaveAsync(Data);
        public override void Save() => Save(Data);
    }
}
