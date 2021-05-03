using Sportsy.Enums;
using System;

namespace Sportsy.Data.Models
{
    public class Measurement
    {
        public long Id { get; set; }
        public string Comments { get; set; }
        public DateTime Time { get; set; }
        public User User { get; set; }
        public decimal Value { get; set; }
        public decimal Fat { get; set; }
        public decimal Water { get; set; }
        public decimal Muscle { get; set; }
        public decimal Height { get; set; }
        public ImportSourceEnum ImportSource { get; set; }
    }
}
