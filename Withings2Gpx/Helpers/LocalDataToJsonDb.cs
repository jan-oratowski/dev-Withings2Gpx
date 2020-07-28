using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Withings2Gpx.Models.Data;
using Withings2Gpx.Models.Db;
using Activity = Withings2Gpx.Models.Db.Activity;

namespace Withings2Gpx.Helpers
{
    public class LocalDataToJsonDb
    {
        private readonly string _source;
        private readonly string _destination;

        public LocalDataToJsonDb(string source, string destination)
        {
            _source = source;
            _destination = destination;
        }

        public bool NeedsMigration =>
            File.Exists(Path.Combine(_source, LocalData.ActFile)) ||
            File.Exists(Path.Combine(_source, LocalData.HrFile)) ||
            File.Exists(Path.Combine(_source, LocalData.LatFile)) ||
            File.Exists(Path.Combine(_source, LocalData.LonFile));

        public Data Migrate(bool deleteAfter = false)
        {
            var localData = LocalData.Load(_source);

            var data = new Data
            {
                Nodes = MigrateNodes(localData),
                Activities = MigrateActivities(localData.Activities)
            };

            if (deleteAfter)
            {
                File.Delete(Path.Combine(_source, LocalData.ActFile));
                File.Delete(Path.Combine(_source, LocalData.HrFile));
                File.Delete(Path.Combine(_source, LocalData.LatFile));
                File.Delete(Path.Combine(_source, LocalData.LonFile));
            }

            data.Save(_destination);

            return data;
        }

        private static List<Activity> MigrateActivities(IEnumerable<Models.Data.Activity> activities) =>
            activities.Select(a => new Activity
            {
                Name = a.Value,
                Source = a.Source,
                Start = a.Start,
                End = a.End
            }).ToList();

        private static List<Node> MigrateNodes(LocalData localData)
        {
            var list = new List<Node>();

            foreach (var hr in localData.HeartRates)
            {
                var node = list.FirstOrDefault(n => n.TimeStamp == hr.Key);
                if (node == null)
                    list.Add(new Node
                    {
                        HeartRate = hr.Value.Value,
                        Source = hr.Value.Source,
                        TimeStamp = hr.Key
                    });
                else if (!node.HeartRate.HasValue)
                    node.HeartRate = hr.Value.Value;
            }

            foreach (var lat in localData.Latitudes)
            {
                var node = list.FirstOrDefault(n => n.TimeStamp == lat.Key);
                if (node == null)
                {
                    list.Add(new Node
                    {
                        Latitude = lat.Value.Value,
                        Source = lat.Value.Source,
                        TimeStamp = lat.Key
                    });
                }
                else if (!node.Latitude.HasValue)
                    node.Latitude = lat.Value.Value;
            }

            foreach (var lon in localData.Latitudes)
            {
                var node = list.FirstOrDefault(n => n.TimeStamp == lon.Key);
                if (node == null)
                {
                    list.Add(new Node
                    {
                        Longitude = lon.Value.Value,
                        Source = lon.Value.Source,
                        TimeStamp = lon.Key
                    });
                }
                else if (!node.Latitude.HasValue)
                    node.Longitude = lon.Value.Value;
            }

            return list;
        }
    }
}
