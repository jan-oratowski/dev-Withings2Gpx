using System;

namespace Sportsy.WithingsHacks.Withings
{
    public class WithingsCsvEntry<T>
    {
        public DateTime TimeStamp;
        public T Value;
    }
}
