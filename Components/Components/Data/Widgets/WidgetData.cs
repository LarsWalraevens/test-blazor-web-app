using System.Text;
using System.IO;
using System.IO.Compression;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using System.Globalization;
using Newtonsoft.Json.Linq;

namespace Widget.Forecast
{
    public class VesselRouteData
    {
        public List<Sailed> Sailed { get; set; }
    }

    public class Sailed
    {
        public bool IsOriginal { get; set; }
        public DateTime RecordChanged { get; set; }
        public DateTime RecordCreated { get; set; }
        public DateTime Timestamp { get; set; }
        public double Bearing { get; set; }
        public double Heading { get; set; }
        public double DistanceToEnd { get; set; }
        public double DistanceToNext { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public Guid PassageRouteId { get; set; }
        public Guid RefinedWaypointId { get; set; }
        public Guid VesselId { get; set; }
        public int Number { get; set; }
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
                //             "sailed": [
                // {
                //                 "isOriginal": false,
                //   "recordChanged": "2024-07-29T10:03:42.1635104+02:00",
                //   "recordCreated": "2024-07-29T10:03:42.1635104+02:00",
                //   "timestamp": "2023-12-20T11:21:43.973+01:00",
                //   "bearing": 0.0,
                //   "heading": 0.0,
                //   "distanceToEnd": 0.0,
                //   "distanceToNext": 0.0,
                //   "latitude": 49.28909,
                //   "longitude": -123.07868,
                //   "passageRouteId": "00000000-0000-0000-0000-000000000000",
                //   "refinedWaypointId": "8d0f5e64-e062-4e6c-bde6-011d8621108c",
                //   "vesselId": "a1a6d02b-9b38-4e8d-9bd1-0cffe327f2b0",
                //   "number": 1
                // }
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
                    DegreeBearing = (decimal?)dynamicData.planned?.bearing ?? 0,
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

