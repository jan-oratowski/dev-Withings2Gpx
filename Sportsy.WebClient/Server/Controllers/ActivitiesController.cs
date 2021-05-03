using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Sportsy.Data.Database;
using Sportsy.WebClient.Shared.Models;
using Sportsy.WebClient.Shared.Responses;
using System.Linq;
using System.Threading.Tasks;


namespace Sportsy.WebClient.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ActivitiesController : ControllerBase
    {
        private readonly ILogger<ActivitiesController> _logger;
        private readonly SportsyContext _context;

        public ActivitiesController(ILogger<ActivitiesController> logger, SportsyContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet]
        public async Task<ActivitiesResponse> Get(int userId) =>
            new ActivitiesResponse
            {
                Activities = await _context.Activities
                    .Include(a => a.ImportedActivities)
                    .Where(a => a.User.Id == userId)
                    .Select(a => new ActivityBase
                    {
                        Id = a.Id,
                        Name = a.Name,
                        Comments = a.Comments,
                        StartTime = a.StartTime,
                        EndTime = a.EndTime,
                        User = new UserBase
                        {
                            Id = a.User.Id,
                            Name = a.User.Name,
                        },
                        ImportedActivities = a.ImportedActivities.Select(i => new ImportedActivityBase
                        {
                            Id = i.Id,
                            ImportSource = i.ImportSource
                        }).ToList()
                    })
                    .ToListAsync(),
            };
    }
}
