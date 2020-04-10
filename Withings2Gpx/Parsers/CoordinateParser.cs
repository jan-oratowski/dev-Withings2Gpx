using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Withings2Gpx.Models.Withings;

namespace Withings2Gpx.Parsers
{
    class CoordinateParser : CsvParser<Coordinate>
    {
        public CoordinateParser(CoordinateType coordinate) : base(coordinate == CoordinateType.Latitude
            ? "raw_tracker_latitude.csv" : "raw_tracker_longitude.csv")
        {

        }

        protected override Coordinate Parser(string line)
        {
            var vals = line.Split(',');

            return new Coordinate
            {
                TimeStamp = DateTime.Parse(vals[0]),
                Value = decimal.Parse(vals[2].Replace("[","").Replace("]",""), CultureInfo.InvariantCulture)
            };
        }
    }

    public enum CoordinateType
    {
        Longitude,
        Latitude
    }
}
