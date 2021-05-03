using Sportsy.Enums;

namespace Sportsy.WebClient.Shared.Models
{
    public class ImportedActivityBase
    {
        public long Id { get; set; }
        public ImportSourceEnum ImportSource { get; set; }
    }
}
