using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Sportsy.Data.Models;

namespace Sportsy.Connections.Gpx
{
    public interface IGpxParser
    {
        ImportedActivity Parse(Stream stream);
    }
}
