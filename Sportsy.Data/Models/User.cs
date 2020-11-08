using System;
using System.Collections.Generic;
using System.Text;

namespace Sportsy.Data.Models
{
    class User
    {
        public string Name;
        public List<Activity> Activities { get; set; }
    }
}
