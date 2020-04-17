using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Withings2Gpx.Models.Withings;

namespace Withings2Gpx.Parsers
{
    class HeartRateParser : CsvParser<HeartRate>
    {
        public HeartRateParser(string path) :
            base(System.IO.Path.Combine(path, "raw_tracker_hr.csv"))
        {
        }

        protected override HeartRate Parser(string line)
        {
            var vals = line.Split(',');

            return new HeartRate
            {
                TimeStamp = DateTime.Parse(vals[0]),
                Value = int.Parse(vals[2].Replace("[", "").Replace("]", ""), CultureInfo.InvariantCulture)
            };
        }
    }
}
