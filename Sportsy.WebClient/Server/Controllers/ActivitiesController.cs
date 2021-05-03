using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sportsy.Data.Database;
using Sportsy.Services.ActivityTools;
using Sportsy.WebClient.Shared.Responses;
using System.Linq;

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
        public ActivitiesResponse Get(int userId, int page = 1, int take = 25) =>
            new ActivitiesResponse
            {
                Activities = _context.Activities
                    .Where(a => a.User.Id == userId)
                    .Skip(take * (page - 1))
                    .Take(take)
                    .AsEnumerable()
                    .Select(ActivityMapper.ToActivityBase)
                    .ToList(),
            };
    }
}
