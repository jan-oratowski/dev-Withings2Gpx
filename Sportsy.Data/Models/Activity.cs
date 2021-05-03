using Sportsy.Enums;
using System;
using System.Collections.Generic;

namespace Sportsy.Data.Models
{
    public class Activity
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Comments { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public User User { get; set; }
        public ActivityTypeEnum ActivityType { get; set; }
        public ICollection<Point> Points { get; set; }
        public ICollection<ImportedActivity> ImportedActivities { get; set; }
    }
}
