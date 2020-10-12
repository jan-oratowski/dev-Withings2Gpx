using System;

namespace Sportsy.Data.Models
{
    public class Activity : Data<string>
    {
        public DateTime Start;
        public DateTime End;

        public Activity()
        {

        }
    }
}
