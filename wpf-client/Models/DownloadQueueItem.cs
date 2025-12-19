using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AsmrDownloader.Client.Models
{
    public class DownloadQueueItem : INotifyPropertyChanged
    {
        public string MediaId { get; set; } = "";
        public string? Title { get; set; }

        private string _status = "Pending";
        public string Status { get => _status; set { _status = value; OnPropertyChanged(); } }

        private double _progress = 0;
        public double Progress { get => _progress; set { _progress = value; OnPropertyChanged(); } }

        public object? Tag { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
