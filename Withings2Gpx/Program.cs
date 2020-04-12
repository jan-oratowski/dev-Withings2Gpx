using System;
using System.Linq;
using Withings2Gpx.Parsers;

namespace Withings2Gpx
{
    class Program
    {
        static void Main(string[] args)
        {
            var activities = new ActivityParser().Get().OrderByDescending(a => a.TimeStamp).ToList();
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
                    ExportActivity(item);
                }
                return;
            }

            if (!int.TryParse(command, out i) || i > activities.Count)
                return;

            var activity = activities[i - 1];
            ExportActivity(activity);
        }

        private static void ExportActivity(Models.Withings.Activity activity)
        {
            var heartRates = new HeartRateParser().Get(activity.TimeStamp.ToString("yyyy-MM-dd"));
            var longitudes = new CoordinateParser(CoordinateType.Longitude).Get(activity.TimeStamp.ToString("yyyy-MM-dd"));
            var latitudes = new CoordinateParser(CoordinateType.Latitude).Get(activity.TimeStamp.ToString("yyyy-MM-dd"));

            var activityHrs = heartRates.Where(h => h.TimeStamp >= activity.TimeStamp && h.TimeStamp <= activity.End).ToList();
            var activityLongs = longitudes.Where(l => l.TimeStamp >= activity.TimeStamp && l.TimeStamp <= activity.End).ToList();
            var activityLats = latitudes.Where(l => l.TimeStamp >= activity.TimeStamp && l.TimeStamp <= activity.End).ToList();

            var gpx = new GpxCreator(activity);

            gpx.AddData(activityLongs, activityLats, activityHrs);
            gpx.ValidateHr();
            gpx.SaveGpx(@"D:\test\");
        }
    }
}
