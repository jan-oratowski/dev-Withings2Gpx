using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Withings2Gpx.Models.Withings;
using Withings2Gpx.Models.WithingsHttp;

namespace Withings2Gpx.Parsers
{
    class JsonParser
    {
        private readonly string _path;
        private RecordedHistory _recordedHistory;
        private bool _parsed;
        public List<Coordinate> Longitudes, Latitudes;
        public List<HeartRate> HeartRates;

        public static Task<JsonParser> Run(string path) => Task.Factory.StartNew(() => new JsonParser(path));

        public JsonParser(string path)
        {
            _path = path;
            
            ParseHistory();
            var heartRateParser = Task.Factory.StartNew(GetHeartRates);
            var longitudesParser = Task.Factory.StartNew(() => GetCoordinates(CoordinateType.Longitude));
            var latitudeParser = Task.Factory.StartNew(() => GetCoordinates(CoordinateType.Latitude));
            Task.WaitAll(heartRateParser, longitudesParser, latitudeParser);

            HeartRates = heartRateParser.Result;
            Longitudes = longitudesParser.Result;
            Latitudes = latitudeParser.Result;
        }

        private void ParseHistory()
        {
            var file = System.IO.Path.Combine(_path, "history.json");
            if (!System.IO.File.Exists(file))
                return;

            var text = System.IO.File.ReadAllText(file);
            _recordedHistory = Newtonsoft.Json.JsonConvert.DeserializeObject<RecordedHistory>(text);
            _parsed = true;
        }

        private List<Coordinate> GetCoordinates(CoordinateType coordinateType)
        {
            Console.WriteLine("Started Coordinate JsonParser " + (coordinateType == CoordinateType.Longitude ? "longitude" : "latitude"));

            var coordinates = new List<Coordinate>();
            if (!_parsed)
                return coordinates;

            var field = coordinateType == CoordinateType.Longitude ? 3 : 2;
            var items = _recordedHistory.History.Where(h => h.DataType == RecordedHistory.DataType.Location);
            foreach (var item in items)
            {
                var bodyData = item.Response.Body.Data();
                if (bodyData == null || bodyData.Body.Series.Count == 0)
                    continue;

                var data = bodyData.Body.Series[0];
                for (var i = 0; i < data.Dates.Length; i++)
                {
                    var coordinate = new Coordinate
                    {
                        TimeStamp = FromUnixTime(data.Dates[i]),
                        Value = data.Vasistas[i][field]
                    };
                    coordinates.Add(coordinate);
                }
            }
            
            Console.WriteLine("Finished Coordinate JsonParser " + (coordinateType == CoordinateType.Longitude ? "longitude" : "latitude"));

            return coordinates;
        }

        private List<HeartRate> GetHeartRates()
        {
            Console.WriteLine("Started HeartRate JsonParser");

            var hrs = new List<HeartRate>();
            if (!_parsed)
                return hrs;

            var items = _recordedHistory.History.Where(h => h.DataType == RecordedHistory.DataType.HR);
            foreach (var item in items)
            {
                var bodyData = item.Response.Body.Data();
                if (bodyData == null || bodyData.Body.Series.Count == 0)
                    continue;

                var data = bodyData.Body.Series[0];
                for (var i = 0; i < data.Dates.Length; i++)
                {
                    var hr = new HeartRate
                    {
                        TimeStamp = FromUnixTime(data.Dates[i]),
                        Value = (int)data.Vasistas[i][0]
                    };
                    hrs.Add(hr);
                }
            }

            Console.WriteLine("Finished HeartRate JsonParser");

            return hrs;
        }

        private List<Activity> GetActivities()
        {
            Console.WriteLine("Started Activity JsonParser");
            var activities = new List<Activity>();
            if (!_parsed)
                return activities;

            var items = _recordedHistory.History.Where(h => h.DataType == RecordedHistory.DataType.Activity);
            throw new NotImplementedException();

        }

        private static DateTime FromUnixTime(long unixTime) => new DateTime(1970, 01, 01, 0, 0, 0).AddSeconds(unixTime).ToLocalTime();
    }
}
