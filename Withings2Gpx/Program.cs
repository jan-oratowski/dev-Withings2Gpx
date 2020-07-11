﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Withings2Gpx.Models;
using Withings2Gpx.Models.Data;
using Withings2Gpx.Parsers;

namespace Withings2Gpx
{
    class Program
    {
        public static Config Config;
        public static string ConfigPath;
        public static LocalData Data;

        [STAThread]
        static void Main(string[] args)
        {
            ConfigPath = GetArgument(args, "--config", "config.json");
            Config = Config.Load(ConfigPath);

            var key = "";
            var fbd = new FolderBrowserDialog {SelectedPath = Config.LastPath};

            if (fbd.ShowDialog() != DialogResult.OK)
                return;

            Config.LastPath = fbd.SelectedPath;
            Config.Save(Config, ConfigPath);

            LoadData(fbd.SelectedPath);

            while (key.ToLower() != "q")
            {
                var i = 0;
                foreach (var item in Data.Activities.Take(50))
                {
                    i++;
                    Console.WriteLine($"{i}. {item.Start} {item.Value}");
                }

                var command = Console.ReadLine();

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

        private static void LoadData(string path)
        {
            Console.WriteLine($"Started loading data... {DateTime.Now}");

            var dataTask = Task.Factory.StartNew(() => LocalData.Load(path));
            var activitiesCsvParse = Task.Factory.StartNew(() => new ActivityParser(path).Get().OrderByDescending(a => a.TimeStamp));
            var heartRateCsvParse = Task.Factory.StartNew(() => new HeartRateParser(path).Get());
            var longitudeCsvParse = Task.Factory.StartNew(() => new CoordinateParser(path, CoordinateType.Longitude).Get());
            var latitudeCsvParse = Task.Factory.StartNew(() => new CoordinateParser(path, CoordinateType.Latitude).Get());
            var jsonTask = JsonParser.Run(path);
            
            Task.WaitAll(dataTask, activitiesCsvParse, heartRateCsvParse, longitudeCsvParse, latitudeCsvParse, jsonTask);

            Data = dataTask.Result;

            foreach (var csvActivity in activitiesCsvParse.Result)
            {
                if (!Data.Activities.Any(a =>
                    a.Value == csvActivity.Value &&
                    a.Start == csvActivity.TimeStamp &&
                    a.End == csvActivity.TimeStamp))

                    Data.Activities.Add(new Activity
                    {
                        Start = csvActivity.TimeStamp,
                        End = csvActivity.End,
                        Value = csvActivity.Value,
                        Source = Source.Csv
                    });
            }

            AddEntries(heartRateCsvParse.Result, longitudeCsvParse.Result, latitudeCsvParse.Result, Source.Csv);
            AddEntries(jsonTask.Result.HeartRates, jsonTask.Result.Longitudes, jsonTask.Result.Latitudes, Source.Json);

            Console.WriteLine($"Started saving data... {DateTime.Now}");
            Data.Save(path);
        }

        public static void AddEntries(List<Models.Withings.HeartRate> hrs, List<Models.Withings.Coordinate> lons, List<Models.Withings.Coordinate> lats, Source src)
        {
            var hr = Task.Factory.StartNew(() => AddHeartRates(hrs, src));
            var lon = Task.Factory.StartNew(() => AddLongitudes(lons, src));
            var lat = Task.Factory.StartNew(() => AddLatitudes(lats, src));
            Task.WaitAll(hr, lon, lat);
        }

        private static void AddHeartRates(List<Models.Withings.HeartRate> hrs, Source src)
        {
            foreach (var heartRate in hrs.Where(heartRate => !Data.HeartRates.ContainsKey(heartRate.TimeStamp)))
                Data.HeartRates.Add(heartRate.TimeStamp, new HeartRate { Value = heartRate.Value, Source = src });
        }

        private static void AddLongitudes(List<Models.Withings.Coordinate> lons, Source src)
        {
            foreach (var longitude in lons.Where(longitude => !Data.Longitudes.ContainsKey(longitude.TimeStamp)))
                Data.Longitudes.Add(longitude.TimeStamp, new Coordinate { Value = longitude.Value, Source = src });
        }

        private static void AddLatitudes(List<Models.Withings.Coordinate> lats, Source src)
        {
            foreach (var latitude in lats.Where(latitude => !Data.Latitudes.ContainsKey(latitude.TimeStamp)))
                Data.Latitudes.Add(latitude.TimeStamp, new Coordinate { Value = latitude.Value, Source = src });
        }

        private static void ExportActivity(Activity activity, string path)
        {
            try
            {
                //var heartRates = new HeartRateParser(path).Get(activity.TimeStamp.ToString("yyyy-MM-dd"));
                //var longitudes = new CoordinateParser(path, CoordinateType.Longitude).Get(activity.TimeStamp.ToString("yyyy-MM-dd"));
                //var latitudes = new CoordinateParser(path, CoordinateType.Latitude).Get(activity.TimeStamp.ToString("yyyy-MM-dd"));

                //var jsonParser = new JsonParser(path);

                //heartRates.AddRange(jsonParser.HeartRates.Where(hr => !heartRates.Select(h => h.TimeStamp).Contains(hr.TimeStamp)));
                //longitudes.AddRange(jsonParser.Longitudes.Where(ln => !longitudes.Select(l => l.TimeStamp).Contains(ln.TimeStamp)));
                //latitudes.AddRange(jsonParser.Latitudes.Where(la => !latitudes.Select(l => l.TimeStamp).Contains(la.TimeStamp)));

                //var activityHrs = Data.HeartRates.Where(h => h.TimeStamp >= activity.TimeStamp && h.TimeStamp <= activity.End).ToList();
                //var activityLongs = Data.Longitudes.Where(l => l.TimeStamp >= activity.TimeStamp && l.TimeStamp <= activity.End).ToList();
                //var activityLats = Data.Latitudes.Where(l => l.TimeStamp >= activity.TimeStamp && l.TimeStamp <= activity.End).ToList();

                //var gpx = new GpxCreator(activity);

                //gpx.AddData(activityLongs, activityLats, activityHrs);
                //gpx.ValidateHr();
                //gpx.SaveGpx(path);
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
