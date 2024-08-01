using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Widget.Forecast
{
    public class WidgetForecastInstance : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private (WidgetForecastItem main, List<WidgetForecastItem> others) _focusedItem;
        public (WidgetForecastItem main, List<WidgetForecastItem> others) FocusedItem
        {
            get => _focusedItem;
            set
            {
                if (_focusedItem != value)
                {
                    _focusedItem = value;
                    OnPropertyChanged(nameof(FocusedItem)); // Notify UI
                }
            }
        }

        public List<WidgetForecastItem> Items { get; set; }

        public int TargetDotSizeInPx { get; set; } = 60;

        public int DiameterInPx { get; set; } = 250;

        public decimal? DegreePositioning { get; set; }

        public decimal? DegreeBearing { get; set; }

        public void SetFocusedForecastItem((WidgetForecastItem main, List<WidgetForecastItem> others) newItem)
        {
            FocusedItem = newItem;
            // You don't need to call OnPropertyChanged here again, 
            // as setting FocusedItem already calls it
        }
    }

    public class WidgetForecastType
    {
        public int Id { get; set; }

        public string Icon { get; set; }
        public string Name { get; set; }


    }


    public class WidgetForecastItem
    {
        // private Guid Id { get; set; }s
        public WidgetForecastType Type { get; set; }
        public bool IsActual { get; set; }

        public decimal Degree { get; set; }

        public decimal ValueInPercent { get; set; }
        public string ValueText { get; set; }

        public bool IsVisible { get; set; }

    }

    public class WidgetForecast
    {
        public static readonly ReadOnlyCollection<WidgetForecastType> Types = new ReadOnlyCollection<WidgetForecastType>(new List<WidgetForecastType> {
            new WidgetForecastType { Id = 1, Icon = "bi-speedometer2", Name = "Speed" },
            new WidgetForecastType { Id = 2, Icon = "bi-wind", Name = "Wind" },
            new WidgetForecastType { Id = 3, Icon = "bi-tsunami", Name = "Wave" },
            new WidgetForecastType { Id = 3, Icon = "bi-arrow-repeat", Name = "Current" }
        });
    }
}

