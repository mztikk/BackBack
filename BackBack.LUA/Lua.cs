﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using BackBack.Models;
using BackBack.Storage.Settings;
using NLua;
using RFReborn.Files;

namespace BackBack.LUA
{
    public class Lua : IDisposable
    {
        private readonly Settings _settings;
        private bool _disposedValue;

        public readonly NLua.Lua NLua;

        private readonly ConcurrentDictionary<string, LuaTable> _namedTables = new ConcurrentDictionary<string, LuaTable>();

        public Lua(Settings settings)
        {
            _settings = settings;
            NLua = new NLua.Lua();

            SetupLua();
        }

        private void SetupLua()
        {
            NLua.State.Encoding = Encoding.UTF8;

            NLua["settingsDir"] = _settings.GetSettingsDir();
            NLua["storageDir"] = _settings.GetStorageDir();

            NLua.RegisterFunction(nameof(Timestamp), GetStaticMethod(nameof(Timestamp)));
            NLua.RegisterFunction("Zip", GetStaticMethod(typeof(LuaZip), "Zip"));
            NLua.RegisterFunction("debugprint", GetStaticMethod("debugprint"));
            //NLua.RegisterFunction("CombinePath", GetStaticMethods(typeof(Path), "Combine").FirstOrDefault(x => x.GetParameters().Any(y => y.para)));
            foreach (MethodInfo item in GetStaticMethods(typeof(Path), "Combine"))
            {
                ParameterInfo[] p = item.GetParameters();
                if (p.Any(x => x.ParameterType == typeof(string[])))
                {
                    NLua.RegisterFunction("CombinePath", item);
                    break;
                }
            }

            NLua.RegisterFunction("GetNormalPath", GetStaticMethod(typeof(FileUtils), "GetNormalPath"));

            // nasty overload handling
            NLua.RegisterFunction("debugwrite",
                GetStaticMethods(typeof(Debug), "WriteLine")
                .FirstOrDefault(
                    x => x.GetParameters()
                    .Where(y => y.ParameterType == typeof(string)).Count() == 1));
        }

        public void SetValuesFromBackupItem(BackupItem backupItem)
        {
            SetValue("source", backupItem.Source);
            SetValue("destination", backupItem.Destination);
            SetValue("name", backupItem.Name);
        }

        public void SetValue(string name, object value) => NLua[name] = value;

        public object[] Run(string lua, string name)
        {
            LuaFunction f = NLua.LoadString(lua, null);
            return f.Call(name, GetNamedTable(name));
        }

        private LuaTable CreateTable()
        {
            NLua.State.CreateTable(0, 0);
            return NLua.Pop() as LuaTable;
        }

        public LuaTable GetNamedTable(string name) => _namedTables.GetOrAdd(name, CreateTable());

        protected IEnumerable<MethodInfo> GetStaticMethods(Type type, string name) => type.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static).Where(x => x.Name == name);
        protected MethodInfo GetStaticMethod(Type type, string name) => type.GetMethod(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
        protected MethodInfo GetStaticMethod(string name) => GetStaticMethod(typeof(Lua), name);

        private static void debugprint(params object[] args)
        {
            Debug.WriteLine(string.Join("\t", args));
        }

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
