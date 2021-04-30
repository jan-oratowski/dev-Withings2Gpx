using System;
using System.Collections.Generic;
using System.Text;

namespace Sportsy.Data.Models
{
    public class ImportedPoint
    {
        public long Id { get; set; }
        public DateTime Time { get; set; }
        public decimal? Lon { get; set; }
        public decimal? Lat { get; set; }
        public int? HeartRate { get; set; }
        public int? Cadence { get; set; }
        public int? Power { get; set; }
        public ImportedActivity Activity { get; set; }
    }
}
