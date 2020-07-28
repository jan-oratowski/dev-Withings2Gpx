using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Withings2Gpx.Models.Db
{
    public class Node
    {
        public decimal? Longitude;
        public decimal? Latitude;
        public int? HeartRate;
        public DateTime TimeStamp;
        public Source Source;
    }
}
