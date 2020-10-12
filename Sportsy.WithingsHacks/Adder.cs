using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sportsy.Data;
using Sportsy.Data.Models;

namespace Sportsy.WithingsHacks
{
    class Adder
    {
        public LocalData Data;

        public void AddEntries(List<Withings.HeartRate> hrs, List<Withings.Coordinate> lons, List<Withings.Coordinate> lats, Source src)
        {
            var hr = Task.Factory.StartNew(() => AddHeartRates(hrs, src));
            var lon = Task.Factory.StartNew(() => AddLongitudes(lons, src));
            var lat = Task.Factory.StartNew(() => AddLatitudes(lats, src));
            Task.WaitAll(hr, lon, lat);
        }

        public void AddEntries(List<Withings.Activity> activities, List<Withings.HeartRate> hrs,
            List<Withings.Coordinate> lons, List<Withings.Coordinate> lats, Source src)
        {
            var addActivities = Task.Factory.StartNew(() => AddActivities(activities));
            AddEntries(hrs, lons, lats, src);
            addActivities.Wait();
        }

        private void AddHeartRates(List<Withings.HeartRate> hrs, Source src)
        {
            foreach (var heartRate in hrs.Where(heartRate => !Data.HeartRates.ContainsKey(heartRate.TimeStamp)))
                Data.HeartRates.Add(heartRate.TimeStamp, new HeartRate { Value = heartRate.Value, Source = src });
        }

        private void AddLongitudes(List<Withings.Coordinate> lons, Source src)
        {
            foreach (var longitude in lons.Where(longitude => !Data.Longitudes.ContainsKey(longitude.TimeStamp)))
                Data.Longitudes.Add(longitude.TimeStamp, new Coordinate { Value = longitude.Value, Source = src });
        }

        private void AddLatitudes(List<Withings.Coordinate> lats, Source src)
        {
            foreach (var latitude in lats.Where(latitude => !Data.Latitudes.ContainsKey(latitude.TimeStamp)))
                Data.Latitudes.Add(latitude.TimeStamp, new Coordinate { Value = latitude.Value, Source = src });
        }

        public void AddActivities(IEnumerable<Withings.Activity> activities)
        {
            foreach (var csvActivity in activities)
            {
                if (Data.Activities.Any(a =>
                    a.Value == csvActivity.Value &&
                    a.Start == csvActivity.TimeStamp &&
                    a.End == csvActivity.End))
                    continue;

                Data.Activities.Add(new Activity
                {
                    Start = csvActivity.TimeStamp,
                    End = csvActivity.End,
                    Value = csvActivity.Value,
                    Source = Source.Csv
                });
            }
        }

    }
}
