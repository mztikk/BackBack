using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using BackBack.LUA;
using BackBack.Models;
using BackBack.Models.Events;
using Microsoft.Extensions.Logging;
using RF.WPF;
using RF.WPF.MVVM;
using RF.WPF.Navigation;
using RFReborn.Extensions;
using RFReborn.Files;
using RFReborn.Files.FileCollector;
using RFReborn.Files.FileCollector.Modules;
using RFReborn.Routines;
using Stylet;

namespace BackBack.ViewModel
{
    public class BackupItemViewModel : ViewModelBase, IHandle<PostBackupEvent>, IHandle<TickEvent>, IDisposable
    {
        private readonly ILogger _logger;
        private readonly Func<Lua> _luaCreator;
        private readonly IEventAggregator _eventAggregator;

        private RoutineBase? _routine = null;

        public BackupItemViewModel(INavigationService navigationService, Func<Lua> luaCreator, IEventAggregator eventAggregator, Func<Type, ILogger> loggerFactory) : base(navigationService)
        {
            _logger = loggerFactory(typeof(BackupItemViewModel));
            _luaCreator = luaCreator;
            _eventAggregator = eventAggregator;
            BackupCommand = new AsyncCommand(BackupAsync);

            _logger.LogDebug("Subscribing to {type}", eventAggregator.GetType().ToString());
            eventAggregator.Subscribe(this);
        }

        public override void OnNavigatedTo()
        {
            base.OnNavigatedTo();

            _logger.LogDebug("Syncing Properties with {type}: '{name}'", BackupItem.GetType().ToString(), BackupItem.Name);
            PropertySync.Sync(BackupItem, this, null);
        }

        public BackupItem BackupItem { get; set; }

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

        private bool _zipFiles;
        public bool ZipFiles
        {
            get => _zipFiles;
            set { _zipFiles = value; NotifyOfPropertyChange(); }
        }

        private string _zipFileDestination;
        public string ZipFileDestination
        {
            get => _zipFileDestination;
            set { _zipFileDestination = value; NotifyOfPropertyChange(); }
        }

        private bool _limitArchives;
        public bool LimitArchives
        {
            get => _limitArchives;
            set { _limitArchives = value; NotifyOfPropertyChange(); }
        }

        private double _numberOfArchives;
        public double NumberOfArchives
        {
            get => _numberOfArchives;
            set { _numberOfArchives = value; NotifyOfPropertyChange(); }
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

        private TimeSpan _interval;
        private bool _disposedValue;

        public TimeSpan Interval
        {
            get => _interval;
            set { _interval = value; NotifyOfPropertyChange(); _routine = new GenericRoutine(value, BackupAsync); }
        }

        private readonly object _locker = new object();

        public async Task BackupAsync() => await Task.Run(Backup);

        public void Backup()
        {
            using IDisposable scope = _logger.BeginScope($"{Name}: ");
            _logger.LogDebug("Starting backup of '{name}'", Name);

            if (Running)
            {
                _logger.LogInformation("'{name}' already running", Name);
                return;
            }

            lock (_locker)
            {
                Running = true;
                Status = "Running";

                try
                {
                    if (Directory.Exists(Source))
                    {
                        _logger.LogInformation("Backup source is a directory");
                        Status = $"Backing up Directory '{Source}'";

                        var basePath = new DirectoryInfo(Source);
                        var target = new DirectoryInfo(Destination);

                        _logger.LogDebug("Backing up from '{source}' to '{target}'", basePath, target);

                        var collectedFiles = new HashSet<string>();

                        var collector = new FileCollector();
                        if (Ignores is { })
                        {
                            _logger.LogTrace("Adding ignores: {ignores}", Ignores);
                            collector.AddListModule(Ignores.GetLines().SkipEmpty());
                        }

                        Parallel.ForEach(collector.EnumerateFiles(Source), (file) =>
                        {
                            if (file is null)
                            {
                                _logger.LogWarning("Enumerated file is null");
                                return;
                            }

                            _logger.LogTrace("Copying file: '{file}'", file);

                            string newFile = FileUtils.MakePath(basePath, target, file);
                            if (newFile is null)
                            {
                                _logger.LogError("Target of file: '{file}' is null", file);
                                return;
                            }

                            _logger.LogTrace("Target is '{file}'", newFile);

                            collectedFiles.Add(newFile);
                            if (File.Exists(newFile) && FileUtils.AreEqual(file, newFile))
                            {
                                _logger.LogInformation("File: '{file}' already exists and is equal", newFile);
                                return;
                            }

                            _logger.LogTrace("Copying file: '{file}' to '{targetfile}'", file, newFile);
                            FileUtils.Copy(basePath, target, new FileInfo(file), true);
                        });
                    }
                    else if (File.Exists(Source))
                    {
                        _logger.LogInformation("Backup source is a file");
                        Status = $"Backing up File '{Source}'";
                        _logger.LogTrace("Copying file: '{file}' to '{targetfile}'", Source, Destination);
                        FileUtils.Copy(new FileInfo(Source), new FileInfo(Destination));
                    }

                    if (ZipFiles)
                    {
                        _logger.LogDebug("Zipping of backup target enabled");

                        string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
                        string zipFile = Path.ChangeExtension(Path.Combine(ZipFileDestination, $"{Name}_{timestamp}"), "zip");
                        _logger.LogDebug("Target zip file is '{file}'", zipFile);
                        _logger.LogInformation("Creating zip file: '{file}'", zipFile);
                        LuaZip.Zip(Destination, zipFile);

                        if (LimitArchives)
                        {
                            _logger.LogInformation("Number of archives is limited to: {number}", NumberOfArchives);
                            var archives = new List<string>();
                            foreach (string file in FileUtils.GetFiles(ZipFileDestination, null))
                            {
                                string fileName = Path.GetFileName(file);
                                if (fileName.StartsWith(Name) && fileName.EndsWith("zip"))
                                {
                                    _logger.LogTrace("Found archive: '{archive}'", file);
                                    archives.Add(file);
                                }
                            }
                            _logger.LogInformation("Found {number} archives", archives.Count);

                            archives.Sort();

                            if (archives.Count > NumberOfArchives)
                            {
                                int diff = (int)(archives.Count - NumberOfArchives);
                                _logger.LogInformation("{number} over the limit of {number} archives", diff, NumberOfArchives);
                                for (int i = 0; i < diff; i++)
                                {
                                    string toDelete = archives[i];
                                    _logger.LogInformation("Deleting '{file}'", toDelete);
                                    File.Delete(toDelete);
                                }
                            }
                        }
                    }

                    _logger.LogDebug("Creating lua engine");
                    using Lua lua = _luaCreator.Invoke();
                    _logger.LogDebug("Setting values for {type}: {name}", BackupItem.GetType().ToString(), BackupItem.Name);
                    lua.SetValuesFromBackupItem(BackupItem);
                    _logger.LogTrace("PostCompletionScript is: {script}", PostCompletionScript);
                    _logger.LogDebug("Running {name}", nameof(PostCompletionScript));
                    lua.Run(PostCompletionScript);
                }
                finally
                {
                    BackupItem.LastExecution = DateTime.Now;
                    Running = false;
                    Status = "Finished";
                    Task.Delay(5000).ContinueWith((_) => { if (Status == "Finished") { Status = string.Empty; } });

                    _eventAggregator.Publish(new PostBackupEvent(BackupItem));
                }
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
            {
                if (file is null)
                {
                    return;
                }

                string newFile = FileUtils.MakePath(basePath, target, file);
                if (newFile is null)
                {
                    return;
                }

                collectedFiles.Add(newFile);
                if (File.Exists(newFile) && FileUtils.AreEqual(file, newFile))
                {
                    return;
                }

                Debug.WriteLine($"{file} -> {newFile}");
                FileUtils.Copy(basePath, target, new FileInfo(file), true);
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

        public void Handle(PostBackupEvent message) => Debug.WriteLine(message.Name);
        public void Handle(TickEvent message) => Debug.WriteLine(message.Time + ": " + Name);

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                    _eventAggregator.Unsubscribe(this);
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                _disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~BackupItemViewModel()
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
