using System.Collections.Generic;
using System.Threading.Tasks;
using RF.WPF;

namespace BackBack.Settings
{
    public class Settings : Storage<Dictionary<string, string>>
    {
        public Settings() { }

        public Dictionary<string, string> Data { get; set; }

        public override void ILoad() => Data = Load();
        public override async Task SaveAsync() => await SaveAsync(Data);
        public override void Save() => Save(Data);
    }
}
