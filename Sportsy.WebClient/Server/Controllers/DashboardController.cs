using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Sportsy.Data.Database;
using Sportsy.Data.Models;

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
        public async Task<User> Get()
        {
            var userId = 1;
            return await _context.Users.FirstAsync(u => u.Id == userId);
        }
    }
}
