using System;
using ConfigTools;
using Sportsy.Core;
using Sportsy.Data;

namespace Withings2Gpx.NetCore
{
    class Program
    {
        private static readonly Loader<Config> Loader = new Loader<Config>();

        static void Main(string[] args)
        {
            var config = Loader.Load() ?? new Config();

            var path = new EditableString
            {
                Value = config.LastPath ?? string.Empty
            };

            path.Input("Enter path to data");
            
            config.LastPath = path.Value;
            Loader.Save(config);

            var logic = new CommandLineLogic(config, path.Value);
            logic.Run();
        }
    }
}
