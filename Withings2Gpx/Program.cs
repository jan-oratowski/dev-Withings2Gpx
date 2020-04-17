using System;
using System.Linq;
using System.Windows.Forms;
using Withings2Gpx.Parsers;

namespace Withings2Gpx
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            var fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() != DialogResult.OK)
                return;

            var activities = new ActivityParser(fbd.SelectedPath).Get().OrderByDescending(a => a.TimeStamp).ToList();
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

        private static void ExportActivity(Models.Withings.Activity activity, string path)
        {
            var heartRates = new HeartRateParser(path).Get(activity.TimeStamp.ToString("yyyy-MM-dd"));
            var longitudes = new CoordinateParser(path, CoordinateType.Longitude).Get(activity.TimeStamp.ToString("yyyy-MM-dd"));
            var latitudes = new CoordinateParser(path, CoordinateType.Latitude).Get(activity.TimeStamp.ToString("yyyy-MM-dd"));

            var activityHrs = heartRates.Where(h => h.TimeStamp >= activity.TimeStamp && h.TimeStamp <= activity.End).ToList();
            var activityLongs = longitudes.Where(l => l.TimeStamp >= activity.TimeStamp && l.TimeStamp <= activity.End).ToList();
            var activityLats = latitudes.Where(l => l.TimeStamp >= activity.TimeStamp && l.TimeStamp <= activity.End).ToList();

            var gpx = new GpxCreator(activity);

            gpx.AddData(activityLongs, activityLats, activityHrs);
            gpx.ValidateHr();
            gpx.SaveGpx(path);
        }
    }
}
