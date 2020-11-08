using System;
using System.Collections.Generic;
using System.Text;

namespace Sportsy.Data.Models
{
    class Location
    {
        public long Id { get; set; }
        public DateTime Time { get; set; }
        public decimal Lon { get; set; }
        public decimal Lat { get; set; }
        public Activity Activity { get; set; }
    }
}
