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

    public class CustomDashboardInstance
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

        public ObservableCollection<CustomDashboardItem> Items { get; set; } = new ObservableCollection<CustomDashboardItem>();

        public CustomDashboardInstance()
        {
            InitializeItems(MaxX, MaxY); // Initialize items with MaxX and MaxY from constructor
        }

        public CustomDashboardInstance(int maxX, int maxY)
        {
            MaxX = maxX;
            MaxY = maxY;
            InitializeItems(maxX, maxY); // Initialize items with provided MaxX and MaxY
        }

        private void InitializeItems(int maxX, int maxY)
        {
            Items.Clear(); // Clear existing items if any

            Console.WriteLine($"InitializeItems of CustomDashboardInstance {maxX}x, {maxY}y called");

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
    }
}
