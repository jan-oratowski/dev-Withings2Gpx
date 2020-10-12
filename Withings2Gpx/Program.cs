using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ConfigTools;
using Sportsy.Data;
using Sportsy.Data.Models;
using Sportsy.WithingsHacks.Parsers;

namespace Withings2Gpx
{
    class Program
    {
        public static Config Config;
        public static LocalData Data;
        private static readonly Loader<Config> Loader = new Loader<Config>();

        [STAThread]
        static void Main(string[] args)
        {
            Config = Loader.Load() ?? new Config();

            var key = "";
            var fbd = new FolderBrowserDialog { SelectedPath = Config.LastPath ?? string.Empty };

            if (fbd.ShowDialog() != DialogResult.OK)
                return;

            Config.LastPath = fbd.SelectedPath;
            Loader.Save(Config);

            LoadData(fbd.SelectedPath);

            Data.Activities = Data.Activities.OrderByDescending(a => a.Start).ToList();

            while (key.ToLower() != "q")
            {
                var i = 0;
                foreach (var item in Data.Activities.OrderByDescending(a => a.Start).Take(50))
                {
                    i++;
                    Console.WriteLine($"{i}. {item.Start} - {item.End} {item.Value} [{item.Source:G}]");
                }

                var command = Console.ReadLine();

                if (command == "d")
                {
                    DetectActivity();
                    Data.Activities = Data.Activities.OrderByDescending(a => a.Start).ToList();
                    Data.Save(Config.LastPath);
                    continue;
                }

                if (command == "c")
                {
                    CreateActivity();
                    Data.Activities = Data.Activities.OrderByDescending(a => a.Start).ToList();
                    Data.Save(Config.LastPath);
                    continue;
                }

                if (Data.Activities.Any(a => a.Value.ToLower() == command.ToLower()))
                {
                    foreach (var item in Data.Activities.Where(a => a.Value.ToLower() == command.ToLower()))
                    {
                        ExportActivity(item, fbd.SelectedPath);
                    }

                    return;
                }

                if (!int.TryParse(command, out i) || i > Data.Activities.Count)
                    return;

                var activity = Data.Activities[i - 1];
                ExportActivity(activity, fbd.SelectedPath);
            }
        }

        private static void CreateActivity()
        {
            var startString = EditableString.Input("Activity start", null).Value;
            if (!DateTime.TryParse(startString, out var start))
            {
                Console.WriteLine("Wrong date format, aborting!");
                return;
            }

            var endString = EditableString.Input("Activity end", null).Value;
            if (!DateTime.TryParse(endString, out var end) || end < start)
            {
                Console.WriteLine("Wrong activity end time, aborting!");
                return;
            }

            var activity = new Activity
            {
                Start = start,
                End = end,
                Source = Source.Manual,
                Value = EditableString.Input("Activity name", "Manual Activity").Value
            };

            Data.Activities.Add(activity);
        }

        private static void DetectActivity()
        {
            var lats = Data.Latitudes.OrderByDescending(a => a.Key).ToList();
            var actEnd = DateTime.Now;
            var prevLat = actEnd;
            for (int i = 0; i < lats.Count; i++)
            {
                var curLat = lats[i].Key;

                if (i == 0)
                {
                    actEnd = curLat;
                    prevLat = curLat;
                    continue;
                }

                if (curLat.AddMinutes(5) > prevLat)
                {
                    prevLat = curLat;
                    continue;
                }
                
                var activity = new Activity
                {
                    End = actEnd,
                    Start = curLat,
                    Source = Source.Other,
                    Value = "Detected"
                };
                
                if (EditableString
                    .Input($"Activity {activity.Start} - {actEnd} detected, add to Activities? [yes]/no", "yes")
                    .Value == "yes")
                {
                    Data.Activities.Add(activity);
                    if (lats.Count > i)
                    {
                        actEnd = lats[i + 1].Key;
                        prevLat = actEnd;
                    }
                }
                    
                if (EditableString
                    .Input("Detect next? yes/[no]","no")
                    .Value == "no")
                    return;
            }
        }

        private static void LoadData(string path)
        {
            Console.WriteLine($"Started loading data... {DateTime.Now}");
            var jsonParser = new JsonParser(path);
            var harParser = new HarParser(Path.Combine(path, "healthmate.withings.com.har"));

            var dataTask = Task.Factory.StartNew(() => LocalData.Load(path));
            var activitiesCsvParse = Task.Factory.StartNew(() => new ActivityParser(path).Get());
            var heartRateCsvParse = Task.Factory.StartNew(() => new HeartRateParser(path).Get());
            var longitudeCsvParse = Task.Factory.StartNew(() => new CoordinateParser(path, CoordinateType.Longitude).Get());
            var latitudeCsvParse = Task.Factory.StartNew(() => new CoordinateParser(path, CoordinateType.Latitude).Get());
            var jsonTask = jsonParser.LoadDataAsync();
            var harTask = harParser.LoadDataAsync();

            Task.WaitAll(dataTask, activitiesCsvParse, heartRateCsvParse, longitudeCsvParse, latitudeCsvParse, jsonTask, harTask);

            Data = dataTask.Result;

            var addActivities = Task.Factory.StartNew(() => AddActivities(activitiesCsvParse.Result));

            AddEntries(heartRateCsvParse.Result, longitudeCsvParse.Result, latitudeCsvParse.Result, Source.Csv);
            AddEntries(jsonParser.HeartRates, jsonParser.Longitudes, jsonParser.Latitudes, Source.Json);
            AddEntries(harParser.HeartRates, harParser.Longitudes, harParser.Latitudes, Source.Har);

            addActivities.Wait();

            Console.WriteLine($"Started saving data... {DateTime.Now}");
            Data.Save(path);
        }

        public static void AddActivities(IEnumerable<Sportsy.WithingsHacks.Withings.Activity> activities)
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

        public static void AddEntries(List<Sportsy.WithingsHacks.Withings.HeartRate> hrs,
            List<Sportsy.WithingsHacks.Withings.Coordinate> lons, List<Sportsy.WithingsHacks.Withings.Coordinate> lats, Source src)
        {
            var hr = Task.Factory.StartNew(() => AddHeartRates(hrs, src));
            var lon = Task.Factory.StartNew(() => AddLongitudes(lons, src));
            var lat = Task.Factory.StartNew(() => AddLatitudes(lats, src));
            Task.WaitAll(hr, lon, lat);
        }

        private static void AddHeartRates(List<Sportsy.WithingsHacks.Withings.HeartRate> hrs, Source src)
        {
            foreach (var heartRate in hrs.Where(heartRate => !Data.HeartRates.ContainsKey(heartRate.TimeStamp)))
                Data.HeartRates.Add(heartRate.TimeStamp, new HeartRate { Value = heartRate.Value, Source = src });
        }

        private static void AddLongitudes(List<Sportsy.WithingsHacks.Withings.Coordinate> lons, Source src)
        {
            foreach (var longitude in lons.Where(longitude => !Data.Longitudes.ContainsKey(longitude.TimeStamp)))
                Data.Longitudes.Add(longitude.TimeStamp, new Coordinate { Value = longitude.Value, Source = src });
        }

        private static void AddLatitudes(List<Sportsy.WithingsHacks.Withings.Coordinate> lats, Source src)
        {
            foreach (var latitude in lats.Where(latitude => !Data.Latitudes.ContainsKey(latitude.TimeStamp)))
                Data.Latitudes.Add(latitude.TimeStamp, new Coordinate { Value = latitude.Value, Source = src });
        }

        private static void ExportActivity(Activity activity, string path)
        {
            try
            {
                var activityHrs = Data.HeartRates.Where(h => h.Key >= activity.Start && h.Key <= activity.End).ToList();
                var activityLongs = Data.Longitudes.Where(l => l.Key >= activity.Start && l.Key <= activity.End).ToList();
                var activityLats = Data.Latitudes.Where(l => l.Key >= activity.Start && l.Key <= activity.End).ToList();

                var gpx = new GpxCreator(activity);

                gpx.AddData(activityLongs, activityLats, activityHrs);
                gpx.ValidateHr();
                gpx.SaveGpx(path);
            }
            catch (Exception e)
            {
                Console.BackgroundColor = ConsoleColor.DarkRed;
                Console.WriteLine(e);
                Console.WriteLine("     export failed!");
                Console.BackgroundColor = ConsoleColor.Black;
                Thread.Sleep(2500);
            }
        }

        private static string GetArgument(string[] args, string argument, string defaultValue = null)
        {
            return args.Any(a => a.StartsWith($"{argument}="))
                ? args.First(a => a.StartsWith($"{argument}=")).Replace($"{argument}=", string.Empty)
                : defaultValue;
        }

    }
}
