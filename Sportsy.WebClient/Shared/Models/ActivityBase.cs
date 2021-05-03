using System;
using System.Collections.Generic;

namespace Sportsy.WebClient.Shared.Models
{
    public class ActivityBase
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Comments { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public UserBase User { get; set; }
        public List<ImportedActivityBase> ImportedActivities { get; set; }
    }
}
