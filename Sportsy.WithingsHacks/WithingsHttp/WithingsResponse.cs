using System.Collections.Generic;

namespace Sportsy.WithingsHacks.WithingsHttp
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
