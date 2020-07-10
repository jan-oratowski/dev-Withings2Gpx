using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Withings2Gpx.Models.Withings;

namespace Withings2Gpx.Models
{
    class LocalData
    {
        public List<HeartRate> HeartRates;
        public List<Coordinate> Longitudes;
        public List<Coordinate> Latitudes;
        public List<Activity> Activities;

        public LocalData()
        {
            HeartRates = new List<HeartRate>();
            Longitudes = new List<Coordinate>();
            Latitudes = new List<Coordinate>();
            Activities = new List<Activity>();
        }

        public static LocalData Load(string file)
        {
            if (!File.Exists(file))
                new LocalData().Save(file);
            
            var json = File.ReadAllText(file);
            return JsonConvert.DeserializeObject<LocalData>(json);
        }

        public void Save(string file)
        {
            var json = JsonConvert.SerializeObject(this, Formatting.Indented);
            File.WriteAllText(file, json);
        }
    }
}
