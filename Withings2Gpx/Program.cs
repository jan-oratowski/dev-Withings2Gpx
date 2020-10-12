using System;
using System.Windows.Forms;
using ConfigTools;
using Sportsy.Core;
using Sportsy.Data;

namespace Withings2Gpx
{
    class Program
    {
        private static readonly Loader<Config> Loader = new Loader<Config>();

        [STAThread]
        static void Main(string[] args)
        {
            var config = Loader.Load() ?? new Config();

            var fbd = new FolderBrowserDialog { SelectedPath = config.LastPath ?? string.Empty };

            if (fbd.ShowDialog() != DialogResult.OK)
                return;

            config.LastPath = fbd.SelectedPath;
            Loader.Save(config);

            var logic = new CommandLineLogic(config, fbd.SelectedPath);
            logic.Run();
        }
    }
}
