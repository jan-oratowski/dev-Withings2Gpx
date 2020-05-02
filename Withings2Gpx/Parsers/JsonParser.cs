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


        public JsonParser(string path)
        {
            _path = path;
            ParseHistory();
            HeartRates = GetHeartRates();
            Debug.Print(HeartRates.OrderByDescending(i => i.TimeStamp).First().TimeStamp.ToString());
            Longitudes = GetCoordinates(CoordinateType.Longitude);
            Debug.Print(Longitudes.OrderByDescending(i => i.TimeStamp).First().TimeStamp.ToString());
            Latitudes = GetCoordinates(CoordinateType.Latitude);
            Debug.Print(Latitudes.OrderByDescending(i => i.TimeStamp).First().TimeStamp.ToString());
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
            var coordinates = new List<Coordinate>();
            if (!_parsed)
                return coordinates;

            var field = coordinateType == CoordinateType.Longitude ? 3 : 2;
            var items = _recordedHistory.History.Where(h => h.DataType == RecordedHistory.DataType.Location);
            foreach (var item in items)
            {
                var data = item.Response.Body.Data().Body.Series[0];
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

            return coordinates;
        }

        private List<HeartRate> GetHeartRates()
        {
            var hrs = new List<HeartRate>();
            if (!_parsed)
                return hrs;

            var items = _recordedHistory.History.Where(h => h.DataType == RecordedHistory.DataType.HR);
            foreach (var item in items)
            {
                var data = item.Response.Body.Data().Body.Series[0];
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

            return hrs;
        }

        private static DateTime FromUnixTime(long unixTime) => new DateTime(1970,01,01,0,0,0).AddSeconds(unixTime).ToLocalTime();
    }
}
