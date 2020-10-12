using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Sportsy.Data.Models;

namespace Sportsy.Data
{
    public class LocalData
    {
        public Dictionary<DateTime, HeartRate> HeartRates;
        public Dictionary<DateTime, Coordinate> Longitudes;
        public Dictionary<DateTime, Coordinate> Latitudes;
        public List<Activity> Activities;

        private const string HrFile = "data_hr.json";
        private const string LonFile = "data_lon.json";
        private const string LatFile = "data_lat.json";
        private const string ActFile = "data_act.json";

        public LocalData()
        {
            HeartRates = new Dictionary<DateTime, HeartRate>();
            Longitudes = new Dictionary<DateTime, Coordinate>();
            Latitudes = new Dictionary<DateTime, Coordinate>();
            Activities = new List<Activity>();
        }

        public static LocalData Load(string path)
        {
            var data = new LocalData();
            
            var hrLoad = Task.Factory.StartNew(() => Load<Dictionary<DateTime,HeartRate>>(Path.Combine(path, HrFile)));
            var lonLoad = Task.Factory.StartNew(() => Load<Dictionary<DateTime,Coordinate>>(Path.Combine(path, LonFile)));
            var latLoad = Task.Factory.StartNew(() => Load<Dictionary<DateTime,Coordinate>>(Path.Combine(path, LatFile)));
            var actLoad = Task.Factory.StartNew(() => Load<List<Activity>>(Path.Combine(path, ActFile)));

            Task.WaitAll(hrLoad, lonLoad, latLoad, actLoad);

            data.HeartRates = hrLoad.Result;
            data.Longitudes = lonLoad.Result;
            data.Latitudes = latLoad.Result;
            data.Activities = actLoad.Result;

            return data;
        }

        private static T Load<T>(string file) where T : new()
        {
            Console.WriteLine("Started loading data from local file: " + Path.GetFileName(file));
            if (!File.Exists(file))
                new LocalData().Save(file, new T());

            var json = File.ReadAllText(file);
            var data = JsonConvert.DeserializeObject<T>(json);

            Console.WriteLine("Finished loading data from local file: " + Path.GetFileName(file));
            return data;
        }

        public void Save(string path)
        {
            var hrSave = Task.Factory.StartNew(() =>
                Save(Path.Combine(path, HrFile), HeartRates
                    .OrderBy(x => x.Key)
                    .ToDictionary(x => x.Key, x => x.Value)));

            var lonSave = Task.Factory.StartNew(() => 
                Save(Path.Combine(path, LonFile), Longitudes
                    .OrderBy(x => x.Key)
                    .ToDictionary(x => x.Key, x => x.Value)));

            var latSave = Task.Factory.StartNew(() => 
                Save(Path.Combine(path, LatFile), Latitudes
                    .OrderBy(x => x.Key)
                    .ToDictionary(x => x.Key, x => x.Value)));

            var actSave = Task.Factory.StartNew(() => 
                Save(Path.Combine(path, ActFile), Activities
                    .OrderBy(x => x.Start)
                    .ToList()));

            Task.WaitAll(hrSave, lonSave, latSave, actSave);
        }

        private void Save<T>(string file, T values)
        {
            var json = JsonConvert.SerializeObject(values, Formatting.Indented);
            File.WriteAllText(file, json);
        }
    }
}
