using System;
using System.Collections.Generic;
using System.Linq;
using Sportsy.Data.Models;
using Sportsy.Services.ActivityToolService;

namespace Sportsy.Services.ActivityTools
{
    public class ActivityCreator : IActivityCreator
    {
        public Activity CreateFrom(ImportedActivity imported)
        {
            imported.IsMain = true;

            return new Activity
            {
                Name = imported.Name,
                ActivityType = imported.ActivityType,
                StartTime = imported.StartTime ?? DateTime.Now,
                EndTime = imported.EndTime,
                Points = CreateFrom(imported.Points),
                ImportedActivities = new List<ImportedActivity>
                {
                    imported,
                },
            };
        }

        public ICollection<Point> CreateFrom(ICollection<ImportedPoint> imported) =>
            imported.Select(i => new Point
            {
                Cadence = i.Cadence,
                Distance = i.Distance,
                Elevation = i.Elevation,
                HeartRate = i.HeartRate,
                Latitude = i.Latitude,
                Longitude = i.Longitude,
                Power = i.Power,
                Speed = i.Speed,
                Time = i.Time
            }).ToList();
    }
}
