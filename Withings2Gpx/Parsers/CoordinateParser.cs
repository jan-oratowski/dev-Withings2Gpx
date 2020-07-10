﻿using System;
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
        public CoordinateParser(string path, CoordinateType coordinate) :
            base(System.IO.Path.Combine(path, coordinate == CoordinateType.Latitude
            ? "raw_tracker_latitude.csv" : "raw_tracker_longitude.csv"))
        {
            ParseType = "Coordinate CsvParser " +
                        (coordinate == CoordinateType.Latitude
                            ? "latitude"
                            : "longitude");
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
