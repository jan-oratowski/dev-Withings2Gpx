using System;
using Sportsy.WithingsHacks.Withings;

namespace Sportsy.WithingsHacks.Parsers
{
    public class ActivityParser : CsvParser<Activity>
    {
        public ActivityParser(string path) : base(System.IO.Path.Combine(path, "activities.csv"))
        {
            ParseType = "Activity CsvParser";
        }

        protected override Activity Parser(string line)
        {
            var vals = line.Split(',');

            return new Activity
            {
                TimeStamp = DateTime.Parse(vals[0]),
                End = DateTime.Parse(vals[1]),
                Value = string.IsNullOrEmpty(vals[5]) ? string.Empty : vals[5].Replace("\"", "")
            };
        }
    }
}
