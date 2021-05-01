using System;

namespace Sportsy.Data.Models
{
    public class Point
    {
        public long Id { get; set; }
        public DateTime Time { get; set; }
        public decimal? Lon { get; set; }
        public decimal? Lat { get; set; }
        public int? HeartRate { get; set; }
        public int? Cadence { get; set; }
        public int? Power { get; set; }
        public decimal? Speed { get; set; }
        public decimal? Distance { get; set; }
        public decimal? Elevation { get; set; }
        public Activity Activity { get; set; }
    }
}
