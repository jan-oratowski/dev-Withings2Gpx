using Sportsy.Enums;

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
