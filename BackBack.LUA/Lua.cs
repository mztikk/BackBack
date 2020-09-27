using System;
using BackBack.Storage.Settings;

namespace BackBack.LUA
{
    public class Lua : IDisposable
    {
        private readonly Settings _settings;
        private readonly NLua.Lua _lua;
        private bool _disposedValue;

        public Lua(Settings settings)
        {
            _settings = settings;
            _lua = new NLua.Lua();

            SetupLua();
        }

        private void SetupLua()
        {
            _lua["settingsDir"] = _settings.GetSettingsDir();
            _lua["storageDir"] = _settings.GetStorageDir();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                    _lua?.Dispose();
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
