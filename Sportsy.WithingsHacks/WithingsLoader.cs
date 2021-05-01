using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Sportsy.Data;
using Sportsy.Data.JsonDbModels;
using Sportsy.WithingsHacks.Parsers;
using Activity = Sportsy.WithingsHacks.Withings.Activity;
using Coordinate = Sportsy.WithingsHacks.Withings.Coordinate;
using HeartRate = Sportsy.WithingsHacks.Withings.HeartRate;

namespace Sportsy.WithingsHacks
{
    public class WithingsLoader
    {
        private readonly string _path;

        private Task<List<Activity>> _activitiesCsvParse;
        private Task<List<HeartRate>> _heartRateCsvParse;
        private Task<List<Coordinate>> _longitudeCsvParse, _latitudeCsvParse;
        private Task _jsonTask, _harTask;
        private readonly AbstractJsonParser _jsonParser, _harParser;

        public WithingsLoader(string path)
        {
            _path = path;
            _jsonParser = new JsonParser(path);
            _harParser = new HarParser(Path.Combine(path, "healthmate.withings.com.har"));
        }
        public void Start()
        {
            _activitiesCsvParse = Task.Factory.StartNew(() => new ActivityParser(_path).Get());
            _heartRateCsvParse = Task.Factory.StartNew(() => new HeartRateParser(_path).Get());
            _longitudeCsvParse = Task.Factory.StartNew(() => new CoordinateParser(_path, CoordinateType.Longitude).Get());
            _latitudeCsvParse = Task.Factory.StartNew(() => new CoordinateParser(_path, CoordinateType.Latitude).Get());
            _jsonTask = _jsonParser.LoadDataAsync();
            _harTask = _harParser.LoadDataAsync();
        }

        public void WaitForLoad() => 
            Task.WaitAll(_activitiesCsvParse, _heartRateCsvParse, _longitudeCsvParse, _latitudeCsvParse, _jsonTask, _harTask);

        public LocalData Add(LocalData data)
        {
            var adder = new Adder
            {
                Data = data
            };

            adder.AddEntries(_activitiesCsvParse.Result, _heartRateCsvParse.Result, _longitudeCsvParse.Result, _latitudeCsvParse.Result, Source.Csv);
            adder.AddEntries(_jsonParser.HeartRates, _jsonParser.Longitudes, _jsonParser.Latitudes, Source.Json);
            adder.AddEntries(_harParser.HeartRates, _harParser.Longitudes, _harParser.Latitudes, Source.Har);

            return adder.Data;
        }
    }
}
