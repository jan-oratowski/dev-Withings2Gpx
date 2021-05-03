using Sportsy.WebClient.Shared.Models;
using System.Collections.Generic;

namespace Sportsy.WebClient.Shared.Responses
{
    public class DashboardResponse
    {
        public UserBase User { get; set; }
        public List<ActivityBase> Activities { get; set; }
    }
}
