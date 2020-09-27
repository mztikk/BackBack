using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using BackBack.LUA;
using BackBack.Models;
using RF.WPF.MVVM;
using RFReborn.Extensions;
using RFReborn.Files;
using RFReborn.Files.FileCollector;
using RFReborn.Files.FileCollector.Modules;
using Stylet;

namespace BackBack.ViewModel
{
    public class BackupItemViewModel : PropertyChangedBase
    {
        public readonly BackupItem BackupItem;
        private readonly Func<Lua> _luaCreator;

        public BackupItemViewModel(BackupItem backupItem, Func<Lua> luaCreator)
        {
            BackupItem = backupItem;
            _luaCreator = luaCreator;
            Name = BackupItem.Name;
            Source = BackupItem.Source;
            Destination = BackupItem.Destination;
            Ignores = BackupItem.Ignores;
            PostCompletionScript = BackupItem.PostCompletionScript;

            BackupCommand = new Command(Backup);
        }

        private ICommand _backupCommand;

        public ICommand BackupCommand
        {
            get => _backupCommand;
            set { _backupCommand = value; NotifyOfPropertyChange(); }
        }

        private string _name;
        public string Name
        {
            get => _name;
            set { _name = value; NotifyOfPropertyChange(); }
        }

        private string _source;
        public string Source
        {
            get => _source;
            set { _source = value; NotifyOfPropertyChange(); }
        }

        private string _destination;
        public string Destination
        {
            get => _destination;
            set { _destination = value; NotifyOfPropertyChange(); }
        }

        private string _ignores;
        public string Ignores
        {
            get => _ignores;
            set { _ignores = value; NotifyOfPropertyChange(); }
        }

        private string _postCompletionScript;
        public string PostCompletionScript
        {
            get => _postCompletionScript;
            set { _postCompletionScript = value; NotifyOfPropertyChange(); }
        }

        private bool _running;
        public bool Running
        {
            get => _running;
            set { _running = value; NotifyOfPropertyChange(); }
        }


        private string _status;
        public string Status
        {
            get => _status;
            set { _status = value; NotifyOfPropertyChange(); }
        }

        private readonly object _locker = new object();

        public void Backup()
        {
            if (Running)
            {
                return;
            }

            lock (_locker)
            {
                Running = true;
                Status = "Running";

                Task.Run(() =>
                {
                    try
                    {
                        if (Directory.Exists(Source))
                        {
                            Status = $"Backing up Directory '{Source}'";
                            BackupDir();
                        }
                        else if (File.Exists(Source))
                        {
                            Status = $"Backing up File '{Source}'";
                            BackupFile();
                        }

                        using Lua lua = _luaCreator.Invoke();
                        lua.SetValue("source", Source);
                        lua.SetValue("destination", Destination);
                        lua.SetValue("name", Name);
                        lua.Run(PostCompletionScript);
                    }
                    finally
                    {
                        Running = false;
                        Status = "Finished";
                        Task.Delay(5000).ContinueWith((_) => { if (Status == "Finished") { Status = string.Empty; } });
                    }
                });
            }
        }

        private void BackupFile() => FileUtils.Copy(new FileInfo(Source), new FileInfo(Destination));

        private void BackupDir()
        {
            var basePath = new DirectoryInfo(Source);
            var target = new DirectoryInfo(Destination);

            var collectedFiles = new HashSet<string>();

            var collector = new FileCollector();
            if (Ignores is { })
            {
                collector.AddListModule(Ignores.GetLines().SkipEmpty());
            }

            Parallel.ForEach(collector.EnumerateFiles(Source), (file) =>
            //foreach (string file in collector.EnumerateFiles(Source))
            {
                if (file is null)
                {
                    return;
                    //continue;
                }

                string newFile = FileUtils.MakePath(basePath, target, file);
                if (newFile is null)
                {
                    return;
                    //continue;
                }

                collectedFiles.Add(newFile);
                if (File.Exists(newFile) && FileUtils.AreEqual(file, newFile))
                {
                    return;
                    //continue;
                }

                Debug.WriteLine($"{file} -> {newFile}");
                FileUtils.Copy(basePath, target, new FileInfo(file), true);
                //}
            });

            //Parallel.ForEach(FileUtils.Walk(target.FullName, FileSystemEnumeration.FilesOnly), (file) =>
            //{
            //    if (!collectedFiles.Contains(file))
            //    {
            //        Debug.WriteLine($"Removing {file}");
            //        File.Delete(file);
            //    }
            //});
        }
    }
}
