using System;

namespace Withings2Gpx.Models.Data
{
    class Activity : Data<string>
    {
        public DateTime Start;
        public DateTime End;

        public Activity()
        {

        }
    }
}
