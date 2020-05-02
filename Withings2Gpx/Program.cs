using System;
using System.Linq;
using System.Windows.Forms;
using Withings2Gpx.Models;
using Withings2Gpx.Parsers;

namespace Withings2Gpx
{
    class Program
    {
        public static Config Config;
        public static string ConfigPath;

        [STAThread]
        static void Main(string[] args)
        {
            ConfigPath = GetArgument(args, "--config", "config.json");
            Config = Config.Load(ConfigPath);

            var key = "";
            var fbd = new FolderBrowserDialog();
            fbd.SelectedPath = Config.LastPath;

            if (fbd.ShowDialog() != DialogResult.OK)
                return;
            Config.LastPath = fbd.SelectedPath;
            Config.Save(Config, ConfigPath);

            while (key.ToLower() != "q")
            {
                var activities = new ActivityParser(fbd.SelectedPath).Get().OrderByDescending(a => a.TimeStamp)
                    .ToList();
                var i = 0;
                foreach (var a in activities.Take(20))
                {
                    i++;
                    Console.WriteLine($"{i}. {a.TimeStamp} {a.Value}");
                }

                var command = Console.ReadLine();

                if (activities.Any(a => a.Value.ToLower() == command.ToLower()))
                {
                    foreach (var item in activities.Where(a => a.Value.ToLower() == command.ToLower()))
                    {
                        ExportActivity(item, fbd.SelectedPath);
                    }

                    return;
                }

                if (!int.TryParse(command, out i) || i > activities.Count)
                    return;

                var activity = activities[i - 1];
                ExportActivity(activity, fbd.SelectedPath);
            }
        }


        private static void ExportActivity(Models.Withings.Activity activity, string path)
        {
            var heartRates = new HeartRateParser(path).Get(activity.TimeStamp.ToString("yyyy-MM-dd"));
            var longitudes = new CoordinateParser(path, CoordinateType.Longitude).Get(activity.TimeStamp.ToString("yyyy-MM-dd"));
            var latitudes = new CoordinateParser(path, CoordinateType.Latitude).Get(activity.TimeStamp.ToString("yyyy-MM-dd"));

            var jsonParser = new JsonParser(path);

            heartRates.AddRange(jsonParser.HeartRates.Where(hr => !heartRates.Select(h => h.TimeStamp).Contains(hr.TimeStamp)));
            longitudes.AddRange(jsonParser.Longitudes.Where(ln => !longitudes.Select(l => l.TimeStamp).Contains(ln.TimeStamp)));
            latitudes.AddRange(jsonParser.Latitudes.Where(la => !latitudes.Select(l => l.TimeStamp).Contains(la.TimeStamp)));

            var activityHrs = heartRates.Where(h => h.TimeStamp >= activity.TimeStamp && h.TimeStamp <= activity.End).ToList();
            var activityLongs = longitudes.Where(l => l.TimeStamp >= activity.TimeStamp && l.TimeStamp <= activity.End).ToList();
            var activityLats = latitudes.Where(l => l.TimeStamp >= activity.TimeStamp && l.TimeStamp <= activity.End).ToList();

            var gpx = new GpxCreator(activity);

            gpx.AddData(activityLongs, activityLats, activityHrs);
            gpx.ValidateHr();
            gpx.SaveGpx(path);
        }

        private static string GetArgument(string[] args, string argument, string defaultValue = null)
        {
            return args.Any(a => a.StartsWith($"{argument}="))
                ? args.First(a => a.StartsWith($"{argument}=")).Replace($"{argument}=", string.Empty)
                : defaultValue;
        }

    }
}
