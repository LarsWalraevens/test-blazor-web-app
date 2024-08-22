using System.Text;
using System.IO;
using System.IO.Compression;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using System.Globalization;
using Newtonsoft.Json.Linq;

namespace Widget.Forecast
{
    public class TelemetryData
    {
        private List<TelemetryItem> _items = new List<TelemetryItem>
        {
            new TelemetryItem
            {
                Key = "1-a",
                Description = "Aux #1 - Gauge meter - in kiloWatt",
                Value = null,
                Name = "aux1"
            },
            new TelemetryItem
            {
                Key = "2-a",
                Description = "Aux #2 - Gauge meter - in kiloWatt",
                Value = null,
                Name = "aux2"
            },
            new TelemetryItem
            {
                Key = "3-a",
                Description = "Aux #3 - Gauge meter - in kiloWatt",
                Value = null,
                Name = "aux3"
            },
            new TelemetryItem
            {
                Key = "1-kw",
                Description = null,
                Value = null,
                Name = "kw1"
            },
            new TelemetryItem
            {
                Key = "2-kw",
                Description = null,
                Value = null,
                Name = "kw2"
            },
            new TelemetryItem
            {
                Key = "3-kw",
                Description = null,
                Value = null,
                Name = "kw3"
            },
            new TelemetryItem
            {
                Key = "1-v",
                Description = null,
                Value = null,
                Name = "v1"
            },
            new TelemetryItem
            {
                Key = "2-v",
                Description = null,
                Value = null,
                Name = "v2"
            },
            new TelemetryItem
            {
                Key = "2-v",
                Description = null,
                Value = null,
                Name = "v2"
            },
            new TelemetryItem
            {
                Key = "3-v",
                Description = null,
                Value = null,
                Name = "v3"
            },
            new TelemetryItem
            {
                Key = "ais-status",
            },
            new TelemetryItem
            {
                Key = "ais-status-text",
            },
            new TelemetryItem
            {
                Key = "beaufort",
            },
            new TelemetryItem
            {
                Key = "coordinates",
            },
            new TelemetryItem
            {
                Key = "currentDirection",
                Unit = "°"
            },
            new TelemetryItem
            {
                Key = "currentSpeed",
                Unit = "m/s"
            },
            new TelemetryItem
            {
                Key = "distanceNM",
                Unit = "NM"
            },
            new TelemetryItem
            {
                Key = "gb-lo-in",
            },
            new TelemetryItem
            {
                Key = "GGAts",
            },
            new TelemetryItem
            {
                Key = "lat",
            },
            new TelemetryItem
            {
                Key = "LOG",
            },
            new TelemetryItem
            {
                Key = "lon",
            },
            new TelemetryItem
            {
                Key = "me-exhaust-temp",
                Unit = "°C"
            },
            new TelemetryItem
            {
                Key = "me-jacket-cfw-in",
                Unit = "°C"
            },
            new TelemetryItem
            {
                Key = "me-lo-in",
                Unit = "°C"
            },
            new TelemetryItem
            {
                Key = "me-prop-rpm",
                Unit = "RPM"
            },
            new TelemetryItem
            {
                Key = "me-rpm",
                Unit = "RPM"
            },
            new TelemetryItem
            {
                Key = "me-start-air",
                Unit = "bar"
            },
            new TelemetryItem
            {
                Key = "precipitation",
            },
            new TelemetryItem
            {
                Key = "SOG",
                Unit = "knots"
            },
            new TelemetryItem
            {
                Key = "swellDir",
                Unit = "°"
            },
            new TelemetryItem
            {
                Key = "swellHeight",
                Unit = "m"
            },
            new TelemetryItem
            {
                Key = "temperature",
                Unit = "°C"
            },
            new TelemetryItem
            {
                Key = "total-kw",
                Description = "Total Power",
                Unit = "kW"
            },
            new TelemetryItem
            {
                Key = "ts",
            },
            new TelemetryItem
            {
                Key = "turbo-rpm",
                Unit = "RPM"
            },
            new TelemetryItem
            {
                Key = "utc",
            },
            new TelemetryItem
            {
                Key = "vessel",
            },
            new TelemetryItem
            {
                Key = "visibility",
                Unit = "km"
            },
            new TelemetryItem
            {
                Key = "waterTemperature",
                Unit = "°C"
            },
            new TelemetryItem
            {
                Key = "waveDirection",
                Unit = "°"
            },
            new TelemetryItem
            {
                Key = "waveHeight",
                Unit = "m"
            },
            new TelemetryItem
            {
                Key = "weatherTime",
            },
            new TelemetryItem
            {
                Key = "winddirDegree",
                Unit = "°"
            },
            new TelemetryItem
            {
                Key = "windspeedKmph",
                Unit = "km/h"
            }
        };

        public List<TelemetryItem> Items { get; set; }

    }

    public class TelemetryItem
    {
        public string Key { get; set; }
        public string Description { get; set; }
        public string Value { get; set; }
        public string Name { get; set; }
        public string Unit { get; set; }
    }

    public class WidgetDataService
    {
        private readonly HttpClient _http;

        public WidgetDataService(HttpClient http)
        {
            _http = http;
        }

        public async Task<string> DecompressJson(string input)
        {
            using (MemoryStream inputStream = new MemoryStream(Convert.FromBase64String(input)))
            {
                using (MemoryStream outputStream = new MemoryStream())
                {
                    using (ZLibStream gzipStream = new ZLibStream(inputStream, CompressionMode.Decompress))
                    {
                        gzipStream.CopyTo(outputStream);
                    }
                    return Encoding.UTF8.GetString(outputStream.ToArray());
                }
            }
        }

        public async Task<object> GetForecastData(Guid id, DateTime date)
        {
            var requestBody = new
            {
                // Latitude = coordinates.latitude.ToString(), 
                // Longitude = coordinates.longitude.ToString(),
                Timestamp = date.ToString("yyyy-MM-dd"),
                VesselId = id
            };

            var jsonRequestBody = JsonConvert.SerializeObject(requestBody);
            var response = await _http.PostAsync("/api/v1/select/widget/compass", new StringContent(jsonRequestBody, Encoding.UTF8, "application/json"));

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Error response: {errorContent}");
                response.EnsureSuccessStatusCode();
            }

            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject(content);
        }

        public async Task<object> GetVesselTelemetryData(Guid id, DateTime date)
        {
            try
            {
                var requestBody = new
                {
                    VesselId = id,
                    Timestamp = date
                };

                var jsonRequestBody = JsonConvert.SerializeObject(requestBody);
                var response = await _http.PostAsync("/api/v1/select/vessel/telemetry", new StringContent(jsonRequestBody, Encoding.UTF8, "application/json"));

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Error response: {errorContent}");
                    response.EnsureSuccessStatusCode();
                }

                var content = await response.Content.ReadAsStringAsync();
                JObject contentJson = JObject.Parse(content);

                if (contentJson["data"] == null)
                {
                    return null;
                }
                else
                {
                    return JsonConvert.DeserializeObject(contentJson["data"].ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("GetVesselTelemetryData: " + ex.Message);
                return null;
            }
        }

        public async Task<object> GetVesselRouteData(Guid id)
        {
            var requestBody = new
            {
                VesselId = id
            };

            var jsonRequestBody = JsonConvert.SerializeObject(requestBody);
            var response = await _http.PostAsync("/api/v1/select/vessel/route/refined", new StringContent(jsonRequestBody, Encoding.UTF8, "application/json"));

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Error response: {errorContent}");
                response.EnsureSuccessStatusCode();
            }

            var content = await response.Content.ReadAsStringAsync();
            JObject contentJson = JObject.Parse(content);

            if (contentJson["data"] == null)
            {
                return null;
            }
            else
            {
                string decompressedJson = await DecompressJson(Convert.ToBase64String((byte[])contentJson["data"]));
                JObject decompressedObj = JObject.Parse(decompressedJson);

                // Order the sailed array by timestamp
                var sailedArray = decompressedObj["sailed"]
                    .OrderBy(s => DateTime.ParseExact((string)s["timestamp"], "MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture))
                    .ToList();

                decompressedObj["sailed"] = JArray.FromObject(sailedArray);
                return JsonConvert.DeserializeObject(decompressedObj["sailed"].ToString());
            }
        }


        public async Task<WidgetForecastInstance> GetForecastInstanceFromForecastData(object data)
        {
            try
            {
                var jsonData = JsonConvert.SerializeObject(data);
                dynamic dynamicData = JsonConvert.DeserializeObject<dynamic>(jsonData);
                decimal speedKnots = 8m;

                WidgetForecastInstance instance = new WidgetForecastInstance()
                {
                    DegreeBearing = (decimal?)dynamicData.planned?.vessel?.bearing ?? 0,
                    DegreePositioning = (decimal?)dynamicData.dot?.direction ?? 0,
                    // DepthValueInMeter = (decimal?)dynamicData.planned?.geo.oceanCurrentDepth ?? 0,
                    Items = new List<WidgetForecastItem>()
                };

                if (dynamicData.planned?.vessel?.heading != null)
                {
                    instance.Items.Add(new WidgetForecastItem()
                    {
                        Degree = (decimal)dynamicData.planned?.vessel?.heading,
                        Type = WidgetForecast.Types.FirstOrDefault(x => x.Name == "Speed"),
                        IsActual = false,
                        IsVisible = true,
                        ValueText = $"{String.Format("{0:0.##}", speedKnots)}kn, {dynamicData.planned?.vessel?.heading}°",
                        ValueInPercent = 100
                    });
                }

                if (dynamicData.sailed?.vessel?.heading != null)
                {
                    instance.Items.Add(new WidgetForecastItem()
                    {
                        Degree = (decimal)dynamicData.sailed?.vessel?.heading,
                        Type = WidgetForecast.Types.FirstOrDefault(x => x.Name == "Speed"),
                        IsActual = true,
                        IsVisible = true,
                        ValueText = $"{String.Format("{0:0.##}", speedKnots)}kn, {dynamicData.sailed?.vessel?.heading}°",
                        ValueInPercent = 100
                    });
                }




                if (dynamicData.planned?.geo?.waveTotalSeaDirection != null && dynamicData.planned?.geo?.waveTotalSeaSignificantWaveHeight != null)
                {
                    instance.Items.Add(new WidgetForecastItem()
                    {
                        Degree = (decimal)dynamicData.planned?.geo?.waveTotalSeaDirection,
                        Type = WidgetForecast.Types.FirstOrDefault(x => x.Name == "Wave"),
                        IsActual = false,
                        IsVisible = true,
                        ValueText = $"{String.Format("{0:0.##}", dynamicData.planned?.geo?.waveTotalSeaSignificantWaveHeight)}m, {dynamicData.planned?.geo?.waveTotalSeaDirection}°",
                        ValueInPercent = dynamicData.planned?.geo?.waveTotalSeaSignificantWaveHeight / 100 * WidgetForecast.Types.FirstOrDefault(x => x.Name == "Wave").Threshold
                    });
                }

                if (dynamicData.sailed?.geo?.waveTotalSeaDirection != null && dynamicData.sailed?.geo?.waveTotalSeaSignificantWaveHeight != null)
                {
                    instance.Items.Add(new WidgetForecastItem()
                    {
                        Degree = (decimal)dynamicData.sailed?.geo?.waveTotalSeaDirection,
                        Type = WidgetForecast.Types.FirstOrDefault(x => x.Name == "Wave"),
                        IsActual = true,
                        IsVisible = true,
                        ValueText = $"{String.Format("{0:0.##}", dynamicData.sailed?.geo?.waveTotalSeaSignificantWaveHeight)}m, {dynamicData.sailed?.geo?.waveTotalSeaDirection}°",
                        ValueInPercent = dynamicData.sailed?.geo?.waveTotalSeaSignificantWaveHeight / 100 * WidgetForecast.Types.FirstOrDefault(x => x.Name == "Wave").Threshold
                    });
                }

                if (dynamicData.sailed?.geo?.waveWindSeaDirection != null && dynamicData.sailed?.geo?.waveWindSeaSignificantWaveHeight != null)
                {
                    instance.Items.Add(new WidgetForecastItem()
                    {
                        Degree = (decimal)dynamicData.sailed?.geo?.waveWindSeaDirection,
                        Type = WidgetForecast.Types.FirstOrDefault(x => x.Name == "Wind"),
                        IsActual = true,
                        IsVisible = true,
                        ValueText = $"{String.Format("{0:0.##}", dynamicData.sailed?.geo?.waveWindSeaSignificantWaveHeight)}kn, {dynamicData.sailed?.geo?.waveWindSeaDirection}°",
                        ValueInPercent = dynamicData.sailed?.geo?.waveWindSeaSignificantWaveHeight / 100 * WidgetForecast.Types.FirstOrDefault(x => x.Name == "Wind").Threshold
                    });
                }

                if (dynamicData.planned?.geo?.waveWindSeaDirection != null && dynamicData.planned?.geo?.waveWindSeaSignificantWaveHeight != null)
                {
                    instance.Items.Add(new WidgetForecastItem()
                    {
                        Degree = (decimal)dynamicData.planned?.geo?.waveWindSeaDirection,
                        Type = WidgetForecast.Types.FirstOrDefault(x => x.Name == "Wind"),
                        IsActual = false,
                        IsVisible = true,
                        ValueText = $"{String.Format("{0:0.##}", dynamicData.planned?.geo?.waveWindSeaSignificantWaveHeight)}kn, {dynamicData.planned?.geo?.waveWindSeaDirection}°",
                        ValueInPercent = dynamicData.planned?.geo?.waveWindSeaSignificantWaveHeight / 100 * WidgetForecast.Types.FirstOrDefault(x => x.Name == "Wind").Threshold
                    });
                }

                if (dynamicData.planned?.geo?.oceanCurrentDirection != null && dynamicData.planned?.geo?.oceanCurrentValue != null)
                {
                    instance.Items.Add(new WidgetForecastItem()
                    {
                        Degree = (decimal)dynamicData.planned?.geo?.oceanCurrentDirection,
                        Type = WidgetForecast.Types.FirstOrDefault(x => x.Name == "Current"),
                        IsActual = false,
                        IsVisible = true,
                        ValueText = $"{String.Format("{0:0.##}", dynamicData.planned?.geo?.oceanCurrentValue)}kn, {dynamicData.planned?.geo?.oceanCurrentDirection}°",
                        ValueInPercent = dynamicData.planned?.geo?.oceanCurrentValue / 100 * WidgetForecast.Types.FirstOrDefault(x => x.Name == "Current").Threshold
                    });
                }

                if (dynamicData.sailed?.geo?.oceanCurrentDirection != null && dynamicData.sailed?.geo?.oceanCurrentValue != null)
                {
                    instance.Items.Add(new WidgetForecastItem()
                    {
                        Degree = (decimal)dynamicData.sailed?.geo?.oceanCurrentDirection,
                        Type = WidgetForecast.Types.FirstOrDefault(x => x.Name == "Current"),
                        IsActual = true,
                        IsVisible = true,
                        ValueText = $"{String.Format("{0:0.##}", dynamicData.sailed?.geo?.oceanCurrentValue)}kn, {dynamicData.sailed?.geo?.oceanCurrentDirection}°",
                        ValueInPercent = dynamicData.sailed?.geo?.oceanCurrentValue / 100 * WidgetForecast.Types.FirstOrDefault(x => x.Name == "Current").Threshold
                    });
                }

                return instance;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in WidgetData.GetForecastData: " + ex.Message);
                return null;
            }
        }
    }
}

