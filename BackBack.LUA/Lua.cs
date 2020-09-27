using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using BackBack.Storage.Settings;

namespace BackBack.LUA
{
    public class Lua : IDisposable
    {
        private readonly Settings _settings;
        private bool _disposedValue;

        public readonly NLua.Lua NLua;

        public Lua(Settings settings)
        {
            _settings = settings;
            NLua = new NLua.Lua();

            SetupLua();
        }

        private void SetupLua()
        {
            NLua["settingsDir"] = _settings.GetSettingsDir();
            NLua["storageDir"] = _settings.GetStorageDir();

            NLua.RegisterFunction(nameof(Timestamp), GetStaticMethod(nameof(Timestamp)));

            // nasty overload handling
            NLua.RegisterFunction("debugwrite",
                GetStaticMethods(typeof(Debug), "WriteLine")
                .FirstOrDefault(
                    x => x.GetParameters()
                    .Where(y => y.ParameterType == typeof(string)).Count() == 1));
        }

        public void SetValue(string name, object value) => NLua[name] = value;

        public object[] Run(string lua) => NLua.DoString(lua);

        protected IEnumerable<MethodInfo> GetStaticMethods(Type type, string name) => type.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static).Where(x => x.Name == name);
        protected MethodInfo GetStaticMethod(Type type, string name) => type.GetMethod(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
        protected MethodInfo GetStaticMethod(string name) => GetStaticMethod(typeof(Lua), name);

        protected static string Timestamp(string format) => DateTime.Now.ToString(format);

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                    NLua?.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                _disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~Lua()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
