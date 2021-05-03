using System;
using System.Collections.Generic;

namespace Sportsy.Data.Models
{
    public class ImportedActivity
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public bool IsMain { get; set; }
        public string Link { get; set; }
        public string LinkText { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public ImportSourceEnum ImportSource { get; set; }
        public ActivityTypeEnum ActivityType { get; set; }
        public ICollection<ImportedPoint> Points { get; set; }
    }
}
