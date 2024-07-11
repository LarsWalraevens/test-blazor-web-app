using System.ComponentModel;
using System.Collections.ObjectModel;

namespace CustomDashboard.Components
{
    public class CustomDashboardItem : INotifyPropertyChanged
    {
        public Guid Id { get; set; }

        private string _name;
        public string Name
        {
            get => _name;
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        public int SortIndex { get; set; }
        public (int X, int Y) Position { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class CustomDashboardInstance
    {
        public int MaxX { get; set; } = 4;
        public int MaxY { get; set; } = 10;

        public ObservableCollection<CustomDashboardItem> Items { get; set; } = new ObservableCollection<CustomDashboardItem>([]);

    }
}
