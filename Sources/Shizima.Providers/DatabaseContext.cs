using Microsoft.EntityFrameworkCore;
using Shizima.Entities;

namespace Shizima.Providers
{
    public class DatabaseContext : DbContext
    {
        public DbSet<Artist> Artists { get; set; }

        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {
        }

        public DatabaseContext() : base()
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
