using System;
using System.Collections.Generic;

namespace Sportsy.WithingsHacks.Parsers
{
    public abstract class CsvParser<T>
    {
        private readonly string _file;
        protected string ParseType = "Generic CsvParser";

        protected CsvParser(string file)
        {
            _file = file;
        }

        public List<T> Get(string filter = null)
        {
            Console.WriteLine($"Started {ParseType}");
            var lines = System.IO.File.ReadAllLines(_file);
            var list = new List<T>();
            if (lines.Length == 0)
                return list;

            for (int i = 1; i < lines.Length; i++)
            {
                if (ValidateLine(lines[i], filter))
                    try
                    {
                        var line = Parser(lines[i]);
                        if (line != null)
                            list.Add(line);
                    }
                    catch (Exception ex)
                    {
                        //Console.WriteLine(ex);
                    }  
            }
            Console.WriteLine($"Finished {ParseType}");
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
