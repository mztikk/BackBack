using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using BackBack.LUA;
using BackBack.Models;
using BackBack.Models.Events;
using BackBack.Storage.Settings;
using BackBack.Triggers;
using Cronos;
using Microsoft.Extensions.Logging;
using RF.WPF.Extensions;
using RF.WPF.MVVM;
using RF.WPF.Navigation;
using RFReborn.Extensions;
using RFReborn.Files;
using RFReborn.Files.FileCollector;
using RFReborn.Files.FileCollector.Modules;
using RFReborn.Routines;
using Stylet;
using StyletIoC;

namespace BackBack.ViewModel
{
    public class BackupItemViewModel : ViewModelBase, IDisposable
    {
        private readonly ILogger _logger;
        private readonly Func<Lua> _luaCreator;
        private readonly IEventAggregator _eventAggregator;
        private readonly IContainer _container;
        private readonly BackupData _backupData;

        private RoutineBase? _routine = null;

        private TriggerEvent _trigger;

        public BackupItemViewModel(INavigationService navigationService, Func<Lua> luaCreator, IEventAggregator eventAggregator, Func<Type, ILogger> loggerFactory, IContainer container, BackupData backupData) : base(navigationService)
        {
            _logger = loggerFactory(typeof(BackupItemViewModel));
            _luaCreator = luaCreator;
            _eventAggregator = eventAggregator;
            _container = container;
            _backupData = backupData;
            BackupCommand = new AsyncCommand(BackupAsync);
            OpenSourceCommand = new Command(OpenSource);
            OpenDestinationCommand = new Command(OpenDestination);
        }

        public override void OnNavigatedTo()
        {
            base.OnNavigatedTo();

            _logger.LogDebug("Syncing Properties with {type}: '{name}'", BackupItem.TypeName(), BackupItem.Name);
            PropertySync.Sync(BackupItem, this);
        }

        public BackupItem BackupItem { get; set; }

        private ICommand _backupCommand;
        public ICommand BackupCommand
        {
            get => _backupCommand;
            set { _backupCommand = value; NotifyOfPropertyChange(); }
        }

        private ICommand _openSourceCommand;
        public ICommand OpenSourceCommand
        {
            get => _openSourceCommand;
            set { _openSourceCommand = value; NotifyOfPropertyChange(); }
        }

        private ICommand _openDestinationCommand;
        public ICommand OpenDestinationCommand
        {
            get => _openDestinationCommand;
            set { _openDestinationCommand = value; NotifyOfPropertyChange(); }
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

        private string _currentFile;
        public string CurrentFile
        {
            get => _currentFile;
            set { _currentFile = value; NotifyOfPropertyChange(); }
        }

        private long _totalFileCount;
        public long TotalFileCount
        {
            get => _totalFileCount;
            set { _totalFileCount = value; NotifyOfPropertyChange(); }
        }

        private long _runningFileCount;
        public long RunningFileCount
        {
            get => _runningFileCount;
            set { _runningFileCount = value; NotifyOfPropertyChange(); }
        }

        private double _runningFilePercentage;
        public double RunningFilePercentage
        {
            get => _runningFilePercentage;
            set { _runningFilePercentage = value; NotifyOfPropertyChange(); }
        }

        private TriggerInfo _triggerInfo;
        public TriggerInfo TriggerInfo
        {
            get => _triggerInfo;
            set { _triggerInfo = value; NotifyOfPropertyChange(); TriggerInfoChanged(); }
        }

        private DateTime _lastExecution;
        public DateTime LastExecution
        {
            get => _lastExecution;
            set { _lastExecution = value; NotifyOfPropertyChange(); }
        }

        private void TriggerInfoChanged()
        {
            if (_trigger is IDisposable disposable)
            {
                disposable?.Dispose();
            }

            _trigger = null;

            if (TriggerInfo is null)
            {
                return;
            }

            switch (TriggerInfo.Type)
            {
                case TriggerType.None:
                    return;
                case TriggerType.TimedTrigger:
                    {
                        TimedTrigger trigger = _container.Get<TimedTrigger>();
                        _trigger = trigger;
                        trigger.BackupItem = BackupItem;
                        trigger.Interval = TriggerInfo.Interval;
                    }
                    break;
                case TriggerType.BackupItemTrigger:
                    {
                        BackupItemTrigger trigger = _container.Get<BackupItemTrigger>();
                        _trigger = trigger;
                        if (!string.IsNullOrWhiteSpace(TriggerInfo.BackupName))
                        {
                            trigger.BackupItem = _backupData[TriggerInfo.BackupName];
                        }
                    }
                    break;
                case TriggerType.CronTrigger:
                    {
                        CronTrigger trigger = _container.Get<CronTrigger>();
                        _trigger = trigger;
                        if (!string.IsNullOrWhiteSpace(TriggerInfo.Cron))
                        {
                            trigger.BackupItem = BackupItem;
                            trigger.CronExpression = CronExpression.Parse(TriggerInfo.Cron);
                        }
                    }
                    break;
                default:
                    throw new NotImplementedException();
            }

            _trigger.OnTrigger += Trigger_OnTrigger;
        }

        private void Trigger_OnTrigger(object sender, TriggerEventArgs e) => BackupAsync();

        private bool _disposedValue;

        private readonly object _locker = new object();

        public async Task BackupAsync() => await Task.Run(Backup);

        public void OpenSource() => Process.Start("explorer.exe", Source);

        public void OpenDestination() => Process.Start("explorer.exe", Destination);

        public void Backup()
        {
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
                        var copiedFiles = new ConcurrentDictionary<string, bool>();

                        var collector = new FileCollector();
                        if (Ignores is { })
                        {
                            _logger.LogTrace("Adding ignores: {ignores}", Ignores);
                            collector.AddListModule(Ignores.GetLines().SkipEmpty());
                        }

                        try
                        {
                            Status = $"Collecting files to backup from '{Source}'";
                            foreach (string file in collector.EnumerateFiles(Source))
                            {
                                collectedFiles.Add(file);
                            }
                            TotalFileCount = collectedFiles.Count;
                            Status = $"Backing up Directory '{Source}'";
                            Parallel.ForEach(collectedFiles, (file) =>
                            {
                                try
                                {
                                    if (file is null)
                                    {
                                        _logger.LogWarning("Enumerated file is null");
                                        return;
                                    }

                                    _logger.LogTrace("Copying file: '{file}'", file);
                                    CurrentFile = file;

                                    string newFile = FileUtils.MakePath(basePath, target, file);
                                    if (newFile is null)
                                    {
                                        _logger.LogError("Target of file: '{file}' is null", file);
                                        return;
                                    }

                                    _logger.LogTrace("Target is '{file}'", newFile);

                                    copiedFiles.Add(newFile, true);

                                    int retries = 0;
                                    const int maxRetryCount = 5;
                                    bool done = false;
                                    while (retries < maxRetryCount && !done)
                                    {
                                        try
                                        {
                                            if (File.Exists(newFile) && FileUtils.AreEqual(file, newFile))
                                            {
                                                _logger.LogInformation("File: '{file}' already exists and is equal", newFile);
                                                return;
                                            }

                                            _logger.LogTrace("Copying file: '{file}' to '{targetfile}'", file, newFile);
                                            FileUtils.Copy(basePath, target, new FileInfo(file), true);
                                            done = true;
                                        }
                                        catch (IOException) { }
                                        finally
                                        {
                                            retries++;
                                        }
                                    }
                                }
                                catch (Exception e)
                                {
                                    _logger.LogError(e, e.ToString());
                                    throw;
                                }
                                finally
                                {
                                    RunningFileCount++;
                                }
                            });
                        }
                        finally
                        {
                            CurrentFile = string.Empty;
                        }

                        _logger.LogInformation("Removing files not found in source");
                        Status = $"Removing items in target '{Destination}' not found in source {Source}";
                        try
                        {
                            var toDelete = new HashSet<string>();
                            foreach (string file in FileUtils.Walk(target.FullName, FileSystemEnumeration.FilesOnly))
                            {
                                if (!copiedFiles.ContainsKey(file))
                                {
                                    _logger.LogTrace("Adding '{file}' to deletion list", file);
                                    toDelete.Add(file);
                                }
                            }
                            TotalFileCount = toDelete.Count;
                            Parallel.ForEach(toDelete, (file) =>
                            {
                                try
                                {
                                    CurrentFile = file;
                                    _logger.LogTrace("Removing {file}", file);
                                    File.Delete(file);
                                }
                                finally
                                {
                                    RunningFileCount++;
                                }
                            });
                        }
                        finally
                        {
                            TotalFileCount = 0;
                            RunningFileCount = 0;
                        }
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
                    _logger.LogDebug("Setting values for {type}: {name}", BackupItem.TypeName(), BackupItem.Name);
                    lua.SetValuesFromBackupItem(BackupItem);
                    _logger.LogTrace("PostCompletionScript is: {script}", PostCompletionScript);
                    _logger.LogDebug("Running {name}", nameof(PostCompletionScript));
                    if (!string.IsNullOrWhiteSpace(PostCompletionScript))
                    {
                        lua.Run(PostCompletionScript, Name);
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(e, $"Running Backup for '{Name}' failed");
                    throw;
                }
                finally
                {
                    _logger.LogDebug("Backup finished");
                    BackupItem.LastExecution = DateTime.Now;
                    LastExecution = BackupItem.LastExecution;
                    _logger.LogDebug("Last Execution is: {time}", BackupItem.LastExecution);
                    _backupData.Save();
                    Running = false;
                    Status = "Finished";
                    TotalFileCount = 0;
                    RunningFileCount = 0;
                    Task.Delay(5000).ContinueWith((_) => { if (Status == "Finished") { Status = string.Empty; } });

                    _logger.LogDebug("Publishing {type}", typeof(PostBackupEvent).ToString());
                    _eventAggregator.Publish(new PostBackupEvent(DateTime.Now, BackupItem));
                }
            }
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

                    if (_trigger is IDisposable disposable)
                    {
                        disposable?.Dispose();
                    }
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
