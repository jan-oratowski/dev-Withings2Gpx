using System;

namespace Sportsy.Data.Models
{
    public class ImportedPoint
    {
        public long Id { get; set; }
        public DateTime? Time { get; set; }
        public decimal? Longitude { get; set; }
        public decimal? Latitude { get; set; }
        public int? HeartRate { get; set; }
        public int? Cadence { get; set; }
        public int? Power { get; set; }
        public decimal? Speed { get; set; }
        public decimal? Distance { get; set; }
        public decimal? Temperature { get; set; }
        public decimal? Elevation { get; set; }
        public ImportedActivity Activity { get; set; }
    }
}
