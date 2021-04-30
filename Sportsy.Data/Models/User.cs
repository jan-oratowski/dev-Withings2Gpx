using System;
using System.Collections.Generic;
using System.Text;

namespace Sportsy.Data.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<Activity> Activities { get; set; }
        public ICollection<Measurement> Weights { get; set; }
        public ICollection<Gear> Gears { get; set; }
        public ICollection<UserConfig> Configs { get; set; }

    }
}
