using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using RF.WPF;
using RFReborn.Internals;

namespace BackBack.Storage.Settings
{
    public class Settings : Storage<Dictionary<string, string>>
    {
        private static readonly Dictionary<string, string> s_defaultValues = new Dictionary<string, string>()
        {
            { "StartWithWindows", "True" },
            { "MinimizeToTray", "True" }
        };

        public Settings(Func<Type, ILogger> loggerCreator) : base(loggerCreator(typeof(Settings))) { }

        public Dictionary<string, string> Data { get; set; }

        public override void ILoad()
        {
            Data = Load();
            foreach (KeyValuePair<string, string> defaultValue in s_defaultValues)
            {
                if (!Data.ContainsKey(defaultValue.Key))
                {
                    Data[defaultValue.Key] = defaultValue.Value;
                }
            }
        }

        public override async Task SaveAsync() => await SaveAsync(Data);
        public override void Save() => Save(Data);

        public T GetValue<T>(string key)
        {
            if (!Data.ContainsKey(key))
            {
                throw new ArgumentException(key);
            }

            Type type = typeof(T);

            return (T)DynamicProperty.ConvertValue(type, Data[key]);
        }

        public void SetValue<T>(string key, T value) => Data[key] = value.ToString();
    }
}
