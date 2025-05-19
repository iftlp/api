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
        public DbSet<User> Users { get; set; }
        public DbSet<History> Histories { get; set; }
        public DbSet<DatabaseFingerprint> Fingerprints { get; set; }
        public DbSet<Administrator> Administrators { get; set; }

        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        { }

        public DatabaseContext() : base()
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            InitializeArtistsMusicsRelation(modelBuilder);
            InitializeAlbumsMusicsRelation(modelBuilder);
            InitializeHistoryRelations(modelBuilder);
            InitializeFingerprintRelations(modelBuilder);
            base.OnModelCreating(modelBuilder);
        }

        private static void InitializeArtistsMusicsRelation(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Artist>().HasMany(a => a.Musics).WithMany(a => a.Artists);
        }

        private static void InitializeAlbumsMusicsRelation(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Album>().HasMany(a => a.Musics).WithOne(m => m.Album).HasForeignKey(m => m.AlbumId);
            modelBuilder.Entity<Album>().HasOne(a => a.Artist).WithMany().HasForeignKey(a => a.ArtistId);
        }
        
        private static void InitializeHistoryRelations(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<History>().HasOne(h => h.User)
                                            .WithMany(u => u.Histories)
                                            .HasForeignKey(h => h.UserId);
            modelBuilder.Entity<History>().HasOne(h => h.Music)
                                            .WithMany(m => m.Histories)
                                            .HasForeignKey(h => h.MusicId);
        }

        private static void InitializeFingerprintRelations(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Music>().HasMany(m => m.Fingerprints)
                                        .WithOne()
                                        .HasForeignKey(f => f.MusicId);
        }
    }
}
