using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Withings2Gpx.Models.Withings;
using Withings2Gpx.Models.WithingsHttp;

namespace Withings2Gpx.Parsers
{
    class JsonParser : AbstractJsonParser
    {
        public JsonParser(string path) : base(path)
        {
            ParserName = "JsonParser";
        }

        protected override void ParseHistory()
        {
            var file = System.IO.Path.Combine(Path, "history.json");
            if (!System.IO.File.Exists(file))
                return;

            var text = System.IO.File.ReadAllText(file);
            RecordedHistory = Newtonsoft.Json.JsonConvert.DeserializeObject<RecordedHistory>(text);
            IsParsed = true;
        }

 
    }
}
