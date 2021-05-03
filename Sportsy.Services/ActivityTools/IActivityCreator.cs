using Sportsy.Data.Models;
using System.Collections.Generic;

namespace Sportsy.Services.ActivityToolService
{
    public interface IActivityCreator
    {
        public Activity CreateFrom(ImportedActivity imported);
        public ICollection<Point> CreateFrom(ICollection<ImportedPoint> imported);
    }
}
