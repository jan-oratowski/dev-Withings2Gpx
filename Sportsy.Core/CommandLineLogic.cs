using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ConfigTools;
using Sportsy.Connections;
using Sportsy.Data;
using Sportsy.Data.Models;
using Sportsy.WithingsHacks;

namespace Sportsy.Core
{
    public class CommandLineLogic
    {
        private LocalData _data;
        private readonly Config _config;
        private readonly string _path;

        public CommandLineLogic(Config config, string path)
        {
            _config = config;
            _path = path;
        }

        public void Run()
        {
            LoadData();
            var command = "";

            // ReSharper disable once PossibleNullReferenceException
            while (command.ToLower() != "q")
            {
                var i = 0;
                foreach (var item in _data.Activities.OrderByDescending(a => a.Start).Take(50))
                {
                    i++;
                    Console.WriteLine($"{i}. {item.Start} - {item.End} {item.Value} [{item.Source:G}]");
                }

                command = Console.ReadLine();

                if (command == "d")
                {
                    DetectActivity();
                    _data.Activities = _data.Activities.OrderByDescending(a => a.Start).ToList();
                    _data.Save(_config.LastPath);
                    continue;
                }

                if (command == "c")
                {
                    CreateActivity();
                    _data.Activities = _data.Activities.OrderByDescending(a => a.Start).ToList();
                    _data.Save(_config.LastPath);
                    continue;
                }

                if (_data.Activities.Any(a => a.Value.ToLower() == command.ToLower()))
                {
                    foreach (var item in _data.Activities.Where(a => a.Value.ToLower() == command.ToLower()))
                    {
                        ExportActivity(item, _path);
                    }

                    return;
                }

                if (!int.TryParse(command, out i) || i > _data.Activities.Count)
                    return;

                var activity = _data.Activities[i - 1];
                ExportActivity(activity, _path);
            }

        }
        private void LoadData()
        {
            Console.WriteLine($"Started loading data... {DateTime.Now}");

            var dataTask = Task.Factory.StartNew(() => LocalData.Load(_path));
            var withingsLoader = new WithingsLoader(_path);

            withingsLoader.Start();
            dataTask.Wait();
            withingsLoader.WaitForLoad();
            
            _data = withingsLoader.Add(dataTask.Result);
            _data.Activities = _data.Activities.OrderByDescending(a => a.Start).ToList();
            
            Console.WriteLine($"Started saving data... {DateTime.Now}");
            _data.Save(_path);
        }

        private void DetectActivity()
        {
            var lats = _data.Latitudes.OrderByDescending(a => a.Key).ToList();
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
                    _data.Activities.Add(activity);
                    if (lats.Count > i)
                    {
                        actEnd = lats[i + 1].Key;
                        prevLat = actEnd;
                    }
                }

                if (EditableString
                    .Input("Detect next? yes/[no]", "no")
                    .Value == "no")
                    return;
            }
        }


        private void CreateActivity()
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

            _data.Activities.Add(activity);
        }



        private void ExportActivity(Activity activity, string path)
        {
            try
            {
                var activityHrs = _data.HeartRates.Where(h => h.Key >= activity.Start && h.Key <= activity.End).ToList();
                var activityLongs = _data.Longitudes.Where(l => l.Key >= activity.Start && l.Key <= activity.End).ToList();
                var activityLats = _data.Latitudes.Where(l => l.Key >= activity.Start && l.Key <= activity.End).ToList();

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
    }
}
