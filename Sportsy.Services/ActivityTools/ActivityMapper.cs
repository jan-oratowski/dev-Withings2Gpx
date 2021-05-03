using Sportsy.Data.Models;
using Sportsy.Services.UserTools;
using Sportsy.WebClient.Shared.Models;
using System.Linq;

namespace Sportsy.Services.ActivityTools
{
    public static class ActivityMapper
    {
        public static ActivityBase ToActivityBase(this Activity activity) =>
            new ActivityBase
            {
                Id = activity.Id,
                Name = activity.Name,
                Comments = activity.Comments,
                StartTime = activity.StartTime,
                EndTime = activity.EndTime,
                User = activity.User.ToUserBase(),
                ImportedActivities = activity.ImportedActivities.Select(ToImportedActivityBase).ToList()
            };

        public static ImportedActivityBase ToImportedActivityBase(this ImportedActivity activity) =>
            new ImportedActivityBase
            {
                Id = activity.Id,
                ImportSource = activity.ImportSource
            };
    }
}
