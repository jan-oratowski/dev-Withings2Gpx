using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Withings2Gpx.Models.Withings
{
    class Activity : WithingsCsvEntry<string>
    {
        public DateTime End;
    }
}
