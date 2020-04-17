using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Withings2Gpx.Models.Withings;

namespace Withings2Gpx.Parsers
{
    class ActivityParser : CsvParser<Activity>
    {
        public ActivityParser(string path) : base(System.IO.Path.Combine(path, "activities.csv"))
        {

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
