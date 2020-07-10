using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Withings2Gpx.Models;
using Withings2Gpx.Parsers;

namespace Withings2Gpx
{
    class Program
    {
        public static Config Config;
        public static string ConfigPath;
        public static LocalData Data;
        public const string LocalDataFile = "data.json";

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
                foreach (var a in Data.Activities.Take(50))
                {
                    i++;
                    Console.WriteLine($"{i}. {a.TimeStamp} {a.Value}");
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
            var file = Path.Combine(path, LocalDataFile);

            Data = LocalData.Load(file);

            Data.Activities = new ActivityParser(path).Get().OrderByDescending(a => a.TimeStamp)
                .ToList();

            var heartRates = new HeartRateParser(path).Get();
            var longitudes = new CoordinateParser(path, CoordinateType.Longitude).Get();
            var latitudes = new CoordinateParser(path, CoordinateType.Latitude).Get();

            var jsonParser = new JsonParser(path);

            heartRates.AddRange(jsonParser.HeartRates.Where(hr => !heartRates.Select(h => h.TimeStamp).Contains(hr.TimeStamp)));
            longitudes.AddRange(jsonParser.Longitudes.Where(ln => !longitudes.Select(l => l.TimeStamp).Contains(ln.TimeStamp)));
            latitudes.AddRange(jsonParser.Latitudes.Where(la => !latitudes.Select(l => l.TimeStamp).Contains(la.TimeStamp)));
            
            Data.HeartRates.AddRange(heartRates.Where(hr => !Data.HeartRates.Select(h => h.TimeStamp).Contains(hr.TimeStamp)));
            Data.Longitudes.AddRange(longitudes.Where(ln => !Data.Longitudes.Select(l => l.TimeStamp).Contains(ln.TimeStamp)));
            Data.Latitudes.AddRange(latitudes.Where(la => !Data.Latitudes.Select(l => l.TimeStamp).Contains(la.TimeStamp)));

            Data.Save(file);
        }

        private static void ExportActivity(Models.Withings.Activity activity, string path)
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

                var activityHrs = Data.HeartRates.Where(h => h.TimeStamp >= activity.TimeStamp && h.TimeStamp <= activity.End).ToList();
                var activityLongs = Data.Longitudes.Where(l => l.TimeStamp >= activity.TimeStamp && l.TimeStamp <= activity.End).ToList();
                var activityLats = Data.Latitudes.Where(l => l.TimeStamp >= activity.TimeStamp && l.TimeStamp <= activity.End).ToList();

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
