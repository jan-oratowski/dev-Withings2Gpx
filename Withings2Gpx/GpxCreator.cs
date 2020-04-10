using System;
using System.Collections.Generic;
using System.Linq;
using Withings2Gpx.Models;
using Withings2Gpx.Models.Withings;

namespace Withings2Gpx
{
    class GpxCreator
    {
        public List<GpxItem> GpxItems = new List<GpxItem>();

        public void AddData(List<Coordinate> longitudes, List<Coordinate> latitudes, List<HeartRate> heartRates)
        {
            int prevHr = -1;

            foreach (var longitude in longitudes)
            {
                var latitude = latitudes.FirstOrDefault(l => l.TimeStamp == longitude.TimeStamp);
                if (latitude == null)
                    continue;

                var gpx = new GpxItem
                {
                    Latitude = latitude.Value,
                    Longitude = longitude.Value,
                    Time = longitude.TimeStamp
                };

                var hr = heartRates.FirstOrDefault(h => h.TimeStamp == longitude.TimeStamp);
                if (hr != null)
                {
                    gpx.Hr = hr.Value;
                    prevHr = hr.Value;
                }
                else if (prevHr != -1)
                {
                    gpx.Hr = prevHr;
                }
                else if (heartRates.Any())
                {
                    prevHr = heartRates.OrderBy(h => h.TimeStamp).First().Value;
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
                if (gpxItem.Hr > 60)
                    continue;

                gpxItem.Hr = (int)GpxItems.Where(g => g.Hr > 60).OrderBy(g => Math.Abs((g.Time - gpxItem.Time).TotalSeconds)).Take(10).Average(g => g.Hr);
            }
        }

        public void Save(string file)
        {

            var data = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
                "<gpx version=\"1.1\" xmlns=\"http://www.topografix.com/GPX/1/1\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:gte=\"http://www.gpstrackeditor.com/xmlschemas/General/1\" xmlns:gpxtpx=\"http://www.garmin.com/xmlschemas/TrackPointExtension/v1\" xmlns:gpxx=\"http://www.garmin.com/xmlschemas/GpxExtensions/v3\" targetNamespace=\"http://www.topografix.com/GPX/1/1\" elementFormDefault=\"qualified\" xsi:schemaLocation=\"http://www.topografix.com/GPX/1/1 http://www.topografix.com/GPX/1/1/gpx.xsd\">" +
                "<metadata><name>Withings Export</name></metadata><trk><name>Withings Export</name><trkseg>";

            foreach (var item in GpxItems.OrderBy(g => g.Time))
            {
                data = data + item.ToString();
            }

            data = data + "<extensions><gte:name>#1</gte:name><gte:color>#fbaf00</gte:color></extensions></trkseg></trk></gpx>";

            System.IO.File.WriteAllText(file, data);
        }
    }
}
