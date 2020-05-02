using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Withings2Gpx.Models.WithingsHttp
{
    class RecordedHistory
    {
        public List<Item> History;
        
        public class Item
        {
            public DateTime Created;
            public DateTime Received;
            public Response Response;
            public Request Request;
            public DataType DataType 
            {
                get
                {
                    var postData = Request?.PostData;
                    if (string.IsNullOrEmpty(postData))
                        return DataType.Unknown;
                    if (postData.Contains("vasistas_category=hr"))
                        return DataType.HR;
                    if (postData.Contains("vasistas_category=location"))
                        return DataType.Location;
                    return DataType.Unknown;
                }
            }
        }

        public class Response
        {
            public Body Body;
        }

        public class Request
        {
            public string PostData;
            public string Url;
        }

        public class Body
        {
            [JsonProperty("body")]
            public string Content;

            public WithingsResponse Data()
            {
                if (string.IsNullOrEmpty(Content))
                    return null;

                var json = Content.Replace("\\\"", "\"");
                var data = JsonConvert.DeserializeObject<WithingsResponse>(json);

                return data;
            }
        }

        public enum DataType
        {
            Unknown,
            HR,
            Location
        }
    }
}
