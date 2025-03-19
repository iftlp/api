using Microsoft.EntityFrameworkCore;
using Shizima.Entities;

namespace Shizima.Providers
{
    /// <summary>
    /// DatabaseContext used for managing entities.
    /// </summary>
    public class DatabaseContext : DbContext
    {
        public DbSet<Artist> Artists { get; set; }
        public DbSet<Album> Albums { get; set; }
        public DbSet<Music> Musics { get; set; }

        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        { }

        public DatabaseContext() : base()
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            InitializeArtistsAlbumsRelation(modelBuilder);
            InitializeAlbumsMusicsRelation(modelBuilder);
            base.OnModelCreating(modelBuilder);
        }

        private static void InitializeArtistsAlbumsRelation(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Artist>().HasMany(a => a.Albums).WithMany(a => a.Artists);
        }

        private static void InitializeAlbumsMusicsRelation(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Album>().HasMany(a => a.Musics).WithOne(m => m.Album).HasForeignKey(m => m.AlbumId);
        }
    }
}
