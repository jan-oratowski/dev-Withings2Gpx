using Sportsy.Enums;
using System;
using System.Collections.Generic;

namespace Sportsy.Data.Models
{
    public class Gear
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public DateTime Bought { get; set; }
        public DateTime Retired { get; set; }
        public GearEnum GearType { get; set; }
        public ICollection<Gear> SubGear { get; set; }
        public ICollection<Activity> Activities { get; set; }
        public User User { get; set; }
    }
}
