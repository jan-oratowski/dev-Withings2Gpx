using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Withings2Gpx.Parsers
{
    abstract class CsvParser<T>
    {
        private readonly string _file;

        protected CsvParser(string file)
        {
            _file = file;
        }

        public List<T> Get(string filter = null)
        {
            var lines = System.IO.File.ReadAllLines(_file);
            var list = new List<T>();
            if (lines.Length == 0)
                return list;

            for (int i = 1; i < lines.Length; i++)
            {
                if (ValidateLine(lines[i], filter))
                    try
                    {
                        list.Add(Parser(lines[i]));
                    }
                    catch (Exception ex)
                    {
                        //Console.WriteLine(ex);
                    }  
            }
            return list;
        }

        private bool ValidateLine(string line, string filter)
        {
            if (string.IsNullOrWhiteSpace(line))
                return false;

            if (string.IsNullOrWhiteSpace(filter))
                return true;

            if (line.Contains(filter))
                return true;

            return false;
        }

        protected abstract T Parser(string line);
    }
}
