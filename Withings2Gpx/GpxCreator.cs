using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Sportsy.Data;
using Sportsy.Data.Models;

// ReSharper disable PossibleMultipleEnumeration

namespace Withings2Gpx
{
    class GpxCreator
    {
        public List<GpxItem> GpxItems = new List<GpxItem>();
        private readonly Activity _activity;

        public GpxCreator(Activity activity)
        {
            _activity = activity;
        }
        public void AddData(List<KeyValuePair<DateTime,Coordinate>> longitudes, List<KeyValuePair<DateTime, Coordinate>> latitudes, List<KeyValuePair<DateTime, HeartRate>> heartRates)
        {
            int prevHr = -1;

            foreach (var longitude in longitudes)
            {
                var selectLatitude = latitudes.Where(l => l.Key == longitude.Key);
                if (!selectLatitude.Any())
                    continue;

                var latitude = selectLatitude.First();

                var gpx = new GpxItem
                {
                    Latitude = latitude.Value.Value,
                    Longitude = longitude.Value.Value,
                    Time = longitude.Key
                };

                var selectHr = heartRates.Where(h => h.Key == longitude.Key);
                if (selectHr.Any())
                {
                    var hr = selectHr.First();
                    gpx.Hr = hr.Value.Value;
                    prevHr = hr.Value.Value;
                }
                else if (prevHr != -1)
                {
                    gpx.Hr = prevHr;
                }
                else if (heartRates.Any())
                {
                    prevHr = heartRates.OrderBy(h => h.Key).First().Value.Value;
                    gpx.Hr = prevHr;
                }

                GpxItems.Add(gpx);
            }
        }

        public void ValidateHr()
        {
            for (var i = 0; i < GpxItems.Count; i++)
            {
                var gpxItem = GpxItems[i];
                if (gpxItem.Hr > 60 || !GpxItems.Any(g => g.Hr > 60))
                    continue;

                gpxItem.Hr = (int)GpxItems.Where(g => g.Hr > 60).OrderBy(g => Math.Abs((g.Time - gpxItem.Time).TotalSeconds)).Take(10).Average(g => g.Hr);
            }
        }

        public void SaveGpx(string path)
        {
            if (GpxItems?.Count == 0)
            {
                Console.BackgroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("Not saving: " + _activity.Start.ToString("yyyy-MM-dd HHmmss") + " " + _activity.Value + ".gpx - nothing to export!");
                Console.BackgroundColor = ConsoleColor.Black;
                Thread.Sleep(1000);
                return;
            }

            var data = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
                "<gpx version=\"1.1\" xmlns=\"http://www.topografix.com/GPX/1/1\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:gte=\"http://www.gpstrackeditor.com/xmlschemas/General/1\" xmlns:gpxtpx=\"http://www.garmin.com/xmlschemas/TrackPointExtension/v1\" xmlns:gpxx=\"http://www.garmin.com/xmlschemas/GpxExtensions/v3\" targetNamespace=\"http://www.topografix.com/GPX/1/1\" elementFormDefault=\"qualified\" xsi:schemaLocation=\"http://www.topografix.com/GPX/1/1 http://www.topografix.com/GPX/1/1/gpx.xsd\">" +
                "<metadata><name>Withings Export</name></metadata><trk><name>Withings Export</name><trkseg>";

            foreach (var item in GpxItems.OrderBy(g => g.Time))
            {
                data += item.ToString();
            }

            data += "<extensions><gte:name>#1</gte:name><gte:color>#fbaf00</gte:color></extensions></trkseg></trk></gpx>";

            path = System.IO.Path.Combine(path, "gpx");

            if (!System.IO.Directory.Exists(path))
                System.IO.Directory.CreateDirectory(path);

            System.IO.File.WriteAllText(System.IO.Path.Combine(path,
                _activity.Start.ToString("yyyy-MM-dd HHmmss") + " " + _activity.Value + ".gpx"),
                data);

            Console.BackgroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine("File: " + _activity.Start.ToString("yyyy-MM-dd HHmmss") + " " + _activity.Value + ".gpx saved!");
            Console.BackgroundColor = ConsoleColor.Black;
            Thread.Sleep(1000);
        }
    }
}
