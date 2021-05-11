namespace BackBack.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public string Greeting => "Welcome to Avalonia!";

        public BackupItemsViewModel BackupItems { get; set; } = new();
    }
}
