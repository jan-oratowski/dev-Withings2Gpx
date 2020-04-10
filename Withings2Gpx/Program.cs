using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Withings2Gpx.Parsers;

namespace Withings2Gpx
{
    class Program
    {
        static void Main(string[] args)
        {
            var activities = new ActivityParser().Get();
            var activity = activities.Where(a => a.Value == "Cycling").OrderByDescending(a => a.TimeStamp).First();

            var heartRates = new HeartRateParser().Get(activity.TimeStamp.ToString("yyyy-MM-dd"));
            var longitudes = new CoordinateParser(CoordinateType.Longitude).Get(activity.TimeStamp.ToString("yyyy-MM-dd"));
            var latitudes = new CoordinateParser(CoordinateType.Latitude).Get(activity.TimeStamp.ToString("yyyy-MM-dd"));

            var activityHrs = heartRates.Where(h => h.TimeStamp >= activity.TimeStamp && h.TimeStamp <= activity.End).ToList();
            var activityLongs = longitudes.Where(l => l.TimeStamp >= activity.TimeStamp && l.TimeStamp <= activity.End).ToList();
            var activityLats = latitudes.Where(l => l.TimeStamp >= activity.TimeStamp && l.TimeStamp <= activity.End).ToList();

            var gpx = new GpxCreator();

            gpx.AddData(activityLongs, activityLats, activityHrs);
            gpx.Save(@"D:\test\export.gpx");
        }
    }
}
