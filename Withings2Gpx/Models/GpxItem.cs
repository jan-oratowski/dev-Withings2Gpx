using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Withings2Gpx.Models
{
    class GpxItem
    {
        public decimal Latitude;
        public decimal Longitude;
        public DateTime Time;
        public int Hr;

        public override string ToString()
        {
            Time = Time.ToUniversalTime();
            var time = Time.ToString("yyyy-MM-dd") + "T" + Time.ToString("HH:mm:ss") + "Z";

            var str = $"<trkpt lat=\"{Latitude.ToString(CultureInfo.InvariantCulture)}\"" +
                $" lon=\"{Longitude.ToString(CultureInfo.InvariantCulture)}\"><time>{time}</time>";
            if (Hr != 0)
                str += $"<extensions><gpxtpx:TrackPointExtension><gpxtpx:hr>{Hr}</gpxtpx:hr></gpxtpx:TrackPointExtension></extensions>";
            str += "</trkpt>";
            return str;
        }
    }
}
