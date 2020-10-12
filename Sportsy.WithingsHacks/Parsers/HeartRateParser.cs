using System;
using System.Globalization;
using Sportsy.WithingsHacks.Withings;

namespace Sportsy.WithingsHacks.Parsers
{
    public class HeartRateParser : CsvParser<HeartRate>
    {
        public HeartRateParser(string path) :
            base(System.IO.Path.Combine(path, "raw_tracker_hr.csv"))
        {
            ParseType = "HeartRate CsvParser";
        }

        protected override HeartRate Parser(string line)
        {
            var vals = line.Split(',');
            if (vals.Length > 3)
                return null;

            return new HeartRate
            {
                TimeStamp = DateTime.Parse(vals[0]),
                Value = int.Parse(vals[2].Replace("[", "").Replace("]", ""), CultureInfo.InvariantCulture)
            };
        }
    }
}
