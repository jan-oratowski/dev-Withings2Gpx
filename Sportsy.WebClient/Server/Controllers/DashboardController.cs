using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sportsy.Data.Database;
using Sportsy.Services.ActivityTools;
using Sportsy.Services.UserTools;
using Sportsy.WebClient.Shared.Responses;
using System.Linq;

namespace Sportsy.WebClient.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly ILogger<DashboardController> _logger;
        private readonly SportsyContext _context;

        public DashboardController(ILogger<DashboardController> logger, SportsyContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet]
        public DashboardResponse Get(int userId)
        {
            return new DashboardResponse
            {
                Activities = _context.Activities
                    .Where(a => a.User.Id == userId)
                    .OrderByDescending(a => a.StartTime)
                    .Take(10)
                    .AsEnumerable()
                    .Select(ActivityMapper.ToActivityBase)
                    .ToList(),
                User = _context.Users
                    .Single(u => u.Id == userId)
                    .ToUserBase()
            };
        }
    }
}
