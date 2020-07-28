using System.Collections.Generic;
using ConfigTools;
using Newtonsoft.Json;

namespace Withings2Gpx.Models.Db
{
    public class Data
    {
        private const string FileName = "db.json";

        [JsonIgnore]
        public string Path;
        public List<Node> Nodes;
        public List<Activity> Activities;

        public Data()
        {
            Nodes = new List<Node>();
            Activities = new List<Activity>();
        }

        public static Data Load(string path)
        {
            var loader = new Loader<Data>(FileName, path);
            var data = loader.Load();
            data.Path = path;
            return data;
        }

        public void Save(string path = null)
        {
            if (string.IsNullOrEmpty(path))
                path = Path;

            var loader = new Loader<Data>(FileName, path);
            loader.Save(this);
        }
    }
}
