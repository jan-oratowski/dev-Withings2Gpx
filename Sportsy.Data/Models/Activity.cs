using System;
using System.Collections.Generic;
using System.Text;

namespace Sportsy.Data.Models
{
    class Activity
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public User User { get; set; }
        public List<Location> Locations { get; set; }
    }
}
