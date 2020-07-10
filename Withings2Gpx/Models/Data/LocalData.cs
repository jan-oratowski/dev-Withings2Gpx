using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace Withings2Gpx.Models.Data
{
    class LocalData
    {
        public Dictionary<DateTime, HeartRate> HeartRates;
        public Dictionary<DateTime, Coordinate> Longitudes;
        public Dictionary<DateTime, Coordinate> Latitudes;
        public List<Activity> Activities;

        public LocalData()
        {
            HeartRates = new Dictionary<DateTime, HeartRate>();
            Longitudes = new Dictionary<DateTime, Coordinate>();
            Latitudes = new Dictionary<DateTime, Coordinate>();
            Activities = new List<Activity>();
        }

        public static LocalData Load(string file)
        {
            Console.WriteLine("Started loading data from local file...");
            if (!File.Exists(file))
                new LocalData().Save(file);
            
            var json = File.ReadAllText(file);
            var data = JsonConvert.DeserializeObject<LocalData>(json);

            Console.WriteLine("Finished loading data from local file...");
            return data;
        }

        public void Save(string file)
        {
            var json = JsonConvert.SerializeObject(this, Formatting.Indented);
            File.WriteAllText(file, json);
        }
    }
}
