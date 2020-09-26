using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
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

        public BackupItemViewModel(BackupItem backupItem)
        {
            BackupItem = backupItem;

            Name = BackupItem.Name;
            Source = BackupItem.Source;
            Destination = BackupItem.Destination;
            Ignores = BackupItem.Ignores;

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

        private bool _running;
        public bool Running
        {
            get => _running;
            set { _running = value; NotifyOfPropertyChange(); }
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

                Task.Run(() =>
                {
                    try
                    {
                        if (Directory.Exists(Source))
                        {
                            BackupDir();
                        }
                        else if (File.Exists(Source))
                        {
                            BackupFile();
                        }
                    }
                    finally
                    {
                        Running = false;
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
