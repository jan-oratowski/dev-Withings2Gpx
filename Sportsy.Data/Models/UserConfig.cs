using System;
using System.Collections.Generic;
using System.Text;

namespace Sportsy.Data.Models
{
    public class UserConfig
    {
         public int Id { get; set; }
         public User User { get; set; }
         public ConfigEnum Key { get; set; }
         public string Value { get; set; }
    }
}
