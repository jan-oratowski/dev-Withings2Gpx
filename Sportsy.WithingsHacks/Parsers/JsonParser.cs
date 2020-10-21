using Sportsy.WithingsHacks.WithingsHttp;

namespace Sportsy.WithingsHacks.Parsers
{
    public class JsonParser : AbstractJsonParser
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
