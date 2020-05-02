using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Withings2Gpx.Models.WithingsHttp
{
    public class WithingsResponse
    {
        public int Status;
        public ResponseBody Body;

        public class ResponseBody
        {
            public List<Item> Series;
        }

        public class Item
        {
            public int[] Types;
            public List<decimal[]> Vasistas;
            public long[] Dates;
        }

    }
}
