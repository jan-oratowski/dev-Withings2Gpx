using Sportsy.Data.JsonDbModels;
using Sportsy.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Activity = Sportsy.Data.JsonDbModels.Activity;
// ReSharper disable PossibleMultipleEnumeration


namespace Sportsy.Core
{
    class DbWriter
    {
        private ImportContext _context = new ImportContext();

        public void SaveActivity(Activity activity, List<KeyValuePair<DateTime, HeartRate>> hrs,
            List<KeyValuePair<DateTime, Coordinate>> longs, List<KeyValuePair<DateTime, Coordinate>> lats)
        {
            var importedPoints = AddData(longs, lats, hrs);

            var importedActivity = new ImportedActivity
            {
                StartTime = activity.Start,
                EndTime = activity.End,
                ImportSource = ConvertSource(activity.Source),
                IsMain = true,
                Points = importedPoints,
                Name = activity.Value,
            };

            _context.Activities.Add(new Data.Models.Activity
            {
                StartTime = activity.Start,
                EndTime = activity.End,
                ImportedActivities = new List<ImportedActivity>
                {
                    importedActivity,
                },
                Points = importedPoints.Select(p => new Point
                {
                    HeartRate = p.HeartRate,
                    Lat = p.Latitude,
                    Lon = p.Longitude,
                }).ToList(),
                User = _context.Users.First(u => u.Id == 1),
                Name = activity.Value,
            });
        }

        public List<ImportedPoint> AddData(
            List<KeyValuePair<DateTime, Coordinate>> longitudes, List<KeyValuePair<DateTime, Coordinate>> latitudes,
            List<KeyValuePair<DateTime, HeartRate>> heartRates)
        {
            var importedPoints = new List<ImportedPoint>();
            int prevHr = -1;

            foreach (var longitude in longitudes)
            {
                var selectLatitude = latitudes.Where(l => l.Key == longitude.Key);
                if (!selectLatitude.Any())
                    continue;

                var latitude = selectLatitude.First();

                var importedPoint = new ImportedPoint
                {
                    Latitude = latitude.Value.Value,
                    Longitude = longitude.Value.Value,
                    Time = longitude.Key
                };

                var selectHr = heartRates.Where(h => h.Key == longitude.Key);
                if (selectHr.Any())
                {
                    var hr = selectHr.First();
                    importedPoint.HeartRate = hr.Value.Value;
                    prevHr = hr.Value.Value;
                }
                else if (prevHr != -1)
                {
                    importedPoint.HeartRate = prevHr;
                }
                else if (heartRates.Any())
                {
                    prevHr = heartRates.OrderBy(h => h.Key).First().Value.Value;
                    importedPoint.HeartRate = prevHr;
                }

                importedPoints.Add(importedPoint);
            }

            return importedPoints;
        }

        private ImportSourceEnum ConvertSource(Source source)
        {
            switch (source)
            {
                case Source.Other:
                    return ImportSourceEnum.File;
                case Source.Csv:
                case Source.Har:
                case Source.Json:
                    return ImportSourceEnum.Withings;
                case Source.Strava:
                    return ImportSourceEnum.Strava;
                default:
                    return ImportSourceEnum.Manual;
            }
        }

    }
}
