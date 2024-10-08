using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

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

        public (int X, int Y) Position { get; set; }
        public (int X, int Y) Size { get; set; }
        public bool IsAvailable { get; set; } // Whether the item space is available or not

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class CustomDashboardInstance : INotifyPropertyChanged
    {
        private int _maxX = 4; // Default value for MaxX
        public int MaxX
        {
            get => _maxX;
            set => _maxX = value > 0 ? value : 4;
        }

        private int _maxY = 5; // Default value for MaxY
        public int MaxY
        {
            get => _maxY;
            set => _maxY = value > 0 ? value : 5;
        }

        public bool IsEditing { get; set; } = false;

        public ObservableCollection<CustomDashboardItem> Items { get; set; }

        public CustomDashboardInstance()
        {
            InitializeDefaults();
        }

        public CustomDashboardInstance((int maxX, int maxY) sizes)
        {
            MaxX = sizes.maxX;
            MaxY = sizes.maxY;
            InitializeDefaults();
        }

        public void InitializeDefaults()
        {
            Items ??= new ObservableCollection<CustomDashboardItem>();

            InitializeItems(MaxX, MaxY);
        }

        private void InitializeItems(int maxX, int maxY)
        {
            Items.Clear();

            Console.WriteLine($"New CustomDashboardInstance triggered - creating new grid items using the max values of {maxX}x, {maxY}y");

            for (int y = 0; y < maxY; y++)
            {
                for (int x = 0; x < maxX; x++)
                {
                    Items.Add(new CustomDashboardItem
                    {
                        Id = Guid.NewGuid(),
                        Name = string.Empty,
                        IsAvailable = true,
                        Position = (x, y),
                        Size = (1, 1),
                    });
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
