using System;

namespace Sportsy.Data.JsonDbModels
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
