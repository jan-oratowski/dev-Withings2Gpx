using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sportsy.WithingsHacks.Withings;
using Sportsy.WithingsHacks.WithingsHttp;

namespace Sportsy.WithingsHacks.Parsers
{
    public class AbstractJsonParser
    {
        protected readonly string Path;
        protected RecordedHistory RecordedHistory;
        protected string ParserName = "AbstractJsonParser";
        protected bool IsParsed;
        public List<Coordinate> Longitudes, Latitudes;
        public List<HeartRate> HeartRates;

        public AbstractJsonParser(string path)
        {
            Path = path;
        }

        public Task LoadDataAsync() => Task.Factory.StartNew(LoadData);

        public void LoadData()
        {
            ParseHistory();

            var heartRateParser = Task.Factory.StartNew(GetHeartRates);
            var longitudesParser = Task.Factory.StartNew(() => GetCoordinates(CoordinateType.Longitude));
            var latitudeParser = Task.Factory.StartNew(() => GetCoordinates(CoordinateType.Latitude));
            Task.WaitAll(heartRateParser, longitudesParser, latitudeParser);

            HeartRates = heartRateParser.Result;
            Longitudes = longitudesParser.Result;
            Latitudes = latitudeParser.Result;
        }

        private List<Coordinate> GetCoordinates(CoordinateType coordinateType)
        {
            Console.WriteLine($"Started Coordinate {ParserName} {(coordinateType == CoordinateType.Longitude ? "longitude" : "latitude")}");

            var coordinates = new List<Coordinate>();
            if (!IsParsed)
                return coordinates;

            var field = coordinateType == CoordinateType.Longitude ? 3 : 2;
            var items = RecordedHistory.History.Where(h => h.DataType == RecordedHistory.DataType.Location);
            foreach (var item in items)
            {
                var bodyData = item.Response?.Body.Data();
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

            Console.WriteLine($"Finished Coordinate {ParserName} {(coordinateType == CoordinateType.Longitude ? "longitude" : "latitude")}");

            return coordinates;
        }

        private List<HeartRate> GetHeartRates()
        {
            Console.WriteLine($"Started HeartRate {ParserName}");

            var hrs = new List<HeartRate>();
            if (!IsParsed)
                return hrs;

            var items = RecordedHistory.History.Where(h => h.DataType == RecordedHistory.DataType.HR);
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

            Console.WriteLine($"Finished HeartRate {ParserName}");

            return hrs;
        }

        private List<Activity> GetActivities()
        {
            Console.WriteLine("Started Activity JsonParser");
            var activities = new List<Activity>();
            if (!IsParsed)
                return activities;

            var items = RecordedHistory.History.Where(h => h.DataType == RecordedHistory.DataType.Activity);
            throw new NotImplementedException();

        }

        private static DateTime FromUnixTime(long unixTime) => new DateTime(1970, 01, 01, 0, 0, 0).AddSeconds(unixTime).ToLocalTime();

        protected virtual void ParseHistory()
        {
        }
    }
}
