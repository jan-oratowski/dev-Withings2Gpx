using System.Linq;
using HarSharp;
using Sportsy.WithingsHacks.WithingsHttp;

namespace Sportsy.WithingsHacks.Parsers
{
    public class HarParser : AbstractJsonParser
    {
        public HarParser(string path) : base(path)
        {
            ParserName = "HarParser";
        }

        protected override void ParseHistory()
        {
            RecordedHistory = new RecordedHistory();
            var harData = HarConvert.DeserializeFromFile(Path);
            
            RecordedHistory.History = harData.Log.Entries.Select(e => new RecordedHistory.Item(e)).ToList();
            IsParsed = true;
        }
    }
}
