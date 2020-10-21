using System;
using System.Collections.Generic;
using HarSharp;
using Newtonsoft.Json;

namespace Sportsy.WithingsHacks.WithingsHttp
{
    public class RecordedHistory
    {
        public List<Item> History = new List<Item>();
        
        public class Item
        {
            public Item()
            {
            }

            public Item(Entry harEntry)
            {
                if (harEntry == null || harEntry.Request.Method != "POST" || !harEntry.Request.Url.ToString().Contains("measure"))
                    return;

                if (harEntry.Request?.PostData?.Text != null)
                    Request = new Request
                    {
                        PostData = harEntry.Request.PostData.Text,
                        Url = harEntry.Request.Url?.ToString(),
                    };

                if (harEntry.Response?.Content?.Text != null)
                    Response = new Response
                    {
                        Body = new Body
                        {
                            Content = harEntry.Response.Content.Text
                        },
                    };

                Created = harEntry.StartedDateTime;
            }

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
            // ReSharper disable once InconsistentNaming
            HR,
            Location,
            Activity
        }
    }
}
