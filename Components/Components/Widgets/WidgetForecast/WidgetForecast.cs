using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Widget.Forecast
{
    public class WidgetForecastInstance
    {
        public List<WidgetForecastItem> Items { get; set; }

        public int TargetDotSizeInPx { get; set; } = 60;

        public decimal DepthValueInMeter { get; set; }

        public int DiameterInPx { get; set; } = 250;

        public decimal? DegreePositioning { get; set; }
        public bool? PositioningIsWarning { get; set; }

        public decimal? DegreeBearing { get; set; }
    }

    public class WidgetForecastType
    {
        public int Id { get; set; }

        public string Icon { get; set; }
        public string Name { get; set; }
        public decimal? Threshold { get; set; }


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
            new WidgetForecastType { Id = 2, Icon = "bi-wind", Name = "Wind", Threshold= 50m},
            new WidgetForecastType { Id = 3, Icon = "bi-tsunami", Name = "Wave", Threshold= 2.5m },
            new WidgetForecastType { Id = 3, Icon = "bi-arrow-repeat", Name = "Current", Threshold= 1m }
        });
    }
}

