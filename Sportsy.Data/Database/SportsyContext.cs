using Microsoft.EntityFrameworkCore;
using Sportsy.Data.Models;

namespace Sportsy.Data
{
    public class SportsyContext : DbContext
    {
        public SportsyContext(DbContextOptions<SportsyContext> options) : base(options)
        {
        }

        public DbSet<Activity> Activities { get; set; }
        public DbSet<Gear> Gears { get; set; }
        public DbSet<ImportedActivity> ImportedActivity { get; set; }
        public DbSet<ImportedPoint> ImportedPoints { get; set; }
        public DbSet<Measurement> Measurements { get; set; }
        public DbSet<Point> Points { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserConfig> UserConfigs { get; set; }
    }
}
