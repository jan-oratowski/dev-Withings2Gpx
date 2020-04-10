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
        public ActivityParser() : base("activities.csv")
        {

        }

        protected override Activity Parser(string line)
        {
            var vals = line.Split(',');

            return new Activity
            {
                TimeStamp = DateTime.Parse(vals[0]),
                End = DateTime.Parse(vals[1]),
                Value = vals[5]
            };
        }
    }
}
